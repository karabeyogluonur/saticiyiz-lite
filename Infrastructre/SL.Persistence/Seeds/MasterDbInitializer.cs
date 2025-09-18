using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SL.Persistence.Contexts;

namespace SL.Persistence.Seeds
{
    public static class DbInitializer
    {
        public static void Initialize(IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<MasterDbContext>();

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Database.Migrate();
        }

    }
}

