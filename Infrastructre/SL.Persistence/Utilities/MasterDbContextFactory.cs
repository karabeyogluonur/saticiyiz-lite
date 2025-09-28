using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using SL.Application.Interfaces.Services;
using SL.Domain.Entities;
using SL.Persistence.Contexts;
using SL.Persistence.Services;
namespace SL.Persistence.Utilities
{
    public class MasterDbContextFactory : IDesignTimeDbContextFactory<MasterDbContext>
    {
        public MasterDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasterDbContext>();
            optionsBuilder.UseNpgsql(Configuration.MasterConnectionString);
            return new MasterDbContext(optionsBuilder.Options);
        }
    }
}
