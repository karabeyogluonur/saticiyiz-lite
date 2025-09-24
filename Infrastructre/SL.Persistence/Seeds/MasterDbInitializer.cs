using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SL.Application.Interfaces.Services;
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
            UserManager<ApplicationUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var tenantDatabaseService = serviceScope.ServiceProvider.GetRequiredService<ITenantDatabaseService>();

            string defaultAdminEmail = "root@root.com";
            string defaultAdminPassword = "Nbr169++";

            ApplicationUser? existingAdmin = await userManager.FindByEmailAsync(defaultAdminEmail);

            if (existingAdmin != null) return;

            string tenantDatabaseName = $"SL_{Guid.NewGuid():N}";

            ApplicationUser adminUser = new ApplicationUser { UserName = defaultAdminEmail, Email = defaultAdminEmail, FirstName = "Root", LastName = "Root", TenantDatabaseName = tenantDatabaseName };

            await userManager.CreateAsync(adminUser, defaultAdminPassword);
            await userManager.AddToRoleAsync(adminUser, AppRole.Admin.ToString());
            await tenantDatabaseService.CreateDatabaseAsync(tenantDatabaseName);

        }

    }
}


