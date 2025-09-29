using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SL.Application.Defaults.Caching;
using SL.Application.Interfaces.Repositories;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Application.Interfaces.Services.Caching;
using SL.Application.Interfaces.Services.Membership;
using SL.Application.Models.ViewModels.Account;
using SL.Domain.Entities.Membership;
using SL.Domain.Enums.Membership;
using SL.Persistence.Contexts;

namespace SL.Persistence.Services.Membership
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork<MasterDbContext> _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cacheManager;
        public UserService(IUnitOfWork<MasterDbContext> unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, ICacheManager cacheManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheManager = cacheManager;
        }
        public async Task<ApplicationUser> CreateUserAsync(RegisterViewModel registerViewModel, Guid tenantId, AppRole appRole)
        {
            ApplicationUser applicationUser = _mapper.Map<ApplicationUser>(registerViewModel);
            applicationUser.TenantId = tenantId;
            await _userManager.CreateAsync(applicationUser, registerViewModel.Password);
            await _userManager.AddToRoleAsync(applicationUser, appRole.ToString());
            return applicationUser;
        }
        public async Task DeleteUserByTenantIdAsync(Guid tenantId)
        {
            var userToDelete = await _userManager.Users.FirstOrDefaultAsync(u => u.TenantId == tenantId);
            if (userToDelete is not null)
                await _userManager.DeleteAsync(userToDelete);
        }
        public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<ApplicationUser> GetUserByIdAsync(Guid userId)
        {
            var cacheKey = UserCacheDefaults.ByIdCacheKey(userId);

            return await _cacheManager.GetAsync(cacheKey, async () =>
            {
                return await _userManager.FindByIdAsync(userId.ToString());
            });
        }
        public async Task<ApplicationUser> GetUserByTenantIdAsync(Guid tenantId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(user => user.TenantId == tenantId);
        }

        public async Task<Guid?> GetUserIdByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var normalizedEmail = email.ToUpperInvariant();
            var cacheKey = UserCacheDefaults.IdByEmailCacheKey(normalizedEmail);

            return await _cacheManager.GetAsync(cacheKey, async () =>
            {
                var user = await _userManager.FindByEmailAsync(normalizedEmail);
                return Guid.Parse(user?.Id);
            });

        }
    }
}
