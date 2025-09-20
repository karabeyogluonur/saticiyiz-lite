using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SL.Application.Framework;
using SL.Domain.Defaults;
using SL.Web.Mvc.Controllers;

namespace SL.Web.Areas.Dashboard.Controller
{
    [Area(AreaNames.CUSTOMER)]
    [Authorize(Policy = PolicyNames.REQUIRE_USER_ROLE)]

    public class BaseDashboardController : BaseController
    {
        public BaseDashboardController()
        {

        }
    }
}
