using System.Threading.Tasks;

using SL.Domain.Entities.Membership;

namespace SL.Application.Interfaces.Services.Context;

public interface IWorkContext
{




    Task<ApplicationUser> GetCurrentUserAsync();





    Task<Tenant> GetCurrentTenantAsync();
}