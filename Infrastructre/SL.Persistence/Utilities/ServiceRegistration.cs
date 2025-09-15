using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SL.Domain.Entities;
using SL.Persistence.Contexts;

namespace SL.Persistence.Utilities
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceService(this IServiceCollection services)
        {
            services.AddDbContext<MasterDbContext>(options =>options.UseNpgsql(Configuration.MasterConnectionString));
            services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<MasterDbContext>();
        }
    }
}

