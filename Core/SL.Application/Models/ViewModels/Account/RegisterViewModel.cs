using System;
namespace SL.Application.Models.ViewModels.Account
{
	public class RegisterViewModel
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
	}
}

