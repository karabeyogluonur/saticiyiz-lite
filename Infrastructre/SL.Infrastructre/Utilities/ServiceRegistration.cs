using Microsoft.Extensions.DependencyInjection;
using SL.Application.Interfaces.Services.Caching;
using SL.Application.Interfaces.Services.Messages;
using SL.Application.Interfaces.Services.Security;
using SL.Infrastructre.Services.Messages;
using SL.Infrastructre.Services.Security;
using SL.Infrastructure.Security;
using SL.Infrastructure.Services.Caching;
using StackExchange.Redis;

namespace SL.Infrastructre.Utilities
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructreService(this IServiceCollection services)
        {
            services.AddSingleton<IPasswordHasherService, BCryptPasswordHasherService>();
            services.AddScoped<ITokenizerService, TokenizerService>();
            services.AddScoped<IMessageTokenProvider, CustomerTokenProvider>();
            services.AddSingleton<IDataProtectionService, DataProtectionService>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.RedisConnectionString;
                options.InstanceName = "SL_App_";
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(Configuration.RedisConnectionString));


            services.AddSingleton<ILocker, RedisLocker>();
            services.AddSingleton<ICacheKeyService, CacheKeyService>();
            services.AddSingleton<ICacheManager, HybridCacheManager>();
            services.AddSingleton<IStaticCacheManager>(provider =>
            provider.GetRequiredService<ICacheManager>() as IStaticCacheManager);
            services.AddSingleton<IMemoryCacheTokenService, MemoryCacheTokenService>();
        }
    }
}
