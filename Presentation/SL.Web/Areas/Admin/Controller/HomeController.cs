using Microsoft.AspNetCore.Mvc;

namespace SL.Web.Areas.Admin.Controller
{
    public class HomeController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
