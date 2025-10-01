using Microsoft.Extensions.DependencyInjection;
using SL.Application.Defaults.Caching;
using SL.Application.Interfaces.Services.Caching;
using SL.Application.Interfaces.Services.Context;
using SL.Application.Interfaces.Services.Messages;
using SL.Application.Interfaces.Services.Security;
using SL.Application.Interfaces.Services.Tenants;
using SL.Application.Providers.Messages;
using SL.Infrastructre.Services.Messages;
using SL.Infrastructre.Services.Security;
using SL.Infrastructre.Services.Resilience;
using SL.Infrastructure.Security;
using SL.Infrastructure.Services.Caching;
using SL.Infrastructure.Services.Context;
using SL.Persistence.Utilities;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace SL.Infrastructre.Utilities
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructreService(this IServiceCollection services)
        {
            services.AddSingleton<IPasswordHasherService, BCryptPasswordHasherService>();
            
                services.AddScoped<ITokenRegistryService, TokenRegistryService>();
                services.AddScoped<ITokenFactoryService, TokenFactoryService>();
                services.AddScoped<IMessageTemplateService, MessageTemplateService>();
                
                services.AddScoped<IEmailWorkflowService, EmailWorkflowService>();
                
                services.AddScoped<ITokenProvider, UserTokenProvider>();
                services.AddScoped<ITokenProvider, SystemTokenProvider>();
            
            services.AddSingleton<IDataProtectionService, DataProtectionService>();
            services.AddScoped<ITenantContext, TenantContext>();

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


            services.AddScoped<IWorkContext, WorkContext>();

            services.AddScoped<IResilienceService, ResilienceService>();
        }
    }
}
