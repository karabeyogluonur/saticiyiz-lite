using Microsoft.AspNetCore.Http;
using SL.Application.Interfaces.Services.Caching;
using SL.Application.Interfaces.Services.Context;
using SL.Application.Interfaces.Services.Membership;
using SL.Domain.Entities.Membership; // ApplicationUser ve Tenant entity'lerinizin namespace'i
using System.Security.Claims;

namespace SL.Infrastructure.Services.Context;






public class WorkContext : IWorkContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheManager _cacheManager;
    private readonly ICacheKeyService _cacheKeyService;
    private readonly IUserService _userService;
    private readonly ITenantService _tenantService;



    private ApplicationUser _cachedUser;
    private Tenant _cachedTenant;

    public WorkContext(
        IHttpContextAccessor httpContextAccessor,
        ICacheManager cacheManager,
        ICacheKeyService cacheKeyService,
        IUserService userService,
        ITenantService tenantService)
    {
        _httpContextAccessor = httpContextAccessor;
        _cacheManager = cacheManager;
        _cacheKeyService = cacheKeyService;
        _userService = userService;
        _tenantService = tenantService;
    }





    public async Task<ApplicationUser> GetCurrentUserAsync()
    {

        if (_cachedUser != null)
        {
            return _cachedUser;
        }

        var userClaimsPrincipal = _httpContextAccessor.HttpContext?.User;
        if (userClaimsPrincipal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userIdClaim = userClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        if (!Guid.TryParse(userIdClaim, out var userId) || userId == Guid.Empty)
        {
            return null;
        }


        var cacheKey = _cacheKeyService.PrepareKeyFor<ApplicationUser>(userId);

        var user = await _cacheManager.GetAsync(cacheKey, async () =>
        {

            return await _userService.GetUserByIdAsync(userId);
        });


        _cachedUser = user;
        return _cachedUser;
    }





    public async Task<Tenant> GetCurrentTenantAsync()
    {

        if (_cachedTenant != null)
        {
            return _cachedTenant;
        }

        var userClaimsPrincipal = _httpContextAccessor.HttpContext?.User;
        if (userClaimsPrincipal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }


        var tenantIdClaim = userClaimsPrincipal.FindFirst("TenantId")?.Value;


        if (!Guid.TryParse(tenantIdClaim, out var tenantId) || tenantId == Guid.Empty)
        {
            return null;
        }


        var cacheKey = _cacheKeyService.PrepareKeyFor<Tenant>(tenantId);

        var tenant = await _cacheManager.GetAsync(cacheKey, async () =>
        {

            return await _tenantService.GetTenantByIdAsync(tenantId);
        });


        _cachedTenant = tenant;
        return _cachedTenant;
    }
}