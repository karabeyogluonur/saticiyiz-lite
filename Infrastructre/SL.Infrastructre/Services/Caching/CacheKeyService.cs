using SL.Application.Defaults.Caching; // Defaults klasörünün namespace'i
using SL.Application.Interfaces.Services.Caching;

namespace SL.Infrastructure.Services.Caching;




public class CacheKeyService : ICacheKeyService
{
    public CacheKey PrepareKeyFor<TEntity>(object id) where TEntity : class
    {
        return EntityCacheDefaults<TEntity>.ByIdCacheKey(id);
    }

    public CacheKey PrepareKeyForList<TEntity>() where TEntity : class
    {
        return EntityCacheDefaults<TEntity>.AllCacheKey;
    }

    public string GetPrefix<TEntity>() where TEntity : class
    {
        return EntityCacheDefaults<TEntity>.Prefix;
    }
}