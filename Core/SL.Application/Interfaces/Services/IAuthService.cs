using System;
using SL.Application.Models.ViewModels.Account;
using SL.Domain;
using SL.Domain.Entities;

namespace SL.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<string>> LoginAsync(string email, string password, bool rememberMe);
        Task<Result<bool>> LogoutAsync();
    }
}

