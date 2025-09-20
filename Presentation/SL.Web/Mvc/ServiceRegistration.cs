using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using SL.Application.Utilities;
using SL.Domain.Defaults;
using SL.Domain.Enums;
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
                options.AddPolicy(PolicyNames.REQUIRE_USER_ROLE, policy =>
                policy.RequireRole(AppRoleEnum.User.ToString(), AppRoleEnum.Admin.ToString()));

                options.AddPolicy(PolicyNames.REQUIRE_ADMIN_ROLE, policy =>
                    policy.RequireRole(AppRoleEnum.Admin.ToString()));
            });
        }
    }
}

