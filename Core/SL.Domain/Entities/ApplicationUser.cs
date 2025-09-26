using System;
using Microsoft.AspNetCore.Identity;

namespace SL.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid TenantId { get; set; }     
        public Tenant Tenant { get; set; }
    }
}

