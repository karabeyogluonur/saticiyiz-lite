using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Application.Interfaces.Services;
using SL.Application.Models.ViewModels.Account;
using SL.Domain.Entities;
using SL.Domain.Enums;
using SL.Persistence.Contexts;

namespace SL.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork<MasterDbContext> _unitOfWork;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork<MasterDbContext> unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<ApplicationUser> GetUserByTenantIdAsync(Guid tenantId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(user => user.TenantId == tenantId);
        }
    }
}

