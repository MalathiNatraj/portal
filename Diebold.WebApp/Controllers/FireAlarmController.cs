using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.WebApp.Controllers
{
    public class FireAlarmController : BaseController
    {
        //
        // GET: /FireAlarm/
        private readonly ISiteService _siteService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IFireAlarmService _FireAlarmService;
        private readonly IEventDescriptionFiltersService _EventDescriptionService;
        private readonly IUserDefaultsService _userDefaultService;
        private readonly IUserService _objUserService;

        public FireAlarmController(ISiteService siteService, ICurrentUserProvider currentUserProvider, IFireAlarmService fireAlarmService, 
            IEventDescriptionFiltersService eventDescriptionService, IUserDefaultsService userDefaultService,
            IUserService userService)
        {
            _currentUserProvider = currentUserProvider;
            _siteService = siteService;
            _FireAlarmService = fireAlarmService;
            _EventDescriptionService = eventDescriptionService;
            _userDefaultService = userDefaultService;
            _objUserService = userService;
        }
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetSitesWithFireMonitoringAccNumber()
        {
             //int SiteId = 0;
           // IList<SiteViewModel> objlstSiteView = new List<SiteViewModel>();
            //var siteList = _siteService.GetSitewithFireMonitoringAccNumber(_currentUserProvider.CurrentUser.Id);
            //siteList.ToList().ForEach(x => objlstSiteView.Add(new SiteViewModel(x)));
            //// Get Value from Session and return 
            //if (Session["SelectedFASiteId"] != null)
            //{
            //    SiteId = (int)Session["SelectedFASiteId"];
            //    Site SiteDetail = _siteService.Get(SiteId);
            //    return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, AccNumber = c.AccountNumber, DefaultSelectedValue = SiteDetail.Name }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            //}
            //// GetDefault Selection Item
            //IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "FIREALARM");
            //if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
            //{
            //    Site SiteDetail = _siteService.Get(lstUserDefaults.First().FilterValue);
            //    Session["SelectedFASiteId"] = SiteDetail.Id;
            //    return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, AccNumber = c.AccountNumber, DefaultSelectedValue = SiteDetail.Name }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            //}
           
            
            //return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, AccNumber = c.AccountNumber }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            
            int SiteId = 0;
            IList<SiteAccNumberViewModel> objlstSiteView = new List<SiteAccNumberViewModel>();

            var siteList = _siteService.GetSitewithFireMonitoringAccNumber(_currentUserProvider.CurrentUser.Id);
            
            siteList.ToList().ForEach(x => objlstSiteView.Add(new SiteAccNumberViewModel(x)));

            // Get Value from Session and return 
            if (Session["SelectedFASiteId"] != null)
            {
                SiteId = (int)Session["SelectedFASiteId"];
                Site SiteDetail = _siteService.Get(SiteId);
                //return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, AccNumber = c.AccountNumber, DefaultSelectedValue = SiteDetail.Name }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
                return Json(objlstSiteView.Select(c => new { Id = SiteDetail.Id, Name = SiteDetail.Name, AccNumber = c.AccountNumber, DefaultSelectedValue = SiteDetail.Name }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            }
            // GetDefault Selection Item
            IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "FIREALARM");
            if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
            {
                Site SiteDetail = _siteService.Get(lstUserDefaults.First().FilterValue);
                Session["SelectedFASiteId"] = SiteDetail.Id;
                //return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, AccNumber = c.AccountNumber, DefaultSelectedValue = SiteDetail.Name }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
                return Json(objlstSiteView.Select(c => new { Id = SiteDetail.Id, Name = SiteDetail.Name, AccNumber = c.AccountNumber, DefaultSelectedValue = SiteDetail.Name }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            }

            
            //return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, AccNumber = c.AccountNumber }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            return Json(objlstSiteView.Select(c => new { Id = _siteService.GetSitesBySiteId(c.SiteId).Id, Name = _siteService.GetSitesBySiteId(c.SiteId).Name, AccNumber = c.AccountNumber }).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFireAlarmDetails(string SiteName, string AccountNumber)
        {
            // Store the Site Id in the session
            Site SiteDetail = _siteService.GetAll().Where(x => x.Name.Equals(SiteName)).First();
            Session["SelectedFASiteId"] = SiteDetail.Id;
            var ResultSet = _FireAlarmService.FireAlarmReport(AccountNumber);
            List<string> LstEventIds = new List<string>();
            ResultSet.ForEach(x => 
            {
                if(x.err_msg == null)
                {
                    if(x.sig_code != null)
                    {
                        LstEventIds.Add(x.sig_code.Trim());
                    }
                }
            });
            Dictionary<string, int> dctFireAlarm = GetFireAlarmCount(LstEventIds);
            return Json(dctFireAlarm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFireAlarmEventDetails(string SiteName, string AccountNumber)
        {
            // Store the Site Id in the session
            Site SiteDetail = _siteService.GetAll().Where(x => x.Name.Equals(SiteName)).First();
            Session["SelectedFASiteId"] = SiteDetail.Id;
            var ResultSet = _FireAlarmService.FireAlarmReport(AccountNumber);
            List<string> LstEventIds = new List<string>();
            ResultSet.ForEach(x =>
            {
                if (x.err_msg == null)
                {
                    if (x.sig_code != null)
                    {
                        LstEventIds.Add(x.sig_code.Trim());
                    }
                }
            });
            List<ReportsDTO> lstReportDetails = GetFireAlarmEvents(LstEventIds, ResultSet);
            return Json(lstReportDetails, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveFireAlarmDefaultValue(string SiteName, string InternalName, string ControlName)
        {
            try
            {
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName);
                Site SiteDetail = _siteService.GetAll().Where(x => x.Name.Equals(SiteName)).First();
                if (lstUserDefaults.Count() > 0)
                {
                    
                    lstUserDefaults.First().FilterValue = SiteDetail.Id;
                    _userDefaultService.Update(lstUserDefaults.First());
                }
                else
                {
                    UserDefaults objUserDefaults = new UserDefaults();
                    objUserDefaults.FilterName = ControlName;
                    objUserDefaults.FilterValue = SiteDetail.Id;
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

        public ActionResult ClearFireAlarmDefaultValue(string InternalName, string ControlName)
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

        # region "Get Values from Event Description Filter table"

        private Dictionary<string, int> GetFireAlarmCount(List<string> EventIds)
        {
            var EventDescrResultSet = _EventDescriptionService.GetEventDescriptionByEventIds(EventIds);
            Dictionary<string, int> dctFireAlarmCount = new Dictionary<string, int>();
            int intAlarmCount = 0;
            int intTroubleCount = 0;
            List<string> lstFireDetails = new List<string>();
            List<string> lstTroubleDetails = new List<string>();
            EventDescrResultSet.ToList().ForEach(x => 
            {
                if (!String.IsNullOrEmpty(x.Type))
                {
                    if (x.Type.Equals("FIRE"))
                    {
                        // intAlarmCount += 1;
                        lstFireDetails.Add(x.EventId.Trim());
                    }
                    else if (x.Type.Equals("FTBL") || x.Type.Equals("FSUP") || x.Type.Equals("FSUPV"))
                    {
                        // intTroubleCount += 1;
                        lstTroubleDetails.Add(x.EventId.Trim());
                    }
                }
            });

            foreach (string item in EventIds)
            {
                if (lstFireDetails.Contains(item))
                    intAlarmCount += 1;
                if (lstTroubleDetails.Contains(item))
                    intTroubleCount += 1;
            }
            dctFireAlarmCount.Add("ALARM", intAlarmCount);
            dctFireAlarmCount.Add("TRBL", intTroubleCount);
            return dctFireAlarmCount;
        }


        private List<ReportsDTO> GetFireAlarmEvents(List<string> EventIds, List<ReportsDTO> lstFireEventReports)
        {
            var EventDescrResultSet = _EventDescriptionService.GetEventDescriptionByEventIds(EventIds);
            List<string> lstFireDetails = new List<string>();
            List<string> lstTroubleDetails = new List<string>();
            List<ReportsDTO> lstEventsDetail = new List<ReportsDTO>();
            EventDescrResultSet.ToList().ForEach(x =>
            {
                if (x.Type.Equals("FIRE"))
                {
                    // intAlarmCount += 1;
                    lstFireDetails.Add(x.EventId.Trim());
                }
                else if (x.Type.Equals("FTBL") || x.Type.Equals("FSUP") || x.Type.Equals("FSUPV"))
                {
                    // intTroubleCount += 1;
                    lstTroubleDetails.Add(x.EventId.Trim());
                }
            });

            foreach (ReportsDTO item in lstFireEventReports)
            {
                if (item.sig_code != null)
                {
                    if (lstFireDetails.Contains(item.sig_code.Trim()))
                    {
                        lstEventsDetail.Add(item);
                    }
                    if (lstTroubleDetails.Contains(item.sig_code.Trim()))
                    {
                        lstEventsDetail.Add(item);
                    }
                }
                else
                {
                    lstEventsDetail = null;
                }
            }

            return lstEventsDetail;
        }
        #endregion
    }
}
