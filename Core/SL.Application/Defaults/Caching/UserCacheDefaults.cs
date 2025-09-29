using SL.Application.Interfaces.Services.Caching;
using SL.Domain.Entities.Membership; // ApplicationUser entity'nizin bulunduğu namespace

namespace SL.Application.Defaults.Caching;





public static class UserCacheDefaults
{




    public static string Prefix => EntityCacheDefaults<ApplicationUser>.Prefix;






    public static CacheKey ByIdCacheKey(Guid id) => EntityCacheDefaults<ApplicationUser>.ByIdCacheKey(id);





    public static CacheKey AllCacheKey => EntityCacheDefaults<ApplicationUser>.AllCacheKey;







    public static CacheKey IdByEmailCacheKey(string email) =>
        new CacheKey($"{Prefix}:IdByEmail:{{0}}", email.ToLowerInvariant());







    public static CacheKey ByUsernameCacheKey(string username) =>
        new CacheKey($"{Prefix}:ByUsername:{{0}}", username.ToLowerInvariant());
}