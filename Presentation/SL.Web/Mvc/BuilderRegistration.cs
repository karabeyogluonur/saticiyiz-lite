using System;
namespace SL.Web.Mvc
{
    public static class BuilderRegistration
    {
        public static void AddBaseBuilder(this WebApplication application)
        {
            application.UseHttpsRedirection();
            application.UseStaticFiles();
        }
        public static void AddDevelopmentBuilder(this WebApplication application)
        {
            if (!application.Environment.IsDevelopment())
            {
                application.UseExceptionHandler("/Home/Error");
                application.UseHsts();
            }
        }
        public static void AddRouteBuilder(this WebApplication app)
        {
            app.UseRouting();
        }
        public static void AddAuthBuilder(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

        }
        public static void UseSpecialRoute(this WebApplication app)
        {
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}

