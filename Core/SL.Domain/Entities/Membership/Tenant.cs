using System;
using Microsoft.AspNetCore.Identity;
namespace SL.Domain.Entities.Membership
{
    public class Tenant : BaseEntity
    {
        public string DatabaseName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
