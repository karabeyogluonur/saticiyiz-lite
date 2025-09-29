using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SL.Application.Interfaces.Services.Membership;
using SL.Application.Interfaces.Services.Security;
using SL.Application.Models.DTOs.Tenant;
using SL.Application.Models.ViewModels.Account;
using SL.Domain.Defaults.Messages;
using SL.Domain.Entities.Membership;
using SL.Domain.Entities.Messages;
using SL.Domain.Enums.Membership;
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
            await SeedDefaultEmailAccountAsync(scope);
            await SeedEmailTemplatesAsync(scope);
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
        public static async Task SeedDefaultEmailAccountAsync(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<MasterDbContext>();
            var dataProtectionService = serviceScope.ServiceProvider.GetRequiredService<IDataProtectionService>();
            string defaultEmail = "noreply@saticiyiz.com";
            if (await context.EmailAccounts.AnyAsync(e => e.Email == defaultEmail))
                return;
            var defaultAccount = new EmailAccount
            {
                Id = Guid.NewGuid(),
                DisplayName = "Sistem Bildirim Hesabı",
                Email = defaultEmail,
                Username = defaultEmail,
                Password = dataProtectionService.Encrypt("@default@password"),
                Host = "smtp.saticiyiz.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            await context.EmailAccounts.AddAsync(defaultAccount);
            await context.SaveChangesAsync();
            Console.WriteLine($"Default Email Account created: {defaultEmail}");
        }
        public static async Task SeedEmailTemplatesAsync(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<MasterDbContext>();

            var systemName = MessageTemplateSystemName.CustomerWelcome;

            if (await context.EmailTemplates.AnyAsync(t => t.SystemName == systemName))
                return;

            EmailAccount defaultEmailAccount = await context.EmailAccounts.FirstOrDefaultAsync(e => e.Email == "noreply@saticiyiz.com");

            if (defaultEmailAccount == null)
            {
                Console.WriteLine($"Warning: Default sender email 'noreply@sirket.com' not found. Skipping seed for '{systemName}'.");
                return;
            }

            EmailTemplate newTemplate = new EmailTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Müşteri Hoş Geldin Mesajı",
                SystemName = systemName,
                Subject = "Hoşgeldin! | Satıcıyız",
                Body = @"
            <!DOCTYPE html>
            <html lang=""tr"">
            <head>
                <meta charset=""UTF-8"">
                <style>
                    body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                    .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }
                    .header { font-size: 24px; font-weight: bold; color: #444; margin-bottom: 20px; }
                    .footer { margin-top: 20px; font-size: 12px; color: #888; text-align: center; }
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""header""> aramıza Hoş Geldiniz!</div>
                    <p>Merhaba {{ApplicationUser.FullName}},</p>
                    <p> aramıza katıldığınız için çok mutluyuz. Hesabınız başarıyla oluşturuldu. Aşağıdaki linki kullanarak platformumuza giriş yapabilir ve tüm özelliklerimizi keşfetmeye başlayabilirsiniz.</p>
                    <p style=""text-align: center; margin: 30px 0;"">
                        <a href=""{{LoginLink}}"" style=""background-color: #007bff; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;"">Hesabınıza Giriş Yapın</a>
                    </p>
                    <p>İyi günler dileriz,<br/><strong>{{Store.Name}} Ekibi</strong></p>
                    <div class=""footer"">
                        Bu e-posta, saticiyiz.com'a üye olduğunuz için otomatik olarak gönderilmiştir.
                    </div>
                </div>
            </body>
            </html>
        ",
                BccEmailAddresses = null,
                IsActive = true,
                EmailAccountId = defaultEmailAccount.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await context.EmailTemplates.AddAsync(newTemplate);
            await context.SaveChangesAsync();

            Console.WriteLine($"Email Template seeded: {systemName}");
        }
    }
}
