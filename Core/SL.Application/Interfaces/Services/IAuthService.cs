using System;
using SL.Application.Models.ViewModels.Account;

namespace SL.Application.Interfaces.Services
{
	public interface IAuthService
	{
        Task RegisterAsync(RegisterViewModel registerViewModel, string tenantDatabaseName);
    }
}

