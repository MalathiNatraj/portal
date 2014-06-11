using System.Web.Mvc;
using AttributeRouting;
using DieboldMobile.Infrastructure.Authentication;

namespace DieboldMobile.Areas.PlataformFake.Controllers
{
    [RouteArea("PlataformFake")]
    public class ApiCallController : Controller
    {

        [GET("device/{id}/status")]

        [GET("unassignedSlots/{deviceTypeName}")]
        [AllowAnonymous]
        public ActionResult GetUnassignedSlots(string deviceTypeName)
        {
            return Json(new
            {
                unassignedSlots = new[] { "AA:BB:CC:11", "AA:BB:CC:22", "AA:BB:CC:33", "AA:BB:CC:44", "AA:BB:CC:55", "AA:BB:CC:66", 
                                                        "AA:BB:CC:77", "AA:BB:CC:88", "AA:BB:CC:99"}
            }, JsonRequestBehavior.AllowGet);
        }

        [GET("deviceTypes")]
        [AllowAnonymous]
        public ActionResult GetDeviceTypes()
        {
            return Json(new
            {
                deviceTypes = new[]
                                {
                                    new
                                        {
                                            name = "DVR"
                                        },
                                    new
                                        {
                                            name = "Gateway"
                                        }
                                }
            }, JsonRequestBehavior.AllowGet);
        }

        [POST("device")]
        [AllowAnonymous]
        public ActionResult AddDevice()
        {
            return Json(new { deviceId = "123456" });
        }

        [PUT("device/{id}")]
        [AllowAnonymous]
        public ActionResult EditDevice(string id)
        {
            return Json(new { deviceId = "123456" });
        }

        [DELETE("device/{id}")]
        [AllowAnonymous]
        public ActionResult DeleteDevice(string id)
        {
            return new HttpStatusCodeResult(200);
        }

        [PUT("device/{id}/enable")]
        [AllowAnonymous]
        public ActionResult EnabledDevice(string id)
        {
            return new HttpStatusCodeResult(200);
        }

        [PUT("device/{id}/disable")]
        [AllowAnonymous]
        public ActionResult DisableDevice(string id)
        {
            return new HttpStatusCodeResult(200);
        }

        [GET("device/{id}/status")]
        [AllowAnonymous]
        public ActionResult DeviceStatus(string id)
        {
            return new HttpStatusCodeResult(500);
        }

    }
}
