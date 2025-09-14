using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SL.Web.Controllers;

public class HomeController : BasePublicController
{
    public HomeController()
    {
    }

    public IActionResult Index()
    {
        return View();
    }
}

