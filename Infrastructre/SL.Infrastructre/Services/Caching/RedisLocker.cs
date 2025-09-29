using SL.Application.Interfaces.Services.Caching;
using StackExchange.Redis;

namespace SL.Infrastructure.Services.Caching;

/// <summary>
/// Cache stampede sorununu önlemek için Redis tabanlı bir distributed lock mekanizması sağlar.
/// </summary>
public class RedisLocker : ILocker
{
    private readonly IDatabase _database;

    public RedisLocker(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    /// <summary>
    /// Belirtilen bir kaynak için kilit alarak verilen eylemi güvenli bir şekilde gerçekleştirir.
    /// </summary>
    /// <param name="resource">Kilitlenecek kaynağın anahtarı (genellikle cache key)</param>
    /// <param name="expirationTime">Kilidin ne kadar süre sonra otomatik serbest kalacağı</param>
    /// <param name="action">Kilit başarılı olursa çalıştırılacak eylem</param>
    /// <returns>Eylem başarılı ise true, kilit alınamadıysa false döner.</returns>
    public async Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action)
    {
        // Her kilit denemesi için benzersiz bir kimlik oluştur.
        // Bu, kendi süresi dolmuş kilidimizi yanlışlıkla serbest bırakmamızı önler.
        var token = Guid.NewGuid().ToString();

        if (await _database.LockTakeAsync(resource, token, expirationTime))
        {
            try
            {
                // Kilit alındı, güvenli eylemi gerçekleştir.
                await action();
                return true;
            }
            finally
            {
                // Eylem bittikten sonra kilidi serbest bırak.
                await _database.LockReleaseAsync(resource, token);
            }
        }

        // Kilit alınamadı.
        return false;
    }
}