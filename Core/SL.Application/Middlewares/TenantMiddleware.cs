using System;
using Microsoft.AspNetCore.Identity;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Domain.Entities;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SL.Application.Interfaces.Services;

namespace SL.Application.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Bağımlılıklar artık InvokeAsync içinde değil, Constructor'da veya 
        // aşağıda gösterildiği gibi GetRequiredService ile alınabilir.
        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var tenantIdClaim = context.User.Claims
                    .FirstOrDefault(c => c.Type == "TenantId")?.Value;

                if (Guid.TryParse(tenantIdClaim, out Guid tenantId))
                    await tenantService.SetTenantContext(tenantId);
            }

            await _next(context);
        }
    }

}

