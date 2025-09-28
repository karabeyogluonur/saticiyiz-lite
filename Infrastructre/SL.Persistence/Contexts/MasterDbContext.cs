using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SL.Domain.Entities;
namespace SL.Persistence.Contexts
{
    public class MasterDbContext : IdentityDbContext<ApplicationUser>
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<EmailAccount> EmailAccounts { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.TenantId).IsRequired();
                b.HasIndex(u => u.TenantId);
            });
            builder.Entity<Tenant>().HasKey(t => t.Id);
            builder.Entity<EmailAccount>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
