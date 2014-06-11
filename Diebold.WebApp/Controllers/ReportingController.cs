using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Enums;
using Diebold.Exporter;
using Diebold.WebApp.Models;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Diebold.WebApp.Controllers
{
    public class ReportingController : BaseController
    {
        //
        // GET: /Reporting/

        //private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IAlarmConfigurationService _alarmConfigurationService;
        private readonly IAlertService _alertService;
        private readonly ICompanyService _companyService;
        private readonly IDvrService _deviceService;
        private readonly ISiteService _siteService;

        public ReportingController(IUserService userService, IAlarmConfigurationService alarmConfigurationService, IAlertService alertService,
                                   ICurrentUserProvider currentUserProvider, ICompanyService companyService, IDvrService deviceService, 
                                   ISiteService siteService)
        {
            _userService = userService;
            _alarmConfigurationService = alarmConfigurationService;
            _alertService = alertService;
            _currentUserProvider = currentUserProvider;
            _companyService = companyService;
            _deviceService = deviceService;
            _siteService = siteService;
        }

        public ActionResult Index()
        {
            var level1List = _userService.GetMonitoringGrouping1LevelsByUser(_currentUserProvider.CurrentUser.Id);
            level1List.Add(new CompanyGrouping1Level() {Id = -1, Name = "-- Next Level --"});

            ViewBag.AvailableFirstLevelGroups = new SelectList(level1List, "Id", "Name");
            ViewBag.AvailableSecondLevelGroups = new SelectList(Enumerable.Empty<CompanyGrouping2Level>(), "Id", "Name");
            ViewBag.AvailableSites = new SelectList(Enumerable.Empty<Site>(), "Id", "Name");
            ViewBag.AvailableDevices = new SelectList(Enumerable.Empty<Device>(), "Id", "Name");
            ViewBag.FirstLevelName = _currentUserProvider.CurrentUser.Company.FirstLevelGrouping;
            ViewBag.SecondLevelName = _currentUserProvider.CurrentUser.Company.SecondLevelGrouping;

            var filterModel = new AlertFilterViewModel()
            {
                AvailableAlertTypeList = _alarmConfigurationService.GetAllAlarmTypes(),
                AvailableUserList = _userService.GetUsersByCompany(_currentUserProvider.CurrentUser.Company.Id, UserStatus.AllUsers),
                DateFrom = DateTime.Now.ToShortDateString(),
                DateTo = DateTime.Now.ToShortDateString()
            };

            ViewBag.AlertFilterViewModel = filterModel;

            return View();
        }

        public ActionResult GetAvailableDevice()
        {
          //  SelectList AvailableDevice = new SelectList(new SelectList(Enumerable.Empty<Device>(), "Id", "Name"));
          //  return Json(AvailableDevice.ToList(), JsonRequestBehavior.AllowGet);
            var availableDeviceStatus = Enum.GetNames(typeof(DeviceStatusFilter)).Select(status => new SelectListItem
            {
                Text = status.ToString().SplitByUpperCase(),
                Value = status
            }).ToList();
            return Json(availableDeviceStatus.ToList(), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ExportToExcel(string alertTypes, string userIds, string deviceStatus,
            string selectedGroup1Ids, string selectedGroup2Ids, string selectedSiteIds, string selectedDeviceIds,
            string dateType, DateTime dateFrom, DateTime dateTo)
        {
            var deviceIds = new List<int>();
            IList<int> lstALLCompanyLevel2Id = new List<int>();
            IList<int> lstALLDeviceId = new List<int>();
            IList<int> lstALLSiteId = new List<int>();
            // SetDevicesIds(selectedGroup1Ids, selectedGroup2Ids, selectedSiteIds, selectedDeviceIds, deviceIds);
            deviceIds = GetMonitoredDevicesIds(_currentUserProvider.CurrentUser.Id);
            // Get Selected Group Level 1 Ids
            IList<int> lstGroup1LevelIds = new List<int>();
            string[] arGroup1LevelIds;
            if (!string.IsNullOrEmpty(selectedGroup1Ids))
            {
                arGroup1LevelIds = selectedGroup1Ids.Split(',');
                for (int i = 0; i < arGroup1LevelIds.Count(); i++)
                {
                    lstGroup1LevelIds.Add(Convert.ToInt32(arGroup1LevelIds[i]));
                }
            }
            // Get Selected Group Level 2 Ids
            IList<int> lstGroup2LevelIds = new List<int>();
            string[] arGroup2LevelIds;
            if (!string.IsNullOrEmpty(selectedGroup2Ids))
            {
                arGroup2LevelIds = selectedGroup2Ids.Split(',');
                for (int i = 0; i < arGroup2LevelIds.Count(); i++)
                {
                    lstGroup2LevelIds.Add(Convert.ToInt32(arGroup2LevelIds[i]));
                }
            }
            // Get Selected Site Ids
            IList<int> lstSiteIds = new List<int>();
            string[] arSiteIds;
            if (!string.IsNullOrEmpty(selectedSiteIds))
            {
                arSiteIds = selectedSiteIds.Split(',');
                for (int i = 0; i < arSiteIds.Count(); i++)
                {
                    lstSiteIds.Add(_siteService.GetAll().Where(x => x.Name.Equals(arSiteIds[i])).FirstOrDefault().Id);
                }
            }
            // Get Selected Site Ids
            IList<int> lstDeviceIds = new List<int>();
            string[] arDeviceIds;
            if (!string.IsNullOrEmpty(selectedDeviceIds))
            {
                arDeviceIds = selectedDeviceIds.Split(',');
                for (int i = 0; i < arDeviceIds.Count(); i++)
                {
                    lstDeviceIds.Add(Convert.ToInt32(arDeviceIds[i]));
                }
            }

            #region "First Level Selected All others are not selected"
            // Get District Details
            if (lstGroup1LevelIds != null && lstGroup1LevelIds.Count() > 0 && (lstGroup2LevelIds == null || lstGroup2LevelIds.Count <= 0) && (selectedSiteIds == null || selectedSiteIds == "") && (selectedDeviceIds == null || selectedDeviceIds == ""))
            {
                deviceIds = new List<int>();
                foreach (int item in lstGroup1LevelIds)
                {
                    // IList<CompanyGrouping2Level> lstCompanyLevel2 = _userService.GetMonitoringGrouping2LevelsByUser(_currentUserProvider.CurrentUser.Id, item);
                    IList<CompanyGrouping2Level> lstCompanyLevel2 = _companyService.GetGrouping2LevelsByGrouping1LevelId(item);
                    if (lstCompanyLevel2 != null && lstCompanyLevel2.Count() > 0)
                    {
                        for (int j = 0; j < lstCompanyLevel2.Count() - 1; j++)
                        {
                            lstALLCompanyLevel2Id.Add(lstCompanyLevel2[j].Id);
                        }

                    }
                }
                // Get Site Details by Company Level2
                foreach (int companyLevel2Id in lstALLCompanyLevel2Id)
                {
                    IList<Site> lstSite = _siteService.GetSitesByCompanyGrouping2Level(companyLevel2Id);
                    if (lstSite != null && lstSite.Count() > 0)
                    {
                        for (int k = 0; k < lstSite.Count(); k++)
                        {
                            lstALLSiteId.Add(lstSite[k].Id);
                        }
                    }
                }
                // Get Device Details by Site 
                foreach (int SiteId in lstALLSiteId)
                {
                    IList<Dvr> lstDVR = _deviceService.GetDevicesBySiteId(SiteId);
                    if (lstDVR != null && lstDVR.Count() > 0)
                    {
                        for (int l = 0; l < lstDVR.Count(); l++)
                        {
                            lstALLDeviceId.Add(lstDVR[l].Id);
                        }
                    }
                }
            }
            #endregion
            #region "First Level and Second Level Selected All others are empty"
            // Get District Details
            else if (lstGroup1LevelIds != null && lstGroup1LevelIds.Count() > 0 && lstGroup2LevelIds != null && lstGroup2LevelIds.Count() > 0 && (selectedSiteIds == null || selectedSiteIds == "") && (selectedDeviceIds == null || selectedDeviceIds == ""))
            {
                // Get Site Details by Company Level2
                deviceIds = new List<int>();
                foreach (int companyLevel2Id in lstGroup2LevelIds)
                {
                    IList<Site> lstSite = _siteService.GetSitesByCompanyGrouping2Level(companyLevel2Id);
                    if (lstSite != null && lstSite.Count() > 0)
                    {
                        for (int k = 0; k < lstSite.Count(); k++)
                        {
                            lstALLSiteId.Add(lstSite[k].Id);
                        }
                    }
                }
                // Get Device Details by Site 
                foreach (int SiteId in lstALLSiteId)
                {
                    IList<Dvr> lstDVR = _deviceService.GetDevicesBySiteId(SiteId);
                    if (lstDVR != null && lstDVR.Count() > 0)
                    {
                        for (int l = 0; l < lstDVR.Count(); l++)
                        {
                            lstALLDeviceId.Add(lstDVR[l].Id);
                        }
                    }
                }
            }
            #endregion
            #region "First Level and Second Level Selected and Site Id Selected others are empty "
            else if (lstGroup1LevelIds != null && lstGroup1LevelIds.Count() > 0 && lstGroup2LevelIds != null && lstGroup2LevelIds.Count() > 0 && (selectedSiteIds != null || selectedSiteIds != "") && (selectedDeviceIds == null || selectedDeviceIds == ""))
            {
                deviceIds = new List<int>();
                // Get Site Id by Site Name
                string[] siteNames = selectedSiteIds.Split(',');
                for (int i = 0; i < siteNames.Count(); i++)
                {
                    lstALLSiteId.Add(_siteService.GetAll().Where(x => x.Name.Equals(siteNames[i])).FirstOrDefault().Id);
                }
                // Get Device Details by Site 
                foreach (int SiteId in lstALLSiteId)
                {
                    IList<Dvr> lstDVR = _deviceService.GetDevicesBySiteId(SiteId);
                    if (lstDVR != null && lstDVR.Count() > 0)
                    {
                        for (int l = 0; l < lstDVR.Count(); l++)
                        {
                            lstALLDeviceId.Add(lstDVR[l].Id);
                        }
                    }
                }
            }
            #endregion
            #region "All Filters are selected"
            else if (lstGroup1LevelIds != null && lstGroup1LevelIds.Count() > 0 && lstGroup2LevelIds != null && lstGroup2LevelIds.Count() > 0 && (selectedSiteIds != null || selectedSiteIds != "") && (selectedDeviceIds != null || selectedDeviceIds != ""))
            {
                deviceIds = new List<int>();
                string[] DeviceId = selectedDeviceIds.Split(',');
                for (int i = 0; i < DeviceId.Count(); i++)
                {
                    lstALLDeviceId.Add(Convert.ToInt32(DeviceId[i]));
                }

            }
            #endregion
            if (lstALLDeviceId != null && lstALLDeviceId.Count() > 0)
            {
                deviceIds.AddRange(lstALLDeviceId);
            }

            var alerts = GetAlerts(alertTypes, userIds, deviceStatus, deviceIds, dateType, dateFrom, dateTo);

            var columnSizes = new float[] { 11, 10, 10, 11, 11, 13, 9, 13, 11 };
            var bytes = new ExcelExporter().Export("Alerts Report", OrientationPageType.Portrait, MapEntities(alerts), dateFrom, dateTo, columnSizes);

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ExportToPdf(string alertTypes, string userIds, string deviceStatus,
            string selectedGroup1Ids, string selectedGroup2Ids, string selectedSiteIds, string selectedDeviceIds,
            string dateType, DateTime dateFrom, DateTime dateTo)
        {
            var deviceIds = new List<int>();
            IList<int> lstALLCompanyLevel2Id = new List<int>();
            IList<int> lstALLDeviceId = new List<int>();
            IList<int> lstALLSiteId = new List<int>();
           // SetDevicesIds(selectedGroup1Ids, selectedGroup2Ids, selectedSiteIds, selectedDeviceIds, deviceIds);
            deviceIds = GetMonitoredDevicesIds(_currentUserProvider.CurrentUser.Id);
            // Get Selected Group Level 1 Ids
            IList<int> lstGroup1LevelIds = new List<int>();
            string[] arGroup1LevelIds;
            if (!string.IsNullOrEmpty(selectedGroup1Ids))
            {
                arGroup1LevelIds = selectedGroup1Ids.Split(',');
                for (int i = 0; i < arGroup1LevelIds.Count(); i++)
                {
                    lstGroup1LevelIds.Add(Convert.ToInt32(arGroup1LevelIds[i]));
                }
            }
            // Get Selected Group Level 2 Ids
            IList<int> lstGroup2LevelIds = new List<int>();
            string[] arGroup2LevelIds;
            if (!string.IsNullOrEmpty(selectedGroup2Ids))
            {
                arGroup2LevelIds = selectedGroup2Ids.Split(',');
                for (int i = 0; i < arGroup2LevelIds.Count(); i++)
                {
                    lstGroup2LevelIds.Add(Convert.ToInt32(arGroup2LevelIds[i]));
                }
            }
            // Get Selected Site Ids
            IList<int> lstSiteIds = new List<int>();
            string[] arSiteIds;
            if (!string.IsNullOrEmpty(selectedSiteIds))
            {
                arSiteIds = selectedSiteIds.Split(',');
                for (int i = 0; i < arSiteIds.Count(); i++)
                {
                    lstSiteIds.Add(_siteService.GetAll().Where(x=>x.Name.Equals(arSiteIds[i])).FirstOrDefault().Id);
                }
            }
            // Get Selected Site Ids
            IList<int> lstDeviceIds = new List<int>();
            string[] arDeviceIds;
            if (!string.IsNullOrEmpty(selectedDeviceIds))
            {
                arDeviceIds = selectedDeviceIds.Split(',');
                for (int i = 0; i < arDeviceIds.Count(); i++)
                {
                    lstDeviceIds.Add(Convert.ToInt32(arDeviceIds[i]));
                }
            }

            #region "First Level Selected All others are not selected"
            // Get District Details
            if (lstGroup1LevelIds != null && lstGroup1LevelIds.Count() > 0 && (lstGroup2LevelIds == null || lstGroup2LevelIds.Count <= 0) && (selectedSiteIds == null || selectedSiteIds == "") && (selectedDeviceIds == null || selectedDeviceIds == ""))
            {
                deviceIds = new List<int>();
                foreach (int item in lstGroup1LevelIds)
                {
                    // IList<CompanyGrouping2Level> lstCompanyLevel2 = _userService.GetMonitoringGrouping2LevelsByUser(_currentUserProvider.CurrentUser.Id, item);
                    IList<CompanyGrouping2Level> lstCompanyLevel2 = _companyService.GetGrouping2LevelsByGrouping1LevelId(item);
                    if (lstCompanyLevel2 != null && lstCompanyLevel2.Count() > 0)
                    {
                        for (int j = 0; j < lstCompanyLevel2.Count() - 1; j++)
                        {
                            lstALLCompanyLevel2Id.Add(lstCompanyLevel2[j].Id);
                        }

                    }
                }
                // Get Site Details by Company Level2
                foreach (int companyLevel2Id in lstALLCompanyLevel2Id)
                {
                    IList<Site> lstSite = _siteService.GetSitesByCompanyGrouping2Level(companyLevel2Id);
                    if (lstSite != null && lstSite.Count() > 0)
                    {
                        for (int k = 0; k < lstSite.Count(); k++)
                        {
                            lstALLSiteId.Add(lstSite[k].Id);
                        }
                    }
                }
                // Get Device Details by Site 
                foreach (int SiteId in lstALLSiteId)
                {
                    IList<Dvr> lstDVR = _deviceService.GetDevicesBySiteId(SiteId);
                    if (lstDVR != null && lstDVR.Count() > 0)
                    {
                        for (int l = 0; l < lstDVR.Count(); l++)
                        {
                            lstALLDeviceId.Add(lstDVR[l].Id);
                        }
                    }
                }
            }
            #endregion
            #region "First Level and Second Level Selected All others are empty"
            // Get District Details
            else if (lstGroup1LevelIds != null && lstGroup1LevelIds.Count() > 0 && lstGroup2LevelIds != null && lstGroup2LevelIds.Count() > 0 && (selectedSiteIds == null || selectedSiteIds == "") && (selectedDeviceIds == null || selectedDeviceIds == ""))
            {
                // Get Site Details by Company Level2
                deviceIds = new List<int>();
                foreach (int companyLevel2Id in lstGroup2LevelIds)
                {
                    IList<Site> lstSite = _siteService.GetSitesByCompanyGrouping2Level(companyLevel2Id);
                    if (lstSite != null && lstSite.Count() > 0)
                    {
                        for (int k = 0; k < lstSite.Count(); k++)
                        {
                            lstALLSiteId.Add(lstSite[k].Id);
                        }
                    }
                }
                // Get Device Details by Site 
                foreach (int SiteId in lstALLSiteId)
                {
                    IList<Dvr> lstDVR = _deviceService.GetDevicesBySiteId(SiteId);
                    if (lstDVR != null && lstDVR.Count() > 0)
                    {
                        for (int l = 0; l < lstDVR.Count(); l++)
                        {
                            lstALLDeviceId.Add(lstDVR[l].Id);
                        }
                    }
                }
            }
            #endregion
            #region "First Level and Second Level Selected and Site Id Selected others are empty "
            else if (lstGroup1LevelIds != null && lstGroup1LevelIds.Count() > 0 && lstGroup2LevelIds != null && lstGroup2LevelIds.Count() > 0 && (selectedSiteIds != null || selectedSiteIds != "") && (selectedDeviceIds == null || selectedDeviceIds == ""))
            {
                deviceIds = new List<int>();
                // Get Site Id by Site Name
                string[] siteNames = selectedSiteIds.Split(',');
                for (int i = 0; i < siteNames.Count(); i++)
                {
                    lstALLSiteId.Add(_siteService.GetAll().Where(x => x.Name.Equals(siteNames[i])).FirstOrDefault().Id);
                }
                // Get Device Details by Site 
                foreach (int SiteId in lstALLSiteId)
                {
                    IList<Dvr> lstDVR = _deviceService.GetDevicesBySiteId(SiteId);
                    if (lstDVR != null && lstDVR.Count() > 0)
                    {
                        for (int l = 0; l < lstDVR.Count(); l++)
                        {
                            lstALLDeviceId.Add(lstDVR[l].Id);
                        }
                    }
                }
            }
            #endregion
            #region "All Filters are selected"
            else if (lstGroup1LevelIds != null && lstGroup1LevelIds.Count() > 0 && lstGroup2LevelIds != null && lstGroup2LevelIds.Count() > 0 && (selectedSiteIds != null || selectedSiteIds != "") && (selectedDeviceIds != null || selectedDeviceIds != ""))
            {
                deviceIds = new List<int>();
                string[] DeviceId = selectedDeviceIds.Split(',');
                for (int i = 0; i < DeviceId.Count(); i++)
                {
                    lstALLDeviceId.Add(Convert.ToInt32(DeviceId[i]));
                }

            }
            #endregion
            if (lstALLDeviceId != null && lstALLDeviceId.Count() > 0)
            {
                deviceIds.AddRange(lstALLDeviceId);
            }

            var alerts = GetAlerts(alertTypes, userIds, deviceStatus, deviceIds, dateType, dateFrom, dateTo);

            var columnSizes = new float[] {11,10,10,11,11,13,9,13,11};
            var bytes = new PdfExporter().Export("Alerts Report", OrientationPageType.Portrait, MapEntities(alerts), dateFrom, dateTo, columnSizes);

            return File(bytes, "application/pdf", "report.pdf");
        }

        private IEnumerable<ResultsReport> GetAlerts(string alertTypes, string userIds, string deviceStatus,
            IList<int> selectedDeviceIdsList, string dateType, DateTime dateFrom, DateTime dateTo)
        {
            IList<string> alertTypesList = new List<string>();
            if (!string.IsNullOrEmpty(alertTypes) && alertTypes.ToLower() != "all")
                alertTypesList = alertTypes.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            var userIdsList = new List<int>();
            if (!string.IsNullOrEmpty(userIds) && userIds.ToLower() != "all")
                userIdsList =
                    userIds.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToList();

            var alerts = _alertService.GetAlertsForReport(alertTypesList, userIdsList, deviceStatus, dateType,
                                                          dateFrom, dateTo, selectedDeviceIdsList, "Date", false, true); // Need to be false - Last param
            return alerts;
        }

         [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult BindReport([DataSourceRequest] DataSourceRequest request, IList<string> param) 
        {
            logger.Debug("Bind Report Started - Custom Activity Report.");
            //string[] paramval = param[0].Split(':', StringSplitOptions.RemoveEmptyEntries);
            var QueryParem = param[0].Split(new string[] { ";" }, StringSplitOptions.None).ToList();
            logger.Debug("Bind Report Parameters in Custom Activity Report" + QueryParem);
            //return null;

            var alertTypes = new List<string>();
            var userIds = new List<int>();
            var deviceIds = new List<int>();
            var group1Ids = new List<int>();
            var group2Ids = new List<int>();
            var siteIds = new List<int>();
            IList<int> lstALLCompanyLevel2Id = new List<int>();
            IList<int> lstALLDeviceId = new List<int>();
            IList<int> lstALLSiteId = new List<int>();
            var selectedGroup1Ids = QueryParem.Where(x => x.StartsWith("selectedGroup1Ids")).FirstOrDefault().Split(':')[1];
            var selectedGroup2Ids = QueryParem.Where(x => x.StartsWith("selectedGroup2Ids")).FirstOrDefault().Split(':')[1];
            var selectedSiteIds = QueryParem.Where(x => x.StartsWith("selectedSiteIds")).FirstOrDefault().Split(':')[1]; //request.ExtraParams["selectedSiteIds"];
            var selectedDeviceIds = QueryParem.Where(x => x.StartsWith("selectedDeviceIds")).FirstOrDefault().Split(':')[1]; //request.ExtraParams["selectedDeviceIds"];

            
            var groupLevelSelected = false;
             bool isGroupOneSelected = false;
             bool isGroupTwoSelected = false;
            if (!string.IsNullOrEmpty(selectedGroup1Ids))
            {
                group1Ids.AddRange(selectedGroup1Ids.Split(',').Select(int.Parse));
                if(group1Ids.Contains(Convert.ToInt32(-1)))
                {
                    isGroupOneSelected = true;
                }
            }
            if (!string.IsNullOrEmpty(selectedGroup2Ids))
            {
                group2Ids.AddRange(selectedGroup2Ids.Split(',').Select(int.Parse));
                 if(group2Ids.Contains(Convert.ToInt32(-1)))
                {
                    isGroupTwoSelected = true;
                }
            }
            if (((string.IsNullOrEmpty(selectedGroup1Ids) && isGroupOneSelected == false) || (string.IsNullOrEmpty(selectedGroup2Ids) && isGroupTwoSelected == false)))
            {
                groupLevelSelected = SetDevicesIds(selectedGroup1Ids, selectedGroup2Ids, selectedSiteIds, selectedDeviceIds, deviceIds);
            }
            else
            {
                groupLevelSelected = false;
            }
            string AlertType = QueryParem.Where(x => x.StartsWith("alertTypes")).FirstOrDefault().Split(':')[1].ToString();
            string userId = QueryParem.Where(x => x.StartsWith("userIds")).FirstOrDefault().Split(':')[1].ToString();
            string dateFrom = QueryParem.Where(x => x.StartsWith("dateFrom")).FirstOrDefault().Split(':')[1].ToString();
            string dateTo = QueryParem.Where(x => x.StartsWith("dateTo")).FirstOrDefault().Split(':')[1].ToString();
            string deviceStatus = QueryParem.Where(x => x.StartsWith("deviceStatus")).FirstOrDefault().Split(':')[1].ToString();
            string dateType = QueryParem.Where(x => x.StartsWith("dateType")).FirstOrDefault().Split(':')[1].ToString();


            if (!groupLevelSelected)
            {
                deviceIds = GetMonitoredDevicesIds(_currentUserProvider.CurrentUser.Id);
                groupLevelSelected = true;
            }

            if (!string.IsNullOrEmpty(AlertType) && AlertType.ToLower() != "all")
            {
                alertTypes.AddRange(AlertType.Split(','));
            }

            if (!string.IsNullOrEmpty(userId) && userId.ToLower() != "all")
            {
                userIds.AddRange(userId.Split(',').Select(uId => Convert.ToInt32(uId)));
            }

            #region "First Level Selected All others are not selected"
            // Get District Details
            if (group1Ids != null && group1Ids.Count() > 0 && (group2Ids == null || group2Ids.Count <= 0) && ( selectedSiteIds == null || selectedSiteIds == "" ) && (selectedDeviceIds == null || selectedDeviceIds == ""))
            {
                deviceIds = new List<int>();
                foreach (int item in group1Ids)
                {
                    // IList<CompanyGrouping2Level> lstCompanyLevel2 = _userService.GetMonitoringGrouping2LevelsByUser(_currentUserProvider.CurrentUser.Id, item);
                    IList<CompanyGrouping2Level> lstCompanyLevel2 = _companyService.GetGrouping2LevelsByGrouping1LevelId(item);
                    if (lstCompanyLevel2 != null && lstCompanyLevel2.Count() > 0)
                    {
                        for (int j = 0; j < lstCompanyLevel2.Count() - 1; j++)
                        {
                            lstALLCompanyLevel2Id.Add(lstCompanyLevel2[j].Id);
                        }

                    }
                }
                // Get Site Details by Company Level2
                foreach (int companyLevel2Id in lstALLCompanyLevel2Id)
                {
                    IList<Site> lstSite = _siteService.GetSitesByCompanyGrouping2Level(companyLevel2Id);
                    if (lstSite != null && lstSite.Count() > 0)
                    {
                        for (int k = 0; k < lstSite.Count(); k++)
                        {
                            lstALLSiteId.Add(lstSite[k].Id);
                        }
                    }
                }
                // Get Device Details by Site 
                foreach (int SiteId in lstALLSiteId)
                {
                    IList<Dvr> lstDVR = _deviceService.GetDevicesBySiteId(SiteId);
                    if (lstDVR != null && lstDVR.Count() > 0)
                    {
                        for (int l = 0; l < lstDVR.Count(); l++)
                        {
                            lstALLDeviceId.Add(lstDVR[l].Id);
                        }
                    }
                }
            }
            #endregion
            #region "First Level and Second Level Selected All others are empty"
            // Get District Details
            else if (group1Ids != null && group1Ids.Count() > 0 && group2Ids != null && group2Ids.Count() > 0 && (selectedSiteIds == null || selectedSiteIds == "") && (selectedDeviceIds == null || selectedDeviceIds == ""))
            {
                // Get Site Details by Company Level2
                deviceIds = new List<int>();
                foreach (int companyLevel2Id in group2Ids)
                {
                    IList<Site> lstSite = _siteService.GetSitesByCompanyGrouping2Level(companyLevel2Id);
                    if (lstSite != null && lstSite.Count() > 0)
                    {
                        for (int k = 0; k < lstSite.Count(); k++)
                        {
                            lstALLSiteId.Add(lstSite[k].Id);
                        }
                    }
                }
                // Get Device Details by Site 
                foreach (int SiteId in lstALLSiteId)
                {
                    IList<Dvr> lstDVR = _deviceService.GetDevicesBySiteId(SiteId);
                    if (lstDVR != null && lstDVR.Count() > 0)
                    {
                        for (int l = 0; l < lstDVR.Count(); l++)
                        {
                            lstALLDeviceId.Add(lstDVR[l].Id);
                        }
                    }
                }
            }
            #endregion
            #region "First Level and Second Level Selected and Site Id Selected others are empty "
            else if (group1Ids != null && group1Ids.Count() > 0 && group2Ids != null && group2Ids.Count() > 0 && (selectedSiteIds != null || selectedSiteIds != "") && (selectedDeviceIds == null || selectedDeviceIds == ""))
            {
                deviceIds = new List<int>();
                // Get Site Id by Site Name
                string[] siteNames = selectedSiteIds.Split(',');
                for (int i = 0; i < siteNames.Count(); i++)
                {
                    lstALLSiteId.Add(_siteService.GetAll().Where(x => x.Name.Equals(siteNames[i])).FirstOrDefault().Id);
                }
                // Get Device Details by Site 
                foreach (int SiteId in lstALLSiteId)
                {
                    IList<Dvr> lstDVR = _deviceService.GetDevicesBySiteId(SiteId);
                    if (lstDVR != null && lstDVR.Count() > 0)
                    {
                        for (int l = 0; l < lstDVR.Count(); l++)
                        {
                            lstALLDeviceId.Add(lstDVR[l].Id);
                        }
                    }
                }
            }
            #endregion
            #region "All Filters are selected"
            else if (group1Ids != null && group1Ids.Count() > 0 && group2Ids != null && group2Ids.Count() > 0 && (selectedSiteIds != null || selectedSiteIds != "") && (selectedDeviceIds != null || selectedDeviceIds != ""))
            {
                deviceIds = new List<int>();
                string[] DeviceId = selectedDeviceIds.Split(',');
                for (int i = 0; i < DeviceId.Count(); i++)
			    {
                    lstALLDeviceId.Add(Convert.ToInt32(DeviceId[i]));
			    }
                
            }
            #endregion
            if (lstALLDeviceId != null && lstALLDeviceId.Count() > 0)
            {
                deviceIds.AddRange(lstALLDeviceId);
            }
            IList<ResultsReport> alerts = _alertService.GetAlertsForReport(alertTypes, userIds, deviceStatus, dateType, DateTime.Parse(dateFrom),
                                                     DateTime.Parse(dateTo), deviceIds, "Date", false, true).ToList(); // Need to be false - Last param as per export functionality

            logger.Debug("Bind Report (Custom Activity Report) - alerts list" + alerts);
            List<ReportingViewModel> lstReportingViewModel = new List<ReportingViewModel>();
            foreach (var item in alerts)
            {
                ReportingViewModel objReportingViewModel = new ReportingViewModel();
                objReportingViewModel.Date = item.Date.ToString("yyyy/MM/dd hh:mm tt");
                objReportingViewModel.Area = item.Area.ToString();
                objReportingViewModel.Site = item.Site;
                objReportingViewModel.DeviceName = item.DeviceName;
                objReportingViewModel.AlertDescription = item.AlertDescription;
                objReportingViewModel.ResolvedBy = item.ResolvedBy;
                objReportingViewModel.CurrentStatus = item.CurrentStatus;
                objReportingViewModel.LastNoteBy = item.LastNoteBy;
                objReportingViewModel.DVRType = item.DVRType;
                lstReportingViewModel.Add(objReportingViewModel);

            }
            logger.Debug("Bind Audit Report Completed.");
            return Json(lstReportingViewModel.ToDataSourceResult(request)); 

        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FillReport(JqGridRequest request)
        {
            var alertTypes = new List<string>();
            var userIds = new List<int>();
            var deviceIds = new List<int>();

            var selectedGroup1Ids = request.ExtraParams["selectedGroup1Ids"];
            var selectedGroup2Ids = request.ExtraParams["selectedGroup2Ids"];
            var selectedSiteIds = request.ExtraParams["selectedSiteIds"];
            var selectedDeviceIds = request.ExtraParams["selectedDeviceIds"];

            var groupLevelSelected = SetDevicesIds(selectedGroup1Ids, selectedGroup2Ids, selectedSiteIds, selectedDeviceIds, deviceIds);

            if (!groupLevelSelected)
            {
                deviceIds = GetMonitoredDevicesIds(_currentUserProvider.CurrentUser.Id);
                groupLevelSelected = true;
            }

            if (request.ExtraParams.ContainsKey("alertTypes") && !string.IsNullOrEmpty(request.ExtraParams["alertTypes"]) 
                                                              && request.ExtraParams["alertTypes"].ToLower() != "all" )
            {
                alertTypes.AddRange(request.ExtraParams["alertTypes"].Split(','));
            }

            if (request.ExtraParams.ContainsKey("userIds") && !string.IsNullOrEmpty(request.ExtraParams["userIds"])
                                                           && request.ExtraParams["userIds"].ToLower() != "all")
            {
                userIds.AddRange(request.ExtraParams["userIds"].Split(',').Select(userId => Convert.ToInt32(userId)));
            }

            var alerts = _alertService.GetAlertsForReport(request.PageIndex + 1, request.RecordsCount,
                                                        request.SortingName,
                                                        request.SortingOrder.Equals(JqGridSortingOrders.Asc), alertTypes,
                                                        userIds, request.ExtraParams["deviceStatus"],
                                                        request.ExtraParams["dateType"],
                                                        DateTime.Parse(request.ExtraParams["dateFrom"]),
                                                        DateTime.Parse(request.ExtraParams["dateTo"]), deviceIds, groupLevelSelected);

            
            var response = GetJqGridResponse(alerts, alerts.Select(MapEntity));

            return new JqGridJsonResult { Data = response };
        }

        protected bool SetDevicesIds(string selectedGroup1Ids, string selectedGroup2Ids, string selectedSiteIds, string selectedDeviceIds,
            List<int> deviceIds)
        {
            var groupLevelSelected = false;

            if (!string.IsNullOrEmpty(selectedGroup1Ids))
            {
                groupLevelSelected = true;
                var processed = new List<string>();
                foreach (var group1Id in selectedGroup1Ids.Split(',').Where(group1Id => !processed.Contains(group1Id)))
                {
                    foreach (var group2Item in _companyService.GetGrouping2LevelsByGrouping1LevelId(Convert.ToInt32(group1Id)))
                        AppendDevicesByGroup2(group2Item, deviceIds);

                    processed.Add(group1Id);
                }
            }
            if (!string.IsNullOrEmpty(selectedGroup2Ids))
            {
                groupLevelSelected = true;
                var processed = new List<string>();
                foreach (var group2Id in selectedGroup2Ids.Split(',').Where(group2Id => !processed.Contains(group2Id)))
                {
                    AppendDevicesByGroup2(_companyService.GetGrouping2LevelById(int.Parse(group2Id)), deviceIds);
                    processed.Add(group2Id);
                }
            }
            if (!string.IsNullOrEmpty(selectedSiteIds))
            {
                groupLevelSelected = true;
                var processed = new List<string>();
                foreach (var siteId in selectedSiteIds.Split(',').Where(siteId => !processed.Contains(siteId))
                    )
                {
                    AppendDevicesBySite(_siteService.Get(int.Parse(siteId)), deviceIds);
                    processed.Add(siteId);
                }
            }
            if (!string.IsNullOrEmpty(selectedDeviceIds))
            {
                groupLevelSelected = true;
                foreach (var deviceId in selectedDeviceIds.Split(',').Where(deviceId => !deviceIds.Contains(int.Parse(deviceId))))
                {
                    deviceIds.Add(int.Parse(deviceId));
                }
            }
            return groupLevelSelected;
        }

        private void AppendDevicesByGroup2(CompanyGrouping2Level group2, IList<int> devices)
        {
            if (group2 != null)
                foreach (var site in group2.Sites)
                    AppendDevicesBySite(site, devices);
        }

        private void AppendDevicesBySite(Site site, IList<int> devices)
        {
            if (site != null)
                foreach (var device in _deviceService.GetDevicesBySiteId(site.Id))
                {
                    if (!devices.Contains(device.Id))
                        devices.Add(device.Id);
                }
        }

        private IList<ReportingViewModel> MapEntities(IEnumerable<ResultsReport> alerts)
        {
            return (from ResultsReport alert in alerts select MapEntity(alert)).ToList();
        }

        private ReportingViewModel MapEntity(ResultsReport alert)
        {
            return new ReportingViewModel(alert);
        }

        private List<int> GetMonitoredDevicesIds(int userId)
        {
            var deviceIds = new List<int>();
            var monitoredLevel1List = _userService.GetMonitoringGrouping1LevelsByUser(userId);
            var monitoredLevel2List = _userService.GetMonitoringGrouping2LevelsByUser(userId, null);
            var monitoredSiteList = _userService.GetMonitoringSitesByUser(userId, null);
            var monitoredDeviceList = _userService.GetMonitoringDevicesByUser(userId, null);

            foreach (var monitoredLevel2 in monitoredLevel1List.SelectMany(monitoredLevel1 => monitoredLevel1.CompanyGrouping2Levels))
                AppendDevicesByGroup2(monitoredLevel2, deviceIds);

            foreach (var monitoredLevel2 in monitoredLevel2List)
                AppendDevicesByGroup2(monitoredLevel2, deviceIds);

            foreach (var monitoredSite in monitoredSiteList)
                AppendDevicesBySite(monitoredSite, deviceIds);

            deviceIds.AddRange(monitoredDeviceList.Select(monitoredDevice => monitoredDevice.Id));

            return deviceIds;
        }

       
        public ActionResult GetFirstLevelGrouping()
        {
            var level1List = _userService.GetMonitoringGrouping1LevelsByUser(_currentUserProvider.CurrentUser.Id);
            level1List.Add(new CompanyGrouping1Level() { Id = -1, Name = "-- Next Level --" });

            ViewBag.AvailableFirstLevelGroups = new SelectList(level1List, "Id", "Name");
            ViewBag.AvailableSecondLevelGroups = new SelectList(Enumerable.Empty<CompanyGrouping2Level>(), "Id", "Name");
            ViewBag.AvailableSites = new SelectList(Enumerable.Empty<Site>(), "Id", "Name");
            ViewBag.AvailableDevices = new SelectList(Enumerable.Empty<Device>(), "Id", "Name");
            ViewBag.FirstLevelName = _currentUserProvider.CurrentUser.Company.FirstLevelGrouping;
            ViewBag.SecondLevelName = _currentUserProvider.CurrentUser.Company.SecondLevelGrouping;

            var filterModel = new AlertFilterViewModel()
            {
                AvailableAlertTypeList = _alarmConfigurationService.GetAllAlarmTypes(),
                AvailableUserList = _userService.GetUsersByCompany(_currentUserProvider.CurrentUser.Company.Id, UserStatus.AllUsers),
                DateFrom = DateTime.Now.ToShortDateString(),
                DateTo = DateTime.Now.ToShortDateString()
            };

            ViewBag.AlertFilterViewModel = filterModel;

            List<CompanyLevel1> lstCompanyLevel1 = new List<CompanyLevel1>();
            foreach (CompanyGrouping1Level item in level1List)
            {
                CompanyLevel1 objCompanyLevel1 = new CompanyLevel1();
                objCompanyLevel1.Id = item.Id;
                objCompanyLevel1.Name = item.Name;
                lstCompanyLevel1.Add(objCompanyLevel1);
            }

            return Json(lstCompanyLevel1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsyncSecondLevelGroup(int FirstLevelGroup)
        {
            List<CompanyLevel2> lstCompanyLevel2 = new List<CompanyLevel2>();
            var secondLevelItems = (FirstLevelGroup != -1) ? _companyService.GetGrouping2LevelsByGrouping1LevelId(FirstLevelGroup)
                                                        : _userService.GetMonitoringGrouping2LevelsByUser(_currentUserProvider.CurrentUser.Id, null);
            secondLevelItems.Add(new CompanyGrouping2Level() { Id = -1, Name = "-- Next Level --" });
            var select = new SelectList(secondLevelItems, "Id", "Name");
            // return Json(secondLevelItems.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
            foreach (var item in select)
            {
                CompanyLevel2 objCompanyLevel2 = new CompanyLevel2();
                objCompanyLevel2.Id = Convert.ToInt32(item.Value);
                objCompanyLevel2.Name = item.Text;
                lstCompanyLevel2.Add(objCompanyLevel2);
            }
            return Json(lstCompanyLevel2, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsyncSites(int SecondLevelGroup)
        {
            var siteList = (SecondLevelGroup != -1) ? _siteService.GetSitesByCompanyGrouping2Level(SecondLevelGroup)
                                              : _userService.GetMonitoringSitesByUser(_currentUserProvider.CurrentUser.Id, null);
            siteList.Add(new Site() { Id = -1, Name = "-- Next Level --" });

            var select = new SelectList(siteList, "Id", "Name");
            return Json(siteList.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsyncDevices(int Site)
        {
            var deviceList = (Site != -1) ? _deviceService.GetDevicesBySiteId(Site)
                                               : _userService.GetMonitoringDevicesByUser(_currentUserProvider.CurrentUser.Id, null);

            var select = new SelectList(deviceList, "Id", "Name");
            return Json(deviceList.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAlertType()
        {
            var AvailableAlertTypeList = _alarmConfigurationService.GetAllAlarmTypes();
            AvailableAlertTypeList.Add("All");
            List<SelectListItem> lstSelectedItem = new List<SelectListItem>();
            foreach (var item in AvailableAlertTypeList)
            {
                SelectListItem objselectedListItem = new SelectListItem();
                objselectedListItem.Text = item;
                objselectedListItem.Value = item;
                lstSelectedItem.Add(objselectedListItem);
            }
            return Json(lstSelectedItem.ToList().OrderBy(x=>x.Text), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserList()
        {
          
            var AvailableUserList = _userService.GetUsersByCompany(_currentUserProvider.CurrentUser.Company.Id, UserStatus.AllUsers);
            List<SelectListItem> lstSelectedUser = new List<SelectListItem>();
            foreach (var item in AvailableUserList)
            {
                SelectListItem objselectedListItem = new SelectListItem();
                objselectedListItem.Text = item.Role.Name + "," + item.FirstName + "(" + item.Username + ")";
                objselectedListItem.Value = item.Id.ToString();
                lstSelectedUser.Add(objselectedListItem);
            }
            SelectListItem objSelectListItemAll = new SelectListItem();
            objSelectListItemAll.Text = "All";
            objSelectListItemAll.Value = "All";
            lstSelectedUser.Add(objSelectListItemAll);
            return Json(lstSelectedUser.ToList().OrderBy(x=>x.Text), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmptyAlertType()
        {
            List<string> lstString = new List<string>();
            return Json(lstString, JsonRequestBehavior.AllowGet);
        }
    }

    public class DevicesAdded
    {
        public string DeviceAdded { get; set; }
    }

    public class CompanyLevel1
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CompanyLevel2
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
}
