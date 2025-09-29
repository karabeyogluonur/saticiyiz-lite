using SL.Application.Interfaces.Services.Caching;
using SL.Domain.Entities.Membership; // ApplicationUser entity'nizin bulunduğu namespace

namespace SL.Application.Defaults.Caching;

/// <summary>
/// ApplicationUser entity'si için standart cache anahtarlarını ve şablonlarını sağlar.
/// Bu sınıf, uygulama genelinde tutarlılığı sağlamak için kullanılır.
/// </summary>
public static class TenantCacheDefaults
{
    /// <summary>
    /// Tenant ile ilgili cache anahtarları için kullanılacak ön ek (prefix). Varsayılan olarak "Tenant"dır.
    /// </summary>
    public static string Prefix => EntityCacheDefaults<Tenant>.Prefix;

    /// <summary>
    /// Tam bir Tenant nesnesini ID'sine göre getiren cache anahtarı.
    /// Bu, genellikle WorkContext tarafından tenant'ın adı gibi bilgileri almak için kullanılır.
    /// Format: "Tenant:{id}"
    /// </summary>
    /// <param name="id">Tenant'ın Guid tipindeki ID'si</param>
    public static CacheKey ByIdCacheKey(Guid id) => EntityCacheDefaults<Tenant>.ByIdCacheKey(id);

    /// <summary>
    /// Sadece tenant konfigürasyonunu (özellikle connection string'i içeren DTO'yu) 
    /// ID'sine göre getiren cache anahtarı.
    /// Bu, TenantStore ve TenantMiddleware tarafından DbContext'i yapılandırmak için kullanılır.
    /// Format: "Tenant:Config:{id}"
    /// </summary>
    /// <param name="id">Tenant'ın Guid tipindeki ID'si</param>
    public static CacheKey ConfigurationByIdCacheKey(Guid id) =>
        new CacheKey($"{EntityCacheDefaults<Tenant>.Prefix}:Config:{{0}}", id);
}