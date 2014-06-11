using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Infrastructure.Helpers;
using Diebold.WebApp.Models;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.Services.Exceptions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Diebold.Services.Impl;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.WebApp.Controllers
{
    public enum Action
    {
        TestConnection,
        Reload,
        Restart,
        Audit,
        ShowStatus
    }

    public enum DeviceInfo
    {
        Device,
        Gateway
    }

    public class DiagnosticController : BaseController
    {
        private readonly IDvrService _deviceService;
        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;
        private readonly IGatewayService _gatewayService;
        private readonly IAccessService _accessService;
        private readonly IIntrusionService _intrusionService;

        public DiagnosticController(IDvrService deviceService, ICompanyService companyService, ISiteService siteService, IGatewayService gatewayService,
            IAccessService accessService,IIntrusionService intrusionService)
        {
            _deviceService = deviceService;
            _companyService = companyService;
            _siteService = siteService;
            _gatewayService = gatewayService;
            _accessService = accessService;
            _intrusionService = intrusionService;
        }
        
        // GET: /Diagnostic/
        public ActionResult Index()
        {
            return View(new DiagnosticViewModel());
        }

        // TODO: sacar a un lugar comun a todas las grillas que lo usan
        private JqGridResponse CreateGridResponse(JqGridRequest request, int totalRecordsCount)
        {
            return new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecordsCount / request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecordsCount
            };
        }

        #region Devices

        //GET: /Diagnostic/DeviceList
        public ActionResult DeviceList()
        {
            var model = new DiagnosticConfigurationViewModel()
            {
                AvailableCompaniesList = _companyService.GetAll().Select(c => new CompanyPair() { CompanyId = c.Id, CompanyName = c.Name }).ToList(),
                AvailableSitesList = new List<SitePair>(),
                AvailableGatewaysList = new List<GatewayPair>()
            };
            return PartialView("_DeviceList", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FillDevicesReport(JqGridRequest request)
        {
            var deviceList = GetDeviceList(request);
            var response = CreateGridResponse(request, deviceList.Count);

            foreach (DeviceDiagnosticViewModel device in deviceList)
            {
                response.Records.Add(new JqGridRecord<DeviceDiagnosticViewModel>(Convert.ToString(device.Id), device));
            }

            return new JqGridJsonResult { Data = response };
        }
        
        private List<DeviceDiagnosticViewModel> GetDeviceList(JqGridRequest request)
        {
            var companyId = request.ExtraParams["companyId"];
            var siteId = request.ExtraParams["siteId"];
            var gatewayId = request.ExtraParams["gatewayId"];

            var devices = _deviceService.GetDiagnosticPage(request.PageIndex + 1, request.RecordsCount,
                                                 request.SortingName, request.SortingOrder == JqGridSortingOrders.Asc, companyId, siteId, gatewayId);

            return devices.Select(device => new DeviceDiagnosticViewModel(device)).ToList();
        }

        public ActionResult AsyncSites(int companyId)
        {
            var items = _siteService.GetSitesByCompany(companyId, false);
            var sitePairList = items.Select(s => new SitePair {SiteId = s.Id, SiteName = s.Name}).ToList();
            return Json(new SelectList(sitePairList, "SiteId", "SiteName"));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TestConnectionDevice(int id)
        {
            try
            {
                var dvrResult = _deviceService.Get(id);
                string deviceType = dvrResult.DeviceType.ToString();               
                _deviceService.TestConnection(id, dvrResult.Gateway.MacAddress + "-" + dvrResult.DeviceKey,deviceType);
                return JsonOK("The test connection was successful");
            }
            catch (ServiceException serviceException)
            {
                LogError("Service Exception occured while testing device connection", serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError(e.Message, e);
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// Reboot the device computer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReloadDevice(int id)
        {
            try
            {
                var dvrResult = _deviceService.Get(id);
                string deviceType = dvrResult.DeviceType.ToString();
                _deviceService.Reload(id, dvrResult.Gateway.MacAddress + "-" + dvrResult.DeviceKey, deviceType);
                //_deviceService.Reload(id, dvrResult.Gateway.MacAddress + "-" + dvrResult.DeviceKey);
                return JsonOK("The reload device was successful");
            }
            catch (ServiceException serviceException)
            {
                LogError(serviceException.Message, serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        /// <summary>
        /// Will restart the single agent communicating with this device.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RestartDevice(int id)
        {
            try
            {
                var dvrResult = _deviceService.Get(id);
                string deviceType = dvrResult.DeviceType.ToString();
                _deviceService.Restart(id, dvrResult.Gateway.MacAddress + "-" + dvrResult.DeviceKey,deviceType);
                return JsonOK("The restart device was successful");
            }
            catch (ServiceException serviceException)
            {
                LogError(serviceException.Message, serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError(e.Message, e);
                return JsonError(e.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AuditDevice(int id)
        {
            try
            {
                var dvrResult = _deviceService.Get(id);
                string deviceType = string.Empty;
                _gatewayService.Audit(id, dvrResult.Gateway.MacAddress,deviceType);
                return JsonOK("The restart audit was successful");
            }
            catch (ServiceException serviceException)
            {
                LogError(serviceException.Message, serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError(e.Message, e);
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// Will show current status for this device.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowStatusDevice(int id)
        {
            try
            {
                return CurrentReadings(id, DeviceInfo.Device, false);
            }
            catch (ServiceException serviceException)
            {
                LogError(serviceException.Message, serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError(e.Message, e);
                return JsonError(e.Message);
            }
        }

        #endregion

        #region Gateway

        //GET: /Diagnostic/GatewayList
        public ActionResult GatewayList()
        {
            var model = new DiagnosticConfigurationViewModel();
            return PartialView("_GatewayList", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FillGatewaysReport(JqGridRequest request)
        {
            var gatewayList = GetGatewayList(request);
            var response = CreateGridResponse(request, gatewayList.Count);

            foreach (GatewayDiagnosticViewModel gateway in gatewayList)
            {
                response.Records.Add(new JqGridRecord<GatewayDiagnosticViewModel>(Convert.ToString(gateway.Id), gateway));
            }

            return new JqGridJsonResult() { Data = response };
        }

        private List<GatewayDiagnosticViewModel> GetGatewayList(JqGridRequest request)
        {
            var search = request.ExtraParams["search"];
            var status = request.ExtraParams["gatewayStatus"];

            var gateways = _gatewayService.GetPage(request.PageIndex + 1, request.RecordsCount,
                                                 request.SortingName, request.SortingOrder == JqGridSortingOrders.Asc,
                                                 search, status);

            return gateways.Select(gateway => new GatewayDiagnosticViewModel(gateway)).ToList();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TestConnectionGateway(int id)
        {
            try
            {
                var gwayResult = _gatewayService.Get(id);
                _gatewayService.TestConnection(id, gwayResult.MacAddress,"sparkGateway");
                return JsonOK("The test connection was successful.");
            }
            catch (ServiceException serviceException)
            {
                LogError(serviceException.Message, serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError(e.Message, e);
                return JsonError(e.Message);
            }
        }


        /// <summary>
        /// Reboot the device computer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReloadGateway(int id)
        {
            try
            {
                var gwayResult = _gatewayService.Get(id);
                _gatewayService.Reload(id, gwayResult.MacAddress, "sparkGateway");
                return JsonOK("the reload gateway was successful");
            }
            catch (ServiceException serviceException)
            {
                LogError(serviceException.Message, serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError(e.Message, e);
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// Will restart the single agent communicating with this device.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RestartGateway(int id)
        {
            try
            {
                var gwayResult = _gatewayService.Get(id);
                _gatewayService.Restart(id, gwayResult.MacAddress, "sparkGateway");
                return JsonOK("The restart gateway was successful");
            }
            catch (ServiceException serviceException)
            {
                LogError(serviceException.Message, serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError(e.Message, e);
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// Will show current status for this gateway.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowStatusGateway(int id)
        {
            try
            {
                return CurrentReadings(id, DeviceInfo.Gateway, true);
            }
            catch (ServiceException serviceException)
            {
                LogError(serviceException.Message, serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError(e.Message, e);
                return JsonError(e.Message);
            }
        }
        
        #endregion

        #region Other

        public ActionResult CurrentReadings(int id, DeviceInfo deviceType, bool isGateway)
        {
            try
            {
                IEnumerable<DeviceStatusViewModel> deviceStatusModel;
                StatusDTO status = null;
                if (id > 0)
                {
                    ViewBag.DeviceId = id;
                    ViewBag.ShowViewMore = false;
                    ViewBag.DeviceType = deviceType;

                    Dvr device = null;
                    
                    if (deviceType == DeviceInfo.Gateway)
                    {
                        var gateway = _gatewayService.Get(id);
                        status = _gatewayService.GetLiveStatus(id, gateway.MacAddress, true, true);
                    }
                    else
                    {
                        device = _deviceService.Get(id);
                        switch (device.DeviceType.ToString().ToLower())
                        {
                            case "costar111":
                            case "verintedgevr200":
                            case "ipconfigure530":
                                status = _deviceService.GetLiveStatus(id, device.Gateway.MacAddress + "-" + device.DeviceKey, false, true);      
                                break;
                            case "edata524":
                            case "edata300":
                            case "dmpxr100access":
                            case "dmpxr500access":
                                status = PrepareDeviceStatus(_accessService.GetAccessDetails(device.Id).Properties);
                                break;
                            case "dmpxr100":
                            case "dmpxr500":
                            case "bosch_d9412gv4":
                            case "videofied01":	
                                status = PrepareDeviceStatus(_intrusionService.GetIntrusionDetails(device.Id).Properties);
                                break;
                        }
                    }
                    //ViewBag.DeviceName = device.Name;
                }
                else
                {
                    deviceStatusModel = new List<DeviceStatusViewModel>();
                }

                return PartialView("_CurrentStatus", status);
            }
            catch (ServiceException ex)
            {
                LogError(ex.Message, ex);
                return new ContentResult {Content = "An error occurred while executing the action. The status cannot be shown."};
            }
        }
        
        #endregion

        private StatusDTO PrepareDeviceStatus(List<DeviceProperty> objResponse)
        {
            StatusDTO objStatus = new StatusDTO();
            objStatus.payload = new StausPayload();
            objStatus.payload.SparkDvrReport = new SparkDvrReport();
            objStatus.payload.SparkDvrReport.properties = new Properties();
            ResponseProperty[] properties = new ResponseProperty[objResponse.Count];
            ResponseProperty objResponseProperty = null;
            for(int count=0;count<objResponse.Count; count++)
            {
                objResponseProperty = new ResponseProperty();
                objResponseProperty.name = objResponse[count].name;
                objResponseProperty.value = objResponse[count].value;
                properties.SetValue(objResponseProperty, count);
            }
            objStatus.payload.SparkDvrReport.properties.property = properties;
            objStatus.isGateWay = "NO";
            return objStatus;
        }

        #region Device for Kendo UI
        public JsonResult GetAllDevice()
        {
            var Device = new DiagnosticViewModel();
            return Json(Device.AvailableOptions.Select(c => new { Id = c.Text, Name = c.Value }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCompanyList()
        {
            var model = new DiagnosticConfigurationViewModel()
            {
                AvailableCompaniesList = _companyService.GetAll().Select(c => new CompanyPair() { CompanyId = c.Id, CompanyName = c.Name }).ToList(),
                AvailableSitesList = new List<SitePair>(),
                AvailableGatewaysList = new List<GatewayPair>()
            };
            return Json(model.AvailableCompanies.OrderBy(x => x.Text).Select(x => new { Id = x.Text, Name = x.Value }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCascadeCompanySite(string Company)
        {
            var items = _siteService.GetSitesByCompany(Convert.ToInt32(Company), false);
            var sitePairList = items.Select(s => new SitePair { SiteId = s.Id, SiteName = s.Name }).ToList();
            return Json(sitePairList.OrderBy(x=>x.SiteName).ToList(), JsonRequestBehavior.AllowGet);
           
        }

        public JsonResult GetCascadeSiteGateway(string Company)
        {
            var items = _gatewayService.GetGatewaysByCompanyId(Convert.ToInt32(Company), false);
            var GatewayPairList = items.Select(s => new GatewayPair { GatewayId = s.Id, GatewayName = s.Name }).ToList();
            return Json(GatewayPairList.OrderBy(x=>x.GatewayName).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeviceDiagnostic_Read([DataSourceRequest] DataSourceRequest request, string Company, string Site, string Gateway)
        {
            var devices = _deviceService.GetAll();
            IQueryable<Dvr> query = devices.AsQueryable();
            if (!string.IsNullOrEmpty(Company))
            {
                query = query.Where(x => x.Company.Id.Equals(Convert.ToInt32(Company)));
            }
            if (!string.IsNullOrEmpty(Gateway))
            {
                query = query.Where(x => x.Gateway.Id.Equals(Convert.ToInt32(Gateway)));
            }
            if (!string.IsNullOrEmpty(Site))
            {
                query = query.Where(x => x.Site.Id.Equals(Convert.ToInt32(Site)));
            }
            return Json(query.Select(device => new DeviceDiagnosticViewModel(device)).ToList().ToDataSourceResult(request));
        }

        #endregion

        #region "Gateway"

        public JsonResult GetAllStatus()
        {
            var model = new DiagnosticConfigurationViewModel();
            return Json(model.AvailableGatewayStatus.Select(x => new { Id = x.Text, Name = x.Value }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GatewayDiagnostic_Read([DataSourceRequest] DataSourceRequest request, string Status)
        {
            var gatewayResultSet = _gatewayService.GetAllByStatus(Status);
            return Json(gatewayResultSet.Select(gw => new GatewayDiagnosticViewModel(gw)).ToList().ToDataSourceResult(request));
        }

        public ActionResult FilterByStatusforGateway(string Status)
        {
            var gatewayResultSet = _gatewayService.GetAllByStatus(Status);
            return Json(gatewayResultSet.Select(gw => new GatewayDiagnosticViewModel(gw)).ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }

}
