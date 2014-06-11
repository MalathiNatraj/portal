using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.WebApp.Infrastructure.Authentication;

namespace Diebold.WebApp.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /LogError/
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

    }
}
