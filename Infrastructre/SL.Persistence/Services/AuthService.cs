using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SL.Application.Interfaces.Services;
using SL.Domain;
using SL.Domain.Entities;
using SL.Domain.Enums;


namespace SL.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<Result> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Result.Failure("Giriş bilgileri hatalı.", ErrorCode.AuthenticationFailed);

            //if (!user.EmailConfirmed)
            //return Result.Failure("E-posta adresinizi onaylamanız gerekmektedir.", ErrorCode.EmailNotConfirmed);

            var result = await _signInManager.PasswordSignInAsync(
                user,
                password,
                rememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
                return Result.Success();

            if (result.IsLockedOut)
            {
                string lockoutMessage = $"Hesabınız kilitlenmiştir. Lütfen {user.LockoutEnd?.ToLocalTime():g} zamanından sonra tekrar deneyin.";
                _logger.LogWarning("Hesap kilitli: {Email}", email);

                return Result.Failure(lockoutMessage, ErrorCode.AccountLocked);
            }

            if (result.IsNotAllowed)
            {
                _logger.LogWarning("Hesap, kısıtlamalar nedeniyle giriş yapamıyor: {Email}", email);
                return Result.Failure("Giriş yapmanıza izin verilmiyor. Lütfen yöneticinize başvurun.", ErrorCode.AuthenticationFailed);
            }

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
                // Çıkış yapamama, oturum yönetimi sorununu işaret eder.
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

