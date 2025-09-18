using System;
using Microsoft.AspNetCore.Identity;

namespace SL.Domain.Entities
{
	public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TenantDatabaseName { get; set; }
    }
}

