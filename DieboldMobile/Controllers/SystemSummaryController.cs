using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DieboldMobile.Services;

namespace DieboldMobile.Controllers
{
    public class SystemSummaryController : Controller
    {
        //
        // GET: /SystemSummary/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SystemSummary()
        {
            return View();
        }

        public JsonResult GetAllSystemSummaryDeviceDetails()
        {
            // Bind Device Details inside Combo Box
            List<SystemSummaryDevice> objSystemSummaryDevice = new List<SystemSummaryDevice>();
            objSystemSummaryDevice = (new SystemSummaryService().GetAllSystemSummaryDevice()).OrderBy(x => x.Name).ToList();
            return Json(objSystemSummaryDevice.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterByDeviceTypeId(int DeviceId)
        {
            var resultSet = new SystemSummaryService().GetAllSystemSummaryDetails().Where(x => x.DeviceTypeId == DeviceId).Select(y => y);
            return Json(resultSet);
        }

    }
}
