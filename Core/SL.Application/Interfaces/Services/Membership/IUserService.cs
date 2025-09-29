using SL.Application.Models.ViewModels.Account;
using SL.Domain.Entities.Membership;
using SL.Domain.Enums.Membership;

namespace SL.Application.Interfaces.Services.Membership
{
    public interface IUserService
    {
        Task<ApplicationUser> CreateUserAsync(RegisterViewModel registerViewModel, Guid tenantId, AppRole appRole);
        Task<ApplicationUser> GetUserByIdAsync(Guid userId);
        Task<ApplicationUser> GetUserByTenantIdAsync(Guid tenantId);
        Task<ApplicationUser?> FindUserByEmailAsync(string email);
        Task<Guid?> GetUserIdByEmailAsync(string email);
        Task DeleteUserByTenantIdAsync(Guid tenantId);
    }
}
