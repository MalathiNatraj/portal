using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Enums;
using Diebold.Services.Exceptions;
using Diebold.Services.Extensions;
using DieboldMobile.Infrastructure.Helpers;
using DieboldMobile.Models;
using Diebold.Services.Contracts;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.Domain.Entities;
using System.Data;
using DieboldMobile.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Configuration;

namespace DieboldMobile.Controllers
{
    public class DashboardController : BaseController
    {
        protected readonly IAlertService _alertService;
        protected readonly IDvrService _deviceService;
        protected readonly ISiteService _siteService;
        protected readonly INoteService _noteService;
        protected readonly IUserService _userService;
        protected readonly INotificationService _notificationService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ILinkService _linkService;
        private readonly IRSSFeedService _RssFeedService;
        private readonly IUserPortletsPreferences _userPortletsPreferences;
        private readonly IAccessService _accessService;

        public DashboardController(IAlertService alertService,
            IDvrService deviceService,
            ISiteService siteService,
            INoteService noteService,
            IUserService userService,
            INotificationService notificationService,
            ICurrentUserProvider currentUserProvider,
            ILinkService linkService,
            IRSSFeedService RssFeedService,
            IUserPortletsPreferences userPortletsPreferences,
            IAccessService accessService
            )
        {
            _alertService = alertService;
            _deviceService = deviceService;
            _siteService = siteService;
            _noteService = noteService;
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _notificationService = notificationService;
            _linkService = linkService;
            _RssFeedService = RssFeedService;
            _userPortletsPreferences = userPortletsPreferences;
            _accessService = accessService;

        }

        public ActionResult Index()
        {
            return View();
        }

        private static AlertDetailViewModel GetInitializedViewModel()
        {
            return new AlertDetailViewModel
            {
                SiteName = "",
                GatewayName = "",
                Address = "",
                State = "",
                MonitoredDevicesAlarmsCount = "",
                MonitoredDevicesCount = "",
                DeviceName = "",
                AlertName = "",
                DeviceIpHostname = "",
                Recorded = "",
                Unattended = ""
            };
        }

        #region Overview

        //GET: /Dashboard/Overview
        public ActionResult Overview()
        {
            var userId = _currentUserProvider.CurrentUser.Id;

            var viewmodel = new OverviewDashboardViewModel
            {
                DeviceCount = _userService.GetMonitoredDevicesCount(userId),
                GatewayCount = _userService.GetMonitoredGatewaysCount(userId),
                SiteCount = _userService.GetMonitoredSitesCount(userId),

                DevicesOnRedCount = _alertService.GetAlertedDevicesCount(userId),
                DevicesOnGreenCount = _alertService.GetOkDevicesCount(userId)
            };

            var lastAlert = _alertService.GetLastAlertTimeStamp(userId);
            if (lastAlert.HasValue)
            {
                viewmodel.LastAlert = lastAlert.Value.GetTimeAgo();
            }

            return PartialView("_MonitoringOverview", viewmodel);
        }

        #endregion

        #region Alerts/Devices List

        //GET: /Dashboard/AlertList
        public ActionResult AlertList()
        {
            return PartialView("_AlertList");
        }

        //GET: /Dashboard/DevicesList
        public ActionResult DevicesList()
        {
            return PartialView("_DevicesList");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FillAlertsList(JqGridRequest request)
        {
            var pagedList = _alertService.GetAlertsByStatus(request.PageIndex + 1, request.RecordsCount,
                request.SortingName, request.SortingOrder == JqGridSortingOrders.Asc, DashboardFilter.Alerts);

            var response = GetJqGridResponse(pagedList, pagedList.Select(MapAlertEntity));

            return new JqGridJsonResult { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FillDevicesList(JqGridRequest request)
        {
            var pagedList = _alertService.GetAlertsByStatus(request.PageIndex + 1, request.RecordsCount,
                request.SortingName, request.SortingOrder == JqGridSortingOrders.Asc, DashboardFilter.Normal);

            var response = GetJqGridResponse(pagedList, pagedList.Select(MapDeviceEntity));

            return new JqGridJsonResult { Data = response };
        }

        //
        //GET: /Dashboard/LoadViewMoreAlerts
        public ActionResult LoadViewMoreAlerts()
        {
            return PartialView("_ViewMoreAlertsView");
        }

        //
        //GET: /Dashboard/LoadViewMoreDevices
        public ActionResult LoadViewMoreDevices(int? DeviceId)
        {
            int intDeviceId = Convert.ToInt32(DeviceId);
            // return Json(qry).ToDataSourceResult(request);
            var ResultSet = _alertService.GetAlertsByDevice(intDeviceId);
            IList<DeviceListDashboardViewModel> lstDeviceListDashboardViewModel = new List<DeviceListDashboardViewModel>();
            ResultSet.ForEach(x =>
            {
                lstDeviceListDashboardViewModel.Add(new DeviceListDashboardViewModel { Id = x.Id, DeviceName = x.Device.Name, AlertName = x.Device.Name + x.Alarm.AlarmType, IsDeviceOk = x.IsOk, Ack = x.IsAcknowledged.ToString() });
            });

            return PartialView("_ViewMoreDevicesView", lstDeviceListDashboardViewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendEmail(int alertStatusId)
        {
            try
            {
                var alertStatus = _alertService.GetAlertStatusByPK(alertStatusId);

                var notification = new Notification()
                {
                    DeviceId = alertStatus.Device.Id,
                    DeviceName = alertStatus.Device.Name,

                    AlarmName = alertStatus.Alarm.AlarmType.Value.GetDescription() + " " + AlarmHelper.GetAlertDescriptionForAlert(
                                    (AlarmType)alertStatus.Alarm.AlarmType, alertStatus.ElementIdentifier, (Dvr)alertStatus.Device),

                    DateOccur = (alertStatus.FirstAlertTimeStamp != null) ? alertStatus.FirstAlertTimeStamp.Value.ToString(
                                        "MMMM dd, yyyy H:mm", CultureInfo.CreateSpecificCulture("en-US")) : string.Empty,

                    AlertCleared = alertStatus.IsOk
                };

                if (alertStatus.Device.IsDvr)
                {
                    var dvr = (Dvr)alertStatus.Device;
                    notification.TimeZone = dvr.TimeZone;
                    notification.SiteName = dvr.Site.Name;
                    notification.SiteAddress1 = dvr.Site.Address1;
                    notification.SiteAddress2 = dvr.Site.Address2;
                }

                _notificationService.SendMailNotification(notification);
                return new JsonResult() { Data = new { Status = "OK", Message = "An email has been sent to all the users monitoring the device" } };
            }
            catch (ServiceException serviceException)
            {
                LogError("Service Exception occured while sending mail", serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError("Exception occured while sending mail", e);
                return JsonError("An error occurred while executing the action");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendEmc(int alertStatusId)
        {
            try
            {
                var alertStatus = _alertService.GetAlertStatusByPK(alertStatusId);

                var notification = new Notification
                {
                    EmcAccontNumber = ((Dvr)alertStatus.Device).Gateway.EMCId.ToString(),
                    EmcDevicezone = ((Dvr)alertStatus.Device).ZoneNumber
                };

                _notificationService.SendEmcNotification(notification);
                return new JsonResult() { Data = new { Status = "OK", Message = "A message has been sent to EMC" } };
            }
            catch (ServiceException serviceException)
            {
                LogError("Service Exception occured while sending emc", serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError("Exception occured while sending emc", e);
                return JsonError("An error occurred while executing the action");
            }
        }

        #endregion

        #region Site LogInfo

        //GET: /Dashboard/SiteInfo
        public ActionResult SiteInfo()
        {
            ViewBag.NotesEnabled = false;
            return PartialView("_SiteDetail", GetInitializedViewModel());
        }

        //POST: /Dashboard/ReloadSiteInfo
        public ActionResult ReloadSiteInfo(int id)
        {
            AlertDetailViewModel viewmodel;

            if (id > 0)
            {
                ViewBag.NotesEnabled = true;

                viewmodel = new AlertDetailViewModel(_alertService.GetAlertStatusByPK(id));

                viewmodel.MonitoredDevicesCount =
                    _userService.GetMonitoredDevicesCountBySite(_currentUserProvider.CurrentUser.Id,
                                                                viewmodel.SiteId).ToString();

                viewmodel.MonitoredDevicesAlarmsCount =
                    _alertService.GetConfiguredAlarmsBySiteAndCurrentUserCount(viewmodel.SiteId).ToString();

            }
            else
            {
                ViewBag.NotesEnabled = false;
                viewmodel = GetInitializedViewModel();
            }

            return PartialView("_SiteDetail", viewmodel);
        }

        #endregion

        #region Alert Detail

        //GET: /Dashboard/AlertInfo
        public ActionResult AlertInfo()
        {
            AlertDetailViewModel viewmodel = GetInitializedViewModel();
            ViewBag.AcknowledgeButtonEnabled = false;
            return PartialView("_AlertInfo", viewmodel);
        }

        //POST: /Dashboard/ReeloadAlertInfo
        public ActionResult ReloadAlertInfo(int id)
        {
            AlertDetailViewModel viewmodel;

            if (id > 0)
            {
                var alertStatus = _alertService.GetAlertStatusByPK(id);

                ViewBag.AcknowledgeButtonEnabled = !alertStatus.IsAcknowledged;

                viewmodel = new AlertDetailViewModel(alertStatus);
            }
            else
            {
                ViewBag.AcknowledgeButtonEnabled = false;
                viewmodel = GetInitializedViewModel();
            }

            return PartialView("_AlertInfo", viewmodel);
        }

        public ActionResult Acknowledge(int id)
        {
            try
            {
                this._alertService.AcknowledgeAlert(id);

                return JsonOK();
            }
            catch (ServiceException ex)
            {
                LogError("Service Exception occured while acknowledging", ex);
                return JsonError(ex.Message);
            }
        }

        #endregion

        #region Notes

        //GET: /Dashboard/LoadViewMoreNotes
        public ActionResult LoadViewMoreNotes(int deviceId)
        {
            ViewBag.DeviceId = deviceId;

            return PartialView("_ViewMoreNotesView");
        }

        //POST: /Dashboard/AddNote
        public ActionResult AddNote(int deviceId, string text)
        {
            try
            {
                var note = new Note
                {
                    Date = DateTime.Now,
                    Device = _deviceService.GetDevice(deviceId),
                    Text = text,
                    User = _userService.GetUserByUserName(User.Identity.Name)
                };

                _noteService.Create(note);

                return JsonOK();
            }
            catch (ServiceException ex)
            {
                LogError("Service Exception occured while adding note", ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult LastNotes(/*int id*/)
        {
            //ViewBag.DeviceId = id;
            return PartialView("_LastNotes");
        }

        //public ActionResult FillNoteList(JqGridRequest request)
        //{
        //    if (!request.ExtraParams.Keys.Contains("deviceId"))
        //    {
        //        return new JqGridJsonResult { Data = GetJqGridResponseEmpty<Note, NoteListDashboardViewModel>() };
        //    }

        //    var deviceId = int.Parse(request.ExtraParams["deviceId"]);

        //    if (!(deviceId > 0))
        //        return new JqGridJsonResult { Data = GetJqGridResponseEmpty<Note, NoteListDashboardViewModel>() };

        //    var notesPage = _noteService.GetLastPagedByDevice(deviceId, request.PageIndex + 1, request.RecordsCount);

        //    var response = GetJqGridResponse(notesPage, notesPage.Select(x => new NoteListDashboardViewModel(x)));

        //    return new JqGridJsonResult { Data = response };
        //}

        #endregion

        #region Current Status

        //GET: /Dashboard/CurrentReadings
        //public ActionResult CurrentReadings(int id, bool liveFromDevice = false)
        //{
        //    try
        //    {
        //        if (id > 0)
        //        {
        //            var device = _deviceService.Get(id);
        //            ViewBag.DeviceId = id;
        //            ViewBag.DeviceName = device.Name;
        //            ViewBag.ShowViewMore = true;

        //            //var status = device.DeviceStatus.Count > 0 ? 

        //            var status = _deviceService.GetLiveStatus(id, device.Gateway.MacAddress, liveFromDevice, false);

        //            IEnumerable<DeviceStat usViewModel> deviceStatusModel = null;//PredefinedDeviceStatusHelper.SortStatus(status)
        //            //.Select(x => new DeviceStatusViewModel(x, false)).ToList();
        //            foreach (var model in deviceStatusModel)
        //            {
        //                if (!model.IsDictionary) continue;

        //                var driveModels = ((IDictionary<string, object>)model.Value).Select(item =>
        //                                    PredefinedDeviceStatusHelper.GetDvrDriveViewModel(device, model, item.Key, item.Value))
        //                                    .Where(driveModel => driveModel != null).ToList();

        //                model.Value = driveModels;
        //            }

        //            return PartialView("_CurrentStatus", deviceStatusModel);
        //        }

        //        ViewBag.ShowViewMore = false;
        //        return PartialView("_CurrentStatus", new List<DeviceStatusViewModel>());
        //    }
        //    catch (ServiceException ex)
        //    {
        //        LogError("Service Exception occured while processing current readings", ex);
        //        return JsonError(ex.Message);
        //    }
        //}

        #endregion

        #region Last Resolved Alerts

        //GET: /Dashboard/LoadViewMoreResolved
        public ActionResult LoadViewMoreResolved(int deviceId)
        {
            ViewBag.DeviceId = deviceId;

            return PartialView("_ViewMoreResolvedView");
        }

        public ActionResult LastResolved(/*int id*/)
        {
            //var alert = _alertService.GetAlertStatusByPK(id);
            //ViewBag.DeviceId = id;

            return PartialView("_LastResolved");
        }

        public ActionResult FillResolvedList(JqGridRequest request)
        {
            if (!request.ExtraParams.Keys.Contains("deviceId"))
                return new JqGridJsonResult { Data = GetJqGridResponseEmpty<ResolvedAlert, ResolvedAlertListDashboardViewModel>() };

            var deviceId = int.Parse(request.ExtraParams["deviceId"]);

            if (!(deviceId > 0))
                return new JqGridJsonResult { Data = GetJqGridResponseEmpty<ResolvedAlert, ResolvedAlertListDashboardViewModel>() };

            var resolvedAlerts = _alertService.GetResolvedAlertsFormDevice(request.PageIndex + 1, request.RecordsCount,
                request.SortingName, request.SortingOrder.Equals(JqGridSortingOrders.Asc), deviceId);

            var response = GetJqGridResponse(resolvedAlerts, resolvedAlerts.Select(x => new ResolvedAlertListDashboardViewModel(x)));

            return new JqGridJsonResult { Data = response };
        }

        #endregion

        #region MapEntity

        protected IEnumerable<JqGridRecord> MapAlertEntityRecords(IList<AlertStatus> items)
        {
            return from item in items
                   select new JqGridRecord<AlertListDashboardViewModel>(Convert.ToString(item.Id),
                       this.MapAlertEntity(item));
        }

        protected IEnumerable<JqGridRecord> MapDeviceEntityRecords(IList<AlertStatus> items)
        {
            return from item in items
                   select new JqGridRecord<DeviceListDashboardViewModel>(Convert.ToString(item.Id),
                       this.MapDeviceEntity(item));
        }

        protected AlertListDashboardViewModel MapAlertEntity(AlertStatus item)
        {
            return new AlertListDashboardViewModel(item);
        }

        protected DeviceListDashboardViewModel MapDeviceEntity(AlertStatus item)
        {
            return new DeviceListDashboardViewModel(item);
        }

        #endregion

        #region Portal code

        public ActionResult LogOn()
        {
            return RedirectToAction("Index");
        }

        private void UpdateUserPrefrences()
        {
            var UserDetails = _userService.Get(_currentUserProvider.CurrentUser.Id);
            var RolePortlet = UserDetails.Role.RolePortlets.ToList();
            IList<UserPortletsPreferences> UserPortlet = new List<UserPortletsPreferences>();
            IList<UserPortletsPreferences> UserUpdatedPortlet = new List<UserPortletsPreferences>();
            UserPortlet = UserDetails.userPortletsPreferences;
            IList<UserPortletsPreferences> itemsToDelete = new List<UserPortletsPreferences>();
            IList<RolePortlets> itemsToAdd = new List<RolePortlets>();
            RolePortlet.ForEach(x =>
            {
                if (!UserPortlet.Select(y => y.Portlets).ToList().Contains(x.Portlets))
                    itemsToAdd.Add(x);
            });
            UserPortlet.ToList().ForEach(x =>
            {
                if (!RolePortlet.Select(port => port.Portlets).ToList().Contains(x.Portlets))
                    itemsToDelete.Add(x);
            });

            itemsToDelete.ForEach(x =>
            {
                UserPortlet.Remove(x);
            });

            int Col1MaxSeqNo = 0;
            int Col2MaxSeqNo = 0;
            int Col3MaxSeqNo = 0;

            if (UserPortlet != null && UserPortlet.Count > 0)
            {
                if (UserPortlet.Where(x => x.ColumnNo == 1).ToList().Count > 0)
                {
                    Col1MaxSeqNo = UserPortlet.Where(x => x.ColumnNo == 1).ToList().Max(x => x.SeqNo);
                }
                if (UserPortlet.Where(x => x.ColumnNo == 2).ToList().Count > 0)
                {
                    Col2MaxSeqNo = UserPortlet.Where(x => x.ColumnNo == 2).ToList().Max(x => x.SeqNo);
                }
                if (UserPortlet.Where(x => x.ColumnNo == 3).ToList().Count > 0)
                {
                    Col3MaxSeqNo = UserPortlet.Where(x => x.ColumnNo == 3).ToList().Max(x => x.SeqNo);
                }
            }

            IList<UserPortletsPreferences> objlstUserPortletsPreferences = new List<UserPortletsPreferences>();
            //Role objRole = _roleService.Get(item.Role.Id);
            itemsToAdd.ToList().ForEach(x =>
            {
                UserPortletsPreferences objUserPortletsPreferences = new UserPortletsPreferences();
                objUserPortletsPreferences.User = UserDetails;
                objUserPortletsPreferences.PortletId = x.Portlets.Id;
                objUserPortletsPreferences.Portlets = x.Portlets;
                objUserPortletsPreferences.ColumnNo = x.Portlets.ColumnNo;
                objUserPortletsPreferences.IsDisabled = true;
                if (x.Portlets.ColumnNo == 1)
                {
                    Col1MaxSeqNo += 1;
                    objUserPortletsPreferences.SeqNo = Col1MaxSeqNo;
                }
                else if (x.Portlets.ColumnNo == 2)
                {
                    Col2MaxSeqNo += 1;
                    objUserPortletsPreferences.SeqNo = Col2MaxSeqNo;
                }
                else if (x.Portlets.ColumnNo == 3)
                {
                    Col3MaxSeqNo += 1;
                    objUserPortletsPreferences.SeqNo = Col3MaxSeqNo;
                }
                UserPortlet.Add(objUserPortletsPreferences);
            });

            if (itemsToAdd.Count > 0 || itemsToDelete.Count > 0)
            {
                UserDetails.userPortletsPreferences = UserPortlet;
                _userService.Update(UserDetails);
            }
        }


        public ActionResult Home()
        {
            //UpdateUserPrefrences();
            DashboardModel objDashboardModel = new DashboardModel();


            // RSS Feed            
            //RSSFeedViewModel objRSSFeedViewModel = new Models.RSSFeedViewModel();
            //objRSSFeedViewModel.MaxRSSLinksCount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxRSSFeedLinks"]);
            //objRSSFeedViewModel.RSSFeedInterval = Convert.ToInt32(ConfigurationManager.AppSettings["RssFeedInterval"]);
            //// To get the max count at first time
            //objRSSFeedViewModel.RSSFeedCount = _RssFeedService.GetAllActiveRSSFeedsByUser(_currentUserProvider.CurrentUser.Id).Count;
            //objDashboardModel.RSS = objRSSFeedViewModel;

            // Video Health Check
            //VideoHealthCheckModel objVideoHealthCheckModel = new VideoHealthCheckModel();
            //objVideoHealthCheckModel = (new VideoHealthCheckService().GetVideoHealthCheckDetails()).FirstOrDefault();
            //objDashboardModel.VideoHealthCheck = objVideoHealthCheckModel;

            List<DeviceListDashboardViewModel> objlstAlerts = new List<Models.DeviceListDashboardViewModel>();
            objDashboardModel.AlertListDashboardViewModel = objlstAlerts;

            List<DeviceListDashboardViewModel> objlstAlertsAccess = new List<Models.DeviceListDashboardViewModel>();
            objDashboardModel.AlertListDashboardViewModelAccess = objlstAlertsAccess;


            //SiteInfo objSiteInfo = new SiteInfo();
            //objSiteInfo = (new SiteInfoService().GetSiteInfoDetails()).FirstOrDefault();
            //objDashboardModel.SiteInformation = objSiteInfo;

            AccountDetailModel objAccount = new AccountDetailModel();
            // To get company name of a user
            objAccount.CompanyName = _currentUserProvider.CurrentUser.Company.Name;
            objAccount.CompanyLogo = _currentUserProvider.CurrentUser.Company.CompanyLogo;
            // To get a site count
            objAccount.SiteCount = _userService.GetMonitoredSitesCount(_currentUserProvider.CurrentUser.Id);
            // To get Device count            
            objAccount.HealthDevices = _userService.GetDevicesCountByParentType(_currentUserProvider.CurrentUser.Id, AlarmParentType.DVR.ToString());
            objAccount.AccessDevices = _userService.GetDevicesCountByParentType(_currentUserProvider.CurrentUser.Id, AlarmParentType.Access.ToString());
            objAccount.IntrusionDevices = _userService.GetDevicesCountByParentType(_currentUserProvider.CurrentUser.Id, AlarmParentType.Intrusion.ToString());
            objAccount.DeviceCount = objAccount.HealthDevices + objAccount.AccessDevices + objAccount.IntrusionDevices;

            //objDashboardModel.AccountDetail = objAccount;

            //FeaturedNewsModel objFeaturedNewsModel = new FeaturedNewsModel();
            //objDashboardModel.FeaturedNews = objFeaturedNewsModel;


            //// Site Information

            //BaseLocationViewModel objBaseLocationViewModel = new Models.BaseLocationViewModel();
            //IList<SiteNoteViewModel> objlstSiteNoteViewModel = new List<SiteNoteViewModel>();
            //objBaseLocationViewModel.SiteNote = objlstSiteNoteViewModel;
            //objDashboardModel.baseLocationViewModel = objBaseLocationViewModel;

            IList<UserPortletsPreferencesViewModel> objlstUserPortletsPreferencesViewModel = new List<UserPortletsPreferencesViewModel>();


            IList<UserPortletsPreferences> objLstUserPortletsPreferences = new List<UserPortletsPreferences>();
            //objLstUserPortletsPreferences = _userPortletsPreferences.GetAllActivePortletsByUser(_currentUserProvider.CurrentUser.Id);

            //UserPortletsPreferencesViewModel objAccessControl = new UserPortletsPreferencesViewModel();
            //DashboardModel objAccessControlDashboard = new DashboardModel();

            //UserPortletsPreferencesViewModel objProfile = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardProfile = new DashboardModel();
            //UserPortletsPreferences objPreference = new UserPortletsPreferences();
            //var objLstUserProfile = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("UserProfile".ToUpper()));
            //if (objLstUserProfile != null && objLstUserProfile.Count() > 0)
            //{
            //    objPreference = new UserPortletsPreferences();
            //    objPreference = objLstUserProfile.First();

            //    objProfile.Id = objPreference.Id;
            //    objProfile.PortletName = objPreference.Portlets.Name;
            //    objProfile.PortletInternalName = objPreference.Portlets.InternalName;
            //    objProfile.ColumnNo = objPreference.ColumnNo;
            //    objProfile.SeqNo = objPreference.SeqNo;
            //    objDashboardProfile.UserProfile = objDashboardModel.UserProfile;
            //    objProfile.DashboardDetails = objDashboardProfile;
            //    if (objPreference.IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objProfile);
            //    }
            //}
            //UserPortletsPreferencesViewModel objFeaturedNews = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardFeaturedNews = new DashboardModel();
            //var strFN = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("FeaturedNews".ToUpper()));
            //if (strFN != null && strFN.Count() > 0)
            //{
            //    objFeaturedNews.Id = strFN.First().Id;
            //    objFeaturedNews.PortletName = strFN.First().Portlets.Name;
            //    objFeaturedNews.PortletInternalName = strFN.First().Portlets.InternalName;
            //    objFeaturedNews.ColumnNo = strFN.First().ColumnNo;
            //    objFeaturedNews.SeqNo = strFN.First().SeqNo;
            //    objDashboardFeaturedNews.FeaturedNews = objDashboardModel.FeaturedNews;
            //    objFeaturedNews.DashboardDetails = objDashboardFeaturedNews;
            //    if (strFN.First().IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objFeaturedNews);
            //    }
            //}


            //UserPortletsPreferencesViewModel objSiteInformation = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardSiteInformation = new DashboardModel();
            //var strSiteInfo = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("SiteInformation".ToUpper()));
            //if (strSiteInfo != null && strSiteInfo.Count() > 0)
            //{
            //    objSiteInformation.Id = strSiteInfo.First().Id;
            //    objSiteInformation.PortletName = strSiteInfo.First().Portlets.Name;
            //    objSiteInformation.PortletInternalName = strSiteInfo.First().Portlets.InternalName;
            //    objSiteInformation.ColumnNo = strSiteInfo.First().ColumnNo;
            //    objSiteInformation.SeqNo = strSiteInfo.First().SeqNo;
            //    //objDashboardSiteInformation.SiteInformation = objDashboardModel.SiteInformation;
            //    objDashboardSiteInformation.baseLocationViewModel = objDashboardModel.baseLocationViewModel;
            //    objSiteInformation.DashboardDetails = objDashboardSiteInformation;
            //    if (strSiteInfo.First().IsDisabled == false) // To Do
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objSiteInformation);
            //    }
            //}
            ////For Links Portlet

            //UserPortletsPreferencesViewModel objLinks = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardLinksInfo = new DashboardModel();
            //var strLinksInfo = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("Links".ToUpper()));
            //if (strLinksInfo != null && strLinksInfo.Count() > 0)
            //{
            //    objLinks.Id = strLinksInfo.First().Id;
            //    objLinks.PortletName = strLinksInfo.First().Portlets.Name;
            //    objLinks.PortletInternalName = strLinksInfo.First().Portlets.InternalName;
            //    objLinks.ColumnNo = strLinksInfo.First().ColumnNo;
            //    objLinks.SeqNo = strLinksInfo.First().SeqNo;
            //    objDashboardLinksInfo.Links = objDashboardModel.Links;
            //    objLinks.DashboardDetails = objDashboardLinksInfo;
            //    if (strLinksInfo.First().IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objLinks);
            //    }
            //}
            //// Video Health Check
            //UserPortletsPreferencesViewModel objVideoHealthCheck = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardVideoHealthCheck = new DashboardModel();
            //var strVideoHealthCheck = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("VIDEOHEALTHCHECK"));
            //if (strVideoHealthCheck != null && strVideoHealthCheck.Count() > 0)
            //{
            //    objVideoHealthCheck.Id = strVideoHealthCheck.First().Id;
            //    objVideoHealthCheck.PortletName = strVideoHealthCheck.First().Portlets.Name;
            //    objVideoHealthCheck.PortletInternalName = strVideoHealthCheck.First().Portlets.InternalName;
            //    objVideoHealthCheck.ColumnNo = strVideoHealthCheck.First().ColumnNo;
            //    objVideoHealthCheck.SeqNo = strVideoHealthCheck.First().SeqNo;
            //    objDashboardVideoHealthCheck.VideoHealthCheck = objDashboardModel.VideoHealthCheck;
            //    objVideoHealthCheck.DashboardDetails = objDashboardVideoHealthCheck;
            //    if (strVideoHealthCheck.First().IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objVideoHealthCheck);
            //    }
            //}
            //// SiteMap
            //// Required on 18 Feb
            //UserPortletsPreferencesViewModel objSiteMap = new UserPortletsPreferencesViewModel();
            //DashboardModel ovjSiteMapModel = new DashboardModel();
            //var strSiteMap = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("SiteMap".ToUpper()));
            //if (strSiteMap != null && strSiteMap.Count() > 0)
            //{
            //    objSiteMap.Id = strSiteMap.First().Id;
            //    objSiteMap.PortletName = strSiteMap.First().Portlets.Name;
            //    objSiteMap.PortletInternalName = strSiteMap.First().Portlets.InternalName;
            //    objSiteMap.ColumnNo = strSiteMap.First().ColumnNo;
            //    objSiteMap.SeqNo = strSiteMap.First().SeqNo;
            //    ovjSiteMapModel.SiteMap = ovjSiteMapModel.SiteMap;
            //    objSiteMap.DashboardDetails = ovjSiteMapModel;
            //    if (strSiteMap.First().IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objSiteMap);
            //    }
            //}
            //// IntrusionDetail

            //UserPortletsPreferencesViewModel objIntrusion = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardIntrusion = new DashboardModel();
            //var strIntrusion = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("INTRUSION".ToUpper()));
            //if (strIntrusion != null && strIntrusion.Count() > 0)
            //{
            //    objIntrusion.Id = strIntrusion.First().Id;
            //    objIntrusion.PortletName = strIntrusion.First().Portlets.Name;
            //    objIntrusion.PortletInternalName = strIntrusion.First().Portlets.InternalName;
            //    objIntrusion.ColumnNo = strIntrusion.First().ColumnNo;
            //    objIntrusion.SeqNo = strIntrusion.First().SeqNo;
            //    objDashboardIntrusion.MasterRooms = objDashboardModel.MasterRooms;
            //    objDashboardIntrusion.BaseMasterRoom = objDashboardModel.BaseMasterRoom;
            //    objIntrusion.DashboardDetails = objDashboardIntrusion;
            //    if (strIntrusion.First().IsDisabled == false) // To Do
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objIntrusion);
            //    }
            //}
            ////AccountDetail
            //UserPortletsPreferencesViewModel objAccountDetail = new UserPortletsPreferencesViewModel();
            //DashboardModel objAccountDetailModel = new DashboardModel();
            //var strAccountDetail = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("ACCOUNTDETAIL"));
            //if (strAccountDetail != null && strAccountDetail.Count() > 0)
            //{
            //    objAccountDetail.Id = strAccountDetail.First().Id;
            //    objAccountDetail.PortletName = strAccountDetail.First().Portlets.Name;
            //    objAccountDetail.PortletInternalName = strAccountDetail.First().Portlets.InternalName;
            //    objAccountDetail.ColumnNo = strAccountDetail.First().ColumnNo;
            //    objAccountDetail.SeqNo = strAccountDetail.First().SeqNo;
            //    objAccountDetailModel.AccountDetail = objDashboardModel.AccountDetail;
            //    objAccountDetail.DashboardDetails = objAccountDetailModel;
            //    if (strAccountDetail.First().IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objAccountDetail);
            //    }
            //}

            //UserPortletsPreferencesViewModel objRss = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardRssInfo = new DashboardModel();
            //var strRssInfo = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("RSS"));
            //if (strRssInfo != null && strRssInfo.Count() > 0)
            //{
            //    objRss.Id = strRssInfo.First().Id;
            //    objRss.PortletName = strRssInfo.First().Portlets.Name;
            //    objRss.PortletInternalName = strRssInfo.First().Portlets.InternalName;
            //    objRss.ColumnNo = strRssInfo.First().ColumnNo;
            //    objRss.SeqNo = strRssInfo.First().SeqNo;
            //    objDashboardRssInfo.RSS = objDashboardModel.RSS;
            //    objRss.DashboardDetails = objDashboardRssInfo;
            //    if (strRssInfo.First().IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objRss);
            //    }
            //}
            //// System Summary   
            //UserPortletsPreferencesViewModel objStatus = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardStatusModel = new DashboardModel();
            //var strStatus = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("SYSTEMSUMMARY"));
            //if (strStatus != null && strStatus.Count() > 0)
            //{
            //    objStatus.Id = strStatus.First().Id;
            //    objStatus.PortletName = strStatus.First().Portlets.Name;
            //    objStatus.PortletInternalName = strStatus.First().Portlets.InternalName;
            //    objStatus.ColumnNo = strStatus.First().ColumnNo;
            //    objStatus.SeqNo = strStatus.First().SeqNo;
            //    objDashboardStatusModel.SystemSummary = objDashboardModel.SystemSummary;
            //    objStatus.DashboardDetails = objDashboardStatusModel;
            //    if (strStatus.First().IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objStatus);
            //    }
            //}

            //UserPortletsPreferencesViewModel objMAS = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardMASModel = new DashboardModel();

            //var objLstMAS = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("MAS"));

            //if (objLstMAS != null && objLstMAS.Count() > 0)
            //{
            //    objPreference = new UserPortletsPreferences();
            //    objPreference = objLstMAS.First();
            //    objMAS.Id = objPreference.Id;
            //    objMAS.PortletName = objPreference.Portlets.Name;
            //    objMAS.PortletInternalName = objPreference.Portlets.InternalName;
            //    objMAS.ColumnNo = objPreference.ColumnNo;
            //    objMAS.SeqNo = objPreference.SeqNo;
            //    objDashboardStatusModel.MASDetail = objDashboardModel.MASDetail;
            //    objMAS.DashboardDetails = objDashboardStatusModel;
            //    if (objPreference.IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objMAS);
            //    }
            //}

            // Alerts  
            UserPortletsPreferencesViewModel objAlerts = new UserPortletsPreferencesViewModel();
            DashboardModel objDashboardAlertModel = new DashboardModel();
            var strAlert = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("ALERTS"));
            if (strAlert != null && strAlert.Count() > 0)
            {
                objAlerts.Id = strAlert.First().Id;
                objAlerts.PortletName = strAlert.First().Portlets.Name;
                objAlerts.PortletInternalName = strAlert.First().Portlets.InternalName;
                objAlerts.ColumnNo = strAlert.First().ColumnNo;
                objAlerts.SeqNo = strAlert.First().SeqNo;
                objDashboardAlertModel.AlertListDashboardViewModel = objDashboardModel.AlertListDashboardViewModel;
                objAlerts.DashboardDetails = objDashboardAlertModel;
                if (strAlert.First().IsDisabled == false)
                {
                    objlstUserPortletsPreferencesViewModel.Add(objAlerts);
                }
            }

            //// Live View
            //UserPortletsPreferencesViewModel objLiveView = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardLiveViewModel = new DashboardModel();
            //LiveViewModel objLiveVideoModel = new LiveViewModel();
            //objLiveVideoModel.IPConfugureManagementUri = ConfigurationManager.AppSettings["IPConfugureManagementUri"];
            //var strLiveView = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("LIVEVIEW"));
            //if (strLiveView != null && strLiveView.Count() > 0)
            //{
            //    objLiveView.Id = strLiveView.First().Id;
            //    objLiveView.PortletName = strLiveView.First().Portlets.Name;
            //    objLiveView.PortletInternalName = strLiveView.First().Portlets.InternalName;
            //    objLiveView.ColumnNo = strLiveView.First().ColumnNo;
            //    objLiveView.SeqNo = strLiveView.First().SeqNo;
            //    objDashboardModel.LiveView = objLiveVideoModel;
            //    objDashboardLiveViewModel.LiveView = objDashboardModel.LiveView;
            //    objLiveView.DashboardDetails = objDashboardLiveViewModel;
            //    if (strLiveView.First().IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objLiveView);
            //    }
            //}

            //// Live View Two
            //UserPortletsPreferencesViewModel objLiveViewTwo = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardLiveViewModelTwo = new DashboardModel();
            //LiveViewModel objLiveVideoModelTwo = new LiveViewModel();
            //objLiveVideoModelTwo.IPConfugureManagementUri = ConfigurationManager.AppSettings["IPConfugureManagementUri"];
            //var strLiveViewTwo = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("LIVEVIEW_TWO"));
            //if (strLiveViewTwo != null && strLiveViewTwo.Count() > 0)
            //{
            //    objLiveViewTwo.Id = strLiveViewTwo.First().Id;
            //    objLiveViewTwo.PortletName = strLiveViewTwo.First().Portlets.Name;
            //    objLiveViewTwo.PortletInternalName = strLiveViewTwo.First().Portlets.InternalName;
            //    objLiveViewTwo.ColumnNo = strLiveViewTwo.First().ColumnNo;
            //    objLiveViewTwo.SeqNo = strLiveViewTwo.First().SeqNo;
            //    objDashboardModel.LiveView = objLiveVideoModelTwo;
            //    objDashboardLiveViewModelTwo.LiveView = objDashboardModel.LiveView;
            //    objLiveViewTwo.DashboardDetails = objDashboardLiveViewModelTwo;
            //    if (strLiveViewTwo.First().IsDisabled == false)
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objLiveViewTwo);
            //    }
            //}
            //// ACCESSCONTROL 
            //UserPortletsPreferencesViewModel objAccessParent = new UserPortletsPreferencesViewModel();
            //DashboardModel objDashboardobjAccessParent = new DashboardModel();
            //var strAccessParent = objLstUserPortletsPreferences.Where(x => x.Portlets.InternalName.ToUpper().Equals("ACCESSCONTROL"));
            //if (strAccessParent != null && strAccessParent.Count() > 0)
            //{
            //    objAccessParent.Id = strAccessParent.First().Id;
            //    objAccessParent.PortletName = strAccessParent.First().Portlets.Name;
            //    objAccessParent.PortletInternalName = strAccessParent.First().Portlets.InternalName;
            //    objAccessParent.ColumnNo = strAccessParent.First().ColumnNo;
            //    objAccessParent.SeqNo = strAccessParent.First().SeqNo;
            //    objDashboardobjAccessParent.AccessControl = objDashboardModel.AccessControl;
            //    objAccessParent.DashboardDetails = objDashboardobjAccessParent;
            //    if (strAccessParent.First().IsDisabled == false) // To Do
            //    {
            //        objlstUserPortletsPreferencesViewModel.Add(objAccessParent);
            //    }
            //}
            ViewBag.DashboardModel = objDashboardModel;
            return View("Home", objlstUserPortletsPreferencesViewModel);
        }

        public JsonResult InsertData(string input)
        {
            string[] ColumnwiseItems = input.Split('&');
            string[] temp; string[] RowItems;
            IDictionary<string, string> dctPortletPostion = new Dictionary<string, string>();
            bool IsportletinCorrectPosition = true;
            foreach (var item in ColumnwiseItems)
            {
                if (item.Contains("C1"))
                {
                    temp = item.Split('~');
                    if (temp != null && temp.Length > 1)
                    {
                        if (temp[1].Contains("FeaturedNews") && temp[1].Length > "FeaturedNews".Length)
                        {
                            IsportletinCorrectPosition = false;
                            break;
                        }
                        RowItems = temp[1].Split(',');
                        for (int i = 0; i < RowItems.Length; i++)
                        {
                            dctPortletPostion.Add(RowItems[i].ToString(), "1~" + (i + 1).ToString());
                        }
                    }
                }
                if (item.Contains("C2"))
                {
                    temp = item.Split('~');
                    if (temp != null && temp.Length > 1)
                    {
                        RowItems = temp[1].Split(',');
                        for (int i = 0; i < RowItems.Length; i++)
                        {
                            dctPortletPostion.Add(RowItems[i].ToString(), "2~" + (i + 1).ToString());

                        }
                    }
                }
                if (item.Contains("C3"))
                {
                    temp = item.Split('~');
                    if (temp != null && temp.Length > 1)
                    {
                        if (temp[1].Contains("FeaturedNews") && temp[1].Length > "FeaturedNews".Length)
                        {
                            IsportletinCorrectPosition = false;
                            break;
                        }
                        RowItems = temp[1].Split(',');
                        for (int i = 0; i < RowItems.Length; i++)
                        {
                            dctPortletPostion.Add(RowItems[i].ToString(), "3~" + (i + 1).ToString());
                        }
                    }
                }

            }

            IList<UserPortletsPreferences> objLstUserPortletsPreferences = new List<UserPortletsPreferences>();
            IList<UserPortletsPreferences> objLstModifiedUserPortletsPreferences = new List<UserPortletsPreferences>();
            var resultSet = _userService.Get(_currentUserProvider.CurrentUser.Id);
            foreach (UserPortletsPreferences row in resultSet.userPortletsPreferences)
            {
                switch (row.Portlets.InternalName.ToUpper())
                {

                    case "LIVEVIEW":
                        var str = dctPortletPostion.Where(x => x.Key.Equals("LIVEVIEW")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "LIVEVIEW_TWO":
                        str = dctPortletPostion.Where(x => x.Key.Equals("LIVEVIEW_TWO")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "INTRUSION":
                        str = dctPortletPostion.Where(x => x.Key.Equals("INTRUSION")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "ACCESSCONTROL":
                        str = dctPortletPostion.Where(x => x.Key.Equals("ACCESSCONTROL")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "VIDEOHEALTHCHECK":
                        str = dctPortletPostion.Where(x => x.Key.Equals("VIDEOHEALTHCHECK")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "ALERTS":
                        str = dctPortletPostion.Where(x => x.Key.Equals("ALERTS")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "FEATUREDNEWS":
                        str = dctPortletPostion.Where(x => x.Key.Equals("FEATUREDNEWS")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "USERPROFILE":
                        str = dctPortletPostion.Where(x => x.Key.Equals("USERPROFILE")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                            objLstModifiedUserPortletsPreferences.Add(row);
                        }
                        break;
                    case "SITEINFORMATION":
                        str = dctPortletPostion.Where(x => x.Key.Equals("SITEINFORMATION")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "SITEMAP":
                        str = dctPortletPostion.Where(x => x.Key.Equals("SITEMAP")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "RSS":
                        str = dctPortletPostion.Where(x => x.Key.Equals("RSS")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "LINKS":
                        str = dctPortletPostion.Where(x => x.Key.Equals("LINKS")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "ACCOUNTDETAIL":
                        str = dctPortletPostion.Where(x => x.Key.Equals("ACCOUNTDETAIL")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "SystemSummary":
                        str = dctPortletPostion.Where(x => x.Key.Equals("SystemSummary")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    case "MAS":
                        str = dctPortletPostion.Where(x => x.Key.Equals("MAS")).FirstOrDefault().Value;
                        if (str != null)
                        {
                            string[] rs = str.Split('~');
                            row.SeqNo = Convert.ToInt32(rs[1]);
                            row.ColumnNo = Convert.ToInt32(rs[0]);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (IsportletinCorrectPosition == true)
            {
                _userService.Update(resultSet);
            }
            return Json(new { name = "Success" });
        }
        public ActionResult Returnfunc(string input)
        {
            return RedirectToAction("Index");
        }

        //public ActionResult SiteInformationDetails()
        //{
        //    return View(new SiteInfoService().GetSiteInfoDetails());
        //}

        //public JsonResult GetSiteInfoData()
        //{
        //    return Json(new SiteInfoService().GetSiteInfoDetails());
        //}

        //public JsonResult GetSiteInfoBySite(int SiteId)
        //{
        //    var resultSet = new SiteInfoService().GetSiteInfoDetails().Where(x => x.Id == SiteId);
        //    return Json(resultSet);
        //}

        public JsonResult GetAllDeviceDetails()
        {
            // Bind Device Details inside Combo Box
            List<DeviceModel> objDeviceModel = new List<DeviceModel>();
            objDeviceModel = (new DeviceService().GetDeviceDetails()).ToList();
            return Json(objDeviceModel.Select(c => new { Id = c.Id, Name = c.Name, Device = c.Device, Location = c.Location, Address = c.Address, Address1 = c.Address1, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllDeviceDetailsforHealthCheck()
        {
            // Bind Device Details inside Combo Box
            List<DeviceModel> objDeviceModel = new List<DeviceModel>();
            objDeviceModel = (new DeviceService().GetDeviceDetailsforHealthCheck()).ToList();
            return Json(objDeviceModel.Select(c => new { Id = c.Id, Device = c.Device, Location = c.Location, Address = c.Address, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetDeviceDetailByDeviceId(int DeviceId)
        //{
        //    var resultSet = new VideoHealthCheckService().GetVideoHealthCheckDetails().Where(x => x.DeviceListId == DeviceId);
        //    return Json(resultSet);
        //}

        public JsonResult GetDeviceDetailforAccessByDeviceId(int DeviceId)
        {

            var result = _accessService.GetAccessPortletDetails().Where(x => x.DeviceId == DeviceId);
            return Json(result);

        }

        //public JsonResult GetAllReportTypes()
        //{
        //    // Bind Report Types inside Combo Box
        //    List<ReportModel> objReportModel = new List<ReportModel>();
        //    objReportModel = (new ReportService().GetReportTypes()).ToList();
        //    return Json(objReportModel.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        //}
        #endregion

    }
}
