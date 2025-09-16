using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SL.Application.Interfaces.Services;
using SL.Domain.Entities;
using SL.Persistence.Contexts;
using SL.Persistence.Services;

namespace SL.Persistence.Utilities
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceService(this IServiceCollection services)
        {
            services.AddDbContext<MasterDbContext>(options =>options.UseNpgsql(Configuration.MasterConnectionString)).AddUnitOfWork<MasterDbContext>();
            services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<MasterDbContext>();

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ITenantDatabaseService, TenantDatabaseService>();

            
        }
    }
}

