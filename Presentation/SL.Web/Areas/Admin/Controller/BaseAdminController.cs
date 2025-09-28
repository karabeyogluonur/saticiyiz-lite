using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SL.Application.Framework;
using SL.Domain.Defaults;
using SL.Web.Mvc.Controllers;
namespace SL.Web.Areas.Admin.Controller
{
    [Area(AreaNames.ADMIN)]
    [Authorize(Policy = PolicyName.RequireAdminRole)]
    public class BaseAdminController : BaseController
    {
        public BaseAdminController()
        {
        }
    }
}
