namespace SL.Application.Interfaces.Services.Context;






public interface ITenantContext
{



    Guid TenantId { get; }




    string? ConnectionString { get; }




    bool IsTenantSet { get; }




    void SetTenant(Guid tenantId, string connectionString);
}