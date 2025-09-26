using System;
using SL.Application.Models.DTOs.Tenant;
using SL.Domain.Entities;
namespace SL.Application.Interfaces.Services
{
    public interface ITenantService
    {
        Task CreateDatabaseAsync(Tenant tenant);
        Task<Tenant> InsertTenantAsync(TenantCreateModel tenantCreateModel);
        Task DeleteTenantAsync(Guid tenantId);
        Task SetTenantContext(Guid tenantId);
    }
}

