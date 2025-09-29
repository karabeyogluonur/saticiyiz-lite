using Microsoft.AspNetCore.Authentication.Cookies;
using SL.Application.Utilities;
using SL.Domain.Defaults.Membership;
using SL.Domain.Enums.Membership;
using SL.Infrastructre.Utilities;
using SL.Persistence.Utilities;
namespace SL.Web.Mvc
{
    public static class ServiceRegistration
    {
        public static void AddBaseServices(this IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }
        public static void AddFactoryServices(this IServiceCollection services)
        {
            services.AddScoped<IQueuedEmailModelFactory, QueuedEmailModelFactory>();
        }
        public static void AddLayerServices(this IServiceCollection services)
        {
            services.AddApplicationService();
            services.AddPersistenceService();
            services.AddInfrastructreService();
        }
        public static void AddAuthServices(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyName.RequireUserRole, policy =>
                policy.RequireRole(AppRole.User.ToString(), AppRole.Admin.ToString()));
                options.AddPolicy(PolicyName.RequireAdminRole, policy =>
                    policy.RequireRole(AppRole.Admin.ToString()));
            });
        }
    }
}
