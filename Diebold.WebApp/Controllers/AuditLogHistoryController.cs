using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Domain.Entities;
using Diebold.Domain.Enums;
using Diebold.Exporter;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.WebApp.Models;
using Lib.Web.Mvc.JQuery.JqGrid;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Diebold.WebApp.Controllers
{
    public class AuditLogHistoryController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ILogService _logService;
        private readonly IDvrService _deviceService;
        private readonly ISiteService _siteService;
        private readonly ICompanyService _companyService;

        public AuditLogHistoryController(IUserService userService, ICurrentUserProvider currentUserProvider, ILogService logService,
                                    IDvrService deviceService, ISiteService siteService, ICompanyService companyService)
        {
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _currentUserProvider = currentUserProvider;
            _logService = logService;
            _deviceService = deviceService;
            _companyService = companyService;
            _siteService = siteService;
        }
        //
        // GET: /AuditLog/

        public ActionResult Index()
        {
            ViewBag.AvailableFirstLevelGroups = new SelectList(_currentUserProvider.CurrentUser.Company.CompanyGrouping1Levels, "Id", "Name");
            ViewBag.AvailableSecondLevelGroups = new SelectList(Enumerable.Empty<CompanyGrouping2Level>(), "Id", "Name");
            ViewBag.AvailableSites = new SelectList(Enumerable.Empty<Site>(), "Id", "Name");
            ViewBag.AvailableDevices = new SelectList(Enumerable.Empty<Device>(), "Id", "Name");
            ViewBag.FirstLevelName = _currentUserProvider.CurrentUser.Company.FirstLevelGrouping;
            ViewBag.SecondLevelName = _currentUserProvider.CurrentUser.Company.SecondLevelGrouping;

            var filterModel = new LogHistoryFilterViewModel
            {
                AvailableActionTypeList = Enum.GetNames(typeof(LogAction)).Select(GetLogAction).ToList(),
                AvailableUserList = _userService.GetUsersByCompany(_currentUserProvider.CurrentUser.Company.Id, UserStatus.AllUsers),
                DateFrom = DateTime.Now.ToShortDateString(),
                DateTo = DateTime.Now.ToShortDateString()
            };

            ViewBag.LogHistoryFilterViewModel = filterModel;

            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FillReport(JqGridRequest request)
        {
            var actionTypes = new List<LogAction>();
            var userIds = new List<int>();

            var group1Ids = new List<int>();
            var group2Ids = new List<int>();
            var siteIds = new List<int>();
            var deviceIds = new List<int>();
            var groupLevelSelected = false;

            if (request.ExtraParams.ContainsKey("selectedGroup1Ids") && !string.IsNullOrEmpty(request.ExtraParams["selectedGroup1Ids"]))
            {
                groupLevelSelected = true;
                group1Ids.AddRange(request.ExtraParams["selectedGroup1Ids"].Split(',').Select(int.Parse));
            }
            if (request.ExtraParams.ContainsKey("selectedGroup2Ids") && !string.IsNullOrEmpty(request.ExtraParams["selectedGroup2Ids"]))
            {
                groupLevelSelected = true;
                group2Ids.AddRange(request.ExtraParams["selectedGroup2Ids"].Split(',').Select(int.Parse));
            }
            if (request.ExtraParams.ContainsKey("selectedSiteIds") && !string.IsNullOrEmpty(request.ExtraParams["selectedSiteIds"]))
            {
                groupLevelSelected = true;
                siteIds.AddRange(request.ExtraParams["selectedSiteIds"].Split(',').Select(int.Parse));
            }
            if (request.ExtraParams.ContainsKey("selectedDeviceIds") && !string.IsNullOrEmpty(request.ExtraParams["selectedDeviceIds"]))
            {
                groupLevelSelected = true;
                deviceIds.AddRange(request.ExtraParams["selectedDeviceIds"].Split(',').Select(int.Parse));
            }

            if (request.ExtraParams.ContainsKey("actionTypes") && !string.IsNullOrEmpty(request.ExtraParams["actionTypes"])
                                                              && request.ExtraParams["actionTypes"].ToLower() != "all"
                                                              && request.ExtraParams["actionTypes"].ToLower() != string.Empty)
            {
                actionTypes.AddRange(request.ExtraParams["actionTypes"].Split(',').Select(GetLogAction));
            }

            if (request.ExtraParams.ContainsKey("userIds") && !string.IsNullOrEmpty(request.ExtraParams["userIds"])
                                                           && request.ExtraParams["userIds"].ToLower() != "all"
                                                           && request.ExtraParams["userIds"].ToLower() != string.Empty)
            {
                userIds.AddRange(request.ExtraParams["userIds"].Split(',').Select(userId => Convert.ToInt32(userId)));
            }

            var historyLogs = _logService.GetPagedLogHistory(request.PageIndex + 1, request.RecordsCount,
                                                             request.SortingName,
                                                             request.SortingOrder.Equals(JqGridSortingOrders.Asc),
                                                             actionTypes, userIds,
                                                             request.ExtraParams["dateType"],
                                                             DateTime.Parse(request.ExtraParams["dateFrom"]),
                                                             DateTime.Parse(request.ExtraParams["dateTo"]),
                                                             group1Ids, group2Ids, siteIds, deviceIds,
                                                             groupLevelSelected,
                                                             (UserStatus)Enum.Parse(typeof(UserStatus), request.ExtraParams["userStatus"], true));

            var response = GetJqGridResponse(historyLogs, historyLogs.Select(MapEntity));

            return new JqGridJsonResult { Data = response };
        }

        private LogHistoryViewModel MapEntity(HistoryLog historyLog)
        {
            return new LogHistoryViewModel(historyLog);
        }

        private IList<LogHistoryViewModel> MapEntities(IEnumerable<HistoryLog> historyLogs)
        {
            return (from HistoryLog historyLog in historyLogs select MapEntity(historyLog)).ToList();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ExportToExcel(string sortingName, bool ascending, string actionTypes, string userIds,
            string selectedGroup1Ids, string selectedGroup2Ids, string selectedSiteIds, string selectedDeviceIds,
            string dateType, DateTime dateFrom, DateTime dateTo)
        {
            var historyLogs = GetLogHistory(sortingName, ascending, actionTypes, userIds, selectedGroup1Ids, selectedGroup2Ids, selectedSiteIds, selectedDeviceIds, dateType, dateFrom, dateTo);

            var bytes = new ExcelExporter().Export("Log History Report", OrientationPageType.Portrait, MapEntities(historyLogs), dateFrom, dateTo, null);

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "history.xlsx");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ExportToPdf(string sortingName, bool ascending, string actionTypes, string userIds,
            string selectedGroup1Ids, string selectedGroup2Ids, string selectedSiteIds, string selectedDeviceIds,
            string dateType, DateTime dateFrom, DateTime dateTo)
        {
            var historyLogs = GetLogHistory(sortingName, ascending, actionTypes, userIds, selectedGroup1Ids, selectedGroup2Ids, selectedSiteIds, selectedDeviceIds, dateType, dateFrom, dateTo);

            var bytes = new PdfExporter().Export("Log History Report", OrientationPageType.Portrait, MapEntities(historyLogs), dateFrom, dateTo, null);

            return File(bytes, "application/pdf", "history.pdf");
        }
        private IEnumerable<HistoryLog> GetLogHistory(string sortingName, bool ascending, string actionTypes, string userIds,
            string selectedGroup1Ids, string selectedGroup2Ids, string selectedSiteIds, string selectedDeviceIds,
            string dateType, DateTime dateFrom, DateTime dateTo)
        {
            var actionTypesList = new List<LogAction>();
            if (!string.IsNullOrEmpty(actionTypes) && actionTypes.ToLower() != "all")
                actionTypesList = actionTypes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(GetLogAction).ToList();

            var userIdsList = GetIdsList(userIds);
            var selectedGroup1IdsList = GetIdsList(selectedGroup1Ids);
            var selectedGroup2IdsList = GetIdsList(selectedGroup2Ids);
            var selectedSiteIdsList = GetIdsList(selectedSiteIds);
            var selectedDeviceIdsList = GetIdsList(selectedDeviceIds);
            IList<int> lstALLCompanyLevel2Id = new List<int>();
            IList<int> lstALLDeviceId = new List<int>();
            IList<int> lstALLSiteId = new List<int>();



            #region "First Level Selected All others are not selected"
            // Get District Details
            if (selectedGroup1IdsList != null && selectedGroup1IdsList.Count() > 0 && (selectedGroup2IdsList == null || selectedGroup2IdsList.Count <= 0) && (selectedSiteIdsList == null || selectedSiteIdsList.Count() <= 0) && (selectedDeviceIdsList == null || selectedDeviceIdsList.Count() <= 0))
            {
                selectedDeviceIdsList = new List<int>();
                foreach (int item in selectedGroup1IdsList)
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
            else if (selectedGroup1IdsList != null && selectedGroup1IdsList.Count() > 0 && selectedGroup2IdsList != null && selectedGroup2IdsList.Count() > 0 && (selectedSiteIdsList == null || selectedSiteIdsList.Count() <= 0) && (selectedDeviceIdsList == null || selectedDeviceIdsList.Count() <= 0))
            {
                // Get Site Details by Company Level2
                selectedDeviceIdsList = new List<int>();
                foreach (int companyLevel2Id in selectedGroup2IdsList)
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
            else if (selectedGroup1IdsList != null && selectedGroup1IdsList.Count() > 0 && selectedGroup2IdsList != null && selectedGroup2IdsList.Count() > 0 && (selectedSiteIdsList != null || selectedSiteIdsList.Count > 0) && (selectedDeviceIdsList == null || selectedDeviceIdsList.Count() <= 0))
            {
                selectedDeviceIdsList = new List<int>();
                // Get Site Id by Site Name
                string[] siteNames = selectedSiteIds.Split(',');
                for (int i = 0; i < siteNames.Count(); i++)
                {
                    lstALLSiteId.Add(Convert.ToInt32(siteNames[i]));
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
            else if (selectedGroup1IdsList != null && selectedGroup1IdsList.Count() > 0 && selectedGroup2IdsList != null && selectedGroup2IdsList.Count() > 0 && (selectedSiteIdsList != null || selectedSiteIdsList.Count() > 0) && (selectedDeviceIdsList != null || selectedDeviceIdsList.Count() > 0))
            {
                selectedDeviceIdsList = new List<int>();
                string[] DeviceId = selectedDeviceIds.Split(',');
                for (int i = 0; i < DeviceId.Count(); i++)
                {
                    lstALLDeviceId.Add(Convert.ToInt32(DeviceId[i]));
                }

            }
            #endregion
            if (lstALLDeviceId != null && lstALLDeviceId.Count() > 0)
            {
                selectedDeviceIdsList.AddRange(lstALLDeviceId);
            }









            var groupLevelSelected = selectedGroup1IdsList.Count > 0 || selectedGroup2IdsList.Count > 0
                                      || selectedSiteIdsList.Count > 0 || selectedDeviceIdsList.Count > 0;

            var logHistory = _logService.GetAllLogHistory(sortingName, ascending, actionTypesList, userIdsList,
                                                        dateType, dateFrom, dateTo, selectedGroup1IdsList, selectedGroup2IdsList,
                                                        selectedSiteIdsList, selectedDeviceIdsList, groupLevelSelected, UserStatus.AllUsers);
            return logHistory;
        }

        private static LogAction GetLogAction(string value)
        {
            LogAction y;
            Enum.TryParse(value, out y);
            return y;
        }

        private static List<int> GetIdsList(string ids)
        {
            var list = new List<int>();
            if (!string.IsNullOrEmpty(ids) && ids.ToLower() != "all")
                list = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            return list;
        }

        public ActionResult AsyncUsers(string userStatus)
        {
            var select = new SelectList(new List<string>());
            var users = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(userStatus))
            {
                var status = (UserStatus)Enum.Parse(typeof(UserStatus), userStatus, true);
                var userList = _userService.GetUsersByCompany(_currentUserProvider.CurrentUser.Company.Id, status);

                users.Add(new SelectListItem { Text = "All", Value = "all" });
                users.AddRange(userList.Select(user => new SelectListItem
                {
                    Text = user.LastName.ToString() + ", " + user.FirstName.ToString() + " (" + user.Username.ToString() + ")",
                    Value = user.Id.ToString()
                }).ToList());

                select = new SelectList(users, "Value", "Text");
            }

            return Json(select);
        }

        public ActionResult GetActionType()
        {
            var AvailableActionTypeList = Enum.GetNames(typeof(LogAction)).Select(GetLogAction).ToList();
            List<SelectListItem> lstSelectedListItem = new List<SelectListItem>();
            AvailableActionTypeList.Remove(LogAction.All);
            foreach (var item in AvailableActionTypeList)
            {
                SelectListItem objSelectListItem = new SelectListItem();
                objSelectListItem.Text = item.ToString();
                objSelectListItem.Value = item.ToString();
                lstSelectedListItem.Add(objSelectListItem);
            }
            SelectListItem objSelectListItemAll = new SelectListItem();
            objSelectListItemAll.Text = "All";
            objSelectListItemAll.Value = "All";

            lstSelectedListItem = lstSelectedListItem.Where(y => y.Value != "SignIn").OrderBy(x => x.Value).ToList();
            lstSelectedListItem.Insert(0, objSelectListItemAll);
            return Json(lstSelectedListItem.ToList(), JsonRequestBehavior.AllowGet);
            //return Json(lstSelectedListItem.Where(y => y.Value != "SignIn").OrderBy(x => x.Value).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserList()
        {
            var AvailableUserList = _userService.GetUsersByCompany(_currentUserProvider.CurrentUser.Company.Id, UserStatus.AllUsers);
            List<SelectListItem> lstSelectedUser = new List<SelectListItem>();            
            foreach (var item in AvailableUserList)
            {
                SelectListItem objselectedListItem = new SelectListItem();
                objselectedListItem.Text = item.Role.Name + " , " + item.FirstName + " ( " + item.Username + " ) ";
                objselectedListItem.Value = item.Id.ToString();
                lstSelectedUser.Add(objselectedListItem);
            }
            SelectListItem objSelectListItemAll = new SelectListItem();
            objSelectListItemAll.Text = "All";
            objSelectListItemAll.Value = "All";
            
            lstSelectedUser = lstSelectedUser.ToList().OrderBy(x => x.Text).ToList();
            lstSelectedUser.Insert(0,objSelectListItemAll);
            return Json(lstSelectedUser.ToList(), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult BindAuditReport([DataSourceRequest] DataSourceRequest request, IList<string> param)
        {
            logger.Debug("Bind Audit Report Started.");
            var QueryParem = param[0].Split(new string[] { ";" }, StringSplitOptions.None).ToList();
            logger.Debug("Bind Audit Report Parameters" + QueryParem);
            var actionTypes = new List<LogAction>();
            var userIds = new List<int>();

            var group1Ids = new List<int>();
            var group2Ids = new List<int>();
            var siteIds = new List<int>();
            var deviceIds = new List<int>();
            IList<int> lstALLCompanyLevel2Id = new List<int>();
            IList<int> lstALLDeviceId = new List<int>();
            IList<int> lstALLSiteId = new List<int>();
            var selectedGroup1Ids = QueryParem.Where(x => x.StartsWith("selectedGroup1Ids")).FirstOrDefault().Split(':')[1];
            var selectedGroup2Ids = QueryParem.Where(x => x.StartsWith("selectedGroup2Ids")).FirstOrDefault().Split(':')[1];
            var selectedSiteIds = QueryParem.Where(x => x.StartsWith("selectedSiteIds")).FirstOrDefault().Split(':')[1]; //request.ExtraParams["selectedSiteIds"];
            var selectedDeviceIds = QueryParem.Where(x => x.StartsWith("selectedDeviceIds")).FirstOrDefault().Split(':')[1]; //request.ExtraParams["selectedDeviceIds"];


            string ActionType = QueryParem.Where(x => x.StartsWith("actionTypes")).FirstOrDefault().Split(':')[1].ToString();
            string userId = QueryParem.Where(x => x.StartsWith("userIds")).FirstOrDefault().Split(':')[1].ToString();
            string dateFrom = QueryParem.Where(x => x.StartsWith("dateFrom")).FirstOrDefault().Split(':')[1].ToString();
            string dateTo = QueryParem.Where(x => x.StartsWith("dateTo")).FirstOrDefault().Split(':')[1].ToString();
            string UserStatus = QueryParem.Where(x => x.StartsWith("deviceStatus")).FirstOrDefault().Split(':')[1].ToString();
            // string dateType = QueryParem.Where(x => x.StartsWith("dateType")).FirstOrDefault().Split(':')[1].ToString();
            string dateType = "";


            if (!string.IsNullOrEmpty(ActionType) && ActionType.ToLower() != "all")
            {
                actionTypes.AddRange(ActionType.Split(',').Select(GetLogAction));
            }

            if (!string.IsNullOrEmpty(userId) && userId.ToLower() != "all")
            {
                userIds.AddRange(userId.Split(',').Select(uId => Convert.ToInt32(uId)));
            }
            if (!string.IsNullOrEmpty(selectedGroup1Ids))
            {
                group1Ids.AddRange(selectedGroup1Ids.Split(',').Select(y => Convert.ToInt32(y)));
            }
            if (!string.IsNullOrEmpty(selectedGroup2Ids))
            {
                group2Ids.AddRange(selectedGroup2Ids.Split(',').Select(x => Convert.ToInt32(x)));
            }
            if (!string.IsNullOrEmpty(selectedSiteIds))
            {
                siteIds.AddRange(selectedSiteIds.Split(',').Select(sId => Convert.ToInt32(sId)));
            }
            if (!string.IsNullOrEmpty(selectedDeviceIds))
            {
                deviceIds.Add(Convert.ToInt32(selectedDeviceIds));
            }

            var userIdsList = GetIdsList(userId);
            var selectedGroup1IdsList = GetIdsList(selectedGroup1Ids);
            var selectedGroup2IdsList = GetIdsList(selectedGroup2Ids);
            var selectedSiteIdsList = GetIdsList(selectedSiteIds);
            var selectedDeviceIdsList = GetIdsList(selectedDeviceIds);

            var groupLevelSelected = selectedGroup1IdsList.Count > 0 || selectedGroup2IdsList.Count > 0
                                      || selectedSiteIdsList.Count > 0 || selectedDeviceIdsList.Count > 0;

            IList<HistoryLog> HistoryLog = new List<HistoryLog>();

            #region "First Level Selected All others are not selected"
            // Get District Details
            if (group1Ids != null && group1Ids.Count() > 0 && (group2Ids == null || group2Ids.Count <= 0) && (selectedSiteIds == null || selectedSiteIds == "") && (selectedDeviceIds == null || selectedDeviceIds == ""))
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
                    lstALLSiteId.Add(Convert.ToInt32(siteNames[i]));
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
            group2Ids.AddRange(lstALLCompanyLevel2Id);
            siteIds.AddRange(lstALLSiteId);
            if (!string.IsNullOrEmpty(UserStatus))
            {
                HistoryLog = _logService.GetAllLogHistory("Date", false, actionTypes, userIds, dateType, DateTime.Parse(dateFrom),
                                                         DateTime.Parse(dateTo), group1Ids, group2Ids, siteIds, deviceIds, groupLevelSelected, (UserStatus)Enum.Parse(typeof(UserStatus), UserStatus, true)).ToList();
            }

            List<LogHistoryViewModel> lstLogHistoryViewModel = new List<LogHistoryViewModel>();
            foreach (var item in HistoryLog)
            {
                LogHistoryViewModel objLogHistoryViewModel = new LogHistoryViewModel();
                objLogHistoryViewModel.ViewDate = item.Date.ToString("MM/dd hh:mm tt");
                objLogHistoryViewModel.LogAction = item.Action.ToString();
                objLogHistoryViewModel.User = item.User.Name;
                lstLogHistoryViewModel.Add(objLogHistoryViewModel);

            }
            logger.Debug("Bind Audit Report Completed.");
            return Json(lstLogHistoryViewModel.ToDataSourceResult(request));
        }

        public ActionResult GetEmptyActionType()
        {
            List<string> lstString = new List<string>();
            return Json(lstString, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAvailableUserStatus()
        {
            //  SelectList AvailableDevice = new SelectList(new SelectList(Enumerable.Empty<Device>(), "Id", "Name"));
            //  return Json(AvailableDevice.ToList(), JsonRequestBehavior.AllowGet);
            var availableUserStatus = Enum.GetNames(typeof(UserStatus)).Select(status => new SelectListItem
            {
                Text = status.ToString().SplitByUpperCase(),
                Value = status
            }).ToList();
            return Json(availableUserStatus.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAvailableFilters()
        {            
            var availableFilters = Enum.GetNames(typeof(ReportFilters)).Select(filters => new SelectListItem
            {
                Text = filters.ToString().SplitByUpperCase(),
                Value = filters
            }).ToList();
            return Json(availableFilters.ToList(), JsonRequestBehavior.AllowGet);
        }

    }
}
