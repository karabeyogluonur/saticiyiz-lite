using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SL.Application.Framework;
using SL.Domain.Defaults.Membership;
using SL.Web.Mvc.Controllers;
namespace SL.Web.Areas.Dashboard.Controller
{
    [Area(AreaNames.CUSTOMER)]
    [Authorize(Policy = PolicyName.RequireUserRole)]
    public class BaseDashboardController : BaseController
    {
        public BaseDashboardController()
        {
        }
    }
}
