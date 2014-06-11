using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.WebApp.Infrastructure.Authentication;

namespace Diebold.WebApp.Controllers
{
    public class TermofUseController : Controller
    {
        //
        // GET: /TermsandConditions/

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult TermofUse()
        {
            return View();
        }
    }
}
