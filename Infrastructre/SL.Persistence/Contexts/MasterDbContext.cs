using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SL.Domain.Entities;

namespace SL.Persistence.Contexts
{
	public class MasterDbContext : IdentityDbContext<ApplicationUser>
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

    }
}

