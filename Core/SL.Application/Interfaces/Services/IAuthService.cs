using System;
using SL.Application.Models.ViewModels.Account;
using SL.Domain;

namespace SL.Application.Interfaces.Services
{
	public interface IAuthService
	{
        Task RegisterAsync(RegisterViewModel registerViewModel, string tenantDatabaseName);

        Task<Result<string>> LoginAsync(string email, string password, bool rememberMe);

        Task<Result<bool>> LogoutAsync();
    }
}

