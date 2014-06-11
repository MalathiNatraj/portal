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
    public class AccessController : Controller
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
            var user = _currentUserProvider.CurrentUser;
            var monitoredDeviceList = _userService.GetMonitoringDevicesByUser(user.Id, null);
            List<int> lstDeviceId = _dvrService.GetAll().Where(x => x.DeviceType.Equals(DeviceType.eData300)).Select(x => x.Id).ToList();
            List<int> lstAccessDeviceId = _dvrService.GetAll().Where(x => x.DeviceType.Equals(DeviceType.eData524)).Select(x => x.Id).ToList();

            lstDeviceId.AddRange(lstAccessDeviceId);
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
            IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "ACCESSCONTROL");
            if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
            {
                return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, DefaultSelectedValue = lstUserDefaults.First().FilterValue }), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);

        }
    }
}
