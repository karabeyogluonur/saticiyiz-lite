using SL.Application.Interfaces.Services.Caching;
using StackExchange.Redis;

namespace SL.Infrastructure.Services.Caching;




public class RedisLocker : ILocker
{
    private readonly IDatabase _database;

    public RedisLocker(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }








    public async Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action)
    {


        var token = Guid.NewGuid().ToString();

        if (await _database.LockTakeAsync(resource, token, expirationTime))
        {
            try
            {

                await action();
                return true;
            }
            finally
            {

                await _database.LockReleaseAsync(resource, token);
            }
        }


        return false;
    }
}