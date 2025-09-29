using SL.Application.Interfaces.Services.Caching;
using SL.Domain.Entities.Membership; // ApplicationUser entity'nizin bulunduğu namespace

namespace SL.Application.Defaults.Caching;





public static class TenantCacheDefaults
{



    public static string Prefix => EntityCacheDefaults<Tenant>.Prefix;







    public static CacheKey ByIdCacheKey(Guid id) => EntityCacheDefaults<Tenant>.ByIdCacheKey(id);








    public static CacheKey ConfigurationByIdCacheKey(Guid id) =>
        new CacheKey($"{EntityCacheDefaults<Tenant>.Prefix}:Config:{{0}}", id);
}