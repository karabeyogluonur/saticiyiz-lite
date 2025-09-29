using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SL.Application.Interfaces.Services.Context;
using SL.Application.Interfaces.Services.Membership;
using SL.Application.Interfaces.Services.Messages;
using SL.Application.Interfaces.Services.Tenants;
using SL.Domain.Entities.Membership;
using SL.Infrastructure.Services.Membership;
using SL.Persistence.Contexts;
using SL.Persistence.Services.Membership;
using SL.Persistence.Services.Messages;

namespace SL.Persistence.Utilities
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceService(this IServiceCollection services)
        {
            services.AddDbContext<MasterDbContext>(options => options.UseNpgsql(Configuration.MasterConnectionString)).AddUnitOfWork<MasterDbContext>();
            services.AddDbContext<TenantDbContext>((serviceProvider, options) =>
            {
                var tenantContext = serviceProvider.GetRequiredService<ITenantContext>();
                var connectionString = tenantContext.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                    return;

                options.UseNpgsql(connectionString);
            }).AddUnitOfWork<TenantDbContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<MasterDbContext>()
            .AddDefaultTokenProviders();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ITenantService, TenantService>();
            services.AddTransient<IRegistrationWorkflowService, RegistrationWorkflowService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEmailAccountService, EmailAccountService>();
            services.AddTransient<IEmailTemplateService, EmailTemplateService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IQueuedEmailService, QueuedEmailService>();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();
            services.AddScoped<ITenantStore, TenantStore>();
        }
    }
}
