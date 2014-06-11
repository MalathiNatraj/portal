using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;
using Diebold.WebApp.Infrastructure.Authentication;

namespace Diebold.WebApp.Controllers
{
    public class AccessController : BaseController
    {
        //
        // GET: /Access/
        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;
        protected readonly IUserDefaultsService _userDefaultService;
        private readonly IDvrService _dvrService;
        private readonly IAccessService _accessService;
        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;

        public AccessController(IUserService userService, ICurrentUserProvider currentUserProvider, IUserDefaultsService userDefaultService, IDvrService dvrService, IAccessService accessService, ICompanyService companyService, ISiteService siteService)
        {
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _userDefaultService = userDefaultService;
            _dvrService = dvrService;
            _accessService = accessService;
            _companyService = companyService;
            _siteService = siteService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAccessDevicesforSearch()
        {
            try
            {
                var user = _currentUserProvider.CurrentUser;
                IList<Dvr> objLstDevices = _userService.GetDevicesByParentType(_currentUserProvider.CurrentUser.Id, AlarmParentType.Access.ToString());

                IList<DeviceModel> objlstDevice = new List<DeviceModel>();
                objLstDevices.ToList().ForEach(x =>
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
                    objdevice.DeviceType = x.DeviceType.ToString();
                    objlstDevice.Add(objdevice);
                });

                return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, DeviceType = c.DeviceType }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public ActionResult GetAccessDevices()
        {
            int AccessDeviceId = 0;
            try
            {
                var user = _currentUserProvider.CurrentUser;
                IList<Dvr> objLstDevices = _userService.GetDevicesByParentType(_currentUserProvider.CurrentUser.Id, AlarmParentType.Access.ToString());

                IList<DeviceModel> objlstDevice = new List<DeviceModel>();
                objLstDevices.ToList().ForEach(x =>
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
                    objdevice.DeviceType = x.DeviceType.ToString();
                    objlstDevice.Add(objdevice);
                });

                objLstDevices = null;
                // GetDefault Selection Item
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "ACCESSCONTROL");
                int accDeviceResult = 0;

                if (Session["AccessDeviceId"] != null)
                {
                    accDeviceResult = GetDeviceStatus((int)Session["AccessDeviceId"]);
                }
                else if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
                {
                    accDeviceResult = GetDeviceStatus(lstUserDefaults.First().FilterValue);
                }

                // Get values from Session if available
                if (Session["AccessDeviceId"] != null)
                {
                    AccessDeviceId = (int)Session["AccessDeviceId"];
                    if (accDeviceResult == 1)
                    {
                        return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, DeviceType = c.DeviceType, DefaultSelectedValue = AccessDeviceId }), JsonRequestBehavior.AllowGet);
                    }
                }


                if (lstUserDefaults != null && lstUserDefaults.Count() > 0 && lstUserDefaults.First().FilterValue > 0)
                {
                    if (accDeviceResult == 1)
                    {
                        return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, DeviceType = c.DeviceType, DefaultSelectedValue = lstUserDefaults.First().FilterValue }), JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, DeviceType = c.DeviceType }), JsonRequestBehavior.AllowGet);

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

        public ActionResult SaveAccessDefaultValue(int DeviceId, string InternalName, string ControlName)
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

        public ActionResult ClearAccessDefaultValue(string InternalName, string ControlName)
        {
            try
            {
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName);
                if (lstUserDefaults.Count() > 0)
                {
                    _userDefaultService.Delete(lstUserDefaults.First().Id);
                    return Json("Defaults Cleared", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("No Defaults", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public ActionResult GetAccessDetails(int deviceId)
        {
            try
            {
                AccessViewModel model = new AccessViewModel();
                model.DoorModelList = new List<DoorModel>();
                if (deviceId > 0)
                {
                    Session["AccessDeviceId"] = deviceId;
                    Access objResult = _accessService.GetAccessDetails(deviceId);
                    BindModel(model, objResult);
                    Session["AccessObject"] = objResult;
                }
                return Json(model.DoorModelList);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public ActionResult GetPlatformAccessDetails(int deviceId)
        {
            try
            {
                AccessViewModel model = new AccessViewModel();
                model.DoorModelList = new List<DoorModel>();

                if (deviceId > 0)
                {
                    Session["AccessDeviceId"] = deviceId;
                    Access objResult = _accessService.GetPlatformAccessStatus(deviceId);                    
                    BindModel(model, objResult);                    
                    Session["AccessObject"] = objResult;
                }
                return Json(model.DoorModelList);
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
                Access objResult = (Access)Session["AccessObject"];
                Access objPollingObject = new Access();
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


        private void BindModel(AccessViewModel model, Access objResult)
        {
            DoorModel objDoorModel = new DoorModel();
            List<DoorModel> objLstDoorModel = new List<DoorModel>();
            foreach (Door door in objResult.DoorList)
            {
                objDoorModel = new DoorModel();               
                objDoorModel.DoorName = door.DoorName;
                objDoorModel.Status = door.Online;
                objDoorModel.DoorStatus = door.DoorStatus;
                objDoorModel.MomentaryUnlock = "MomentaryUnlock";
                if (door.Online.ToLower() == "online")
                    objDoorModel.Status = "Yes";
                else
                    objDoorModel.Status = "No";
                objLstDoorModel.Add(objDoorModel);
            }
            model.DoorModelList = objLstDoorModel;

        }
        public JsonResult AccessGetGroupList(int deviceId)
        {
            try
            {
                AccessGroupList objprofile = new AccessGroupList();
                var result = _accessService.AccessGetGroupList(deviceId);
                return Json(result.Select(c => new { Id = c.AccessGroupValue, Name = c.AccessGroupValue }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public JsonResult CardHolderAdd(string firstName, string lastName, string cardNumber, string pin, string accessGroupId, string activationDate, string expirationDate, string isActive, int deviceId)
        {
            try
            {
                Access objAccess = new Access();

                objAccess.CardHolderId = string.Empty;
                objAccess.firstName = firstName;
                objAccess.lastName = lastName;
                objAccess.middleName = string.Empty;
                objAccess.cardNumber = cardNumber;
                objAccess.pin = pin;
                objAccess.accessGroupId = accessGroupId;
                objAccess.cardActivationDate = activationDate;
                objAccess.cardExpirationDate = expirationDate;
                objAccess.Company = string.Empty;
                objAccess.Department = string.Empty;
                objAccess.Title = string.Empty;
                objAccess.OfficePhone = string.Empty;
                objAccess.Extension = string.Empty;
                objAccess.MobilePhone = string.Empty;
                objAccess.isActive = isActive;
                objAccess.DeviceId = deviceId;

                var result = _accessService.CardHolderAdd(objAccess);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public JsonResult GetCardholdersInformation(string firstName, string lastName, string cardNumber, int deviceId)
        {
            try
            {
                Access objAccess = new Access();
                objAccess.firstName = firstName;
                objAccess.lastName = lastName;
                objAccess.cardNumber = cardNumber;
                objAccess.DeviceId = deviceId;
                Access result = _accessService.GetCardHoldersInformation(objAccess);
                AccessViewModel model = new AccessViewModel();
                AccessCardHolderModel objAccCHModel = new AccessCardHolderModel();
                List<AccessCardHolderModel> objLstAccCHModel = new List<AccessCardHolderModel>();

                foreach (AccCHList cardHolder in result.AccessCardHolderList)
                {
                    objAccCHModel = new AccessCardHolderModel();

                    objAccCHModel.CardHolderId = cardHolder.CardHolderId;
                    objAccCHModel.firstName = cardHolder.firstName;
                    objAccCHModel.lastName = cardHolder.lastName;
                    objAccCHModel.cardNumber = cardHolder.cardNumber;
                    objAccCHModel.pin = cardHolder.pin;
                    objAccCHModel.cardActivationDate = cardHolder.cardActivationDate;
                    objAccCHModel.cardExpirationDate = cardHolder.cardExpirationDate;
                    objAccCHModel.isActive = cardHolder.isActive;
                    objAccCHModel.accessGroupId = cardHolder.accessGroupId;
                    objLstAccCHModel.Add(objAccCHModel);
                }
                model.CardHolderModelList = objLstAccCHModel;
                return Json(model.CardHolderModelList);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public JsonResult CardHolderModify(string cardHolderId, string lastName, string firstName, string cardNumber, string pin, string accessGroupId, string cardActivationDate,
                                           string cardExpirationDate, string isActive, int deviceId)
        {
            try
            {
                Access objAccess = new Access();
                objAccess.CardHolderId = cardHolderId;
                objAccess.lastName = lastName;
                objAccess.firstName = firstName;
                objAccess.middleName = string.Empty;
                objAccess.cardNumber = cardNumber;
                objAccess.DeviceId = deviceId;
                objAccess.pin = pin;
                objAccess.accessGroupId = accessGroupId;
                objAccess.cardActivationDate = cardActivationDate;
                objAccess.cardExpirationDate = cardExpirationDate;
                objAccess.Company = string.Empty;
                objAccess.Department = string.Empty;
                objAccess.Title = string.Empty;
                objAccess.OfficePhone = string.Empty;
                objAccess.Extension = string.Empty;
                objAccess.MobilePhone = string.Empty;
                objAccess.isActive = isActive;
                objAccess.DeviceId = deviceId;

                var result = _accessService.CardHolderModify(objAccess);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public JsonResult AccessGroupCreate(string Name, string Description, string BeginTime, string EndTime, string Day, string Reader, int deviceId)
        {
            try
            {
                Access objAccessgroup = new Access();
                objAccessgroup.Name = Name;
                objAccessgroup.Description = Description;
                objAccessgroup.BeginTime = BeginTime;
                objAccessgroup.EndTime = EndTime;
                objAccessgroup.Day = Day;
                objAccessgroup.Reader = Reader;
                objAccessgroup.DeviceId = deviceId;

                var resultAccessgroup = _accessService.AccessGroupCreate(objAccessgroup);
                return Json(resultAccessgroup, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }


        }
        public JsonResult dmpXRAccessGroupCreate(string Name, string Description, string BeginTime, string EndTime, string Day, string Reader, int deviceId)
        {
            try
            {
                Access objAccessgroup = new Access();
                objAccessgroup.Name = Name;
                objAccessgroup.Description = Description;
                objAccessgroup.BeginTime = BeginTime;
                objAccessgroup.EndTime = EndTime;
                objAccessgroup.Day = Day;
                objAccessgroup.Reader = Reader;
                objAccessgroup.DeviceId = deviceId;

                var resultAccessgroup = _accessService.dmpXRAccessGroupCreate(objAccessgroup);
                return Json(resultAccessgroup, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }


        }

        public JsonResult AccessGroupModify(string Name, string Description, string BeginTime, string EndTime, string Day, string Reader, int deviceId)
        {
            try
            {
                Access objAccessgroup = new Access();
                objAccessgroup.Name = Name;
                objAccessgroup.Description = Description;
                objAccessgroup.accessGroupId = Name;
                objAccessgroup.BeginTime = BeginTime;
                objAccessgroup.EndTime = EndTime;
                objAccessgroup.Day = Day;
                objAccessgroup.Reader = Reader;
                objAccessgroup.DeviceId = deviceId;
                var resultAccessgroup = _accessService.AccessGroupModify(objAccessgroup);
                return Json(resultAccessgroup, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public JsonResult dmpXRAccessGroupModify(string Name, string Description, string BeginTime, string EndTime, string Day, string Reader, int deviceId)
        {
            try
            {
                Access objAccessgroup = new Access();
                objAccessgroup.Name = Name;
                objAccessgroup.Description = Description;
                objAccessgroup.accessGroupId = Name;                
                objAccessgroup.BeginTime = "0";
                objAccessgroup.EndTime = "0";
                objAccessgroup.Day = Day;
                objAccessgroup.Reader = Reader;
                objAccessgroup.DeviceId = deviceId;
                
                var resultAccessgroup = _accessService.dmpXRAccessGroupModify(objAccessgroup);
                return Json(resultAccessgroup, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }


        }
        public JsonResult AccessGroupDelete(string accessgroupId, int deviceId)
        {
            try
            {
                Access objAccessgroup = new Access();
                objAccessgroup.accessGroupId = accessgroupId;
                objAccessgroup.DeviceId = deviceId;

                var resultAccessgroup = _accessService.AccessGroupDelete(objAccessgroup);
                return Json(resultAccessgroup, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        #region CardHolderDelete
        public JsonResult CardHolderDelete(string firstName, string lastName, string cardNumber, string cardHolderId, int deviceId)
        {
            Access objAccess = new Access();
            objAccess.firstName = firstName;
            objAccess.lastName = lastName;
            objAccess.cardNumber = cardNumber;
            objAccess.CardHolderId = cardHolderId;

            objAccess.DeviceId = deviceId;
            var result = _accessService.CardHolderDelete(objAccess);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult GetAccessControlReport(string startDateTime, string endDateTime, int deviceId)
        {
            try
            {
                Access objAccess = new Access();
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


                objAccess.startDateTime = startDate;
                objAccess.endDateTime = endDate;


                objAccess.DeviceId = deviceId;

                Access result = _accessService.GetAccessControlReport(objAccess);
                AccessViewModel model = new AccessViewModel();
                AccessReportModel objAccReportModel = new AccessReportModel();

                List<AccessReportModel> objLstAccReportModel = new List<AccessReportModel>();

                foreach (AccReportList report in result.ReportList)
                {
                    objAccReportModel = new AccessReportModel();
                    objAccReportModel.Acctype = report.Acctype;
                    objAccReportModel.Accdatetime = report.Accdatetime;
                    objAccReportModel.Accuser = report.Accuser;
                    objAccReportModel.Accmessage = report.Accmessage;
                    objLstAccReportModel.Add(objAccReportModel);

                }
                model.AccReportModelList = objLstAccReportModel;
                return Json(model.AccReportModelList);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }


        public JsonResult GetReadersList(int deviceId)
        {
            try
            {
                AccessGroupList objprofile = new AccessGroupList();
                var result = _accessService.GetReadersList(deviceId);
                return Json(result.Select(c => new { Id = c.value, Name = c.name }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public JsonResult AccessMomentaryOpenDoor(string readerId, int deviceId)
        {
            try
            {
                Access objAccess = new Access();

                objAccess.Reader = readerId;
                objAccess.DeviceId = deviceId;

                var result = _accessService.AccessMomentaryOpenDoor(objAccess);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        public ActionResult ValidateUserPin(string UserPin, string submitVal, int deviceId)
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
                    objIntrusion.DeviceId = deviceId;
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
        #region GetAccessGroupInformation
        public JsonResult GetAccessGroupInformation(string AGroupId, int deviceId)
        {
            try
            {
                Access objAccess = new Access();
                objAccess.accessGroupId = AGroupId;
                objAccess.DeviceId = deviceId;
                var result = _accessService.GetAccessGroupInformation(objAccess);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        #endregion

    }
}
