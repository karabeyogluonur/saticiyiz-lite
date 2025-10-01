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
            EmailAccount defaultEmailAccount = await context.EmailAccounts.FirstOrDefaultAsync(e => e.Email == "noreply@saticiyiz.com");

            if (defaultEmailAccount == null)
            {
                Console.WriteLine("Warning: Default sender email 'noreply@saticiyiz.com' not found. Skipping email template seed.");
                return;
            }

            var templates = GetEmailTemplates(defaultEmailAccount.Id);

            foreach (var template in templates)
            {
                if (await context.EmailTemplates.AnyAsync(t => t.SystemName == template.SystemName))
                {
                    Console.WriteLine($"Email Template already exists: {template.SystemName}");
                    continue;
                }

                await context.EmailTemplates.AddAsync(template);
                Console.WriteLine($"Email Template seeded: {template.SystemName}");
            }

            await context.SaveChangesAsync();
        }
        private static List<EmailTemplate> GetEmailTemplates(Guid emailAccountId)
        {

            return new List<EmailTemplate>
            {
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Müşteri Hoş Geldin Mesajı",
                    SystemName = MessageTemplateSystemName.CustomerWelcome,
                    Subject = "Hoşgeldin! | Satıcıyız",
                    Body = GetWelcomeEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Şifre Sıfırlama",
                    SystemName = MessageTemplateSystemName.CustomerPasswordRecovery,
                    Subject = "Şifre Sıfırlama | Satıcıyız",
                    Body = GetPasswordRecoveryEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Sipariş Onayı",
                    SystemName = MessageTemplateSystemName.OrderConfirmation,
                    Subject = "Sipariş Onayı | Satıcıyız",
                    Body = GetOrderConfirmationEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Fatura Kesildi",
                    SystemName = MessageTemplateSystemName.InvoiceIssued,
                    Subject = "Fatura Kesildi | Satıcıyız",
                    Body = GetInvoiceIssuedEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Ödeme Başarısız",
                    SystemName = MessageTemplateSystemName.PaymentFailed,
                    Subject = "Ödeme Başarısız | Satıcıyız",
                    Body = GetPaymentFailedEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Abonelik Yenileme Hatırlatması",
                    SystemName = MessageTemplateSystemName.SubscriptionRenewalReminder,
                    Subject = "Abonelik Yenileme Hatırlatması | Satıcıyız",
                    Body = GetSubscriptionRenewalReminderEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Abonelik Süresi Doldu",
                    SystemName = MessageTemplateSystemName.SubscriptionExpired,
                    Subject = "Abonelik Süresi Doldu | Satıcıyız",
                    Body = GetSubscriptionExpiredEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Bülten Kampanyası",
                    SystemName = MessageTemplateSystemName.Newsletter,
                    Subject = "Bülten Kampanyası | Satıcıyız",
                    Body = GetNewsletterEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Müşteri Aktivasyonu",
                    SystemName = MessageTemplateSystemName.CustomerActivation,
                    Subject = "Hesap Aktivasyonu | Satıcıyız",
                    Body = GetCustomerActivationEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Hesap Değişiklik Bildirimi",
                    SystemName = MessageTemplateSystemName.AccountChangeNotification,
                    Subject = "Hesap Değişiklik Bildirimi | Satıcıyız",
                    Body = GetAccountChangeNotificationEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Sistem Duyurusu",
                    SystemName = MessageTemplateSystemName.SystemAnnouncement,
                    Subject = "Sistem Duyurusu | Satıcıyız",
                    Body = GetSystemAnnouncementEmailBody(),
                    IsActive = true,
                    EmailAccountId = emailAccountId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }
            };
        }

        private static string GetWelcomeEmailBody()
        {
            return @"
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
        <div class=""header"">Aramıza Hoş Geldiniz!</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>Aramıza katıldığınız için çok mutluyuz. Hesabınız başarıyla oluşturuldu. Aşağıdaki linki kullanarak platformumuza giriş yapabilir ve tüm özelliklerimizi keşfetmeye başlayabilirsiniz.</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{{System.Url}}/Account/Login"" style=""background-color: #007bff; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;"">Hesabınıza Giriş Yapın</a>
        </p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, saticiyiz.com'a üye olduğunuz için otomatik olarak gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }

        private static string GetPasswordRecoveryEmailBody()
        {
            return @"
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
        <div class=""header"">Şifre Sıfırlama</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>Hesabınız için şifre sıfırlama talebinde bulundunuz. Aşağıdaki linki kullanarak yeni şifrenizi oluşturabilirsiniz.</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{{PasswordResetLink}}"" style=""background-color: #dc3545; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;"">Şifremi Sıfırla</a>
        </p>
        <p><strong>Önemli:</strong> Bu link 24 saat geçerlidir. Güvenliğiniz için bu linki kimseyle paylaşmayın.</p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, şifre sıfırlama talebinde bulunduğunuz için otomatik olarak gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }

        private static string GetOrderConfirmationEmailBody()
        {
            return @"
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
        <div class=""header"">Sipariş Onayı</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>Siparişiniz başarıyla alınmıştır. Sipariş detayları:</p>
        <p><strong>Sipariş No:</strong> {{Order.OrderNumber}}</p>
        <p><strong>Sipariş Tarihi:</strong> {{Order.CreatedAt}}</p>
        <p><strong>Toplam Tutar:</strong> {{Order.TotalAmount}} TL</p>
        <p>Siparişinizin durumunu takip etmek için hesabınıza giriş yapabilirsiniz.</p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, siparişiniz onaylandığı için otomatik olarak gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }

        private static string GetInvoiceIssuedEmailBody()
        {
            return @"
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
        <div class=""header"">Fatura Kesildi</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>Siparişiniz için fatura kesilmiştir. Fatura detayları:</p>
        <p><strong>Fatura No:</strong> {{Invoice.InvoiceNumber}}</p>
        <p><strong>Fatura Tarihi:</strong> {{Invoice.IssueDate}}</p>
        <p><strong>Toplam Tutar:</strong> {{Invoice.TotalAmount}} TL</p>
        <p>Faturanızı indirmek için hesabınıza giriş yapabilirsiniz.</p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, faturanız kesildiği için otomatik olarak gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }

        private static string GetPaymentFailedEmailBody()
        {
            return @"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }
        .header { font-size: 24px; font-weight: bold; color: #dc3545; margin-bottom: 20px; }
        .footer { margin-top: 20px; font-size: 12px; color: #888; text-align: center; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">Ödeme Başarısız</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>Üzgünüz, ödemeniz işlenirken bir sorun oluştu. Lütfen ödeme bilgilerinizi kontrol ederek tekrar deneyin.</p>
        <p><strong>Sipariş No:</strong> {{Order.OrderNumber}}</p>
        <p><strong>Hata Mesajı:</strong> {{Payment.ErrorMessage}}</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{{PaymentRetryLink}}"" style=""background-color: #dc3545; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;"">Ödemeyi Tekrar Dene</a>
        </p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, ödemeniz başarısız olduğu için otomatik olarak gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }

        private static string GetSubscriptionRenewalReminderEmailBody()
        {
            return @"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }
        .header { font-size: 24px; font-weight: bold; color: #ffc107; margin-bottom: 20px; }
        .footer { margin-top: 20px; font-size: 12px; color: #888; text-align: center; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">Abonelik Yenileme Hatırlatması</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>Aboneliğinizin süresi yakında dolacak. Kesintisiz hizmet alabilmek için aboneliğinizi yenilemenizi öneririz.</p>
        <p><strong>Abonelik Türü:</strong> {{Subscription.PlanName}}</p>
        <p><strong>Bitiş Tarihi:</strong> {{Subscription.EndDate}}</p>
        <p><strong>Yenileme Ücreti:</strong> {{Subscription.RenewalPrice}} TL</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{{SubscriptionRenewalLink}}"" style=""background-color: #28a745; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;"">Aboneliği Yenile</a>
        </p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, aboneliğinizin süresi dolmadan önce hatırlatma amaçlı gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }

        private static string GetSubscriptionExpiredEmailBody()
        {
            return @"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }
        .header { font-size: 24px; font-weight: bold; color: #dc3545; margin-bottom: 20px; }
        .footer { margin-top: 20px; font-size: 12px; color: #888; text-align: center; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">Abonelik Süresi Doldu</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>Aboneliğinizin süresi dolmuştur. Hizmetlerimizden yararlanmaya devam etmek için aboneliğinizi yenilemeniz gerekmektedir.</p>
        <p><strong>Abonelik Türü:</strong> {{Subscription.PlanName}}</p>
        <p><strong>Bitiş Tarihi:</strong> {{Subscription.EndDate}}</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{{SubscriptionRenewalLink}}"" style=""background-color: #dc3545; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;"">Aboneliği Yenile</a>
        </p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, aboneliğinizin süresi dolduğu için otomatik olarak gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }

        private static string GetNewsletterEmailBody()
        {
            return @"
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
        <div class=""header"">Bülten Kampanyası</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>{{Newsletter.Title}}</p>
        <p>{{Newsletter.Content}}</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{{Newsletter.Link}}"" style=""background-color: #007bff; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;"">Detayları Görüntüle</a>
        </p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, bülten kampanyamız kapsamında gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }

        private static string GetCustomerActivationEmailBody()
        {
            return @"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }
        .header { font-size: 24px; font-weight: bold; color: #28a745; margin-bottom: 20px; }
        .footer { margin-top: 20px; font-size: 12px; color: #888; text-align: center; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">Hesap Aktivasyonu</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>Hesabınız başarıyla aktive edilmiştir. Artık tüm hizmetlerimizden yararlanabilirsiniz.</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{{System.Url}}/Account/Login"" style=""background-color: #28a745; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;"">Hesabınıza Giriş Yapın</a>
        </p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, hesabınız aktive edildiği için otomatik olarak gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }

        private static string GetAccountChangeNotificationEmailBody()
        {
            return @"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }
        .header { font-size: 24px; font-weight: bold; color: #ffc107; margin-bottom: 20px; }
        .footer { margin-top: 20px; font-size: 12px; color: #888; text-align: center; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">Hesap Değişiklik Bildirimi</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>Hesabınızda bir değişiklik yapılmıştır. Değişiklik detayları:</p>
        <p><strong>Değişiklik Türü:</strong> {{AccountChange.ChangeType}}</p>
        <p><strong>Değişiklik Tarihi:</strong> {{AccountChange.ChangeDate}}</p>
        <p><strong>IP Adresi:</strong> {{AccountChange.IPAddress}}</p>
        <p>Bu değişikliği siz yapmadıysanız, lütfen hemen bizimle iletişime geçin.</p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, hesabınızda değişiklik yapıldığı için otomatik olarak gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }

        private static string GetSystemAnnouncementEmailBody()
        {
            return @"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }
        .header { font-size: 24px; font-weight: bold; color: #6f42c1; margin-bottom: 20px; }
        .footer { margin-top: 20px; font-size: 12px; color: #888; text-align: center; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">Sistem Duyurusu</div>
        <p>Merhaba {{User.FullName}},</p>
        <p>{{Announcement.Title}}</p>
        <p>{{Announcement.Content}}</p>
        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{{Announcement.Link}}"" style=""background-color: #6f42c1; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;"">Detayları Görüntüle</a>
        </p>
        <p>İyi günler dileriz,<br/><strong>{{System.Name}} Ekibi</strong></p>
        <div class=""footer"">
            Bu e-posta, sistem duyurusu kapsamında gönderilmiştir.
        </div>
    </div>
</body>
</html>";
        }
    }
}
