using SL.Application.Interfaces.Services.Context;

namespace SL.Infrastructure.Services.Context;





public class TenantContext : ITenantContext
{
    public Guid TenantId { get; private set; }
    public string? ConnectionString { get; private set; }

    public bool IsTenantSet => TenantId != Guid.Empty && !string.IsNullOrEmpty(ConnectionString);

    public void SetTenant(Guid tenantId, string connectionString)
    {
        TenantId = tenantId;
        ConnectionString = connectionString;
    }
}