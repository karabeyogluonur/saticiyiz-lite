using SL.Application.Models.DTOs.Tenant; // Bu DTO'yu da oluşturmanız gerekecek.

namespace SL.Application.Interfaces.Services.Tenants;





public interface ITenantStore
{






    Task<TenantConfigurationModel> GetConfigurationByIdAsync(Guid tenantId);
}