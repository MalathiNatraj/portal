using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.WebApp.Infrastructure.Authentication;

namespace Diebold.WebApp.Controllers
{
    public class AccessDeniedController : Controller
    {
        //
        // GET: /AccessDenied/

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

    }
}
