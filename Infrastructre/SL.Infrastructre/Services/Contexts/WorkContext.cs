using Microsoft.AspNetCore.Http;
using SL.Application.Interfaces.Services.Caching;
using SL.Application.Interfaces.Services.Context;
using SL.Application.Interfaces.Services.Membership;
using SL.Domain.Entities.Membership; // ApplicationUser ve Tenant entity'lerinizin namespace'i
using System.Security.Claims;

namespace SL.Infrastructure.Services.Context;

/// <summary>
/// Mevcut HTTP isteği boyunca geçerli olan kullanıcı ve tenant gibi bağlamsal bilgileri yönetir.
/// Verileri lazy-loading (ihtiyaç anında yükleme) prensibiyle ve verimli bir cache mekanizmasıyla getirir.
/// Bu servisin DI container'a "Scoped" olarak kaydedilmesi ZORUNLUDUR.
/// </summary>
public class WorkContext : IWorkContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheManager _cacheManager;
    private readonly ICacheKeyService _cacheKeyService;
    private readonly IUserService _userService;
    private readonly ITenantService _tenantService;

    // Her bir HTTP isteği için bu nesne yeniden oluşturulacağından,
    // bu field'lar istek bazlı bir cache görevi görür.
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

    /// <summary>
    /// Mevcut kimliği doğrulanmış kullanıcıyı getirir. Veri, istek boyunca yalnızca bir kez yüklenir.
    /// Sonraki çağrılar, istek içi cache'den döner.
    /// </summary>
    public async Task<ApplicationUser> GetCurrentUserAsync()
    {
        // 1. İstek içi cache'i kontrol et. (En hızlı yol)
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

        // Claim'den ID'yi al ve doğru tipe parse et. (ID tipiniz int değilse burayı değiştirin)
        if (!Guid.TryParse(userIdClaim, out var userId) || userId == Guid.Empty)
        {
            return null;
        }

        // 2. ICacheManager kullanarak L1/L2 cache'i kontrol et.
        var cacheKey = _cacheKeyService.PrepareKeyFor<ApplicationUser>(userId);

        var user = await _cacheManager.GetAsync(cacheKey, async () =>
        {
            // 3. Cache'de yoksa, veritabanından al (sadece bu lambda çalışır).
            return await _userService.GetUserByIdAsync(userId);
        });

        // Sonucu istek içi cache'e ata ve döndür.
        _cachedUser = user;
        return _cachedUser;
    }

    /// <summary>
    /// Mevcut kullanıcının ait olduğu tenant'ı getirir. Veri, istek boyunca yalnızca bir kez yüklenir.
    /// Sonraki çağrılar, istek içi cache'den döner.
    /// </summary>
    public async Task<Tenant> GetCurrentTenantAsync()
    {
        // 1. İstek içi cache'i kontrol et.
        if (_cachedTenant != null)
        {
            return _cachedTenant;
        }

        var userClaimsPrincipal = _httpContextAccessor.HttpContext?.User;
        if (userClaimsPrincipal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        // "TenantId" ismindeki özel claim'i ara.
        var tenantIdClaim = userClaimsPrincipal.FindFirst("TenantId")?.Value;

        // Claim'den ID'yi al ve doğru tipe parse et.
        if (!Guid.TryParse(tenantIdClaim, out var tenantId) || tenantId == Guid.Empty)
        {
            return null;
        }

        // 2. ICacheManager kullanarak L1/L2 cache'i kontrol et.
        var cacheKey = _cacheKeyService.PrepareKeyFor<Tenant>(tenantId);

        var tenant = await _cacheManager.GetAsync(cacheKey, async () =>
        {
            // 3. Cache'de yoksa, veritabanından al.
            return await _tenantService.GetTenantByIdAsync(tenantId);
        });

        // Sonucu istek içi cache'e ata ve döndür.
        _cachedTenant = tenant;
        return _cachedTenant;
    }
}