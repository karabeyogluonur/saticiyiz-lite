namespace SL.Application.Interfaces.Services.Context;

/// <summary>
/// Mevcut HTTP isteği boyunca tenant'a özel bilgileri (özellikle connection string) taşımak için
/// kullanılan, request-scoped (istek bazlı) bir servis sözleşmesi.
/// "Kurye Çantası" benzetmesiyle, middleware'in bulduğu bilgiyi DbContext'in kullanacağı ana kadar taşır.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Mevcut isteğin tenant kimliği.
    /// </summary>
    Guid TenantId { get; }

    /// <summary>
    /// Mevcut tenant'ın veritabanı bağlantı dizesi.
    /// </summary>
    string? ConnectionString { get; }

    /// <summary>
    /// Tenant bilgisinin bu istek için set edilip edilmediğini belirtir.
    /// </summary>
    bool IsTenantSet { get; }

    /// <summary>
    /// Middleware tarafından çağrılarak o anki isteğin tenant bilgilerini setler.
    /// </summary>
    void SetTenant(Guid tenantId, string connectionString);
}