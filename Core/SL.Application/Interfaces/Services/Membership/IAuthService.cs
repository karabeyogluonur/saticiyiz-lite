using System;
using SL.Application.Models.ViewModels.Account;
using SL.Domain;
using SL.Domain.Entities;
namespace SL.Application.Interfaces.Services.Membership
{
    public interface IAuthService
    {
        Task<Result> LoginAsync(string email, string password, bool rememberMe);
        Task<Result> LogoutAsync();
    }
}
