using SL.Application.Interfaces.Services.Caching;

namespace SL.Application.Defaults.Caching;

/// <summary>
/// Bir entity için varsayılan cache anahtarlarını ve ayarlarını sağlar.
/// </summary>
/// <typeparam name="TEntity">Cache'lenecek entity tipi</typeparam>
public static class EntityCacheDefaults<TEntity> where TEntity : class
{
    /// <summary>
    /// Bu entity tipiyle ilgili tüm cache anahtarları için kullanılacak ön ek (prefix).
    /// Varsayılan olarak sınıfın adıdır (Örn: "User", "Tenant").
    /// Bu değer, uygulama başlangıcında özelleştirilebilir.
    /// </summary>
    public static string Prefix { get; set; } = typeof(TEntity).Name;

    /// <summary>
    /// Entity'nin ID'sine göre oluşturulan cache anahtarı.
    /// Format: "{Prefix}:{id}" (Örn: "User:123")
    /// </summary>
    public static CacheKey ByIdCacheKey(object id) =>
        new CacheKey($"{Prefix}:{{0}}", id);

    /// <summary>
    /// Entity'nin birden fazla ID'sine göre oluşturulan cache anahtarı.
    /// Format: "{Prefix}:ByIds:{id1}-{id2}-..."
    /// </summary>
    public static CacheKey ByIdsCacheKey(params object[] ids) =>
        new CacheKey($"{Prefix}:ByIds:{{0}}", string.Join("-", ids));

    /// <summary>
    /// Entity'nin tüm kayıtlarını içeren listeler için cache anahtarı.
    /// Format: "{Prefix}:All"
    /// </summary>
    public static CacheKey AllCacheKey =>
        new CacheKey($"{Prefix}:All");
}