using System;
namespace SL.Application.Models.ViewModels.Account
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; } = true;
    }
}

