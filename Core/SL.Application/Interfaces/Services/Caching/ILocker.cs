using System;
using System.Threading.Tasks;

namespace SL.Application.Interfaces.Services.Caching;

public interface ILocker
{
    /// <summary>
    /// Belirtilen kaynak için bir kilit mekanizması çalıştırır.
    /// </summary>
    /// <param name="resource">Kilitlenecek kaynağın (cache key) adı</param>
    /// <param name="expirationTime">Kilidin ne kadar süre sonra otomatik serbest kalacağı</param>
    /// <param name="action">Kilit başarılı olursa çalıştırılacak eylem</param>
    /// <returns>Eylem başarılı ise true, değilse false</returns>
    Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action);
}