using System;
using Microsoft.AspNetCore.Identity;

namespace SL.Domain.Entities
{
	public class ApplicationUser : IdentityUser
    {
        public string TenantDatabaseName { get; set; }
    }
}

