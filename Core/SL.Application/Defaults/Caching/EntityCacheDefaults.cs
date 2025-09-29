using SL.Application.Interfaces.Services.Caching;

namespace SL.Application.Defaults.Caching;





public static class EntityCacheDefaults<TEntity> where TEntity : class
{





    public static string Prefix { get; set; } = typeof(TEntity).Name;





    public static CacheKey ByIdCacheKey(object id) =>
        new CacheKey($"{Prefix}:{{0}}", id);





    public static CacheKey ByIdsCacheKey(params object[] ids) =>
        new CacheKey($"{Prefix}:ByIds:{{0}}", string.Join("-", ids));





    public static CacheKey AllCacheKey =>
        new CacheKey($"{Prefix}:All");
}