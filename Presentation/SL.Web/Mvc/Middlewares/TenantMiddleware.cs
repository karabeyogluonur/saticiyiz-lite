using Serilog.Context;
using SL.Application.Interfaces.Services.Context;
using SL.Application.Interfaces.Services.Tenants;

namespace SL.Web.Mvc.Middlewares;

/// <summary>
/// Gelen isteklerdeki TenantId claim'ini okuyarak iki temel görevi yerine getirir:
/// 1. Tenant'a özel veritabanı bağlantı dizesini (connection string) bulup istek context'ine setler.
///    Bu, TenantDbContext'in doğru veritabanına bağlanmasını sağlar.
/// 2. TenantId'yi log context'ine ekleyerek logların tenant bazında filtrelenmesini kolaylaştırır.
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext, ITenantStore tenantStore)
    {
        // Kullanıcının kimliği doğrulanmışsa ve bir TenantId claim'i varsa devam et.
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value;

            if (Guid.TryParse(tenantIdClaim, out var tenantId) && tenantId != Guid.Empty)
            {
                // ITenantStore servisini kullanarak (cache'den veya master db'den) 
                // bu tenant'ın konfigürasyonunu (ve connection string'ini) al.
                var tenantConfig = await tenantStore.GetConfigurationByIdAsync(tenantId);

                // Eğer konfigürasyon bulunduysa ve geçerli bir connection string içeriyorsa
                if (tenantConfig != null && !string.IsNullOrEmpty(tenantConfig.ConnectionString))
                {
                    // 1. Görev: İstek bazlı tenant context'ini doldur.
                    // Bu bilgi, daha sonra TenantDbContext oluşturulurken kullanılacak.
                    tenantContext.SetTenant(tenantConfig.Id, tenantConfig.ConnectionString);

                    // 2. Görev: Loglamayı zenginleştir.
                    using (LogContext.PushProperty("TenantId", tenantId))
                    {
                        await _next(context);
                    }
                    // Görev tamamlandı, middleware zincirinden çık.
                    return;
                }
            }
        }

        // Eğer kullanıcı login değilse, TenantId'si yoksa veya tenant konfigürasyonu bulunamadıysa,
        // hiçbir context setlemeden bir sonraki adıma geç.
        // Bu durumda TenantDbContext, varsayılan connection string'e bağlanacaktır.
        await _next(context);
    }
}