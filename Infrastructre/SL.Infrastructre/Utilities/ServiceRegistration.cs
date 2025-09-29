using Microsoft.Extensions.DependencyInjection;
using SL.Application.Interfaces.Services.Messages;
using SL.Application.Interfaces.Services.Security;
using SL.Infrastructre.Services.Messages;
using SL.Infrastructre.Services.Security;
using SL.Infrastructure.Security;

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
        }
    }
}
