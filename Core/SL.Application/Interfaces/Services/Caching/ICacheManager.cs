using System;
using System.Threading.Tasks;

namespace SL.Application.Interfaces.Services.Caching;


public interface ICacheManager : IDisposable
{









    Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire, int? cacheTimeInMinutes = null);




    Task SetAsync(CacheKey key, object data, int? cacheTimeInMinutes = null);




    Task RemoveAsync(CacheKey key);




    Task RemoveByPrefixAsync(string prefix);




    Task ClearAsync();
}


