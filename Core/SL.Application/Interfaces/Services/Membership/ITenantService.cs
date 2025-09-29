using SL.Application.Models.DTOs.Tenant;
using SL.Domain.Entities.Membership;

namespace SL.Application.Interfaces.Services.Membership
{
    public interface ITenantService
    {
        Task CreateDatabaseAsync(Tenant tenant);
        Task<Tenant> InsertTenantAsync(TenantCreateModel tenantCreateModel);
        Task<Tenant> GetTenantByIdAsync(Guid tenantId);
        Task DeleteTenantAsync(Guid tenantId);
    }
}
