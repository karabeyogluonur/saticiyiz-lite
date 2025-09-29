using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SL.Application.Interfaces.Repositories;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Application.Interfaces.Services.Membership;
using SL.Application.Models.DTOs.Tenant;
using SL.Domain.Entities.Membership;
using SL.Persistence.Contexts;
using SL.Persistence.Utilities;
namespace SL.Persistence.Services.Membership
{
    public class TenantService : ITenantService
    {
        private readonly IUnitOfWork<MasterDbContext> _unitOfWork;
        private readonly IUnitOfWork<TenantDbContext> _tenantUnitOfWork;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IMapper _mapper;
        public TenantService(IUnitOfWork<MasterDbContext> unitOfWork, IMapper mapper, IUnitOfWork<TenantDbContext> tenantUnitOfWork)
        {
            _unitOfWork = unitOfWork;
            _tenantUnitOfWork = tenantUnitOfWork;
            _tenantRepository = _unitOfWork.GetRepository<Tenant>();
            _mapper = mapper;
        }
        public async Task CreateDatabaseAsync(Tenant tenant)
        {
            using var databaseConnection = new NpgsqlConnection(Configuration.PostgresConnectionString);
            await databaseConnection.OpenAsync();
            using var databaseCreateCommand = new NpgsqlCommand($"CREATE DATABASE \"{tenant.DatabaseName}\"", databaseConnection);
            await databaseCreateCommand.ExecuteNonQueryAsync();
            await databaseConnection.CloseAsync();
            var tenantOptions = new DbContextOptionsBuilder<TenantDbContext>()
                .UseNpgsql(Configuration.TenantConnectionString(tenant.DatabaseName))
                .Options;
            using var tenantDb = new TenantDbContext(tenantOptions);
            tenantDb.Database.Migrate();
        }
        public async Task DropDatabaseAsync(string tenantDatabaseName)
        {
            using var databaseConnection = new NpgsqlConnection(Configuration.PostgresConnectionString);
            await databaseConnection.OpenAsync();
            using var databaseCheckCommand = new NpgsqlCommand($"SELECT COUNT(datname) FROM pg_database WHERE datname = {tenantDatabaseName}", databaseConnection);
            var result = await databaseCheckCommand.ExecuteScalarAsync();
            if (Convert.ToInt32(result) > 0)
            {
                using var databaseDropCommand = new NpgsqlCommand($"DROP DATABASE \"{tenantDatabaseName}\"", databaseConnection);
                await databaseDropCommand.ExecuteNonQueryAsync();
            }
            await databaseConnection.CloseAsync();
        }
        public async Task DeleteTenantAsync(Guid tenantId)
        {
            Tenant tenant = await _tenantRepository.GetFirstOrDefaultAsync(predicate: tenant => tenant.Id == tenantId);
            if (tenant is null)
                return;
            _tenantRepository.Delete(tenant);
            await DropDatabaseAsync(tenant.DatabaseName);
        }
        public async Task<Tenant> InsertTenantAsync(TenantCreateModel tenantCreateModel)
        {
            Tenant tenant = _mapper.Map<Tenant>(tenantCreateModel);
            await _tenantRepository.InsertAsync(tenant);
            await _unitOfWork.SaveChangesAsync();
            return tenant;
        }
        public async Task<Tenant> GetTenantByIdAsync(Guid tenantId) => await _tenantRepository.GetFirstOrDefaultAsync(predicate: tenant => tenant.Id == tenantId);
    }
}
