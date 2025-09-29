using System;
using System.Threading.Tasks;

namespace SL.Application.Interfaces.Services.Caching;

public interface ILocker
{







    Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action);
}