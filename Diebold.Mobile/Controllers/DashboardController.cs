using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Enums;
using Diebold.Services.Exceptions;
using Diebold.Services.Extensions;
using DieboldMobile.Infrastructure.Helpers;
using DieboldMobile.Models;
using Diebold.Services.Contracts;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.Domain.Entities;
using System.Data;
using DieboldMobile.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Configuration;

namespace DieboldMobile.Controllers
{
    public class DashboardController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MAS()
        {
            return View();
        }
        #region Portal code

        public ActionResult LogOn()
        {
            return RedirectToAction("Index");
        }

        public ActionResult Home()
        {
            return View("Home", new List<UserPortletsPreferencesViewModel>());
        }
        #endregion

    }
}
