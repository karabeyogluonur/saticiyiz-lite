using System;
using SL.Application.Models.DTOs.Tenant;
using SL.Application.Models.ViewModels.Account;
using SL.Domain.Entities;
using SL.Domain.Enums;
namespace SL.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> CreateUserAsync(RegisterViewModel registerViewModel, Guid tenantId, AppRole appRole);
        Task<ApplicationUser> GetUserByIdAsync(Guid userId);
        Task<ApplicationUser> GetUserByTenantIdAsync(Guid tenantId);
        Task<ApplicationUser?> FindUserByEmailAsync(string email);
        Task DeleteUserByTenantIdAsync(Guid tenantId);
    }
}
