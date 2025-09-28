using System;
using Microsoft.Extensions.DependencyInjection;
using SL.Application.Interfaces.Services;
using SL.Infrastructre.Services;
namespace SL.Infrastructre.Utilities
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructreService(this IServiceCollection services)
        {
            services.AddSingleton<IPasswordHasherService, BCryptPasswordHasherService>();
        }
    }
}
