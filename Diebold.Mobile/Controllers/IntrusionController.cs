using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DieboldMobile.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using DieboldMobile.Models;
using DieboldMobile.Infrastructure.Authentication;
using Diebold.Platform.Proxies.DTO;
using System.Text;

namespace DieboldMobile.Controllers
{
    public class IntrusionController : BaseController
    {
        //
        // GET: /Intrusion/

        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IDeviceService _deviceService;
        private readonly IDvrService _dvrService;
        protected readonly IUserDefaultsService _userDefaultService;
        private readonly IIntrusionService _intrusionService;

        public IntrusionController(IUserService userService, ICurrentUserProvider currentUserProvider, IDeviceService deviceService, IDvrService dvrService, IUserDefaultsService userDefaultService, IIntrusionService intrusionService)
        {
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _deviceService = deviceService;
            _dvrService = dvrService;
            _userDefaultService = userDefaultService;
            _intrusionService = intrusionService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IntrusionDetailPortlet()
        {
            return View();
        }

        public ActionResult GetDeviceListforIntrusion()
        {
            try
            {
                var user = _currentUserProvider.CurrentUser;
                IList<Dvr> objlstDevice = new List<Dvr>();
                String parentType = AlarmParentType.Intrusion.ToString();
                IList<Dvr> deviceList = _userService.GetDevicesByParentType(user.Id, parentType);

                IList<DeviceModel> objlstDevices = new List<DeviceModel>();
                deviceList.ToList().ForEach(x =>
                {
                    DeviceModel objdevice = new DeviceModel();
                    objdevice.Id = x.Id;
                    objdevice.Name = x.Name;
                    objdevice.SiteId = x.Site.Id;
                    objdevice.SiteName = x.Site.Name;
                    objdevice.Address1 = x.Site.Address1;
                    if (!string.IsNullOrEmpty(x.Site.Address2))
                        objdevice.Address2 = x.Site.Address2;
                    else
                        objdevice.Address2 = string.Empty;
                    objdevice.City = x.Site.City;
                    objdevice.State = x.Site.State;
                    objdevice.Zip = x.Site.Zip;
                    objlstDevices.Add(objdevice);
                });

                deviceList = null;

                // GetDefault Selection Item
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "INTRUSION");
                if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
                {
                    return Json(objlstDevices.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, DefaultSelectedValue = lstUserDefaults.First().FilterValue }), JsonRequestBehavior.AllowGet);
                }
                return Json(objlstDevices.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }

        }

        public ActionResult GetIntrusionDetails(int deviceId)
        {
            try
            {
                IntrusionViewModel model = new IntrusionViewModel();
                model.AreModelList = new List<AreaModel>();
                if (deviceId > 0)
                {
                    Intrusion objResult = _intrusionService.GetIntrusionDetails(Convert.ToInt32(deviceId));
                    BindModel(model, objResult);
                    Session["IntrusionObject"] = objResult;
                }
                return Json(model.AreModelList);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public ActionResult GetAreaArmedStatus(string areNumber)
        {
            try
            {
                List<ZoneModel> objLstZoneModel = new List<ZoneModel>();
                ZoneModel objZoneModel = new ZoneModel();
                Intrusion objResult = (Intrusion)Session["IntrusionObject"];
                int areNo = Convert.ToInt16(areNumber);
                string strArmed = string.Empty;
                if (objResult != null)
                {
                    var result = objResult.AreaList.Where(x => x.AreaNumber == areNo).FirstOrDefault();
                    if (result.Armed)
                        strArmed = "Disarm";
                    else
                        strArmed = "Arm";
                }
                return Json(strArmed);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public ActionResult GetPollingStatus()
        {
            try
            {
                Intrusion objResult = (Intrusion)Session["IntrusionObject"];
                Intrusion objPollingObject = new Intrusion();
                if (objResult != null)
                {
                    objPollingObject.Status = objResult.Status;
                    objPollingObject.PollingStatus = DateTime.Now.ToString();
                }
                return Json(objPollingObject);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public ActionResult GetZonesByArea(string areNumber)
        {
            try
            {
                List<ZoneModel> objLstZoneModel = new List<ZoneModel>();
                ZoneModel objZoneModel = new ZoneModel();
                Intrusion objResult = (Intrusion)Session["IntrusionObject"];
                int areNo = Convert.ToInt16(areNumber);
                if (objResult != null)
                {
                    var result = objResult.AreaList.Where(x => x.AreaNumber == areNo).FirstOrDefault();
                    if (result != null && result.Zones != null)
                    {
                        foreach (Zone objZone in result.Zones)
                        {
                            objZoneModel = new ZoneModel();
                            objZoneModel.Name = objZone.Name;
                            objZoneModel.Status = objZone.Status;
                            objZoneModel.ZoneNumber = objZone.Number;

                            if (objZone.Status.ToLower() == "bypassed")
                                objZoneModel.ByPass = true;
                            else
                                objZoneModel.ByPass = false;

                            objLstZoneModel.Add(objZoneModel);
                        }
                    }
                }
                return Json(objLstZoneModel);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public ActionResult ValidateUserPin(string UserPin, string submitVal, string areaNumber, string arm, int deviceId)
        {
            try
            {
                User objuser = _userService.Get(_currentUserProvider.CurrentUser.Id);
                string CurrentUserPin = objuser.UserPin;
                var result = string.Empty;
                if (UserPin.Equals(objuser.UserPin))
                {
                    result = "true";
                    Intrusion objIntrusion = new Intrusion();
                    objIntrusion.AreaNumber = areaNumber;
                    objIntrusion.DeviceId = deviceId;
                    if (arm == "true")
                    {
                        result = _intrusionService.AreaArm(objIntrusion);
                    }
                    else
                    {
                        result = _intrusionService.AreaDisArm(objIntrusion);
                    }
                }
                else
                {
                    result = "false";
                    if (submitVal == "3")
                    {
                        result = "logoff";
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public JsonResult ZoneByPass(string zoneNumber, int deviceId, string byPass)
        {
            try
            {
                Intrusion objIntrusion = new Intrusion();
                objIntrusion.zoneNumber = zoneNumber;
                objIntrusion.DeviceId = deviceId;
                var result = string.Empty;
                if (byPass == "true")
                {
                    result = _intrusionService.ZoneByPass(objIntrusion);
                }
                else if (byPass == "false")
                {
                    result = _intrusionService.ZoneResetByPass(objIntrusion);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public ActionResult GetIntrusionReport(int deviceId)
        {
            try
            {
                Intrusion objIntrusion = new Intrusion();
                objIntrusion.startDateTime = DateTime.Now.ToString();
                objIntrusion.endDateTime = DateTime.Now.ToString();
                objIntrusion.DeviceId = deviceId;

                Intrusion result = _intrusionService.GetIntrusionReport(objIntrusion);
                StringBuilder sbResponse = new StringBuilder();
                foreach (ReportList report in result.ReportList)
                {
                    if (string.IsNullOrEmpty(report.user) == false)
                        sbResponse.Append(report.user.ToUpper() + " ");
                    if (string.IsNullOrEmpty(report.message) == false)
                        sbResponse.Append(report.message + " at ");
                    if (string.IsNullOrEmpty(report.datetime) == false)
                        sbResponse.Append(report.datetime + " ");
                    sbResponse.Append("|");
                }
                return Json(sbResponse.ToString());
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }      

        private void BindModel(IntrusionViewModel model, Intrusion objResult)
        {
            model.PollingStatus = objResult.PollingStatus;

            AreaModel objAreaModel = new AreaModel();
            List<AreaModel> objLstAreaModel = new List<AreaModel>();
            foreach (Area area in objResult.AreaList)
            {
                objAreaModel = new AreaModel();
                objAreaModel.AreaName = area.AreaName;
                objAreaModel.AreaNumber = area.AreaNumber;
                objAreaModel.Armed = area.Armed;
                objAreaModel.LateStatus = area.LateStatus;
                objAreaModel.ScheduleStatus = area.ScheduleStatus;
                if (objResult.Status.IsCaseInsensitiveEqual("Connected"))
                {
                    objAreaModel.Online = "Yes";
                }
                else
                {
                    objAreaModel.Online = "No";
                }

                if (area.Armed == true)
                    objAreaModel.Status = "Armed";
                else
                    objAreaModel.Status = "DisArmed";

                objLstAreaModel.Add(objAreaModel);

            }
            model.AreModelList = objLstAreaModel;
        }

    }
}
