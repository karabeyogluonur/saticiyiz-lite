using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SL.Application.Interfaces.Services;
using SL.Application.Models.DTOs.Tenant;
using SL.Application.Models.ViewModels.Account;
using SL.Domain.Entities;
using SL.Domain.Enums;
using SL.Persistence.Contexts;

namespace SL.Persistence.Seeds
{
    public static class DbInitializer
    {
        public async static Task InitializeAsync(IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<MasterDbContext>();

            if (context == null)
                throw new ArgumentNullException(nameof(context));


            await context.Database.MigrateAsync();
            await SeedRolesAsync(scope);
            await SeedDefaultAdminAsync(scope);
        }

        public static async Task SeedRolesAsync(IServiceScope serviceScope)
        {
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in Enum.GetNames(typeof(AppRole)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    Console.WriteLine($"Role created: {roleName}");
                }
            }
        }

        public static async Task SeedDefaultAdminAsync(IServiceScope serviceScope)
        {
            IUserService userService = serviceScope.ServiceProvider.GetRequiredService<IUserService>();
            ITenantService tenantService = serviceScope.ServiceProvider.GetRequiredService<ITenantService>();

            string defaultAdminEmail = "root@root.com";
            string defaultAdminPassword = "Nbr169++";

            ApplicationUser? existingAdmin = await userService.FindUserByEmailAsync(defaultAdminEmail);

            if (existingAdmin != null) return;

            string tenantDatabaseName = $"SL_{Guid.NewGuid():N}";

            TenantCreateModel tenantCreateModel = new TenantCreateModel
            {
                DatabaseName = tenantDatabaseName,
            };

            Tenant rootTenant = await tenantService.InsertTenantAsync(tenantCreateModel);

            RegisterViewModel adminRegisterModel = new RegisterViewModel
            {
                Email = defaultAdminEmail,
                Password = defaultAdminPassword,
                FirstName = "Onur",
                LastName = "Karabeyoğlu"
            };

            ApplicationUser adminUser = await userService.CreateUserAsync(
                adminRegisterModel,
                rootTenant.Id,
                AppRole.Admin);

            await tenantService.CreateDatabaseAsync(rootTenant);

            Console.WriteLine($"Default Admin created: {defaultAdminEmail}");
        }

    }
}


