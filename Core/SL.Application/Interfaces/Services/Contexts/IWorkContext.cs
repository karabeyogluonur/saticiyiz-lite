using System.Threading.Tasks;
// User ve Tenant entity/DTO sınıflarınızın namespace'i
using SL.Domain.Entities.Membership;

namespace SL.Application.Interfaces.Services.Context;

public interface IWorkContext
{
    /// <summary>
    /// Mevcut kimliği doğrulanmış kullanıcıyı getirir.
    /// Veri, istek boyunca yalnızca bir kez yüklenir.
    /// </summary>
    Task<ApplicationUser> GetCurrentUserAsync();

    /// <summary>
    /// Mevcut kullanıcının ait olduğu tenant'ı getirir.
    /// Veri, istek boyunca yalnızca bir kez yüklenir.
    /// </summary>
    Task<Tenant> GetCurrentTenantAsync();
}