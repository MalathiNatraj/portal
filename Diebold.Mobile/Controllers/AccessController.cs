using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using DieboldMobile.Models;
using DieboldMobile.Infrastructure.Authentication;
using DieboldMobile.Services;

namespace DieboldMobile.Controllers
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

        public AccessController(IUserService userService, ICurrentUserProvider currentUserProvider, IUserDefaultsService userDefaultService, IDvrService dvrService, IAccessService accessService)
        {
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _userDefaultService = userDefaultService;
            _dvrService = dvrService;
            _accessService = accessService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccessView()
        {
            return View();
        }

        public ActionResult GetAccessDevices()
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
                    objlstDevice.Add(objdevice);
                });

                objLstDevices = null;

                // GetDefault Selection Item
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "ACCESSCONTROL");
                if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
                {
                    return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, DefaultSelectedValue = lstUserDefaults.First().FilterValue }), JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);

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

        public ActionResult ValidateUserPin(string UserPin, string submitVal)
        {
            try
            {
                User objuser = _userService.Get(_currentUserProvider.CurrentUser.Id);
                string CurrentUserPin = objuser.UserPin;
                var result = string.Empty;
                if (UserPin.Equals(objuser.UserPin))
                {
                    result = "true";
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
    }
}
