using System;
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using Diebold.Services.Contracts;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.WebApp.Models;
using Diebold.Domain.Entities;
using Diebold.Services.Exceptions;
using System.Collections.Generic;
using Diebold.Domain.Exceptions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Diebold.WebApp.Controllers
{
    public class DeviceController : BaseCRUDTrackeableController<Dvr, DeviceViewModel>
    {
        private readonly IDvrService _deviceService;
        private readonly IGatewayService _gatewayService;
        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;
        private readonly IAlarmConfigurationService _alarmConfigurationService;
        private readonly IAlertService _alertService;
        private static volatile object objDeviceKeyGeneration = new object();

        public DeviceController(IDvrService deviceService, IGatewayService gatewayService, ICompanyService companyService, ISiteService siteService,
                                IAlarmConfigurationService alarmConfigurationService, IAlertService alertService)
            : base(deviceService)
        {
            _deviceService = deviceService;
            _gatewayService = gatewayService;
            _companyService = companyService;
            _siteService = siteService;
            _alarmConfigurationService = alarmConfigurationService;
            _alertService = alertService;
        }

        private DeviceViewModel InitializeViewModelAndCollections(DeviceViewModel item, IEnumerable<CameraViewModel> cameras,
                                                                  IEnumerable<AlarmConfigurationViewModel> alarms)
        {
            foreach (var almConfig in alarms)
            {
                almConfig.AvailableAlarmSeverityList = _alarmConfigurationService.GetAllAlarmSeverities();
                almConfig.AvailableAlarmOperatorList = _alarmConfigurationService.GetAllAlarmOperators();
            }

            @ViewBag.AlarmConfigurationModel = alarms;
            @ViewBag.CameraModel = cameras;
            return InitializeViewModel(item);
        }

        private DeviceViewModelForEdit InitializeViewModelAndCollectionsForEdit(DeviceViewModelForEdit item, Dvr itemToEdit, IEnumerable<CameraViewModel> cameras,
                                                                  IEnumerable<AlarmConfigurationViewModel> alarms)
        {
            foreach (var almConfig in alarms)
            {
                almConfig.AvailableAlarmSeverityList = _alarmConfigurationService.GetAllAlarmSeverities();
                almConfig.AvailableAlarmOperatorList = _alarmConfigurationService.GetAllAlarmOperators();
            }

            @ViewBag.AlarmConfigurationModel = alarms;
            @ViewBag.CameraModel = cameras;

            item.AvailableDeviceTypeList = this._deviceService.GetAllDeviceTypes();
            item.AvailableCompanyList = this._companyService.GetAll();
            item.AvailableGatewayList = this._gatewayService.GetAll();
            item.AvailablePollingFrequencyList = this._deviceService.GetAllPollingFrequencies();
            item.AvailableHealthCheckVersionList = this._deviceService.GetAllHealthCheckVersions();
            item.AvailableTimeZoneList = this._deviceService.GetAllTimeZones();
            item.AvailableSiteList = this._siteService.GetAll();

            if (itemToEdit != null)
            {
                item.DeviceKey = itemToEdit.DeviceKey;
                item.ZoneNumber = itemToEdit.ZoneNumber;
            }

            return item;
        }

        protected override DeviceViewModel InitializeViewModel(DeviceViewModel item)
        {
            item.AvailableDeviceTypeList = this._deviceService.GetAllDeviceTypes();
            item.AvailableCompanyList = this._companyService.GetAll();
            item.AvailableGatewayList = this._gatewayService.GetAll();
            item.AvailablePollingFrequencyList = this._deviceService.GetAllPollingFrequencies();
            item.AvailableHealthCheckVersionList = this._deviceService.GetAllHealthCheckVersions();
            item.AvailableTimeZoneList = this._deviceService.GetAllTimeZones();
            item.AvailableSiteList = this._siteService.GetAll();

            return item;
        }

        public override ActionResult Create()
        {
            try
            {
                ViewBag.CameraModel = new List<CameraViewModel>();
                ViewBag.AlarmConfigurationModel = new List<AlarmConfigurationViewModel>();
                if (TempData["maxdevice"] != null)
                {
                    ViewBag.MaxDeviceId = (int)TempData["maxdevice"] + 1;
                }
                else {
                    TempData["maxdevice"] = _deviceService.GetAllDevicesforMaxId();
                    ViewBag.MaxDeviceId = (int)TempData["maxdevice"] + 1;
                }
                var model = new DeviceViewModel
                {
                    AvailableDeviceTypeList = new List<string>(),
                    AvailableGatewayList = this._gatewayService.GetAll(),
                    AvailableSiteList = new List<Site>(),
                    AvailableCompanyList = this._companyService.GetAllEnabled(),
                    AvailablePollingFrequencyList = this._deviceService.GetAllPollingFrequencies(),
                    AvailableHealthCheckVersionList = this._deviceService.GetAllHealthCheckVersions(),
                    AvailableTimeZoneList = this._deviceService.GetAllTimeZones()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public override ActionResult Edit(int id)
        {
            try
            {
                var device = _deviceService.Get(id);
                var cameraModels = device.Cameras.Select(cam => new CameraViewModel
                                                                    {
                                                                        Id = cam.Id,
                                                                        Active = cam.Active,
                                                                        CameraName = cam.Name,
                                                                        Channel = cam.Channel
                                                                    }).OrderBy(x => x.Id).ToList();

                ViewBag.CameraModel = cameraModels;
                Session["cameramodel"] = cameraModels;
                var alarms = _alarmConfigurationService.GetAlarmConfigurationForEdit(device.DeviceType, device);
                ViewBag.AlarmConfigurationModel = MapAlarmConfigurationEntity(alarms);

                DeviceViewModelForEdit itemToEdit = MapEntityForEdit(device);
                if(!string.IsNullOrEmpty(itemToEdit.UserName))
                itemToEdit.UserName = _deviceService.Decrypt(itemToEdit.UserName, "username");
                if (!string.IsNullOrEmpty(itemToEdit.Password))
                itemToEdit.Password = _deviceService.Decrypt(itemToEdit.Password, "password");
                itemToEdit.AvailableDeviceTypeList = HealthCheckDeviceTypeRelation.GetDeviceTypeNames(device.HealthCheckVersion);
                itemToEdit.AvailableCompanyList = _companyService.GetAllEnabled();
                itemToEdit.AvailableSiteList = _siteService.GetSitesByCompany(device.Company.Id, true);
                itemToEdit.AvailableGatewayList = _gatewayService.GetGatewaysByCompanyId(device.Company.Id, true);
                itemToEdit.AvailablePollingFrequencyList = _deviceService.GetAllPollingFrequencies();
                itemToEdit.AvailableHealthCheckVersionList = _deviceService.GetAllHealthCheckVersions();
                itemToEdit.AvailableTimeZoneList = _deviceService.GetAllTimeZones();

                return View(itemToEdit);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public override ActionResult Index()
        {
            return View();
        }

        [NonAction]
        public override ActionResult Create(DeviceViewModel newItem)
        {
            return View();
        }

        [NonAction]
        public override ActionResult Edit(int id, DeviceViewModel editedItem)
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(DeviceViewModel newItem, IEnumerable<CameraViewModel> cameras, IEnumerable<AlarmConfigurationViewModel> alarms)
        {
            logger.Debug("Create Method Called");
            try
            {
                if (cameras == null)
                {
                    cameras = (IEnumerable<CameraViewModel>)Session["cameramodel"];
                }
                logger.Debug("Create Device Method Started");
                return CreateDevice(newItem, cameras, alarms, false);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateAnother(DeviceViewModel newItem, IEnumerable<CameraViewModel> cameras, IEnumerable<AlarmConfigurationViewModel> alarms)
        {
            return CreateDevice(newItem, cameras, alarms, true);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(DeviceViewModelForEdit editedItem, int id, IEnumerable<CameraViewModel> cameras, IEnumerable<AlarmConfigurationViewModel> alarms)
        {
            try
            {
                Dvr itemToEdit = null;
                if (ModelState.IsValid)
                {
                    try
                    {
                        itemToEdit = _deviceService.Get(id);
                        IEnumerable<Camera> Cameras = itemToEdit.Cameras;
                        var cameraModels = itemToEdit.Cameras.Select(cam => new CameraViewModel
                        {
                            Id = cam.Id,
                            Active = cam.Active,
                            CameraName = cam.Name,
                            Channel = cam.Channel
                        }).OrderBy(x => x.Id).ToList();

                        int NoOfCameras = itemToEdit.NumberOfCameras;
                        var itemFromView = editedItem.MapFromViewModel();

                        Company company = new Company();
                        company.Id = itemToEdit.Company.Id;
                        company.Name = itemToEdit.Company.Name;
                        itemFromView.Company = company;

                        Gateway gateway = new Gateway();
                        gateway.Id = itemToEdit.Id;
                        gateway.Name = itemToEdit.Name;
                        Site objSite = _siteService.Get(itemToEdit.Site.Id);
                        itemFromView.Gateway = gateway;
                        itemFromView.HealthCheckVersion = itemToEdit.HealthCheckVersion;
                        itemFromView.DeviceType = itemToEdit.DeviceType;
                        itemFromView.Site = objSite;
                        itemFromView.Cameras = ((IList<CameraViewModel>)Session["cameramodel"]).Select(x => new Camera
                        {
                            Id = x.Id,
                            Name = x.CameraName,
                            Active = x.Active,
                            Channel = x.Channel,
                        }).ToList();
                        MergeDeviceItems(itemToEdit, itemFromView);
                        IList<string> UpdatedCamera = new List<string>();
                        IEnumerable<Camera> RemovedCamera;
                        if (NoOfCameras > editedItem.NumberOfCameras)
                        {
                            List<CameraViewModel> AttachedCamera = new List<CameraViewModel>();
                            if (cameras != null)
                                AttachedCamera = cameras.ToList();

                            if (editedItem.UpdatedCamera != null && editedItem.UpdatedCamera != "")
                            {
                                var NumberOfCamera = editedItem.UpdatedCamera.Split(',');
                                NumberOfCamera.ToList().ForEach(x => UpdatedCamera.Add(x));
                            }
                            IEnumerable<CameraViewModel> UpdatedCams = (IEnumerable<CameraViewModel>)cameraModels.Where(x => UpdatedCamera.Any(y => y.ToString() == x.Channel.ToString())).ToList();
                            RemovedCamera = (IEnumerable<Camera>)Cameras.Where(x => !UpdatedCamera.Any(y => y.ToString().Equals(x.Channel.ToString()))).ToList();
                            UpdatedCams.ToList().ForEach(x =>
                            {
                                if (AttachedCamera.ToList().Where(y => y.Channel.Equals(x.Channel)).FirstOrDefault() == null)
                                {
                                    AttachedCamera.Add(x);
                                }
                            });
                            cameras = (IEnumerable<CameraViewModel>)AttachedCamera.ToList();
                            string strRemovedCameras = null;
                            RemovedCamera.ToList().ForEach(x =>
                            {
                                if (string.IsNullOrEmpty(strRemovedCameras))
                                    strRemovedCameras = x.Channel;
                                else
                                    strRemovedCameras += "," + x.Channel;
                            });

                            itemToEdit.RemovedCameras = strRemovedCameras;
                        }
                        if (cameras != null)
                            _deviceService.SetCameras(id, MapCameraEntities(cameras));

                        if (alarms != null)
                            alarms.ToList().ForEach(x => x.CompanyId = company.Id);
                        _deviceService.SetAlarmConfigurations(id, MapAlarmConfigurationEntities(alarms));
                        if (alarms != null)                            
                        PopulateDeviceAlarmConfiguration(itemToEdit, alarms);
                        _deviceService.Update(itemToEdit);

                        return RedirectToAction("Index");
                    }
                    catch (ServiceException serviceException)
                    {
                        if (serviceException.InnerException != null)
                        {
                            if (serviceException.InnerException is ValidationException)
                                AddModelErrors((ValidationException)serviceException.InnerException);
                            else if (serviceException.InnerException is RepositoryException)
                            {
                                LogError("Repository Exception occured while editing camera", serviceException);
                                ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                            }
                            else
                            {
                                LogError("Service Exception occured while editing camera", serviceException);
                                ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogError("Exception occured while editing camera", e);
                        ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));
                    }
                }

                InitializeViewModelAndCollectionsForEdit(editedItem, itemToEdit, cameras, alarms);

                return View(editedItem);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }       

        private ActionResult CreateDevice(DeviceViewModel newItem, IEnumerable<CameraViewModel> cameras, IEnumerable<AlarmConfigurationViewModel> alarms, bool createAnother)
        {
            if (HttpContext.Request.Form.AllKeys.ToList().Contains("hHealthCheckVersion")) {
                var version = HttpContext.Request.Form["hHealthCheckVersion"];
                newItem.HealthCheckVersion = version;
                ModelState["HealthCheckVersion"].Errors.Clear();
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            logger.Debug("CreateDevice Method started");
            if (ModelState.IsValid)
            {
                try
                {
                    lock (objDeviceKeyGeneration)
                    {
                        logger.Debug("Started Getting new devicekey inside Create Device Method.");
                        ViewBag.MaxDeviceId = _deviceService.GetAllDevicesforMaxId();
                        TempData["maxdevice"] = ViewBag.MaxDeviceId;
                        newItem.DeviceKey = Convert.ToString(ViewBag.MaxDeviceId + 1);
                        logger.Debug("New devicekey inside Create Device Method: " + newItem.DeviceKey);
                        logger.Debug("Getting new devicekey inside Create Device Method completed.");
                        logger.Debug("ModelState is Valid ");
                        var item = newItem.MapFromViewModel();

                        if (cameras != null)
                            item.Cameras = MapCameraEntities(cameras);

                        if (alarms != null)
                            PopulateDeviceAlarmConfiguration(item, alarms);
                        logger.Debug("ValidateDeviceKey Method Started with Gateway Id :" + item.Gateway.Id.ToString() + " and Device Key " + item.DeviceKey);
                        _deviceService.ValidateDeviceKey(item.Gateway.Id, item.DeviceKey);
                        logger.Debug("ValidateDeviceKey Method Ended and Create Method Started");
                        _service.Create(item);
                        logger.Debug("Create Method Ended");
                        if (!createAnother)
                        {
                            return RedirectToAction("Index");
                        }

                        ViewBag.AnotherDeviceCreated = true;
                        logger.Debug("CreateDevice Method ended");
                    }
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        else if (serviceException.InnerException is RepositoryException)
                        {
                            LogError("Repository Exception occured while creating device", serviceException);
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                        }
                        else
                        {
                            LogError("Service Exception occured while editing camera", serviceException);
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                        }
                    }
                }
                catch (ValidationException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
                catch (Exception e)
                {
                    LogError("Exception occured while editing camera", e);
                    ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));
                }
            }

            InitializeViewModelAndCollections(newItem, cameras, alarms);
            return View("Create", newItem);
        }

        private void PopulateDeviceAlarmConfiguration(Dvr item, IEnumerable<AlarmConfigurationViewModel> alarms)
        {
            IList<AlarmConfiguration> alarmConfiguration = MapAlarmConfigurationEntities(alarms);
            item.AlarmConfigurations = new List<AlarmConfiguration>();
            AlarmConfiguration objAlarmConfiguration = null;
            foreach (var alarmConfig in alarmConfiguration)
            {
                objAlarmConfiguration = new AlarmConfiguration();
                objAlarmConfiguration.Ack = alarmConfig.Ack;
                string devicetype = alarmConfig.DeviceType.ToString().ToLower();
                if (devicetype == DeviceType.dmpXR100.ToString().ToLower() || devicetype == DeviceType.dmpXR500.ToString().ToLower() || devicetype == DeviceType.bosch_D9412GV4.ToString().ToLower() || devicetype == DeviceType.videofied01.ToString().ToLower())
                {
                    objAlarmConfiguration.AlarmParentType = AlarmParentType.Intrusion;
                }
                else if (devicetype == DeviceType.eData300.ToString().ToLower() || devicetype == DeviceType.eData524.ToString().ToLower() || devicetype == DeviceType.dmpXR100Access.ToString().ToLower() || devicetype == DeviceType.dmpXR500Access.ToString().ToLower())
                {
                    objAlarmConfiguration.AlarmParentType = AlarmParentType.Access;
                }
                else if (devicetype == DeviceType.Costar111.ToString().ToLower() || devicetype == DeviceType.ipConfigure530.ToString().ToLower() || devicetype == DeviceType.VerintEdgeVr200.ToString().ToLower())
                {
                    objAlarmConfiguration.AlarmParentType = AlarmParentType.DVR;
                }
                else
                {
                    objAlarmConfiguration.AlarmParentType = alarmConfig.AlarmParentType;
                }
                objAlarmConfiguration.AlarmType = alarmConfig.AlarmType;
                objAlarmConfiguration.Company = alarmConfig.Company;
                objAlarmConfiguration.CompanyId = alarmConfig.CompanyId;
                objAlarmConfiguration.DataType = alarmConfig.DataType;
                objAlarmConfiguration.Device = alarmConfig.Device;
                objAlarmConfiguration.DeviceType = alarmConfig.DeviceType;
                objAlarmConfiguration.Display = alarmConfig.Display;
                objAlarmConfiguration.Email = alarmConfig.Email;
                objAlarmConfiguration.Emc = alarmConfig.Emc;
                objAlarmConfiguration.Log = alarmConfig.Log;
                objAlarmConfiguration.Operator = alarmConfig.Operator;
                objAlarmConfiguration.Severity = alarmConfig.Severity;
                objAlarmConfiguration.Sms = alarmConfig.Sms;
                objAlarmConfiguration.Threshold = alarmConfig.Threshold;
                item.AlarmConfigurations.Add(objAlarmConfiguration);
            }
        }

        private static void MergeDeviceItems(Dvr itemToEdit, Dvr itemFromView)
        {
            itemToEdit.Name = itemFromView.Name;
            itemToEdit.HostName = itemFromView.HostName;
            itemToEdit.PollingFrequency = itemFromView.PollingFrequency;
            itemToEdit.TimeZone = itemFromView.TimeZone;
            itemToEdit.NumberOfCameras = itemFromView.NumberOfCameras;
            itemToEdit.UserName = itemFromView.UserName;
            itemToEdit.Password = itemFromView.Password;
            itemToEdit.IsInDST = itemFromView.IsInDST;
            itemToEdit.PortA = itemFromView.PortA;
            itemToEdit.PortB = itemFromView.PortB;
            itemToEdit.RemovedCameras = itemFromView.RemovedCameras;
        }

        private IList<AlarmConfigurationViewModel> MapAlarmConfigurationEntity(IEnumerable<AlarmConfiguration> alarms)
        {
            return alarms.Select(MapAlarmConfiguration).ToList();
        }

        private static IList<AlarmConfiguration> MapAlarmConfigurationEntities(IEnumerable<AlarmConfigurationViewModel> alarmConfigurations)
        {
            return alarmConfigurations.Select(alarm => alarm.MapFromViewModel()).ToList();
        }

        private AlarmConfigurationViewModel MapAlarmConfiguration(AlarmConfiguration item)
        {
            var model = new AlarmConfigurationViewModel(item)
                            {
                                AvailableAlarmSeverityList = _alarmConfigurationService.GetAllAlarmSeverities(),
                                AvailableAlarmOperatorList = _alarmConfigurationService.GetAllAlarmOperators()
                            };

            return model;
        }

        protected override DeviceViewModel MapEntity(Dvr item)
        {
            return new DeviceViewModel(item);
        }

        private DeviceViewModelForEdit MapEntityForEdit(Dvr item)
        {
            return new DeviceViewModelForEdit(item);
        }

        private static IList<Camera> MapCameraEntities(IEnumerable<CameraViewModel> cameras)
        {
            return cameras.Select(cam => cam.MapFromViewModel()).ToList();
        }

        public ActionResult AsyncGateways(int id)
        {
            try
            {
                // Need to retrive gateways based on company and not on site.
                var items = _gatewayService.GetGatewaysByCompanyId(id, false);

                var select = new SelectList(items, "Id", "Name");
                return Json(select);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult AsyncSites(int id)
        {
            try
            {
                var items = _siteService.GetSitesByCompany(id, false);
                var select = new SelectList(items, "Id", "Name");
                return Json(select);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }


        public ActionResult AsyncDeviceTypes(string healthCheckVersion)
        {
            try
            {
                List<DeviceType> objDeviceTypes;
                if (healthCheckVersion == HealthCheckVersion.Version2.ToString()) {

                    objDeviceTypes = new List<DeviceType> { DeviceType.Costar111, DeviceType.ipConfigure530, DeviceType.VerintEdgeVr200, DeviceType.eData300, DeviceType.eData524, DeviceType.dmpXR100Access, DeviceType.dmpXR500Access, DeviceType.dmpXR100, DeviceType.dmpXR500, DeviceType.bosch_D9412GV4, DeviceType.videofied01 };
                } else {

                    objDeviceTypes = new List<DeviceType> { DeviceType.Costar111, DeviceType.ipConfigure530, DeviceType.VerintEdgeVr200, DeviceType.eData300, DeviceType.eData524, DeviceType.dmpXR100, DeviceType.dmpXR500 };
                }
                return Json(new SelectList(objDeviceTypes));
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AsyncAlarmConfigurations(string deviceType, string companyId)
        {
            try
            {
                if (!string.IsNullOrEmpty(deviceType))
                {
                    int cpnyId = Convert.ToInt32(companyId);

                    var alarms = _alarmConfigurationService.GetDefaultAlarmConfiguration((DeviceType)Enum.Parse(typeof(DeviceType), deviceType, true));
                    IQueryable<AlarmConfiguration> query = alarms.AsQueryable();
                    if (cpnyId > 0)
                    {
                        query = query.Where(x => x.CompanyId == cpnyId);
                    }

                    //if ((deviceType == DeviceType.dmpXR100.ToString()) || (deviceType == DeviceType.dmpXR500.ToString()) || (deviceType == DeviceType.bosch_D9412GV4.ToString()) || (deviceType == DeviceType.videofied01.ToString()))
                    //{
                    //    query = query.Where(x => x.AlarmParentType.ToString().ToLower().Equals("intrusion"));
                    //}
                    //else if ((deviceType == DeviceType.Costar111.ToString()) || (deviceType == DeviceType.ipConfigure530.ToString()) || (deviceType == DeviceType.VerintEdgeVr200.ToString()))
                    //{
                    //    query = query.Where(x => x.AlarmParentType.ToString().ToLower().Equals("dvr"));
                    //}
                    var model = MapAlarmConfigurationEntity(query);

                    //if ((deviceType != DeviceType.eData300.ToString()) && (deviceType != DeviceType.eData524.ToString()) && (deviceType != DeviceType.dmpXR100Access.ToString()) && (deviceType != DeviceType.dmpXR500Access.ToString()))
                    //{
                    //    _deviceService.DeleteDuplicateAlarmConfiguration(deviceType, cpnyId);
                    //}
                    return PartialView("_AlarmConfiguration", model);
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }


        public ActionResult AsyncGetCamerasWithActiveAlerts(int deviceId)
        {
            try
            {
                JsonResult JsonResult = null;
                try
                {
                    var device = _deviceService.Get(deviceId);
                    var alarm = _alarmConfigurationService.GetByDeviceAndCapability(device.Id, AlarmType.VideoLoss);
                    var activeAlarms = _alertService.GetPendingAlertsByDevice(deviceId, alarm.Id);

                    if (activeAlarms == null)
                        JsonResult = JsonOK();

                    var cameraswithAlert = activeAlarms.Select(alertStatus =>
                                                          {
                                                              var camAlerts = device.Cameras.Where(x => x.Channel == alertStatus.ElementIdentifier)
                                                                                                   .OrderBy(x => x.Id).SingleOrDefault();
                                                              return camAlerts != null
                                                                         ? Json(new
                                                                                    {
                                                                                        CameraId = camAlerts.Id,
                                                                                        CameraName = camAlerts.Name,
                                                                                        Channel = alertStatus.ElementIdentifier
                                                                                    })
                                                                         : null;
                                                          }).ToList();
                    JsonResult = Json(cameraswithAlert);
                }
                catch (Exception e)
                {
                    logger.Error("Exception occured while getting camera with active alerts", e);
                }
                return JsonResult;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        #region "Device Grid Using Kendo UI Grid"


        public ActionResult Device_Read([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                var page = _deviceService.GetAllDevicesForDisplay();
                ViewBag.MaxDeviceId = _deviceService.GetAllDevicesforMaxId();
                TempData["maxdevice"] = _deviceService.GetAllDevicesforMaxId();
                var DisplayViewModel = from a in page
                                       select new DeviceViewModel
                                       {
                                           Id = a.Id,
                                           Name = a.Name,
                                           DeviceType = a.DeviceType.ToString(),
                                           HostName = a.HostName,
                                           GatewayName = a.Gateway.Name,
                                           SiteName = a.Site.Name,
                                           CompanyName = a.Company.Name,
                                           IsDisabled = a.IsDisabled
                                       };
                return Json(DisplayViewModel.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult GetAllCompanyDetails()
        {
            try
            {
                var resultSet = _companyService.GetAllEnabled();
                IList<SelectListItem> lstSelectedListItem = new List<SelectListItem>();
                resultSet.ToList().ForEach(x => lstSelectedListItem.Add(new SelectListItem { Text = x.Name, Value = x.Id.ToString() }));
                return Json(lstSelectedListItem, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public JsonResult GetAllSites(string cboCompany)
        {
            try
            {
                if (!cboCompany.Equals("-- Select --"))
                {
                    var items = _siteService.GetSitesByCompany(Convert.ToInt32(cboCompany), false);
                    var select = new SelectList(items, "Id", "Name");
                    return Json(select, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult DisplayCameraDetails(int CameraCount)
        {
            try
            {
                IList<CameraViewModel> lstCameraViewModel = new List<CameraViewModel>();
                for (int i = 1; i <= CameraCount; i++)
                {
                    CameraViewModel objCameraViewModel = new CameraViewModel();
                    objCameraViewModel.Active = false;
                    objCameraViewModel.CameraName = "Camera " + i.ToString();
                    objCameraViewModel.Channel = i.ToString();
                    lstCameraViewModel.Add(objCameraViewModel);
                }
                ViewBag.CameraModel = lstCameraViewModel;
                Session["cameramodel"] = lstCameraViewModel;
                return View("_ViewEditDeviceCamera", lstCameraViewModel);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }


        public ActionResult SelectedCameraData([DataSourceRequest] DataSourceRequest request, string CameraCount)
        {
            try
            {
                if (ViewBag.CameraModel != null)
                    return Json(ViewBag.CameraModel.ToDataSourceResult(request));
                else
                    return new EmptyResult();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelectedCameras_Update([DataSourceRequest] DataSourceRequest request, string cameraView)
        {
            try
            {
                IList<CameraViewModel> lstCameraViewModel = new List<CameraViewModel>();
                if (cameraView != null && ModelState.IsValid)
                {
                    string[] objLstCamera = cameraView.Split('|');
                    foreach (string obj in objLstCamera)
                    {
                        string[] objCamera = obj.Split(',');
                        if (objCamera.Length >= 3)
                        {
                            CameraViewModel objCameraViewModel = new CameraViewModel();
                            objCameraViewModel.CameraName = objCamera[0];
                            objCameraViewModel.Channel = objCamera[1];
                            objCameraViewModel.Active = Convert.ToBoolean(objCamera[2]);
                            lstCameraViewModel.Add(objCameraViewModel);
                        }
                    }

                    ViewBag.CameraModel = lstCameraViewModel;
                    Session["cameramodel"] = lstCameraViewModel;
                }
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingCamera_Cancel([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CameraViewModel> CameraView)
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        [HttpPost]
        public int GetTotalCamerasAvailable(int id)
        {
            var ResultSet = _deviceService.GetAllDevicesForDisplay();
            var result = ResultSet.Where(x => x.DeviceKey.Equals(id.ToString())).Select(y => y.Cameras);
            if (result != null)
            {
                string[] totalcameras = result.First().ToString().Split(',');
                return totalcameras.Count();
            }
            else
                return 0;
        }

        [HttpPost]
        public override ActionResult Delete(int Id)
        {
            try
            {
                this._deviceService.Delete(Id);
                if (Session["SelectedVideoDeviceId"] != null)
                {
                    if (Session["SelectedVideoDeviceId"].ToString() == Id.ToString())
                    {
                        Session["SelectedVideoDeviceId"] = null;
                    }
                }
                if (Session["AccessDeviceId"] != null)
                {
                    if (Session["AccessDeviceId"].ToString() == Id.ToString())
                    {
                        Session["AccessDeviceId"] = null;
                    }
                }
                if (Session["SelectedIntrusionDeviceId"] != null)
                {
                    if (Session["SelectedIntrusionDeviceId"].ToString() == Id.ToString())
                    {
                        Session["SelectedIntrusionDeviceId"] = null;
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }

        }


        [HttpPost]
        public override ActionResult Enable(int Id)
        {
            try
            {
                this._deviceService.Enable(Id);
                return JsonOK();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        [HttpPost]
        public override ActionResult Disable(int Id)
        {
            try
            {
                this._deviceService.Disable(Id);
                return JsonOK();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public int GetDeviceStatus(int deviceId)
        {
            var ResultSet = _deviceService.GetAllDevicesForDisplay();
            var result = ResultSet.Where(x => x.Id.Equals(deviceId) && x.DeletedKey == null);
            if (result != null)
            {
                return 1;
            }
            else
                return 0;
        }

        public JsonResult AddCameraDetails(int increaseCameraby)
        {
            try
            {
                IList<CameraViewModel> lstCameraViewModel = (List<CameraViewModel>)Session["cameramodel"];

                if (increaseCameraby > lstCameraViewModel.Count())
                {
                    for (int i = lstCameraViewModel.Count() + 1; i <= increaseCameraby; i++)
                    {
                        CameraViewModel objCameraViewModel = new CameraViewModel();
                        objCameraViewModel.Active = false;
                        objCameraViewModel.CameraName = "Camera " + i.ToString();
                        objCameraViewModel.Channel = i.ToString();
                        lstCameraViewModel.Add(objCameraViewModel);
                    }
                }
                ViewBag.CameraModel = lstCameraViewModel;
                Session["cameramodel"] = lstCameraViewModel;
                return Json(lstCameraViewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public JsonResult RemoveCameraDetails(int decreaseCameraby)
        {
            try
            {
                IList<CameraViewModel> lstCameraViewModel = (List<CameraViewModel>)Session["cameramodel"];
                if (decreaseCameraby < lstCameraViewModel.Count())
                {
                    for (int i = lstCameraViewModel.Count(); i > decreaseCameraby; i--)
                    {
                        lstCameraViewModel.RemoveAt(i - 1);
                    }
                }
                return Json(lstCameraViewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }
        #endregion
    }
}
