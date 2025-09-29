using System;
using System.Threading.Tasks;

namespace SL.Application.Interfaces.Services.Caching;

// Uygulamanın kullanacağı ana cache yöneticisi arayüzü
public interface ICacheManager : IDisposable
{
    /// <summary>
    /// Cache'den bir veri alır. Eğer cache'de yoksa, 'acquire' fonksiyonunu çalıştırır,
    /// sonucu cache'e ekler ve döndürür.
    /// </summary>
    /// <typeparam name="T">Alınacak verinin tipi</typeparam>
    /// <param name="key">Cache anahtarı</param>
    /// <param name="acquire">Veri cache'de bulunamazsa çalıştırılacak fonksiyon</param>
    /// <param name="cacheTimeInMinutes">Cache'de kalma süresi (dakika)</param>
    /// <returns>Cache'lenmiş veri</returns>
    Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire, int? cacheTimeInMinutes = null);

    /// <summary>
    /// Cache'e bir veri ekler veya günceller.
    /// </summary>
    Task SetAsync(CacheKey key, object data, int? cacheTimeInMinutes = null);

    /// <summary>
    /// Bir anahtara göre cache'i temizler.
    /// </summary>
    Task RemoveAsync(CacheKey key);

    /// <summary>
    /// Bir prefix'e (ön ek) sahip tüm cache'leri temizler.
    /// </summary>
    Task RemoveByPrefixAsync(string prefix);

    /// <summary>
    /// Tüm cache'i temizler.
    /// </summary>
    Task ClearAsync();
}

// Sadece In-Memory Cache'i temsil eden arayüz.
// Bu, L1 cache'e doğrudan erişim gerektiğinde kullanılabilir.