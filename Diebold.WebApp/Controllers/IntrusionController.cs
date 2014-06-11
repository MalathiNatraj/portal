using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;
using Diebold.WebApp.Infrastructure.Authentication;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.WebApp.Controllers
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
        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;
        private readonly IDeviceMediaService _deviceMediaService;

        public IntrusionController(IUserService userService, ICurrentUserProvider currentUserProvider, IDeviceService deviceService, IDeviceMediaService deviceMediaService, IDvrService dvrService, IUserDefaultsService userDefaultService, IIntrusionService intrusionService, ICompanyService companyService, ISiteService siteService)
        {
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _deviceService = deviceService;
            _dvrService = dvrService;
            _userDefaultService = userDefaultService;
            _intrusionService = intrusionService;
            _companyService = companyService;
            _siteService = siteService;
            _deviceMediaService = deviceMediaService;
        }

        public ActionResult GetIntrusionDeviceListforSearch()
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
                    objdevice.Device = x.DeviceType.ToString();
                    objlstDevices.Add(objdevice);
                });

                return Json(objlstDevices.Select(c => new { Id = c.Id, Name = c.Name, Device = c.Device, Commands = c.Commands, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public ActionResult GetDeviceListforIntrusion()
        {
            int IntrusionDeviceId = 0;
            try
            {
                var user = _currentUserProvider.CurrentUser;
                IList<Dvr> objlstDevice = new List<Dvr>();
                String parentType = AlarmParentType.Intrusion.ToString();
                IList<Dvr> deviceList = _userService.GetDevicesByParentType(user.Id, parentType);

                IList<DeviceModel> objlstDevices = new List<DeviceModel>();
                var commands = new String[] { "Image", "Video" };
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
                    objdevice.Device = x.DeviceType.ToString();

                    if (x.DeviceType == DeviceType.videofied01)
                        objdevice.Commands = commands;
                    objlstDevices.Add(objdevice);
                });

                deviceList = null;
                // GetDefault Selection Item
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "INTRUSION");
                int intruResult = 0;

                if (Session["SelectedIntrusionDeviceId"] != null)
                {
                    intruResult = GetDeviceStatus((int)Session["SelectedIntrusionDeviceId"]);
                }
                else if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
                {
                    intruResult = GetDeviceStatus(lstUserDefaults.First().FilterValue);
                }

                // Get Value from Session and return
                if (Session["SelectedIntrusionDeviceId"] != null)
                {
                    IntrusionDeviceId = (int)Session["SelectedIntrusionDeviceId"];
                    if (intruResult == 1)
                    {
                        return Json(objlstDevices.Select(c => new { Id = c.Id, Name = c.Name, Device = c.Device, Commands = c.Commands, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, DefaultSelectedValue = IntrusionDeviceId }), JsonRequestBehavior.AllowGet);
                    }
                }


                if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
                {
                    if (intruResult == 1)
                    {
                        return Json(objlstDevices.Select(c => new { Id = c.Id, Name = c.Name, Device = c.Device, Commands = c.Commands, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, DefaultSelectedValue = lstUserDefaults.First().FilterValue }), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(objlstDevices.Select(c => new { Id = c.Id, Name = c.Name, Device = c.Device, Commands = c.Commands, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }

        }


        public ActionResult SaveIntrusionDefaultValue(int DeviceId, string InternalName, string ControlName)
        {
            try
            {
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName);
                if (lstUserDefaults.Count() > 0)
                {
                    lstUserDefaults.First().FilterValue = DeviceId;
                    _userDefaultService.Update(lstUserDefaults.First());
                }
                else
                {
                    UserDefaults objUserDefaults = new UserDefaults();
                    objUserDefaults.FilterName = ControlName;
                    objUserDefaults.FilterValue = DeviceId;
                    objUserDefaults.InternalName = InternalName;
                    objUserDefaults.User = _userService.Get(_currentUserProvider.CurrentUser.Id);
                    _userDefaultService.Create(objUserDefaults);
                }
                return Json("RecordModified", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public ActionResult ClearIntrusionDefaultValue(string InternalName, string ControlName)
        {
            try
            {
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName);
                if (lstUserDefaults.Count() > 0)
                {
                    _userDefaultService.Delete(lstUserDefaults.First().Id);
                    return Json("Defaults Cleared", JsonRequestBehavior.AllowGet);
                }
                return Json("No Defaults", JsonRequestBehavior.AllowGet);
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
                        result = _intrusionService.AreaDisArm(objIntrusion);
                    }
                    else
                    {
                        result = _intrusionService.AreaArm(objIntrusion);
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
                LogError("Exception occured while validating user ping", e);
                return JsonError(e.Message);
            }
        }

        public ActionResult AddDefaultValues(int DeviceId, string InternalName, string ControlName)
        {
            try
            {
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName);
                if (lstUserDefaults.Count() > 0)
                {
                    lstUserDefaults.First().FilterValue = DeviceId;
                    _userDefaultService.Update(lstUserDefaults.First());
                }
                else
                {
                    UserDefaults objUserDefaults = new UserDefaults();
                    objUserDefaults.FilterName = ControlName;
                    objUserDefaults.FilterValue = DeviceId;
                    objUserDefaults.InternalName = InternalName;
                    objUserDefaults.User = _userService.Get(_currentUserProvider.CurrentUser.Id);
                    _userDefaultService.Create(objUserDefaults);
                }
                return Json("RecordModified", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public JsonResult UserCodeAdd(string name, string userCode, string userNumber, string ProfileNumber, int deviceId, FormCollection formData)
        {
            try
            {
                var device = _dvrService.Get(deviceId);

                Intrusion objIntrusion = new Intrusion();
                objIntrusion.UserName = name;
                objIntrusion.UserCode = (!string.IsNullOrWhiteSpace(userCode)) ? userCode : formData["passcode"];
                objIntrusion.UserNumber = (string.IsNullOrWhiteSpace(userNumber)) ? "0" : userNumber;
                objIntrusion.ProfileNumber = ProfileNumber;
                objIntrusion.Zip = "";
                objIntrusion.DeviceId = deviceId;
                objIntrusion.DeviceInstanceId = device.ExternalDeviceId;

                if (device.DeviceType == DeviceType.bosch_D9412GV4 && formData.AllKeys.Contains("cbAccess_1"))
                {
                    objIntrusion.AreasAuthorityLevel = new Dictionary<string, string>();
                    for (var i = 1; i <= 32; i++)
                    {
                        objIntrusion.AreasAuthorityLevel.Add("area" + i.ToString().PadLeft(2, '0'), formData["cbAccess_" + i]);
                    }
                }

                var result = _intrusionService.UserCodeAdd(objIntrusion);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public JsonResult CaptureMedia(int deviceId, string zoneNumber, string mediaType)
        {
            if (mediaType.ToLower() == DeviceMediaType.Image.ToString().ToLower())
            {
                return Json(_deviceMediaService.CaptureImage(deviceId, zoneNumber), JsonRequestBehavior.AllowGet);
            }
            else if (mediaType.ToLower() == DeviceMediaType.Video.ToString().ToLower())
            {
                return Json(_deviceMediaService.CaptureVideo(deviceId, zoneNumber), JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult UserCodeModify(string name, string userCode, string userNumber, string profileNumber, int deviceId, FormCollection formData)
        //{
        //    try
        //    {
        //        Intrusion objIntrusion = new Intrusion();
        //        objIntrusion.UserName = name;
        //        objIntrusion.UserCode = userCode;
        //        objIntrusion.UserNumber = userNumber;
        //        objIntrusion.ProfileNumber = profileNumber;
        //        objIntrusion.Zip = "";
        //        objIntrusion.DeviceId = deviceId;
        //        var result = _intrusionService.UserCodeModify(objIntrusion);
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonError(e.Message);
        //    }
        //}
        public JsonResult UserCodeModify(string name, string userCode, string userNumber, string ProfileNumber, int deviceId, FormCollection formData)
        {
            try
            {
                var device = _dvrService.Get(deviceId);

                Intrusion objIntrusion = new Intrusion();
                objIntrusion.UserName = name;
                objIntrusion.UserCode = (!string.IsNullOrWhiteSpace(userCode)) ? userCode : formData["passcode"];
                objIntrusion.UserNumber = (string.IsNullOrWhiteSpace(userNumber)) ? "0" : userNumber;
                objIntrusion.ProfileNumber = ProfileNumber;
                objIntrusion.Zip = "";
                objIntrusion.DeviceId = deviceId;
                objIntrusion.DeviceInstanceId = device.ExternalDeviceId;

                if (device.DeviceType == DeviceType.bosch_D9412GV4 && formData.AllKeys.Contains("cbAccess_1"))
                {
                    objIntrusion.AreasAuthorityLevel = new Dictionary<string, string>();
                    for (var i = 1; i <= 32; i++)
                    {
                        objIntrusion.AreasAuthorityLevel.Add("area" + i.ToString().PadLeft(2, '0'), formData["cbAccess_" + i]);
                    }
                }

                var result = _intrusionService.UserCodeModify(objIntrusion);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public JsonResult UserCodeDelete(string name, string userCode, string userNumber, string profileNumber, int deviceId)
        {
            try
            {
                Intrusion objIntrusion = new Intrusion();
                objIntrusion.UserName = name;
                objIntrusion.UserCode = userCode;
                objIntrusion.UserNumber = userNumber;
                objIntrusion.ProfileNumber = profileNumber;
                objIntrusion.Zip = "";
                objIntrusion.DeviceId = deviceId;
                var result = _intrusionService.UserCodeDelete(objIntrusion);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public JsonResult GetProfileNumberList(int deviceId)
        {
            try
            {
                ProfileNumberListModel objprofile = new ProfileNumberListModel();
                var result = _intrusionService.GetProfileNumberList(deviceId);
                var device = _dvrService.Get(deviceId);
                return Json(new { device = new { deviceId = deviceId, type = device.DeviceType.ToString() }, result = result.Select(c => new { Id = c.profileNum, Name = c.profileNum }) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public JsonResult GetUserCodesInformation(string name, string userCode, int deviceId)
        {
            try
            {
                Intrusion objIntrusion = new Intrusion();
                objIntrusion.UserName = name;
                objIntrusion.UserCode = userCode;
                objIntrusion.DeviceId = deviceId;
                Intrusion result = _intrusionService.GetUserCodesInformation(objIntrusion);
                IntrusionViewModel model = new IntrusionViewModel();
                IntrusionUserCodeModel objIntruUCModel = new IntrusionUserCodeModel();
                List<IntrusionUserCodeModel> objLstIntruUCModel = new List<IntrusionUserCodeModel>();

                foreach (UserCodeList cardHolder in result.UserCodeList)
                {
                    objIntruUCModel = new IntrusionUserCodeModel();
                    objIntruUCModel.UserName = cardHolder.UserName;
                    objIntruUCModel.UserCode = cardHolder.UserCode;
                    objIntruUCModel.UserNumber = cardHolder.UserNumber;
                    objIntruUCModel.ProfileNumber = cardHolder.ProfileNumber;
                    objIntruUCModel.AccessLevels = cardHolder.Areas;
                    objLstIntruUCModel.Add(objIntruUCModel);
                }
                model.UserCodeModelList = objLstIntruUCModel;
                return Json(model.UserCodeModelList);
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
                    Session["SelectedIntrusionDeviceId"] = deviceId;
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
        public ActionResult GetPlatformIntrusionDetails(int deviceId)
        {
            try
            {
                IntrusionViewModel model = new IntrusionViewModel();
                model.AreModelList = new List<AreaModel>();
                if (deviceId > 0)
                {
                    Session["SelectedIntrusionDeviceId"] = deviceId;
                    Intrusion objResult = _intrusionService.GetPlatformIntrusionDetails(Convert.ToInt32(deviceId));
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


        public ActionResult GetPollingStatus()
        {
            try
            {
                Intrusion objResult = (Intrusion)Session["IntrusionObject"];
                Intrusion objPollingObject = new Intrusion();
                var user = _currentUserProvider.CurrentUser;
                if (objResult != null)
                {
                    objPollingObject.Status = objResult.Status;
                    objPollingObject.PollingStatus = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone)).ToString();
                }
                return Json(objPollingObject);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }


        public ActionResult GetZonesByArea(int? areNumber)
        {
            try
            {
                List<ZoneModel> objLstZoneModel = new List<ZoneModel>();
                ZoneModel objZoneModel = new ZoneModel();
                Intrusion objResult = (Intrusion)Session["IntrusionObject"];
                if (objResult != null)
                {
                    var result = objResult.AreaList.Where(x => x.AreaNumber == areNumber).FirstOrDefault();
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

        private void BindModel(IntrusionViewModel model, Intrusion objResult)
        {
            model.PollingStatus = objResult.PollingStatus;

            AreaModel objAreaModel = new AreaModel();
            List<AreaModel> objLstAreaModel = new List<AreaModel>();
            if (objResult.AreaList != null)
            {
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
                        objAreaModel.Status = "Disarmed";

                    objLstAreaModel.Add(objAreaModel);

                }
            }
            model.AreModelList = objLstAreaModel;
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
                    result = _intrusionService.ZoneResetByPass(objIntrusion);
                }
                else if (byPass == "false")
                {
                    result = _intrusionService.ZoneByPass(objIntrusion);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public int GetDeviceStatus(int deviceId)
        {
            var ResultSet = _dvrService.GetAllDevicesForDisplay();
            var result = ResultSet.Where(x => x.Id.Equals(deviceId) && x.DeletedKey == null);
            if (result != null)
            {
                return 1;
            }
            else
                return 0;
        }

        public ActionResult GetIntrusionReport(string startDateTime, string endDateTime, int deviceId)
        {
            try
            {
                Intrusion objIntrusion = new Intrusion();

                string startDate = string.Empty;
                string endDate = string.Empty;
                string endTime = string.Empty;
                string currentTime = string.Empty;

                DateTime today = DateTime.Today;
                string curDate = today.ToString("MM/dd/yyyy");

                int hourstoday = DateTime.Now.Hour;
                int mintoday = DateTime.Now.Minute;

                int mintodayPlus = 0;
                mintodayPlus = mintodayPlus + 1;
                int finalhrsToday = 0;

                if (hourstoday >= 12)
                {
                    if (hourstoday != 12)
                    {
                        finalhrsToday = hourstoday - 12;
                    }
                    else
                    {
                        finalhrsToday = hourstoday;
                    }

                    if (finalhrsToday.ToString().Length == 1)
                    {
                        if (mintoday.ToString().Length == 1)
                        {
                            currentTime = "0" + finalhrsToday + ":" + "0" + mintoday + " PM";
                        }
                        else if (mintoday.ToString().Length == 2)
                        {
                            currentTime = "0" + finalhrsToday + ":" + mintoday + " PM";
                        }
                    }
                    else
                    {
                        if (mintoday.ToString().Length == 1)
                        {
                            currentTime = finalhrsToday + ":" + "0" + mintoday + " PM";
                        }
                        else if (mintoday.ToString().Length == 2)
                        {
                            currentTime = finalhrsToday + ":" + mintoday + " PM";
                        }
                    }

                }
                else
                {
                    if (hourstoday.ToString().Length == 1)
                    {
                        if (mintoday.ToString().Length == 1)
                        {
                            currentTime = "0" + (hourstoday) + ":" + "0" + mintoday + " AM";
                        }
                        else if (mintoday.ToString().Length == 2)
                        {
                            currentTime = "0" + (hourstoday) + ":" + mintoday + " AM";
                        }
                    }
                    else if (hourstoday.ToString().Length == 2)
                    {
                        if (mintoday.ToString().Length == 1)
                        {
                            currentTime = (hourstoday) + ":" + "0" + mintoday + " AM";
                        }
                        else if (mintoday.ToString().Length == 2)
                        {
                            currentTime = (hourstoday) + ":" + mintoday + " AM";
                        }
                    }
                }
                if (startDateTime == endDateTime)
                {
                    startDate = startDateTime + " 12:00 AM";
                    if ((Convert.ToDateTime(startDateTime).Ticks == Convert.ToDateTime(curDate).Ticks) && (Convert.ToDateTime(endDateTime).Ticks == Convert.ToDateTime(curDate).Ticks))
                    {
                        endDate = endDateTime + " " + currentTime;
                    }
                    else
                    {
                        endDate = endDateTime + " 11:59 PM";
                    }
                }
                else if ((startDateTime != endDateTime) && (Convert.ToDateTime(endDateTime).Ticks == Convert.ToDateTime(curDate).Ticks))
                {
                    string stTime = string.Empty;
                    if (hourstoday.ToString().Length == 1)
                    {
                        if (mintodayPlus.ToString().Length == 1)
                        {
                            stTime = "0" + (hourstoday) + ":" + "0" + mintodayPlus + " AM";
                        }
                        else if (mintodayPlus.ToString().Length == 2)
                        {
                            stTime = "0" + (hourstoday) + ":" + mintodayPlus + " AM";
                        }
                    }
                    else if (hourstoday.ToString().Length == 2)
                    {
                        if (mintodayPlus.ToString().Length == 1)
                        {
                            stTime = (hourstoday) + ":" + "0" + mintodayPlus + " AM";
                        }
                        else if (mintodayPlus.ToString().Length == 2)
                        {
                            stTime = (hourstoday) + ":" + mintodayPlus + " AM";
                        }
                    }

                    startDate = startDateTime + " " + stTime;
                    endDate = endDateTime + " " + currentTime;
                }

                objIntrusion.startDateTime = startDate;
                objIntrusion.endDateTime = endDate;
                objIntrusion.DeviceId = deviceId;

                Intrusion result = _intrusionService.GetIntrusionReport(objIntrusion);

                IntrusionViewModel model = new IntrusionViewModel();
                IntrusionReportModel objIntruReportModel = new IntrusionReportModel();
                List<IntrusionReportModel> objLstIntruReportModel = new List<IntrusionReportModel>();

                foreach (ReportList report in result.ReportList)
                {
                    objIntruReportModel = new IntrusionReportModel();
                    objIntruReportModel.type = report.type;
                    objIntruReportModel.datetime = report.datetime;
                    objIntruReportModel.user = report.user;
                    objIntruReportModel.message = report.message;
                    objLstIntruReportModel.Add(objIntruReportModel);

                }
                model.IntruReportModelList = objLstIntruReportModel;
                return Json(model.IntruReportModelList);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public ActionResult GetUserCodeView(string action, int deviceId)
        {
            try
            {
                var device = _dvrService.Get(deviceId);
                var model = new IntrusionUserCodeViewModel()
                {
                    DeviceId = deviceId,
                    DeviceName = device.Name,
                    DeviceType = device.DeviceType
                };


                switch (model.DeviceType)
                {
                    case DeviceType.bosch_D9412GV4:
                        try
                        {
                            model.ProfileNumbers = _intrusionService.GetProfileNumberList(deviceId).Select(c => new ProfileNumberModel { ProfileNumberID = c.profileNum, ProfileNumber = c.profileNum }).ToList();
                        }
                        catch (Exception)
                        {

                            throw new Exception("Could not load profile list for device");
                        }
                        if (string.Compare(action, "add", true) == 0)
                        {
                            return PartialView("Bosch/AddUserCode", model);
                        }
                        else if (string.Compare(action, "modify", true) == 0)
                        {
                            return PartialView("Bosch/ModifyUserCode", model);
                        }
                        else if (string.Compare(action, "delete", true) == 0)
                        {
                            return PartialView("Bosch/DeleteUserCode", model);
                        }
                        throw new Exception("No view found for action: " + action);

                    default:
                        if (string.Compare(action, "add", true) == 0)
                        {
                            model.ProfileNumbers = _intrusionService.GetProfileNumberList(deviceId).Select(c => new ProfileNumberModel { ProfileNumberID = c.profileNum, ProfileNumber = c.profileNum }).ToList();
                            return PartialView("AddUserCode", model);
                        }
                        else if (string.Compare(action, "modify", true) == 0)
                        {
                            return PartialView("Step1ModifyUserCode", model);
                        }
                        else if (string.Compare(action, "delete", true) == 0)
                        {
                            return PartialView("Step1DeleteUserCode", model);
                        }
                        throw new Exception("No view found for action: " + action);
                }

            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
    }
}


