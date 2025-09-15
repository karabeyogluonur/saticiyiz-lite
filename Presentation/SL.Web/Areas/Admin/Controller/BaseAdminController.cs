using Microsoft.AspNetCore.Mvc;
using SL.Application.Framework;
using SL.Web.Mvc.Controllers;

namespace SL.Web.Areas.Admin.Controller
{
    [Area(AreaNames.ADMIN)]
    public class BaseAdminController : BaseController
    {
        public BaseAdminController()
        {

        }
    }
}
