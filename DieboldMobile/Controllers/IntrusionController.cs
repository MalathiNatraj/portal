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

namespace DieboldMobile.Controllers
{
    public class IntrusionController : Controller
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
            var user = _currentUserProvider.CurrentUser;
            var monitoredDeviceList = _userService.GetMonitoringDevicesByUser(user.Id, null);

            // Intrusion Device Types
            List<int> lstDeviceId = _dvrService.GetAll().Where(x => x.DeviceType.Equals(DeviceType.dmpXR100)).Select(x => x.Id).ToList();
            List<int> lstIntrusionDeviceId = _dvrService.GetAll().Where(x => x.DeviceType.Equals(DeviceType.dmpXR500)).Select(x => x.Id).ToList();

            lstDeviceId.AddRange(lstIntrusionDeviceId);

            var resultSet = from a in monitoredDeviceList
                            where lstDeviceId.Contains(a.Id)
                            orderby a.Name
                            select a;


            IList<DeviceModel> objlstDevice = new List<DeviceModel>();
            resultSet.ToList().ForEach(x =>
            {
                DeviceModel objdevice = new DeviceModel();
                objdevice.Id = x.Id;
                objdevice.Name = x.Name;
                objdevice.SiteId = x.Site.Id;
                objdevice.SiteName = x.Site.Name;
                objdevice.Address1 = x.Site.Address1;
                objdevice.Address2 = x.Site.Address2;
                objdevice.City = x.Site.City;
                objdevice.State = x.Site.State;
                objdevice.Zip = x.Site.Zip;
                objlstDevice.Add(objdevice);
            });

            resultSet = null;

            // GetDefault Selection Item
            IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "INTRUSION");
            if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
            {
                return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, DefaultSelectedValue = lstUserDefaults.First().FilterValue }), JsonRequestBehavior.AllowGet);
            }
            return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetIntrusionDetails(int deviceId)
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

                if (area.Armed == true)
                    objAreaModel.Status = "Armed";
                else
                    objAreaModel.Status = "DisArmed";

                if (area.LateStatus == true)
                    objAreaModel.Online = "Yes";
                else
                    objAreaModel.Online = "No";
                objLstAreaModel.Add(objAreaModel);

            }
            model.AreModelList = objLstAreaModel;
        }

    }
}
