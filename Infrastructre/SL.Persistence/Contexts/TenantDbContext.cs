using System;
using Microsoft.EntityFrameworkCore;

namespace SL.Persistence.Contexts
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }
    }
}

