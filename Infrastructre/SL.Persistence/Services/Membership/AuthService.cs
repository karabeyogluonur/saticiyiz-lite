using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SL.Application.Interfaces.Services.Membership;
using SL.Domain;
using SL.Domain.Entities.Membership;
using SL.Domain.Enums.Common;

namespace SL.Persistence.Services.Membership
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly ILogger<AuthService> _logger;
        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AuthService> logger, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _logger = logger;
        }
        public async Task<Result> LoginAsync(string email, string password, bool rememberMe)
        {
            _logger.LogInformation("'{Email}' adresi için giriş denemesi başladı.", email);

            var userId = await _userService.GetUserIdByEmailAsync(email);
            if (userId is null)
            {
                _logger.LogWarning("Giriş başarısız. Sistemde bulunamayan e-posta: {Email}", email);
                return Result.Failure("Giriş bilgileri hatalı.", ErrorCode.AuthenticationFailed);
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user is null)
            {
                _logger.LogError("Kritik Hata: ID'si ({UserId}) bulunan kullanıcının nesnesi Master veritabanında bulunamadı.", userId.Value);
                return Result.Failure("Sistemsel bir hata oluştu. Lütfen yönetici ile iletişime geçin.", ErrorCode.InternalServerError);
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("Giriş başarılı. Kullanıcı: {Email}, TenantId: {TenantId}", user.Email, user.TenantId);
                return Result.Success();
            }

            if (result.IsLockedOut)
            {
                string lockoutMessage = $"Hesabınız kilitlenmiştir. Lütfen {user.LockoutEnd?.ToLocalTime():g} zamanından sonra tekrar deneyin.";
                _logger.LogWarning("Hesap kilitlendi: {Email}", user.Email);
                return Result.Failure(lockoutMessage, ErrorCode.AccountLocked);
            }

            if (result.IsNotAllowed)
            {
                _logger.LogWarning("Giriş engellendi (IsNotAllowed): {Email}", user.Email);
                return Result.Failure("Bu hesapla giriş yapmaya izniniz bulunmamaktadır.", ErrorCode.AuthenticationFailed);
            }

            _logger.LogWarning("Hatalı şifre denemesi: {Email}", user.Email);
            return Result.Failure("Giriş bilgileri hatalı.", ErrorCode.AuthenticationFailed);
        }
        public async Task<Result> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı çıkış yaparken beklenmeyen bir hata oluştu.");
                return Result.Failure("Çıkış işlemi sırasında sunucu hatası oluştu.", ErrorCode.InternalServerError);
            }
        }
    }
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AppClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor) { }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            if (user.TenantId != Guid.Empty)
            {
                identity.AddClaim(new Claim("TenantId", user.TenantId.ToString()));
            }
            return identity;
        }
    }
}
