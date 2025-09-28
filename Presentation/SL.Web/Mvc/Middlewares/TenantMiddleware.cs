using System;
using SL.Application.Interfaces.Services;
namespace SL.Web.Mvc.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.Claims
                .FirstOrDefault(c => c.Type == "TenantId")?.Value;
            if (Guid.TryParse(tenantIdClaim, out Guid tenantId) && tenantId != Guid.Empty)
            {
                using (Serilog.Context.LogContext.PushProperty("TenantId", tenantId))
                {
                    await tenantService.SetTenantContext(tenantId);
                    await _next(context);
                }
                return;
            }
        }
        await _next(context);
    }
}
