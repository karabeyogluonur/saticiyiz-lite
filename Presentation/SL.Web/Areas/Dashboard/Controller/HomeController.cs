using Microsoft.AspNetCore.Mvc;

namespace SL.Web.Areas.Dashboard.Controller
{
    public class HomeController : BaseDashboardController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
