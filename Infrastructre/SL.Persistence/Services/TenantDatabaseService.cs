using System;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SL.Application.Interfaces.Services;
using SL.Persistence.Contexts;

namespace SL.Persistence.Services
{
    public class TenantDatabaseService : ITenantDatabaseService
    {
        public async Task CreateDatabaseAsync(string tenantDatabaseName)
        {
                using var conn = new NpgsqlConnection("Host=localhost;Database=postgres;Username=postgres;Password=postgres");
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand($"CREATE DATABASE \"{tenantDatabaseName}\"", conn);
                await cmd.ExecuteNonQueryAsync();

                var tenantOptions = new DbContextOptionsBuilder<TenantDbContext>()
                    .UseNpgsql($"Host=localhost;Database={tenantDatabaseName};Username=postgres;Password=postgres")
                    .Options;

                using var tenantDb = new TenantDbContext(tenantOptions);
                tenantDb.Database.Migrate();
        }
    }
}

