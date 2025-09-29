using SL.Application.Defaults.Caching;
using SL.Application.Interfaces.Repositories;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Application.Interfaces.Services.Caching;
using SL.Application.Interfaces.Services.Tenants;
using SL.Application.Models.DTOs.Tenant;
using SL.Domain.Entities.Membership;
using SL.Persistence.Contexts;



namespace SL.Infrastructure.Services.Membership;

/// <summary>
/// ITenantStore arayüzünün, UnitOfWork ve Repository desenine uygun somut implementasyonu.
/// Tenant konfigürasyonunu önce cache'den arar, bulamazsa Master veritabanından okur ve cache'e ekler.
/// </summary>
public class TenantStore : ITenantStore
{
    private readonly ICacheManager _cacheManager;
    private readonly IRepository<Tenant> _tenantRepository;

    public TenantStore(ICacheManager cacheManager, IUnitOfWork<MasterDbContext> unitOfWork)
    {
        _cacheManager = cacheManager;
        _tenantRepository = unitOfWork.GetRepository<Tenant>();
    }

    public async Task<TenantConfigurationModel> GetConfigurationByIdAsync(Guid tenantId)
    {
        // Cache anahtarını standartlara uygun şekilde alıyoruz.
        var cacheKey = TenantCacheDefaults.ConfigurationByIdCacheKey(tenantId);
        cacheKey.CacheTimeInMinutes = 1440; // 24 saat

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            // Cache'de veri yoksa, Repository'yi kullanarak veritabanından tenant bilgisini oku.
            var tenant = await _tenantRepository.GetFirstOrDefaultAsync(predicate: t => t.Id == tenantId);

            // Eğer tenant bulunursa, onu DTO'ya map'leyerek döndür.
            if (tenant is null)
            {
                return null;
            }

            return new TenantConfigurationModel
            {
                Id = tenant.Id,
                ConnectionString = $"Host=localhost;Database={tenant.DatabaseName};Username=postgres;Password=postgres"
            };
        });
    }
}