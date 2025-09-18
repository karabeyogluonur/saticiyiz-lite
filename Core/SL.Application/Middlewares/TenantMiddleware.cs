using System;
using Microsoft.AspNetCore.Identity;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Domain.Entities;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SL.Application.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var tenantDatabaseName = context.User.Claims
                    .FirstOrDefault(c => c.Type == "TenantDatabaseName")?.Value;

                if (!string.IsNullOrEmpty(tenantDatabaseName))
                {
                    var tenantConnStr = $"Host=localhost;Database={tenantDatabaseName};Username=postgres;Password=postgres";
                    unitOfWork.ChangeDatabase(tenantConnStr);
                }
            }
            await _next(context);
        }
    }

}

