using System;
using System.Configuration;
using System.Web.Mvc;
using Diebold.WebApp.Models;
using System.Collections.Generic;
using System.Linq;
using Diebold.Services.Contracts;
using Diebold.Services.Impl;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.WebApp.Infrastructure.Helpers;
using Diebold.Services.Exceptions;
using Diebold.WebApp.Infrastructure.Authentication;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.WebApp.Controllers
{
    public class VideoController : BaseController
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserService _objUserService;
        protected readonly IDeviceService _deviceService;
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

        public ActionResult Index()
        {
            ViewBag.IpConfigureURL = ConfigurationManager.AppSettings["IpConfigureURL"];

            return View();
        }
        public JsonResult GetDevicesByUserforSearch()
        {
            try
            {
                IList<Dvr> objLstDevices = _objUserService.GetDevicesByParentType(_currentUserProvider.CurrentUser.Id, AlarmParentType.DVR.ToString());
                IList<DeviceModel> objlstDevice = new List<DeviceModel>();
                objLstDevices.ToList().ForEach(x =>
                {
                    DeviceModel objdevice = new DeviceModel();
                    objdevice.Id = x.Id;
                    objdevice.Name = x.Name;
                    objdevice.SiteName = x.Site.Name;
                    objdevice.Address1 = x.Site.Address1;
                    objdevice.City = x.Site.City;
                    objdevice.State = x.Site.State;
                    objdevice.Zip = x.Site.Zip;
                    objlstDevice.Add(objdevice);
                });
                return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, City = c.City, State = c.State, Zip = c.Zip }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }
        public JsonResult GetDevicesByUser()
        {
            int VideoDeviceId = 0;
            string SelectedVideoDeviceName = string.Empty;
            try
            {
                IList<Dvr> objLstDevices = _objUserService.GetDevicesByParentType(_currentUserProvider.CurrentUser.Id, AlarmParentType.DVR.ToString());
                IList<DeviceModel> objlstDevice = new List<DeviceModel>();
                objLstDevices.ToList().ForEach(x =>
                {
                    DeviceModel objdevice = new DeviceModel();
                    objdevice.Id = x.Id;
                    objdevice.Name = x.Name;
                    objdevice.SiteName = x.Site.Name;
                    objdevice.Address1 = x.Site.Address1;
                    objdevice.City = x.Site.City;
                    objdevice.State = x.Site.State;
                    objdevice.Zip = x.Site.Zip;
                    objlstDevice.Add(objdevice);
                });

                objLstDevices = null;
                // GetDefault Selection Item
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "VIDEOHEALTHCHECK");
                int videoDeviceResult = 0;

                if (Session["SelectedVideoDeviceId"] != null)
                {
                    videoDeviceResult = GetDeviceStatus((int)Session["SelectedVideoDeviceId"]);
                }
                else if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
                {
                    videoDeviceResult = GetDeviceStatus(lstUserDefaults.First().FilterValue);
                }


                // Get Value from Session and return 
                if (Session["SelectedVideoDeviceId"] != null)
                {
                    VideoDeviceId = (int)Session["SelectedVideoDeviceId"];
                    if (videoDeviceResult == 1)
                    {
                        return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, City = c.City, State = c.State, Zip = c.Zip, DefaultSelectedValue = VideoDeviceId }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
                    }
                }


                if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
                {
                    if (videoDeviceResult == 1)
                    {
                        return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, City = c.City, State = c.State, Zip = c.Zip, DefaultSelectedValue = lstUserDefaults.First().FilterValue }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, City = c.City, State = c.State, Zip = c.Zip }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
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

        public ActionResult SaveDefaultValue(int DeviceId, string InternalName, string ControlName)
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
                    objUserDefaults.User = _objUserService.Get(_currentUserProvider.CurrentUser.Id);
                    _userDefaultService.Create(objUserDefaults);
                }
                return Json("RecordModified", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult ClearVHCDefaultValue(string InternalName, string ControlName)
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
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }
        #region Current Status

        //GET: /Dashboard/CurrentReadings
        public ActionResult GetDevicePollingStatus(int DeviceId)
        {
            try
            {
                // logger.Debug("GetDevicePollingStatus Method started with Device Id " + DeviceId);
                bool liveFromDevice = false;
                if (DeviceId > 0)
                {
                    // logger.Debug("Get Method started to Get device Details with Device Id " + DeviceId);
                    Session["SelectedVideoDeviceId"] = DeviceId;
                    var device = _dvrService.Get(DeviceId);
                    // logger.Debug("Get Method completed to Get device Details");
                    ViewBag.DeviceId = DeviceId;
                    ViewBag.DeviceName = device.Name;
                    ViewBag.ShowViewMore = true;
                    // logger.Debug("GetLiveStatus Method started with Device Id : " + DeviceId + "Mac Address : " + device.Gateway.MacAddress + "Device Key : " + device.DeviceKey + " Live View Device :" + liveFromDevice);
                    StatusDTO status = _deviceService.GetLiveStatus(DeviceId, device.Gateway.MacAddress + "-" + device.DeviceKey, liveFromDevice, false);
                    // logger.Debug("GetLiveStatus Method Completed");
                    VideoHealthCheckModel videoHealthModel = new VideoHealthCheckModel();
                    if (status != null)
                    {
                        if (status.isGateWay == "NO")
                        {
                            // logger.Debug("Is gateway : " + status.isGateWay);
                            if (status.payload != null && status.payload.SparkDvrReport != null && status.payload.SparkDvrReport.properties != null && status.payload.SparkDvrReport.properties.property != null)
                            {
                                videoHealthModel.Id = DeviceId;
                                var user = _currentUserProvider.CurrentUser;
                                // logger.Debug("Extracting the data from the status started");
                                foreach (var property in status.payload.SparkDvrReport.properties.property)
                                {
                                    if (property.name == "isNotRecording")
                                    {
                                        if (property.value == "true")
                                        {
                                            videoHealthModel.IsCurrentlyRecording = false;
                                        }
                                        else
                                        {
                                            videoHealthModel.IsCurrentlyRecording = true;
                                        }
                                    }
                                    if (property.name == "networkDown")
                                    {
                                        if (property.value == "true")
                                        {
                                            videoHealthModel.Status = "Not Connected";
                                        }
                                        else
                                        {
                                            videoHealthModel.Status = "Connected";
                                        }
                                    }
                                    if (property.name == "daysRecorded")
                                    {
                                        videoHealthModel.DaysRecorded = Convert.ToInt32(property.value);
                                    }
                                    if (property.name == "timeStampRecorder")
                                    {
                                        videoHealthModel.LastUpdated = Convert.ToDateTime(property.value);
                                        videoHealthModel.LastUpdatedS = videoHealthModel.LastUpdated.ToString("MM-dd-yyyy HH:mm:ss");
                                    }
                                    videoHealthModel.PollingStatus = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone)).ToString();
                                }
                                // logger.Debug("Extracting the data from the status completed");
                            }
                        }
                    }
                    //logger.Debug("Get Method started to Get device Details completed");
                    return Json(videoHealthModel, JsonRequestBehavior.AllowGet);
                }
                ViewBag.ShowViewMore = false;
                return new EmptyResult();
            }
            catch (ServiceException ex)
            {
                LogError("Exception occured while getting device polling status", ex);
                return JsonError(ex.Message);
            }
        }

        #endregion

        public ActionResult GetPlatformDevicePollingStatus(int DeviceId)
        {
            try
            {
                bool liveFromDevice = false;
                if (DeviceId > 0)
                {
                    Session["SelectedVideoDeviceId"] = DeviceId;
                    var device = _dvrService.Get(DeviceId);
                    ViewBag.DeviceId = DeviceId;
                    ViewBag.DeviceName = device.Name;
                    ViewBag.ShowViewMore = true;
                    StatusPlatformDTO status = _deviceService.GetPlatformLiveStatus(DeviceId, device.ExternalDeviceId, liveFromDevice, false);
                    VideoHealthCheckModel videoHealthModel = new VideoHealthCheckModel();
                    if (status != null)
                    {
                        if (status.isGateWay == "NO")
                        {
                            if (status.payload != null && status.payload.SparkDvrReport != null && status.payload.SparkDvrReport.properties != null && status.payload.SparkDvrReport.properties.property != null)
                            {
                                videoHealthModel.Id = DeviceId;
                                var user = _currentUserProvider.CurrentUser;
                                PlatformDeviceProperty objDeviceProperty = status.payload.SparkDvrReport.properties.property;
                                if (objDeviceProperty != null)
                                {
                                    if (!String.IsNullOrEmpty(objDeviceProperty.isNotRecording))
                                    {
                                        if (objDeviceProperty.isNotRecording == "true")
                                        {
                                            videoHealthModel.IsCurrentlyRecording = false;
                                        }
                                        else
                                        {
                                            videoHealthModel.IsCurrentlyRecording = true;
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(objDeviceProperty.networkDown))
                                    {
                                        if (objDeviceProperty.networkDown == "true")
                                        {
                                            videoHealthModel.Status = "Not Connected";
                                        }
                                        else
                                        {
                                            videoHealthModel.Status = "Connected";
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(objDeviceProperty.daysRecorded))
                                    {
                                        videoHealthModel.DaysRecorded = Convert.ToInt32(objDeviceProperty.daysRecorded);
                                    }
                                    if (!String.IsNullOrEmpty(objDeviceProperty.timeStampRecorder))
                                    {
                                        videoHealthModel.LastUpdated = Convert.ToDateTime(objDeviceProperty.timeStampRecorder);
                                        videoHealthModel.LastUpdatedS = videoHealthModel.LastUpdated.ToString("MM-dd-yyyy HH:mm:ss");
                                    }
                                }
                                videoHealthModel.PollingStatus = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone)).ToString();
                            }
                        }
                    }
                    return Json(videoHealthModel, JsonRequestBehavior.AllowGet);
                }
                ViewBag.ShowViewMore = false;
                return new EmptyResult();
            }
            catch (ServiceException ex)
            {
                LogError("Exception occured while getting device polling status", ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult SaveLiveViewDefaults(string DefaultValue, string InternalName, string ControlName)
        {
            try
            {
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName, ControlName);
                if (lstUserDefaults.Count() > 0)
                {
                    lstUserDefaults.First().AlertType = DefaultValue; // Using Alert type here because we get the values as string
                    _userDefaultService.Update(lstUserDefaults.First());
                }
                else
                {
                    UserDefaults objUserDefaults = new UserDefaults();
                    objUserDefaults.FilterName = ControlName;
                    objUserDefaults.AlertType = DefaultValue; // Using Alert type here because we get the values as string
                    objUserDefaults.InternalName = InternalName;
                    objUserDefaults.User = _objUserService.Get(_currentUserProvider.CurrentUser.Id);
                    _userDefaultService.Create(objUserDefaults);
                }
                return Json("RecordModified", JsonRequestBehavior.AllowGet);
            }
            catch (ServiceException ex)
            {
                LogError("Exception occured while getting device polling status", ex);
                return JsonError(ex.Message);
            }
        }
        public ActionResult ClearLiveViewDefaults(string InternalName, string ControlName)
        {
            try
            {
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName, ControlName);
                if (lstUserDefaults.Count() > 0)
                {
                    _userDefaultService.Delete(lstUserDefaults.First().Id);
                    return Json("Defaults Cleared", JsonRequestBehavior.AllowGet);
                }
                return Json("No Defaults", JsonRequestBehavior.AllowGet);                
            }
            catch (ServiceException ex)
            {
                LogError("Exception occured while clearing defaults in Live View", ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult GetLIVEVIEWValue(string InternalName)
        {
            try
            {
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName);
                if (lstUserDefaults.Count() > 0)
                {
                    var server = lstUserDefaults.Where(x => x.FilterName.Equals("ServerList")).First().AlertType;
                    var camera = lstUserDefaults.Where(x => x.FilterName.Equals("CameraList")).First().AlertType;
                    return Json(server + "~" + camera, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (ServiceException ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }


        [AllowAnonymous]
        public ActionResult CameraTwo(string cameraId)
        {
            try
            {
                if (string.IsNullOrEmpty(cameraId) == true)
                    ViewBag.CameraId = "";
                else
                    ViewBag.CameraId = cameraId;
                ViewBag.IPConfigUrl = ConfigurationManager.AppSettings["IPConfugureManagementUri"].ToString();
                return View();
            }
            catch (ServiceException ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

    }
}
