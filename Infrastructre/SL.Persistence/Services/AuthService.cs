using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SL.Application.Interfaces.Services;
using SL.Application.Models.ViewModels.Account;
using SL.Domain;
using SL.Domain.Entities;
using SL.Persistence.Contexts;
using Microsoft.Extensions.Options;
using AutoMapper;
using SL.Domain.Enums;

namespace SL.Persistence.Services
{
    public class AuthService : IAuthService
	{
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public AuthService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager, IMapper mapper)
		{
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _mapper = mapper;
		}

        public async Task<Result<string>> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Result<string>.Failure("Geçersiz e-posta veya şifre.");

            var result = await _signInManager.PasswordSignInAsync(
                user,
                password,
                rememberMe,
                lockoutOnFailure: true
            );

            if (!result.Succeeded)
                return Result<string>.Failure("Geçersiz e-posta veya şifre.");

            return Result<string>.Success(user.TenantDatabaseName ?? string.Empty);
        }

        public async Task<Result<bool>> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Çıkış yapılırken hata oluştu: {ex.Message}");
            }
        }

        public async Task RegisterAsync(RegisterViewModel registerViewModel, string tenantDatabaseName)
        {
            ApplicationUser applicationUser = _mapper.Map<ApplicationUser>(registerViewModel);
            applicationUser.TenantDatabaseName = tenantDatabaseName;

            await _userManager.CreateAsync(applicationUser, registerViewModel.Password);
            await _userManager.AddToRoleAsync(applicationUser, AppRole.User.ToString());
        }
    }

    public class AppClaimsPrincipalFactory: UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AppClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor) { }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (!string.IsNullOrEmpty(user.TenantDatabaseName))
            {
                identity.AddClaim(new Claim("TenantDatabaseName", user.TenantDatabaseName));
            }

            return identity;
        }
    }


}

