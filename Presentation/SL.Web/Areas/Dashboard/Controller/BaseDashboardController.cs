using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SL.Application.Framework;
using SL.Web.Mvc.Controllers;

namespace SL.Web.Areas.Dashboard.Controller
{
    [Area(AreaNames.CUSTOMER)]
    [Authorize]
    public class BaseDashboardController : BaseController
    {
        public BaseDashboardController()
        {

        }
    }
}
