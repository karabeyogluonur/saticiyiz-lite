using SL.Application.Defaults.Caching;
using SL.Application.Interfaces.Repositories;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Application.Interfaces.Services.Caching;
using SL.Application.Interfaces.Services.Tenants;
using SL.Application.Models.DTOs.Tenant;
using SL.Domain.Entities.Membership;
using SL.Persistence.Contexts;



namespace SL.Infrastructure.Services.Membership;





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

        var cacheKey = TenantCacheDefaults.ConfigurationByIdCacheKey(tenantId);
        cacheKey.CacheTimeInMinutes = 1440; // 24 saat

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {

            var tenant = await _tenantRepository.GetFirstOrDefaultAsync(predicate: t => t.Id == tenantId);


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