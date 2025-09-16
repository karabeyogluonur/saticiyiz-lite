using SL.Application.Utilities;
using SL.Infrastructre.Utilities;
using SL.Persistence.Utilities;

namespace SL.Web.Mvc
{
    public static class ServiceRegistration
    {
        public static void AddBaseServices(this IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }
        public static void AddLayerServices(this IServiceCollection services)
        {
            services.AddApplicationService();
            services.AddPersistenceService();
            services.AddInfrastructreService();

        }
    }
}

