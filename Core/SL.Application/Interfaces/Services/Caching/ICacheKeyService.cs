using SL.Application.Interfaces.Services.Caching;

namespace SL.Application.Interfaces.Services.Caching;

public interface ICacheKeyService
{



    CacheKey PrepareKeyFor<TEntity>(object id) where TEntity : class;




    CacheKey PrepareKeyForList<TEntity>() where TEntity : class;





    string GetPrefix<TEntity>() where TEntity : class;
}