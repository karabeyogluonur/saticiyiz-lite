using System;
namespace SL.Application.Interfaces.Services
{
	public interface ITenantDatabaseService
	{
        Task CreateDatabaseAsync(string tenantDatabaseName);
    }
}

