using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Enums;
using Diebold.Services.Contracts;
using Diebold.Services.Exceptions;
using Diebold.Services.Impl;
using Diebold.WebApp.Models;
using Diebold.Domain.Entities;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Diebold.Platform.Proxies.DTO;
using System.Configuration;



namespace Diebold.WebApp.Controllers
{
    public class PortletsDefaultFnsController : BaseController
    {
        //
        // GET: /PortletsDefaultFns/

        private readonly ISiteService _siteService;
        private readonly IDvrService _deviceService;
        private readonly IUserService _userService;
        private readonly IMonitoringService _MonitoringService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IEventDescriptionFiltersService _EventDescriptionFilters;
        protected readonly ISiteDocumentService _siteDocumentService;
        protected readonly IRoleActionDetailsService _roleActionDetailsService;
        private readonly IDeviceMediaService _deviceMediaService;
        protected readonly IUserDefaultsService _userDefaultService;
        private readonly ISiteLogoDetailsService _siteLogoDetailsService;
        

        public PortletsDefaultFnsController(ISiteService siteService,
                                  ICurrentUserProvider currentUserProvider,
                                  IUserService userService,
                                  IMonitoringService MonitoringService, IEventDescriptionFiltersService EventDescriptionFilters,
                                  IDeviceMediaService deviceMediaService,
                                  IDvrService deviceService,
                                  ISiteDocumentService siteDocumentService,
                                  IRoleActionDetailsService roleActionDetailsService, IUserDefaultsService userDefaultService,
                                  ISiteLogoDetailsService siteLogoDetailsService)
        {
            this._siteService = siteService;
            this._MonitoringService = MonitoringService;
            this._userService = userService;
            this._currentUserProvider = currentUserProvider;
            this._EventDescriptionFilters = EventDescriptionFilters;
            this._deviceService = deviceService;
            this._deviceMediaService = deviceMediaService;
            this._siteDocumentService = siteDocumentService;
            this._roleActionDetailsService = roleActionDetailsService;
            _userDefaultService = userDefaultService;
            this._siteLogoDetailsService = siteLogoDetailsService;
        }

        public ActionResult Index()
        {
            return View();
        }
        // Creating a Global variable to hold the value
        // List<EventDescriptionFilters> lstEventDescriptionFilters = new List<EventDescriptionFilters>();

        // Place on Test method for MAS Portlet
        public ActionResult PlaceonTest(string SelectedSite, string SelectedHour, string AccountNumber)
        {
            Site objSite = new Site();
            objSite = _siteService.Get(int.Parse(SelectedSite));
            string resultSet = string.Empty;
            try
            {
                resultSet = _MonitoringService.PlaceonTest(objSite, SelectedHour, AccountNumber);
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
                resultSet = _MonitoringService.PlaceonTestDDChange(objSite, SelectedHour, AccountNumber);
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
        

        // Run Report method for MAS Run Report
        public ActionResult RunReport(string fromdate, string todate, string report, string accountNumber)
        {
            try
            {
                //if (lstEventDescriptionFilters == null || lstEventDescriptionFilters.Count() < 1)
                //{
                //    var EventDescriptionFiltersDetails = _EventDescriptionFilters.GetAll();
                //    lstEventDescriptionFilters = EventDescriptionFiltersDetails.ToList();
                //}
                List<ReportsDTO> ResultSet = _MonitoringService.RunReport(Convert.ToDateTime(fromdate), Convert.ToDateTime(todate), report, accountNumber);

                List<MonitoringViewModel> lstMonitoringViewModel = new List<MonitoringViewModel>();
                if (ResultSet != null && ResultSet.First().err_msg == null)
                {
                    if (report.Equals("Contact List") == true)
                    {
                        foreach (var ContactListItem in ResultSet)
                        {
                            MonitoringViewModel objMonitoringViewModel = new MonitoringViewModel();
                            if (ContactListItem != null)
                            {
                                // Display Contact name
                                if (string.IsNullOrEmpty(ContactListItem.first_name) == false && string.IsNullOrEmpty(ContactListItem.last_name) == false)
                                {
                                    objMonitoringViewModel.ContactName = ContactListItem.last_name + ", " + ContactListItem.first_name;
                                }
                                else if (string.IsNullOrEmpty(ContactListItem.first_name) == true && string.IsNullOrEmpty(ContactListItem.last_name) == false)
                                {
                                    objMonitoringViewModel.ContactName = ContactListItem.last_name;
                                }
                                else if (string.IsNullOrEmpty(ContactListItem.first_name) == false && string.IsNullOrEmpty(ContactListItem.last_name) == true)
                                {
                                    objMonitoringViewModel.ContactName = ContactListItem.first_name;
                                }

                                // CS Seq #
                                if (string.IsNullOrEmpty(ContactListItem.cs_seqno) == false)
                                {
                                    objMonitoringViewModel.CSSeqNumber = ContactListItem.cs_seqno;
                                }
                                else
                                {
                                    objMonitoringViewModel.CSSeqNumber = string.Empty;
                                }

                                // Pin
                                if (string.IsNullOrEmpty(ContactListItem.pin) == false)
                                {
                                    objMonitoringViewModel.Pin = ContactListItem.pin;
                                }
                                else
                                {
                                    objMonitoringViewModel.Pin = string.Empty;
                                }

                                // Contact Phone 1
                                if (string.IsNullOrEmpty(ContactListItem.phone1) == false)
                                {
                                    objMonitoringViewModel.Phone1 = ContactListItem.phone1;
                                }
                                else
                                {
                                    objMonitoringViewModel.Phone1 = string.Empty;
                                }

                                // Contact Phone 2
                                if (string.IsNullOrEmpty(ContactListItem.phone2) == false)
                                {
                                    objMonitoringViewModel.Phone2 = ContactListItem.phone2;
                                }
                                else
                                {
                                    objMonitoringViewModel.Phone2 = string.Empty;
                                }

                                // User Id
                                if (string.IsNullOrEmpty(ContactListItem.user_id) == false)
                                {
                                    objMonitoringViewModel.UserId = ContactListItem.user_id;
                                }
                                else
                                {
                                    objMonitoringViewModel.UserId = string.Empty;
                                }
                                lstMonitoringViewModel.Add(objMonitoringViewModel);
                            }
                        }
                    }
                    else if ((report.Equals("Open / Close Normal") == true || report.Equals("Open / Close Irregular") == true || report.Equals("Events") == true))
                    {
                        var AccessPermission = ResultSet.Where(x => x.AccessDenied != null && x.AccessDenied.Equals("Access to this account has been denied."));
                        if (AccessPermission != null && AccessPermission.Count() > 0)
                        {
                            return JsonError("Access to this account has been denied.");
                        }
                        else if ((ResultSet.Where(y => string.IsNullOrEmpty(y.err_msg) == false).Count() > 0))
                        {
                            var errorMsg = ResultSet.Where(x => x.err_msg != null);
                            return JsonError(errorMsg.First().err_msg);
                        }
                        else
                        {
                            ResultSet = ResultSet.Where(x => x.sig_acct != null && x.sig_acct.Equals(accountNumber)).ToList();
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
                                               item.events.ToLower().Equals("Intrusion APP".ToLower()) ||
                                               item.events.ToLower().Equals("Video Reference".ToLower()) ||
                                               item.events.ToLower().Equals("Fire w/video".ToLower()) || item.events.ToLower().Equals("Medical".ToLower()))
                                            {
                                                if (item.events.ToLower().Equals("Video Reference".ToLower()))
                                                {
                                                    string toParse = item.eventhistcomment;
                                                    string[] fields = (toParse.Split(':'))[1].Trim().Split(' ').First().Split('_');
                                                    string panelSerial = fields[0];
                                                    string cnxId = fields[1];
                                                    string serverId = fields[2];
                                                    
                                                    logger.Debug(String.Format("Got Video Reference Event: panelSerial {0}, cnxId {1}, serverId {2}", panelSerial, cnxId, serverId));

                                                    objMonitoringViewModel.extraData1 = cnxId;                                         
                                                }
                                                
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
                    }
                    else
                    {
                        List<string> lstEventIds = new List<string>();
                        ResultSet.ForEach(x => { lstEventIds.Add(x.event_id.Trim()); });
                        IList<EventDescriptionFilters> lstEventDescriptionFilters = _EventDescriptionFilters.GetEventDescriptionByEventIds(lstEventIds);
                        foreach (var item in ResultSet)
                        {
                            MonitoringViewModel objMonitoringViewModel = new MonitoringViewModel();
                            objMonitoringViewModel.zone_id = item.zone_id;
                            if (item.zone_id != null)
                            {
                                if (!(item.zone_id.Trim().ToLower().StartsWith("c#") || item.zone_id.Trim().ToLower().StartsWith("o/c")))
                                {
                                    objMonitoringViewModel.event_id = item.event_id;
                                    if (item.event_id != null)
                                    {
                                        var Descr = lstEventDescriptionFilters.Where(x => x.EventId.ToLower().Equals(item.event_id.ToLower()));
                                        if (Descr != null && Descr.Count() > 0)
                                        {
                                            objMonitoringViewModel.Description = Descr.First().Description;
                                        }
                                        else
                                        {
                                            objMonitoringViewModel.Description = string.Empty;
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

        public JsonResult GetAccountListforSearch()
        {
            try
            {
                IList<SiteViewModel> objlstSiteView = new List<SiteViewModel>();
                //var siteList = _siteService.GetSitesByUser(_currentUserProvider.CurrentUser.Id).Where(x => x.DeletedKey == null).Distinct();
                //if (siteList != null)
                //{
                //    siteList.ToList().ForEach(x =>
                //    {
                //        if (x.AccountNumber != null)
                //        {                 
                //            SiteViewModel objSiteView = new SiteViewModel();
                //            objSiteView.Id = x.Id;
                //            objSiteView.Name = x.Name;
                //            objSiteView.AccountNumber = x.AccountNumber;
                //            objSiteView.Address1 = x.Address1;
                //            if (!string.IsNullOrEmpty(x.Address2))
                //                objSiteView.Address2 = x.Address2;
                //            else
                //                objSiteView.Address2 = string.Empty;
                //            objSiteView.City = x.City;
                //            objSiteView.State = x.State;
                //            objSiteView.Zip = x.Zip;
                //            objlstSiteView.Add(objSiteView);
                //        }
                //    });
                //}     
                var siteList = _siteService.GetSitesByUserForMonitoring(_currentUserProvider.CurrentUser.Id);
                if (siteList != null)
                {
                    siteList.ToList().ForEach(x =>
                    {
                        if (x.AccountNumber != null)
                        {
                            SiteViewModel objSiteView = new SiteViewModel();
                            objSiteView.Id = _siteService.GetSitesBySiteId(x.siteId).Id;
                            objSiteView.Name = _siteService.GetSitesBySiteId(x.siteId).Name;
                            objSiteView.AccountNumber = x.AccountNumber;
                            objSiteView.Address1 = _siteService.GetSitesBySiteId(x.siteId).Address1;
                            if (!string.IsNullOrEmpty(_siteService.GetSitesBySiteId(x.siteId).Address2))
                                objSiteView.Address2 = _siteService.GetSitesBySiteId(x.siteId).Address2;
                            else
                                objSiteView.Address2 = string.Empty;
                            objSiteView.City = _siteService.GetSitesBySiteId(x.siteId).City;
                            objSiteView.State = _siteService.GetSitesBySiteId(x.siteId).State;
                            objSiteView.Zip = _siteService.GetSitesBySiteId(x.siteId).Zip;
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
        // Method to get details of Account List in MAS Portlet. 
        public JsonResult GetAccountList()
        {
             int MASAccountId = 0;
            try
            {
                IList<SiteViewModel> objlstSiteView = new List<SiteViewModel>();
               // var siteList = _siteService.GetSitesByUser(_currentUserProvider.CurrentUser.Id).Where(x => x.DeletedKey == null).Distinct();
                var siteList = _siteService.GetSitesByUserForMonitoring(_currentUserProvider.CurrentUser.Id);
                if (siteList != null)
                {
                    siteList.ToList().ForEach(x =>
                    {
                        if (x.AccountNumber != null)
                        {                            
                            SiteViewModel objSiteView = new SiteViewModel();
                            objSiteView.Id = _siteService.GetSitesBySiteId(x.siteId).Id;
                            objSiteView.Name = _siteService.GetSitesBySiteId(x.siteId).Name;
                            objSiteView.AccountNumber = x.AccountNumber;
                            objSiteView.Address1 = _siteService.GetSitesBySiteId(x.siteId).Address1;
                            if (!string.IsNullOrEmpty(_siteService.GetSitesBySiteId(x.siteId).Address2))
                                objSiteView.Address2 = _siteService.GetSitesBySiteId(x.siteId).Address2;
                            else
                                objSiteView.Address2 = string.Empty;
                            objSiteView.City = _siteService.GetSitesBySiteId(x.siteId).City;
                            objSiteView.State = _siteService.GetSitesBySiteId(x.siteId).State;
                            objSiteView.Zip = _siteService.GetSitesBySiteId(x.siteId).Zip;
                            objlstSiteView.Add(objSiteView);
                        }
                    });
                }
                // Check if value is present in session if so then display the selected value in cbo box
                if (Session["MASAccountNumber"] != null)
                {
                    MASAccountId = (int)Session["MASAccountNumber"];
                    return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, AccountNumber = c.AccountNumber, DefaultSelectedValue = MASAccountId }), JsonRequestBehavior.AllowGet);
                }
                return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, AccountNumber = c.AccountNumber }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
            
        }

        public ActionResult SiteInfo()
        {
            IList<Site> objlstSites = _siteService.GetSitesByUser(_currentUserProvider.CurrentUser.Id);
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            oSerializer.MaxJsonLength = Int32.MaxValue;
            string jsonResponse = oSerializer.Serialize(prepareSiteMapData(objlstSites));
            objlstSites = null;
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWeaterAlerts(string State, string City, string Latitude, string Longitude, string Location, string Address,
                                            string LocationContact, string ContactEmail, string ContactPhone, string DVRDevicesCount,
                                            string AccessDevicesCount, string IntrusionDevicesCount, string Id)
        {
            if (!string.IsNullOrEmpty(State))
            {
                State = State.Trim();
                State = System.Web.HttpUtility.UrlPathEncode(State);
            }
            if (!string.IsNullOrEmpty(City))
            {
                City = City.Trim();
                City = System.Web.HttpUtility.UrlPathEncode(City);
            }
            if (!string.IsNullOrEmpty(Latitude))
            {
                Session["SiteLatitude"] = Latitude;
            }
            if (!string.IsNullOrEmpty(Longitude))
            {
                Session["SiteLongitude"] = Longitude;
            }
            if (!string.IsNullOrEmpty(Location))
            {
                Session["SiteLocation"] = Location;
            }
            if (!string.IsNullOrEmpty(Address))
            {
                Session["SiteAddress"] = Address;
            }
            if (!string.IsNullOrEmpty(LocationContact))
            {
                Session["SiteLocationContact"] = LocationContact;
            }
            if (!string.IsNullOrEmpty(ContactEmail))
            {
                Session["SiteContactEmail"] = ContactEmail;
            }
            if (!string.IsNullOrEmpty(ContactPhone))
            {
                Session["SiteContactPhone"] = ContactPhone;
            }
            if (!string.IsNullOrEmpty(DVRDevicesCount))
            {
                Session["SiteDVRDevicesCount"] = DVRDevicesCount;
            }
            if (!string.IsNullOrEmpty(AccessDevicesCount))
            {
                Session["SiteAccessDevicesCount"] = AccessDevicesCount;
            }
            if (!string.IsNullOrEmpty(IntrusionDevicesCount))
            {
                Session["SiteIntrusionDevicesCount"] = IntrusionDevicesCount;
            }
            if (!string.IsNullOrEmpty(Id))
            {
                Session["SiteId"] = Id;
            }
            var Result = _siteService.GetWeatherAlerts(State, City);
             // Mapping to View Model
            List<WeatherAlertViewModel> lstWeatherAlertViewModel = new List<WeatherAlertViewModel>();
            if (Result != null)
            {
                foreach (var item in Result.alerts)
                {
                    WeatherAlertViewModel objWeatherAlertViewModel = new WeatherAlertViewModel();
                    objWeatherAlertViewModel.Date = item.date;
                    objWeatherAlertViewModel.DateEpoch = item.date_epoch;
                    objWeatherAlertViewModel.Description = item.description;
                    objWeatherAlertViewModel.Expires = item.expires;
                    objWeatherAlertViewModel.ExpiresEpoch = item.expires_epoch;
                    objWeatherAlertViewModel.Message = item.message;
                    objWeatherAlertViewModel.Phenomena = item.phenomena;
                    objWeatherAlertViewModel.Type = item.type;
                    lstWeatherAlertViewModel.Add(objWeatherAlertViewModel);
                }
            }
            else
            {
                return null;
            }
            return Json(lstWeatherAlertViewModel, JsonRequestBehavior.AllowGet);
        }

        private List<SiteMapModel> prepareSiteMapData(IList<Site> objlstSites)
        {
            List<SiteMapModel> objlstSiteMap = new List<SiteMapModel>();
            SiteMapModel objSiteMapModel = null;
            IList<Site> updatedSite = new List<Site>();
            string strCommaWithSpace = ", ";
            IList<int> siteIdList = new List<int>();
            IDictionary<int, IList<Dvr>> siteDeviceMap = new Dictionary<int, IList<Dvr>>();
            foreach (Site obSite in objlstSites)
            {
                siteIdList.Add(obSite.Id);
                IList<Dvr> devicelist = new List<Dvr>();
                siteDeviceMap.Add(obSite.Id, devicelist);
            }
            IList<Dvr> devices = _deviceService.GetAllDevicesBySiteList(siteIdList);
            foreach (Dvr device in devices)
            {
                siteDeviceMap[device.Site.Id].Add(device);
            }
            foreach (Site objSite in objlstSites)
            {
                //if (objSite.Latitude == null)
                //{
                //    _siteService.getGeoCoordinates(objSite);
                //    updatedSite.Add(objSite);
                //}
                objSiteMapModel = new SiteMapModel();
                objSiteMapModel.Id = objSite.Id;
                objSiteMapModel.Location = objSite.Name;
                objSiteMapModel.Address = objSite.Address1 + strCommaWithSpace + objSite.City + strCommaWithSpace + objSite.State + strCommaWithSpace + objSite.Country + strCommaWithSpace + objSite.Zip;
                if (objSite.CompanyGrouping2Level != null && objSite.CompanyGrouping2Level.CompanyGrouping1Level != null)
                {
                    //objSiteMapModel.LocationContact = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactName;
                    //objSiteMapModel.ContactEmail = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactEmail;
                    //objSiteMapModel.ContactPhone = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactOffice;
                    objSiteMapModel.LocationContact = objSite.ContactName;
                    objSiteMapModel.ContactEmail = objSite.ContactEmail;
                    objSiteMapModel.ContactPhone = objSite.ContactNumber;
                    IList<Dvr> objlstDevice = siteDeviceMap[objSite.Id];
                    IDictionary<String, int> parentTypeDeviceCountMap = _userService.GetDevicesCount(objlstDevice);
                    objSiteMapModel.DVRDevicesCount = parentTypeDeviceCountMap["DVR"];
                    objSiteMapModel.AccessDevicesCount = parentTypeDeviceCountMap["ACCESS"];
                    objSiteMapModel.IntrusionDevicesCount = parentTypeDeviceCountMap["INTRUSION"];
                    objSiteMapModel.TotalDevicesCount = (objSiteMapModel.DVRDevicesCount + objSiteMapModel.AccessDevicesCount + objSiteMapModel.IntrusionDevicesCount);
                }
                objSiteMapModel.Latitude = objSite.Latitude;
                objSiteMapModel.Longitude = objSite.Longitude;
                if (Session["SiteLatitude"] != null)
                {
                    objSiteMapModel.SessionLatitude = (string)Session["SiteLatitude"];
                }
                if (Session["SiteLongitude"] != null)
                {
                    objSiteMapModel.SessionLongitude = (string)Session["SiteLongitude"];
                }

                if (Session["SiteLocation"] != null)
                {
                    objSiteMapModel.SessionLocation = (string)Session["SiteLocation"];
                }
                if (Session["SiteAddress"] != null)
                {
                    objSiteMapModel.SessionAddress = (string)Session["SiteAddress"];
                }
                if (Session["SiteLocationContact"] != null)
                {
                    objSiteMapModel.SessionLocationContact = (string)Session["SiteLocationContact"];
                }
                if (Session["SiteContactEmail"] != null)
                {
                    objSiteMapModel.SessionContactEmail = (string)Session["SiteContactEmail"];
                }
                if (Session["SiteContactPhone"] != null)
                {
                    objSiteMapModel.SessionContactPhone = (string)Session["SiteContactPhone"];
                }
                if (Session["SiteDVRDevicesCount"] != null)
                {
                    objSiteMapModel.SessionDVRCount = (string)Session["SiteDVRDevicesCount"];
                }
                if (Session["SiteAccessDevicesCount"] != null)
                {
                    objSiteMapModel.SessionAccessCount = (string)Session["SiteAccessDevicesCount"];
                }
                if (Session["SiteIntrusionDevicesCount"] != null)
                {
                    objSiteMapModel.SessionIntrusionCount = (string)Session["SiteIntrusionDevicesCount"];
                }
                if (Session["SiteId"] != null)
                {
                    objSiteMapModel.SessionId = (string)Session["SiteId"];
                }
                objlstSiteMap.Add(objSiteMapModel);
            }
            if (updatedSite.Count > 0)
            {
                _siteService.Update(objlstSites);
            }
            //Setting default slection for sitmap inorder to display information on map
            if (objlstSiteMap.Count > 0)
            {
                // GetDefault Selection Item
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "SITEMAP");
                if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
                {
                    objlstSiteMap[objlstSiteMap.Count - 1].DefaultSiteLocation = lstUserDefaults.First().FilterValue;
                }
                else
                {
                    objlstSiteMap[objlstSiteMap.Count - 1].DefaultSiteLocation = objlstSiteMap[objlstSiteMap.Count - 1].Id;
                }
            }
            return objlstSiteMap;
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

        public JsonResult SaveAccountinSession(int AccountNumber)
        {
            if (AccountNumber > 0)
            {
                Session["MASAccountNumber"] = AccountNumber;
            }
            return null;
        }

        public JsonResult SaveReportinSession(string ReportName)
        {
            if (!string.IsNullOrEmpty(ReportName))
            {
                Session["MASReportName"] = ReportName;
            }
            return null;
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

        public ActionResult GetSiteMapDocumentsbySiteId(string SiteId)
        {
            List<SiteDocumentViewModel> lstSiteDocumentViewModel = GetSiteNoteDocumentDetails(SiteId);
            return Json(lstSiteDocumentViewModel.ToList(), JsonRequestBehavior.AllowGet);
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
        public ActionResult ViewUserProfile()
        {
            UserViewModel objUserViewModel = new UserViewModel();
            objUserViewModel.Id = _currentUserProvider.CurrentUser.Id;
            var itemToEdit = _userService.Get(objUserViewModel.Id);
            objUserViewModel.FirstName = itemToEdit.FirstName;
            objUserViewModel.LastName = itemToEdit.LastName;
            objUserViewModel.Title = itemToEdit.Title;
            objUserViewModel.Username = itemToEdit.Username;
            objUserViewModel.Email = itemToEdit.Email;
            objUserViewModel.Phone = itemToEdit.Phone;
            objUserViewModel.Mobile = itemToEdit.Mobile;
            objUserViewModel.CompanyName = itemToEdit.Company.Name;
            objUserViewModel.RoleName = itemToEdit.Role.Name;
            objUserViewModel.TimeZone = itemToEdit.TimeZone;
            objUserViewModel.Text1 = itemToEdit.Text1;
            objUserViewModel.Text2 = itemToEdit.Text2;
            return View("ViewUserProfile", objUserViewModel);                       
        }

        public ActionResult ViewSiteMapDocumentFileContent(int Id)
        {
            var SiteNoteDocument = _siteDocumentService.Get(Convert.ToInt32(Id));
            string DocumentFileServerVirtualPath = ConfigurationManager.AppSettings["DocumentFileServerVirtualPath"];
            // string DocumentFileServerVirtualPath = SiteNoteDocument.FileName.Substring(0, SiteNoteDocument.FileName.LastIndexOf('.') - 17) + ".pdf";
            SiteNoteDocument.FileName = SiteNoteDocument.FileName.Replace(@"\", @"/");
            string LoadFrom = DocumentFileServerVirtualPath.Replace("{0}", SiteNoteDocument.FileName.Substring(0, SiteNoteDocument.FileName.LastIndexOf('.')) + ".pdf");
            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            var cd = new System.Net.Mime.ContentDisposition
            {
                // FileName = "Site Document.pdf",
                FileName = SiteNoteDocument.FileName.Substring(SiteNoteDocument.FileName.LastIndexOf('/') + 1, SiteNoteDocument.FileName.Length - SiteNoteDocument.FileName.LastIndexOf('/') - 21) + ".pdf",
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            //Response.AppendHeader("Content-Type", "application/pdf");
            Response.Flush();
            byte[] response = new System.Net.WebClient().DownloadData(LoadFrom);
            Response.OutputStream.Write(response, 0, response.Length);
            Response.Flush();
            Response.Close();
            return Json(LoadFrom, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetTestDurationDetails()
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
                RoleActionDetails.ToList().ForEach(x => {
                    lstSelectedListItem.Add(new SelectListItem { Text = x.ActionDetails.ActionValue , Value = x.ActionDetails.ActionKey.ToString() });
                });
                return Json(lstSelectedListItem, JsonRequestBehavior.AllowGet);
            }
        }
        private List<SiteDocumentViewModel> GetSiteNoteDocumentDetails(string siteId)
        {
            Dictionary<string, bool> dctRoleDetails = GetRoleDetails();
            IList<SiteDocumentViewModel> lstSiteDocumentViewModel = new List<SiteDocumentViewModel>();
            IList<SiteDocument> lstSiteDocuments = _siteDocumentService.GetSiteDocumentbySiteId(Convert.ToInt32(siteId)).OrderByDescending(x => x.Date).ToList();
            if (lstSiteDocuments != null && lstSiteDocuments.Count() > 0)
            {
                lstSiteDocuments.ToList().ForEach(x => lstSiteDocumentViewModel.Add(new SiteDocumentViewModel
                {
                    DisplayDate = x.Date.ToString("MM/dd HH:mm tt"),
                    // FileName = x.FileName.Substring(0, 15) + "...",
                    FileName = x.FileName.Substring(x.FileName.LastIndexOf('\\') + 1, x.FileName.Length - (x.FileName.LastIndexOf('\\') + 22)) + ".pdf",
                    FileURL = x.FileName.Substring(0, x.FileName.LastIndexOf('.') - 17).Replace(@"\", @"\\"),
                    UserName = x.User.Username,
                    Id = x.Id,
                    IsPrimary = x.IsPrimary,
                    isDocumentsViewable = dctRoleDetails["DocumentsViewable"],
                    isDocumentsEditable = dctRoleDetails["DocumentsEditable"],
                    isDocumentsDeleteable = dctRoleDetails["DocumentsDeletable"]
                }));
            }
            else
            {
                SiteDocumentViewModel objSiteDocumentViewModel = new SiteDocumentViewModel();
                objSiteDocumentViewModel.Id = 0;
                objSiteDocumentViewModel.isDocumentsViewable = dctRoleDetails["DocumentsViewable"];
                objSiteDocumentViewModel.isDocumentsEditable = dctRoleDetails["DocumentsEditable"];
                objSiteDocumentViewModel.isDocumentsDeleteable = dctRoleDetails["DocumentsDeletable"];
                lstSiteDocumentViewModel.Add(objSiteDocumentViewModel);
            }
            return lstSiteDocumentViewModel.ToList();
        }

        private Dictionary<string, bool> GetRoleDetails()
        {
            bool isDocumentsViewable;
            bool isDocumentsEditable;
            bool isDocumentsDeletable;
            var RoleDetails = _currentUserProvider.CurrentUser.Role;
            
            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.ViewDocuments))
                isDocumentsViewable = true;
            else
                isDocumentsViewable = false;

            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.EditDocuments))
                isDocumentsEditable = true;
            else
                isDocumentsEditable = false;

            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.DeleteDocuments))
                isDocumentsDeletable = true;
            else
                isDocumentsDeletable = false;

            Dictionary<string, bool> dcRoleDetails = new Dictionary<string, bool>();
            dcRoleDetails.Add("DocumentsViewable", isDocumentsViewable);
            dcRoleDetails.Add("DocumentsEditable", isDocumentsEditable);
            dcRoleDetails.Add("DocumentsDeletable", isDocumentsDeletable);
            return dcRoleDetails;
        }
       // public ActionResult SaveIntrusionDefaultValue(int DeviceId, string InternalName, string ControlName)
        public ActionResult SaveSiteMapDefaultValue(int SiteId, string InternalName, string ControlName)
        {
            try
            {
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName);
                if (lstUserDefaults.Count() > 0)
                {
                    lstUserDefaults.First().FilterValue = SiteId;
                    _userDefaultService.Update(lstUserDefaults.First());
                }
                else
                {
                    UserDefaults objUserDefaults = new UserDefaults();
                    objUserDefaults.FilterName = ControlName;
                    objUserDefaults.FilterValue = SiteId;
                    objUserDefaults.InternalName = InternalName;
                    objUserDefaults.User = _userService.Get(_currentUserProvider.CurrentUser.Id);
                    _userDefaultService.Create(objUserDefaults);
                }
                return Json("RecordModified", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
        //public ActionResult ClearIntrusionDefaultValue(string InternalName, string ControlName)
        public ActionResult ClearSiteMapDefaultValue(string InternalName, string ControlName)
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
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }
    }

   
}
