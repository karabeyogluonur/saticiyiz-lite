using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using SL.Application.Interfaces.Services.Caching;
using StackExchange.Redis;
using System.Text.Json;

namespace SL.Infrastructure.Services.Caching;






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


        _subscriber.Subscribe(InvalidationChannel, (channel, message) =>
        {
            if (message == ClearAllSignal)
            {

                _tokenService.CancelCurrentToken();
            }
            else if (message.HasValue)
            {

                _memoryCache.Remove(message.ToString());
            }
        });
    }




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


        if (_memoryCache.TryGetValue(key.Key, out T finalData))
        {
            return finalData;
        }



        return await acquire();
    }




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




    public async Task RemoveAsync(CacheKey key)
    {
        _memoryCache.Remove(key.Key);
        await _distributedCache.RemoveAsync(key.Key);
        await _subscriber.PublishAsync(InvalidationChannel, key.Key);
    }





    public async Task RemoveByPrefixAsync(string prefix)
    {
        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
        var redisDb = _connectionMultiplexer.GetDatabase();



        var pattern = $"*{prefix}*";
        var keysToDelete = new List<RedisKey>();



        await foreach (var key in server.KeysAsync(pattern: pattern))
        {
            keysToDelete.Add(key);
        }

        if (keysToDelete.Any())
        {
            await redisDb.KeyDeleteAsync(keysToDelete.ToArray());
        }
    }





    public async Task ClearAsync()
    {

        _tokenService.CancelCurrentToken();


        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
        var redisDb = _connectionMultiplexer.GetDatabase();
        var keysToDelete = new List<RedisKey>();


        var pattern = "SL_App_*";



        await foreach (var key in server.KeysAsync(pattern: pattern))
        {
            keysToDelete.Add(key);
        }

        if (keysToDelete.Any())
        {
            await redisDb.KeyDeleteAsync(keysToDelete.ToArray());
        }


        await _subscriber.PublishAsync(InvalidationChannel, ClearAllSignal);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }




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