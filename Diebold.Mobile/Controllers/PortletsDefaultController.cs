using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Enums;
using Diebold.Services.Contracts;
using Diebold.Services.Exceptions;
using DieboldMobile.Models;
using Diebold.Domain.Entities;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Diebold.Platform.Proxies.DTO;

namespace DieboldMobile.Controllers
{
    public class PortletsDefaultController : BaseController
    {
        //
        // GET: /PortletsDefaultFns/

        private readonly ISiteService _siteService;
        private readonly IUserService _userService;
        private readonly IMonitoringService _monitoringService;
        private readonly ICurrentUserProvider _currentUserProvider;
        protected readonly IRoleActionDetailsService _roleActionDetailsService;
        private readonly ISiteLogoDetailsService _siteLogoDetailsService;

        public PortletsDefaultController(ISiteService siteService,
                                  ICurrentUserProvider currentUserProvider,
                                  IUserService userService,
                                  IMonitoringService MonitoringService,
                                  IRoleActionDetailsService roleActionDetailsService,
                                  ISiteLogoDetailsService siteLogoDetailsService)
        {
            this._siteService = siteService;
            this._monitoringService = MonitoringService;
            this._userService = userService;
            this._currentUserProvider = currentUserProvider;
            this._roleActionDetailsService = roleActionDetailsService;
            this._siteLogoDetailsService = siteLogoDetailsService;
        }

        public ActionResult Index()
        {
            return View();
        }

        // Place on Test method for MAS Portlet
        public ActionResult PlaceonTest(string SelectedSite, string SeletedHours, string AccountNumber)
        {
            Site objSite = new Site();
            objSite = _siteService.Get(int.Parse(SelectedSite));
            string resultSet = string.Empty;
            try
            {
                resultSet = _monitoringService.PlaceonTest(objSite, SeletedHours, AccountNumber);
                if (resultSet.ToUpper() == "OK")
                {
                    resultSet = "Test Successful.";
                }
                else if (resultSet.Contains("element does not equal its fixed") || resultSet.Contains("Bad Request"))
                {
                    resultSet = "Bad Request.";
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }

            return Json(resultSet, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PlaceonTestDDChange(string SelectedSite, string SelectedHour, string AccountNumber)
        {
            Site objSite = new Site();
            objSite = _siteService.Get(int.Parse(SelectedSite));
            string resultSet = string.Empty;
            try
            {
                resultSet = _monitoringService.PlaceonTestDDChange(objSite, SelectedHour, AccountNumber);
                if (resultSet.ToUpper() == "OK")
                {
                    resultSet = "Test Successful.";
                }
                else if (resultSet.Contains("element does not equal its fixed") || resultSet.Contains("Bad Request"))
                {
                    resultSet = "Bad Request.";
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }

            return Json(resultSet, JsonRequestBehavior.AllowGet);
        }
        
        // Method to get details of Account List in MAS Portlet. 
        public JsonResult GetAccountList()
        {
            try
            {
                IList<SiteViewModel> objlstSiteView = new List<SiteViewModel>();
                var siteList = _siteService.GetSitesByUserForMonitoring(_currentUserProvider.CurrentUser.Id).Where(x => x.DeletedKey == null).Distinct();
                if (siteList != null)
                {
                    siteList.ToList().ForEach(x =>
                    {
                        if (x.AccountNumber != null)
                        {
                            SiteViewModel objSiteView = new SiteViewModel();
                            var siteDetailsById = _siteService.GetSitesBySiteId(x.siteId);
                            objSiteView.Id = siteDetailsById.Id;
                            objSiteView.Name = siteDetailsById.Name;
                            objSiteView.AccountNumber = x.AccountNumber;
                            objSiteView.Address1 = siteDetailsById.Address1;
                            if (!string.IsNullOrEmpty(siteDetailsById.Address2))
                                objSiteView.Address2 = siteDetailsById.Address2;
                            else
                                objSiteView.Address2 = string.Empty;
                            objSiteView.City = siteDetailsById.City;
                            objSiteView.State = siteDetailsById.State;
                            objSiteView.Zip = siteDetailsById.Zip;
                            objlstSiteView.Add(objSiteView);                            
                        }
                    });
                }
                return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, AccountNumber = c.AccountNumber }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult GetSiteInformationDetails(string SiteId)
        {
            SiteViewModel objSiteViewModel = new SiteViewModel();
            var SiteResultSet = _siteService.Get(Convert.ToInt32(SiteId));
            if (SiteResultSet != null)
            {
                objSiteViewModel.AccountNumber = SiteResultSet.AccountNumber;
                objSiteViewModel.AccountNumber = SiteResultSet.AccountNumber;
                objSiteViewModel.Name = SiteResultSet.Name;
                objSiteViewModel.Address = SiteResultSet.Address1 + " " + SiteResultSet.Address2;
                objSiteViewModel.City = SiteResultSet.City;
                objSiteViewModel.State = SiteResultSet.State;
                objSiteViewModel.Zip = SiteResultSet.Zip;
            }
            return Json(objSiteViewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidateUserPin(string UserPin)
        {
            try
            {
                User objuser = _userService.Get(_currentUserProvider.CurrentUser.Id);
                bool IsUserPinValidated = false;
                if (objuser != null)
                {
                    if (UserPin.Equals(objuser.UserPin))
                    {
                        IsUserPinValidated = true;
                    }
                }
                return Json(IsUserPinValidated, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return Json(ex.Message);
            }

        }

        // Display Site Map 
        public ActionResult GetSiteImage(int siteId)
        {
            SiteLogoDetails objSiteLogo = _siteLogoDetailsService.GetSiteLogoDetailsbySiteId(siteId);
            if (objSiteLogo.SiteLogo != null)
            {
                string siteStream = "data:image/png;base64," + Convert.ToBase64String(objSiteLogo.SiteLogo);
                return Json(siteStream, JsonRequestBehavior.AllowGet);
            }
            return Json("");
        }

        private List<string> OpenCloseNormalFilters()
        {
            List<string> lstOpenCloseFilters = new List<string>();
            lstOpenCloseFilters.Add("Open(Passcode)".ToLower());
            lstOpenCloseFilters.Add("Opening".ToLower());
            lstOpenCloseFilters.Add("Open-Log Only".ToLower());
            lstOpenCloseFilters.Add("Verified Open".ToLower());
            lstOpenCloseFilters.Add("Scheduled Open".ToLower());
            lstOpenCloseFilters.Add("Scheduled Open/Aborts Alarm".ToLower());
            lstOpenCloseFilters.Add("LOG Only Open".ToLower());
            lstOpenCloseFilters.Add("Open - log only".ToLower());
            lstOpenCloseFilters.Add("Sched/Pass Vfy Open #1".ToLower());
            lstOpenCloseFilters.Add("LOG Passcard Open".ToLower());
            lstOpenCloseFilters.Add("Open - Log Passcode".ToLower());
            lstOpenCloseFilters.Add("Pass Lkup/Sched Open".ToLower());
            lstOpenCloseFilters.Add("Vrfy Passcard Open".ToLower());
            lstOpenCloseFilters.Add("Verified Open #2".ToLower());
            lstOpenCloseFilters.Add("Scheduled Open #2".ToLower());
            lstOpenCloseFilters.Add("Log Only Open #2".ToLower());
            lstOpenCloseFilters.Add("#2 Verify Passcard Open".ToLower());
            lstOpenCloseFilters.Add("#3 Sched/Pass Open".ToLower());
            lstOpenCloseFilters.Add("#3 Log Passcard Open".ToLower());
            lstOpenCloseFilters.Add("Normal Opening".ToLower());
            lstOpenCloseFilters.Add("#3 Verify Passcard Open".ToLower());
            lstOpenCloseFilters.Add("Close(Passcode)".ToLower());
            lstOpenCloseFilters.Add("Close".ToLower());
            lstOpenCloseFilters.Add("Close-Log Only".ToLower());
            lstOpenCloseFilters.Add("Scheduled Close".ToLower());
            lstOpenCloseFilters.Add("LOG Only Close".ToLower());
            lstOpenCloseFilters.Add("Close - log only".ToLower());
            lstOpenCloseFilters.Add("Sched/Pass Vfy Close #1".ToLower());
            lstOpenCloseFilters.Add("LOG Passcard Close".ToLower());
            lstOpenCloseFilters.Add("LATE TO Close".ToLower());
            lstOpenCloseFilters.Add("Close - Log Passcode".ToLower());
            lstOpenCloseFilters.Add("Pass Lkup/Sched Close".ToLower());
            lstOpenCloseFilters.Add("Vrfy Passcard Close".ToLower());
            lstOpenCloseFilters.Add("Verified Close #2".ToLower());
            lstOpenCloseFilters.Add("Scheduled Close #2".ToLower());
            lstOpenCloseFilters.Add("Log Only Close #2".ToLower());
            lstOpenCloseFilters.Add("Sched/Pass Close #2".ToLower());
            lstOpenCloseFilters.Add("Log Passcard Close #2".ToLower());
            lstOpenCloseFilters.Add("#2 Pass/V Sched Close".ToLower());
            lstOpenCloseFilters.Add("#3 Log Only Close".ToLower());
            lstOpenCloseFilters.Add("Normal Close".ToLower());
            lstOpenCloseFilters.Add("Open".ToLower());
            lstOpenCloseFilters.Add("Close".ToLower());
            return lstOpenCloseFilters;
        }

        private List<string> OpenCloseIrregularFilters()
        {
            List<string> lstOpenCloseIrrFilters = new List<string>();
            lstOpenCloseIrrFilters.Add("Fail to Open".ToLower());
            lstOpenCloseIrrFilters.Add("Late to Open".ToLower());
            lstOpenCloseIrrFilters.Add("Early Open".ToLower());
            lstOpenCloseIrrFilters.Add("Late Open".ToLower());
            lstOpenCloseIrrFilters.Add("Fail to Close".ToLower());
            lstOpenCloseIrrFilters.Add("Late to Close".ToLower());
            lstOpenCloseIrrFilters.Add("Early Close".ToLower());
            lstOpenCloseIrrFilters.Add("Close Is Late".ToLower());
            lstOpenCloseIrrFilters.Add("Partial Close".ToLower());
            lstOpenCloseIrrFilters.Add("Close Early".ToLower());
            lstOpenCloseIrrFilters.Add("Close Late".ToLower());
            return lstOpenCloseIrrFilters;
        }

        public ActionResult RunReport(string fromdate, string todate, string report, string AccountNumber)
        {
            try
            {
                List<ReportsDTO> ResultSet = _monitoringService.RunReport(Convert.ToDateTime(fromdate), Convert.ToDateTime(todate), report, AccountNumber);
                List<MonitoringViewModel> lstMonitoringViewModel = new List<MonitoringViewModel>();
                if (report.Equals("Zone List") == false)
                {
                    foreach (var item in ResultSet)
                    {
                        MonitoringViewModel objMonitoringViewModel = new MonitoringViewModel();
                        objMonitoringViewModel.sig_acct = item.sig_acct;
                        objMonitoringViewModel.sig_code = item.sig_code;
                        string[] datetimeformat = null;
                        string dateTime = null;
                        if (item.sig_date != null)
                        {
                            datetimeformat = item.sig_date.Split('T');
                            Convert.ToDateTime(datetimeformat[0]);
                            Convert.ToDateTime(datetimeformat[1]);
                            objMonitoringViewModel.sig_date = (Convert.ToDateTime(datetimeformat[0]).ToShortDateString() + " " + Convert.ToDateTime(datetimeformat[1]).ToString("HH:mm:ss"));
                            dateTime = (Convert.ToDateTime(datetimeformat[0]).ToShortDateString() + " " + Convert.ToDateTime(datetimeformat[1]).ToString("HH:mm:ss"));
                        }
                        objMonitoringViewModel.eventhistcomment = item.eventhistcomment;
                        objMonitoringViewModel.events = item.events;

                        if ((item.zone_comment != null && item.zone_comment != "") && (item.additional_info != null && item.additional_info != ""))
                            objMonitoringViewModel.zone_comment_additional_info = item.zone_comment + " , " + item.additional_info;
                        else if ((item.zone_comment != null || item.zone_comment != "") && (item.additional_info == null || item.additional_info == ""))
                            objMonitoringViewModel.zone_comment_additional_info = item.zone_comment;
                        else if ((item.additional_info != null || item.additional_info != "") && (item.zone_comment == null || item.zone_comment == ""))
                            objMonitoringViewModel.zone_comment_additional_info = item.additional_info;
                        else if ((item.additional_info == null || item.additional_info == "") && (item.zone_comment == null || item.zone_comment == ""))
                            objMonitoringViewModel.zone_comment_additional_info = string.Empty;
                        else
                            objMonitoringViewModel.zone_comment_additional_info = string.Empty;
                        if (datetimeformat != null)
                        {
                            if ((Convert.ToDateTime(dateTime) >= Convert.ToDateTime(fromdate)) && (Convert.ToDateTime(dateTime) <= Convert.ToDateTime(todate)))
                            {
                                if (report.Equals("Events"))
                                {
                                    if (item.events.ToLower().Equals("Burglary w/video".ToLower()) || item.events.ToLower().Equals("Burglary".ToLower()) ||
                                       item.events.ToLower().Equals("Holdup".ToLower()) || item.events.ToLower().Equals("Duress".ToLower()) ||
                                       item.events.ToLower().Equals("Holdup w/video".ToLower()) || item.events.ToLower().Equals("Fire".ToLower()) ||
                                       item.events.ToLower().Equals("Environ Auto Notify".ToLower()) || item.events.ToLower().Equals("Environ-Notify Evt w/video".ToLower()) ||
                                       item.events.ToLower().Equals("Environ Delay Notify(15)".ToLower()) ||
                                       item.events.ToLower().Equals("Fire w/video".ToLower()) || item.events.ToLower().Equals("Medical".ToLower()))
                                    {
                                        lstMonitoringViewModel.Add(objMonitoringViewModel);
                                    }
                                }
                                else if (report.Equals("Open / Close Normal"))
                                {
                                    List<string> lstFilterCondition = OpenCloseNormalFilters();
                                    if (lstFilterCondition.Any(X => X.Equals(item.events.ToLower())))
                                    {
                                        lstMonitoringViewModel.Add(objMonitoringViewModel);
                                    }
                                }
                                else if (report.Equals("Open / Close Irregular"))
                                {
                                    List<string> lstIrregularFilter = OpenCloseIrregularFilters();
                                    if (lstIrregularFilter.Any(y => y.Equals(item.events.ToLower())))
                                    {
                                        lstMonitoringViewModel.Add(objMonitoringViewModel);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in ResultSet)
                    {
                        MonitoringViewModel objMonitoringViewModel = new MonitoringViewModel();
                        objMonitoringViewModel.zone_id = item.zone_id;
                        if (!(item.zone_id.Trim().ToLower().StartsWith("c#") || item.zone_id.Trim().ToLower().StartsWith("o/c")))
                        {
                            objMonitoringViewModel.event_id = item.event_id;
                            if (item.event_id != null)
                            {
                                switch (item.event_id.Trim().ToLower())
                                {
                                    case "ba": // BA
                                        objMonitoringViewModel.Description = "Burglary";
                                        break;
                                    case "me": // ME
                                        objMonitoringViewModel.Description = "Medical";
                                        break;
                                    case "du": // DU
                                        objMonitoringViewModel.Description = "Duress";
                                        break;
                                    case "r": // R
                                        objMonitoringViewModel.Description = "Restored";
                                        break;
                                    case "rdx248": // RDX248
                                        objMonitoringViewModel.Description = "ALERT!- Alarm Panel Substitutd";
                                        break;
                                    case "rdx249": // RDX249
                                        objMonitoringViewModel.Description = "ALERT- Acnt disabled by attack";
                                        break;
                                    case "nep": // NEP
                                        objMonitoringViewModel.Description = "Notify Event Priority";
                                        break;
                                    default:
                                        objMonitoringViewModel.Description = item.event_id;
                                        break;
                                }
                            }
                            else
                            {
                                objMonitoringViewModel.Description = string.Empty;
                            }
                            if (string.IsNullOrEmpty(item.comment) == false)
                            {
                                objMonitoringViewModel.comment = item.comment;
                            }
                            else
                            {
                                objMonitoringViewModel.comment = string.Empty;
                            }
                            objMonitoringViewModel.restore_reqd_flag = item.restore_reqd_flag;

                            lstMonitoringViewModel.Add(objMonitoringViewModel);
                        }
                    }
                }
                return Json(lstMonitoringViewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }

        }

        public ActionResult GetTestDuration()
        {
            int CurrentUser = _currentUserProvider.CurrentUser.Role.Id;
            var RoleActionDetails = _roleActionDetailsService.GetAllRoleActionByroleId(CurrentUser);
            if (RoleActionDetails != null && RoleActionDetails.Count() <= 0)
            {
                return Json("No Action Available", JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<SelectListItem> lstSelectedListItem = new List<SelectListItem>();
                RoleActionDetails.ToList().ForEach(x =>
                {
                    lstSelectedListItem.Add(new SelectListItem { Text = x.ActionDetails.ActionValue, Value = x.ActionDetails.ActionKey.ToString() });
                });
                return Json(lstSelectedListItem, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
