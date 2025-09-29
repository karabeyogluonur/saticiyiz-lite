using Serilog.Context;
using SL.Application.Interfaces.Services.Context;
using SL.Application.Interfaces.Services.Tenants;

namespace SL.Web.Mvc.Middlewares;







public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext, ITenantStore tenantStore)
    {

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value;

            if (Guid.TryParse(tenantIdClaim, out var tenantId) && tenantId != Guid.Empty)
            {


                var tenantConfig = await tenantStore.GetConfigurationByIdAsync(tenantId);


                if (tenantConfig != null && !string.IsNullOrEmpty(tenantConfig.ConnectionString))
                {


                    tenantContext.SetTenant(tenantConfig.Id, tenantConfig.ConnectionString);


                    using (LogContext.PushProperty("TenantId", tenantId))
                    {
                        await _next(context);
                    }

                    return;
                }
            }
        }




        await _next(context);
    }
}