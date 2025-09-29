using SL.Application.Models.DTOs.Tenant; // Bu DTO'yu da oluşturmanız gerekecek.

namespace SL.Application.Interfaces.Services.Tenants;

/// <summary>
/// Tenant'ların konfigürasyon bilgilerini (connection string gibi) getirmekten sorumlu servis sözleşmesi.
/// "Adres Defteri" benzetmesiyle, verilen bir kimliğe ait adresi bulur.
/// </summary>
public interface ITenantStore
{
    /// <summary>
    /// Verilen tenant kimliğine ait konfigürasyon bilgilerini getirir.
    /// Performans için bu bilgiler cache'lenir.
    /// </summary>
    /// <param name="tenantId">Tenant kimliği</param>
    /// <returns>Tenant'a ait konfigürasyon DTO'su veya bulunamazsa null.</returns>
    Task<TenantConfigurationModel> GetConfigurationByIdAsync(Guid tenantId);
}