using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SL.Application.Interfaces.Services;
using SL.Application.Models.ViewModels.Account;
using SL.Domain.Entities;
using SL.Persistence.Contexts;

namespace SL.Persistence.Services
{
    public class AuthService : IAuthService
	{
        UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
		{
            _userManager = userManager;
		}

        public async Task RegisterAsync(RegisterViewModel registerViewModel, string tenantDatabaseName)
        {
            var user = new ApplicationUser
            {
                UserName = registerViewModel.Email,
                Email = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                PhoneNumber = registerViewModel.Phone,
                TenantDatabaseName = tenantDatabaseName
            };

            await _userManager.CreateAsync(user, registerViewModel.Password); 
        }
    }
}

