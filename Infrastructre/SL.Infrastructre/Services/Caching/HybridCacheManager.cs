using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using SL.Application.Interfaces.Services.Caching;
using StackExchange.Redis;
using System.Text.Json;

namespace SL.Infrastructure.Services.Caching;

/// <summary>
/// Hem In-Memory (L1) hem de Distributed (L2) cache mekanizmalarını bir arada yöneten hibrit cache yöneticisi.
/// Veri tutarlılığını Redis Pub/Sub kanalı üzerinden sağlar. Cache stampede sorununu ILocker ile önler.
/// Tüm in-memory cache'i temizleme yeteneğine sahiptir.
/// </summary>
public class HybridCacheManager : ICacheManager, IStaticCacheManager
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILocker _locker;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ISubscriber _subscriber;
    private readonly IMemoryCacheTokenService _tokenService;

    private const string InvalidationChannel = "CacheInvalidationChannel";
    private const string ClearAllSignal = "__CLEAR_ALL__";

    public HybridCacheManager(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ILocker locker,
        IConnectionMultiplexer connectionMultiplexer,
        IMemoryCacheTokenService tokenService)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _locker = locker;
        _connectionMultiplexer = connectionMultiplexer;
        _subscriber = _connectionMultiplexer.GetSubscriber();
        _tokenService = tokenService;

        // Diğer sunuculardan gelen invalidasyon ve temizleme sinyallerini dinle
        _subscriber.Subscribe(InvalidationChannel, (channel, message) =>
        {
            if (message == ClearAllSignal)
            {
                // Tüm L1 cache'i temizle sinyali geldi.
                _tokenService.CancelCurrentToken();
            }
            else if (message.HasValue)
            {
                // Tek bir key'i L1'den sil.
                _memoryCache.Remove(message.ToString());
            }
        });
    }

    /// <summary>
    /// Cache'den bir veri alır. Yoksa, acquire fonksiyonunu çalıştırır, sonucu cache'e ekler ve döner.
    /// </summary>
    public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire, int? cacheTimeInMinutes = null)
    {
        if (_memoryCache.TryGetValue(key.Key, out T data))
        {
            return data;
        }

        var locked = await _locker.PerformActionWithLockAsync(key.Key, TimeSpan.FromSeconds(20), async () =>
        {
            if (_memoryCache.TryGetValue(key.Key, out data))
            {
                return;
            }

            var distributedData = await _distributedCache.GetStringAsync(key.Key);
            if (!string.IsNullOrEmpty(distributedData))
            {
                data = JsonSerializer.Deserialize<T>(distributedData);
            }
            else
            {
                data = await acquire();
                if (data != null)
                {
                    var cacheEntryOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeInMinutes ?? key.CacheTimeInMinutes ?? 60)
                    };
                    await _distributedCache.SetStringAsync(key.Key, JsonSerializer.Serialize(data), cacheEntryOptions);
                }
            }

            if (data != null)
            {
                SetMemoryCache(key.Key, data, cacheTimeInMinutes ?? key.CacheTimeInMinutes);
            }
        });

        // Kilit mekanizması sonrasında veriyi tekrar oku
        if (_memoryCache.TryGetValue(key.Key, out T finalData))
        {
            return finalData;
        }

        // Eğer kilit alınamadıysa veya başka bir sorun olduysa (çok nadir bir durum),
        // cache'i bypass edip doğrudan kaynaktan veri al.
        return await acquire();
    }

    /// <summary>
    /// Cache'e bir veri ekler veya günceller. Değişikliği diğer sunuculara bildirir.
    /// </summary>
    public async Task SetAsync(CacheKey key, object data, int? cacheTimeInMinutes = null)
    {
        SetMemoryCache(key.Key, data, cacheTimeInMinutes ?? key.CacheTimeInMinutes);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeInMinutes ?? key.CacheTimeInMinutes ?? 60)
        };
        await _distributedCache.SetStringAsync(key.Key, JsonSerializer.Serialize(data), options);

        await _subscriber.PublishAsync(InvalidationChannel, key.Key);
    }

    /// <summary>
    /// Belirtilen anahtarı cache'den kaldırır. Değişikliği diğer sunuculara bildirir.
    /// </summary>
    public async Task RemoveAsync(CacheKey key)
    {
        _memoryCache.Remove(key.Key);
        await _distributedCache.RemoveAsync(key.Key);
        await _subscriber.PublishAsync(InvalidationChannel, key.Key);
    }

    /// <summary>
    /// Belirtilen ön eke (prefix) sahip tüm anahtarları dağıtık cache'den (L2) siler.
    /// Bu işlem, diğer sunucuların L1 cache'lerini etkilemez, onlar zamanla dolar.
    /// </summary>
    public async Task RemoveByPrefixAsync(string prefix)
    {
        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
        var redisDb = _connectionMultiplexer.GetDatabase();

        // Redis InstanceName'i de desene dahil etmek en güvenli yoldur.
        // Örnek: var pattern = $"Your_Instance_Name_*{prefix}*";
        var pattern = $"*{prefix}*";
        var keysToDelete = new List<RedisKey>();

        // YANLIŞ: await foreach (var key in server.ScanKeysAsync(pattern: pattern))
        // DOĞRU:
        await foreach (var key in server.KeysAsync(pattern: pattern))
        {
            keysToDelete.Add(key);
        }

        if (keysToDelete.Any())
        {
            await redisDb.KeyDeleteAsync(keysToDelete.ToArray());
        }
    }

    /// <summary>
    /// Hem lokal in-memory (L1) hem de dağıtık (L2) cache'i tamamen temizler.
    /// Bu, tehlikeli bir operasyondur ve dikkatli kullanılmalıdır.
    /// </summary>
    public async Task ClearAsync()
    {
        // 1. L1 Cache'i Temizle (Lokal)
        _tokenService.CancelCurrentToken();

        // 2. L2 Cache'i Temizle (Redis - Güvenli Yöntem)
        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
        var redisDb = _connectionMultiplexer.GetDatabase();
        var keysToDelete = new List<RedisKey>();

        // Uygulamanızın InstanceName'ini ekleyerek daha güvenli hale getirin: "SL_App_*"
        var pattern = "SL_App_*";

        // YANLIŞ: await foreach (var key in server.ScanKeysAsync(pattern: pattern))
        // DOĞRU:
        await foreach (var key in server.KeysAsync(pattern: pattern))
        {
            keysToDelete.Add(key);
        }

        if (keysToDelete.Any())
        {
            await redisDb.KeyDeleteAsync(keysToDelete.ToArray());
        }

        // 3. Diğer Sunuculara Haber Ver
        await _subscriber.PublishAsync(InvalidationChannel, ClearAllSignal);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// In-memory cache'e veri eklerken, tüm girişleri merkezi CancellationToken'a bağlayan yardımcı metot.
    /// </summary>
    private void SetMemoryCache(string key, object data, int? cacheTimeInMinutes, int defaultTimeInMinutes = 60)
    {
        var options = new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeInMinutes ?? defaultTimeInMinutes)
        };

        options.AddExpirationToken(new CancellationChangeToken(_tokenService.TokenSource.Token));

        _memoryCache.Set(key, data, options);
    }
}