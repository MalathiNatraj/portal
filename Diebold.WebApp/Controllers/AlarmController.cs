using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;
using log4net;

namespace Diebold.WebApp.Controllers
{
    public class AlarmController : BaseController
    {
        private readonly IAlarmConfigurationService _alarmConfigurationService;
        private readonly IDvrService _deviceService;
        private readonly ICompanyService _companyService;
        private Company objcompany = null;

        public AlarmController(IAlarmConfigurationService alarmConfigurationService, IDvrService deviceService, ICompanyService companyService)
        {
            _alarmConfigurationService = alarmConfigurationService;
            _deviceService = deviceService;
            this._companyService = companyService;
        }

        private void InitializeViewModelAndCollections(AlarmViewModel model, IEnumerable<AlarmConfigurationViewModel> alarms)
        {
            if (alarms != null && alarms.Count() > 0)
            {
                foreach (var almConfig in alarms)
                {
                    almConfig.AvailableAlarmSeverityList = _alarmConfigurationService.GetAllAlarmSeverities();
                    almConfig.AvailableAlarmOperatorList = _alarmConfigurationService.GetAllAlarmOperators();
                    almConfig.AvailableAlarmParentTypeList = _alarmConfigurationService.GetAllAlarmParentType();
                }

                ViewBag.AlarmConfigurationModel = alarms;

                var deviceTypes = HealthCheckDeviceTypeRelation.GetDeviceTypes((HealthCheckVersion)
                                                                     Enum.Parse(typeof(HealthCheckVersion),
                                                                                model.HealthCheckVersion, true));

                model.AvailableDeviceTypeList = deviceTypes.Select(dType => dType.ToString()).ToList();
                model.AvailableHealthCheckVersionList = _deviceService.GetAllHealthCheckVersions();
                model.AvailableCompanyList = _companyService.GetAll();
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            ViewBag.AlarmConfigurationModel = new List<AlarmConfigurationViewModel>();
            var model = new AlarmViewModel
            {
                AvailableDeviceTypeList = new List<string>(),
                AvailableHealthCheckVersionList = _deviceService.GetAllHealthCheckVersions()
            };
            return View(model);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveAlarmConfiguration(AlarmViewModel model, IEnumerable<AlarmConfigurationViewModel> alarms)
        {
            try
            {
                if (alarms != null && alarms.Count() > 0)
                {
                    if (alarms.FirstOrDefault().DataType.Equals("Integer"))
                    {
                        alarms.FirstOrDefault().Threshold = alarms.FirstOrDefault().ThresholdValue.ToString();
                    }
                    if (GetCompanyConfiguration(model.DeviceType, model.CompanyId, model.AlarmType).Count > 0)
                    {
                        var alarmConfig = MapAlarmConfigurationEntities(alarms);
                        updateCompany(alarmConfig, model.CompanyId, model.AlarmType);
                        _alarmConfigurationService.UpdateAlarmConfiguration(alarmConfig);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.CompanyName) == false)
                        {
                            var alarmConfig = MapAlarmConfigurationEntities(alarms);
                            updateCompany(alarmConfig, model.CompanyId, model.AlarmType);
                            _alarmConfigurationService.CreateAlarmConfiguration(alarmConfig);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));

                InitializeViewModelAndCollections(model, alarms);
                LogError("Exception occured while saving alarm configuration", e);
                return View("Index", model);
            }
        }

        private void updateCompany(IList<AlarmConfiguration> companyConfiguration, int companyId, string alarmType)
        {
            if(objcompany ==null)
            {
                objcompany = _companyService.Get(companyId);

            }
            foreach(AlarmConfiguration objAlarmConfiguration in companyConfiguration)
            {
                objAlarmConfiguration.Company = objcompany;
                switch(alarmType)
                {
                    case "Access":
                        objAlarmConfiguration.AlarmParentType = AlarmParentType.Access;
                        break;
                    case "Intrusion":
                        objAlarmConfiguration.AlarmParentType = AlarmParentType.Intrusion;
                        break;
                    case "DVR":
                        objAlarmConfiguration.AlarmParentType = AlarmParentType.DVR;
                        break;
                }
                objAlarmConfiguration.CompanyId = objcompany.Id;
            }
        }



        private DeviceType GetDeviceTypeByName(string name)
        {
            switch(name)
            {
                case "Costar111":
                    return DeviceType.Costar111;
                case "dmpXR100":
                    return DeviceType.dmpXR100;
                case "dmpXR500":
                    return DeviceType.dmpXR500;
                case "eData300":
                    return DeviceType.eData300;
                case "eData524":
                    return DeviceType.eData524;
                case "ipConfigure530":
                    return DeviceType.ipConfigure530;
                case "VerintEdgeVr200":
                    return DeviceType.VerintEdgeVr200;
                case "bosch_D9412GV4":
                    return DeviceType.bosch_D9412GV4;
                case "videofied01":
                    return DeviceType.videofied01;
                case "dmpXR100Access":
                    return DeviceType.dmpXR100Access;
                case "dmpXR500Access":
                    return DeviceType.dmpXR500Access;
                default:
                    return DeviceType.Costar111;
            }
        }

        public ActionResult AsyncAlarmConfigurations(string deviceType, string companyId, string AlarmType)
        {
            if (!string.IsNullOrEmpty(deviceType))
            {
                List<AlarmConfiguration> objResult = GetCompanyConfiguration(deviceType, Convert.ToInt32(companyId), AlarmType);
                if(objResult.Count>0)
                {
                    var alarmModelWithCompany = MapAlarmConfigurationEntity(objResult);
                    return PartialView("_AlarmConfiguration", alarmModelWithCompany);
                }
                else
                {
                    var alarms = _alarmConfigurationService.GetDefaultAlarmConfiguration((DeviceType)Enum.Parse(typeof(DeviceType), deviceType, true));
                     IQueryable<AlarmConfiguration> query = alarms.AsQueryable();
                     query = query.Where(x => x.CompanyId == 0); // Get only the master Data
                    if (!string.IsNullOrEmpty(AlarmType))
                    {
                        query = query.Where(x => x.AlarmParentType.ToString().Equals(AlarmType));
                    }
                    var alarmModel = MapAlarmConfigurationEntity(query);
                    return PartialView("_AlarmConfiguration", alarmModel);
                }
            }
            return new EmptyResult();
        }

        private List<AlarmConfiguration> GetCompanyConfiguration(string deviceType, int companyId, string AlarmType)
        {
            var alarms = _alarmConfigurationService.GetDefaultAlarmConfiguration((DeviceType)Enum.Parse(typeof(DeviceType), deviceType, true));
            IQueryable<AlarmConfiguration> query = alarms.AsQueryable();
            if (companyId > 0)
            {
                query = query.Where(x => x.CompanyId == companyId);
            }
            if (!string.IsNullOrEmpty(AlarmType))
            {
                query = query.Where(x => x.AlarmParentType.ToString().Equals(AlarmType));
            }
            return query.ToList();
        }

        public ActionResult AsyncDeviceTypes(string healthCheckVersion)
        {
            SelectList select = !string.IsNullOrEmpty(healthCheckVersion)
                                    ? new SelectList(HealthCheckDeviceTypeRelation.GetDeviceTypes((HealthCheckVersion)
                                                     Enum.Parse(typeof(HealthCheckVersion), healthCheckVersion, true)))
                                    : new SelectList(new List<DeviceType>());

            return Json(select);
        }

        private static IList<AlarmConfiguration> MapAlarmConfigurationEntities(IEnumerable<AlarmConfigurationViewModel> alarmConfigurations)
        {
            return alarmConfigurations.Select(alarm => alarm.MapFromViewModel()).ToList();
        }

        private IList<AlarmConfigurationViewModel> MapAlarmConfigurationEntity(IEnumerable<AlarmConfiguration> alarms)
        {
            return alarms.Select(item => MapAlarmConfiguration(item)).ToList();
        }

        private AlarmConfigurationViewModel MapAlarmConfiguration(AlarmConfiguration item)
        {
            AlarmConfigurationViewModel model = new AlarmConfigurationViewModel(item);
            if (model.DataType.Equals("Integer"))
            {
                model.ThresholdValue = Convert.ToInt32(model.Threshold); // This is added because to bind a integer value to Numeric textbox
            }
            model.AvailableAlarmSeverityList = _alarmConfigurationService.GetAllAlarmSeverities();
            model.AvailableAlarmOperatorList = _alarmConfigurationService.GetAllAlarmOperators();

            return model;
        }

        public JsonResult GetAllHealthCheckVersion()
        {
            // Bind Device Details inside Combo Box
            int i = 1;
            List<HealthCheckVersionView> objHealthCheckVersionView = new List<HealthCheckVersionView>();
            IList<string> lststrHealthCheckVersionView = _deviceService.GetAllHealthCheckVersions();
            lststrHealthCheckVersionView.ToList().ForEach(x =>
            {
                objHealthCheckVersionView.Add(new HealthCheckVersionView { Id = i, Name = x });
                i += 1;
            }
            );
            return Json(objHealthCheckVersionView.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllDeviceType(string AlarmType)
        {
            List<DeviceTypeView> objLstDeviceType = new List<DeviceTypeView>();
            DeviceTypeView objDeviceTypeView = null;
            switch (AlarmType)
            {
                case "1":
                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.eData300.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);

                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.eData524.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);

                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.dmpXR100Access.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);

                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.dmpXR500Access.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);
                    break;
                case "2":
                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.Costar111.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);

                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.ipConfigure530.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);

                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.VerintEdgeVr200.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);
                    break;
                case "3":
                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.dmpXR100.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);

                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.dmpXR500.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);
                    
                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.bosch_D9412GV4.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);
                    
                    objDeviceTypeView = new DeviceTypeView();
                    objDeviceTypeView.Name = DeviceType.videofied01.ToString();
                    objLstDeviceType.Add(objDeviceTypeView);
                    
                    break;
            }
            return Json(objLstDeviceType, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAlarmConfigurationByDeviceName(string DeviceName)
        {
            if (!string.IsNullOrEmpty(DeviceName))
            {
                var alarms = _alarmConfigurationService.GetDefaultAlarmConfiguration((DeviceType)Enum.Parse(typeof(DeviceType), DeviceName, true));
                var model = MapAlarmConfigurationEntity(alarms);
                return PartialView("_AlarmConfiguration", model);
                // return Json(model);
            }

            return new EmptyResult();
            // return null;
        }

        public JsonResult GetAllCompanies()
        {
            // Bind Device Details inside Combo Box
            
            List<CompanyViewModel> objlstCompanyView = new List<CompanyViewModel>();
            IList<Company> lstCompanyView = _companyService.GetAll().OrderBy(x => x.Name).ToList();
            lstCompanyView.ToList().ForEach(x =>
            {
                objlstCompanyView.Add(new CompanyViewModel { Id = x.Id, Name = x.Name });
            
            }
            );
            return Json(objlstCompanyView.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAlarmParentType(string CompanyName)
        {
            int i = 1;
            IList<SelectListItem> lstSelectedListItem = new List<SelectListItem>();
            IList<string> lstalarmtype = _alarmConfigurationService.GetAllAlarmParentType();            
            lstalarmtype.ToList().ForEach(x =>
            {
                lstSelectedListItem.Add(new SelectListItem { Text = i.ToString(), Value = x });
                i += 1;
            });
            //var model = MapAlarmConfigurationEntity(alarms);
            //return PartialView("_AlarmConfiguration", model);
            return Json(lstSelectedListItem.Select(c => new { Id = c.Text, Name = c.Value }), JsonRequestBehavior.AllowGet);
        }
    }

    public class HealthCheckVersionView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DeviceTypeView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
