using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SL.Web.Mvc.Controllers;

namespace SL.Web.Controllers;

[AllowAnonymous]
public class BasePublicController : BaseController
{
    public BasePublicController()
    {
    }

}

