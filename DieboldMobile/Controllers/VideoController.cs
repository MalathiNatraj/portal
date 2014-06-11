using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Contracts;

namespace DieboldMobile.Controllers
{
    public class VideoController : Controller
    {

        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly Diebold.Services.Contracts.IUserService _objUserService;
        protected readonly Diebold.Services.Contracts.IDeviceService _deviceService;
        protected readonly IUserDefaultsService _userDefaultService;
        protected readonly IDvrService _dvrService;
        public VideoController(IUserService userService, IDvrService dvrService, ICurrentUserProvider currentUserProvider, IDeviceService deviceService, IUserDefaultsService userDefaultService)
        {
            _currentUserProvider = currentUserProvider;
            _objUserService = userService;
            _deviceService = deviceService;
            _dvrService = dvrService;
            _userDefaultService = userDefaultService;
        }

        //
        // GET: /Video/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LiveView()
        {
            return View();
        }

        public ActionResult GetLIVEVIEWValue(string InternalName)
        {
            IList<Diebold.Domain.Entities.UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName);
            if (lstUserDefaults.Count() > 0)
            {
                var server = lstUserDefaults.Where(x => x.FilterName.Equals("ServerList")).First().AlertType;
                var camera = lstUserDefaults.Where(x => x.FilterName.Equals("CameraList")).First().AlertType;
                return Json(server + "~" + camera, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

    }
}
