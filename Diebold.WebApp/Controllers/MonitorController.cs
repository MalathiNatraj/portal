using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Enums;
using Diebold.Services.Contracts;
using Diebold.Services.Exceptions;
using Diebold.WebApp.Models;
using Diebold.Domain.Entities;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.WebApp.Controllers
{
    public class MonitorController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;
        private readonly IDvrService _deviceService;
        private readonly IMonitoringService _MonitoringService;
        private readonly IDeviceMediaService _deviceMediaService;

        public MonitorController(IUserService userService, ICompanyService companyService, ISiteService siteService,
                                 IDvrService deviceService, ICurrentUserProvider currentUserProvider, IMonitoringService MonitoringService, IDeviceMediaService deviceMediaService)
        {
            this._userService = userService;
            this._currentUserProvider = currentUserProvider;
            this._userService = userService;
            this._companyService = companyService;
            this._siteService = siteService;
            this._deviceService = deviceService;
            this._MonitoringService = MonitoringService;
            this._deviceMediaService = deviceMediaService;
        }

        private void InitializeViewModel(MonitorViewModel monitorViewModel, IEnumerable<MonitorAssignmentViewModel> assignments)
        {
            FillItems();

            ViewBag.MonitorAssignmentViewModel = assignments ?? new List<MonitorAssignmentViewModel>();
            
            monitorViewModel.AvailableCompanyList = _companyService.GetAllEnabled().OrderBy(x =>x.Name).ToList();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            try
            {
                _siteService.DeleteDuplicateAlarmConfigurations();
                Session["UserMonitorGroupLevel"] = null;
                var monitorViewModel = new MonitorViewModel() { };

                this.InitializeViewModel(monitorViewModel, null);

                return View(monitorViewModel);
            }
            catch(Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Index(MonitorViewModel monitorViewModel, IEnumerable<MonitorAssignmentViewModel> assignments)
        public ActionResult Index(FormCollection frmCollection)
        {
            MonitorViewModel monitorViewModel = new MonitorViewModel();
            IEnumerable<MonitorAssignmentViewModel> assignments = new List<MonitorAssignmentViewModel>(); ;
            User user = null;
            try
            {
                int UserId = Convert.ToInt32(frmCollection.Get("Users").ToString());
                user = _userService.GetUser(UserId);

                assignments = (List<MonitorAssignmentViewModel>)Session["UserMonitorGroupLevel"];

                if (assignments != null)
                {
                    var newAssignments = assignments.Where(p => p.IsNew && p.Selected);

                    var newGroups = new List<UserMonitorGroup>(
                        from item in newAssignments
                        select item.MapFromViewModel());

                    var deletedAssignments = assignments.Where(p => !p.Selected && !p.IsNew);

                    var deletedGroups = new List<UserMonitorGroup>(
                        from item in deletedAssignments
                        select item.MapFromViewModel());

                    _userService.EditUserAndMonitoredGroupOfDevices(user, newGroups, deletedGroups);
                }

                ViewBag.MonitorSaved = true;
                assignments = new Collection<MonitorAssignmentViewModel>();
                Session["UserMonitorGroupLevel"] = null;
            }
            catch (ServiceException serviceException)
            {
                LogError("Service Exception occured while monitor index call", serviceException);
                ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", serviceException.Message));
            }
            catch (Exception e)
            {
                LogError("Exception occured while monitor index call", e);
                ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));
            }

            InitializeViewModel(monitorViewModel, assignments);

            if (user != null)
            {
                foreach (var model in assignments)
                {
                    model.FirstLevelName = user.Company.FirstLevelGrouping;
                    model.SecondLevelName = user.Company.SecondLevelGrouping;
                }
            }

            return View(monitorViewModel);
        }

        private void FillItems()
        {
            var user = _currentUserProvider.CurrentUser;

            ViewBag.AvailableUsers = new SelectList(Enumerable.Empty<User>(), "Id", "Name");
            ViewBag.AvailableFirstLevelGroups = new SelectList(Enumerable.Empty<CompanyGrouping1Level>(), "Id", "Name");
            ViewBag.AvailableSecondLevelGroups = new SelectList(Enumerable.Empty<CompanyGrouping2Level>(), "Id", "Name");
            ViewBag.AvailableSites = new SelectList(Enumerable.Empty<Site>(), "Id", "Name");
            ViewBag.AvailableDevices = new SelectList(Enumerable.Empty<Device>(), "Id", "Name");
            ViewBag.FirstLevelName = user.Company.FirstLevelGrouping;
            ViewBag.SecondLevelName = user.Company.SecondLevelGrouping;
        }

        private IList<MonitorAssignmentViewModel> GetMonitorAssigments(User user)
        {
            var monitoringDevices = _userService.GetMonitoredGroupOfDevices(user.Id);

            return monitoringDevices.Select(x => new MonitorAssignmentViewModel(x)
                                            {
                                                FirstLevelName = user.Company.FirstLevelGrouping, 
                                                SecondLevelName = user.Company.SecondLevelGrouping
                                            }).ToList();
        }

        public SelectList GetAvailableUsers(IList<User> userList)
        {
            var availableUsers = userList.Select(user => new SelectListItem
                {
                    Text = user.FirstName + " " + user.LastName,
                    Value = user.Id.ToString()
                }).ToList();

                return new SelectList(availableUsers, "Value", "Text");
        }

        public JsonResult GetAccountList()
        {
            try
            {
                IList<SiteViewModel> objlstSiteView = new List<SiteViewModel>();
                var siteList = _siteService.GetSitesByUser(_currentUserProvider.CurrentUser.Id).Where(x => x.DeletedKey == null).Distinct();
                if (siteList != null)
                {
                    // siteList.ToList().ForEach(x => objlstSiteView.Add(new SiteViewModel(x)));
                    siteList.ToList().ForEach(x => {
                        SiteViewModel objSiteView = new SiteViewModel();
                        objSiteView.Id = x.Id;
                        objSiteView.Name = x.Name;
                        objSiteView.AccountNumber = x.AccountNumber;
                        objSiteView.Address1 = x.Address1;
                        if (!string.IsNullOrEmpty(x.Address2))
                            objSiteView.Address2 = x.Address2;
                        else
                            objSiteView.Address2 = string.Empty;
                        objSiteView.City = x.City;
                        objSiteView.State = x.State;
                        objSiteView.Zip = x.Zip;
                        objlstSiteView.Add(objSiteView);
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
        public JsonResult GetMASMedia(int id, int serverId)
        {
            try
            {
                var model = _deviceMediaService.GetVideoMAS(id, string.Empty);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }
        public ActionResult AsyncMonitorAssigments(string id)
        {
            return PartialView("_SelectedGroupLevelForReport",(!string.IsNullOrEmpty(id)
                                                                ? GetMonitorAssigments(_userService.Get(int.Parse(id)))
                                                                : new List<MonitorAssignmentViewModel>()));
        }
        
        // To load company dropdown list
        public ActionResult AsyncCompany()
        {
            var monitorViewModel = new MonitorViewModel() { };
            monitorViewModel.AvailableCompanyList = _companyService.GetAllEnabled().OrderBy(x=>x.Name).ToList();
            var availableCompany = _companyService.GetAllEnabled().OrderBy(x => x.Name).ToList();
            //return Json(objLocationDetail.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
            return Json(availableCompany.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsyncUsers(int companies)
        {
            var users = _userService.GetUsersByCompany(companies, UserStatus.ActiveUsers);
            var select = new SelectList(users, "Id", "Name");
            if (select.Count() > 0)
            {
                return Json(users.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult AsyncFirstLevelGroup(int users)
        {            
            var user = _userService.Get(users);
            var firstLevelItems = _companyService.GetGrouping1LevelsByCompanyId(user.Company.Id);
            var select = new SelectList(firstLevelItems, "Id", "Name");
            return Json(firstLevelItems.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsyncSecondLevelGroup(int FirstLevelGroup, string sender)
        {
            var secondLevelItems = (FirstLevelGroup != -1) ? _companyService.GetGrouping2LevelsByGrouping1LevelId(FirstLevelGroup)
                                                        : _userService.GetMonitoringGrouping2LevelsByUser(_currentUserProvider.CurrentUser.Id, null);
            if (sender == "Report")
                secondLevelItems.Add(new CompanyGrouping2Level() { Id = -1, Name = "-- Next Level --" });
            var select = new SelectList(secondLevelItems, "Id", "Name");            
            return Json(secondLevelItems.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult AsyncSites(int SecondLevelGroup, string sender)
        {            
            var siteList = (SecondLevelGroup != -1) ? _siteService.GetSitesByCompanyGrouping2Level(SecondLevelGroup)
                                              : _userService.GetMonitoringSitesByUser(_currentUserProvider.CurrentUser.Id, null);
            if (sender == "Report")
                siteList.Add(new Site() { Id = -1, Name = "-- Next Level --" });

            var select = new SelectList(siteList, "Id", "Name");        
            return Json(siteList.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult AsyncDevices(int Site, string sender)
        {
            var deviceList = (Site != -1) ? _deviceService.GetDevicesBySiteId(Site)
                                               : _userService.GetMonitoringDevicesByUser(_currentUserProvider.CurrentUser.Id, null);

            var select = new SelectList(deviceList, "Id", "Name");            
            return Json(deviceList.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddGroupLevel(string GroupName, int Id, int UserId, string Text, string LevelName, bool selected, bool isNew)
        {
            IList<MonitorAssignmentViewModel> objResult;
            MonitorAssignmentViewModel objUserMonitorAssgn = null;
            if (Session["UserMonitorGroupLevel"] != null)
            {
                objResult = (List<MonitorAssignmentViewModel>)Session["UserMonitorGroupLevel"];
            }
            else
            {
                objResult = GetMonitorAssigments(_userService.Get(UserId));
            }

            Boolean isGroupAlreadyExist = isUserMonitorGroupAlreadyExist(objResult, GroupName, Id);

            if (isGroupAlreadyExist)
            {
                Session["UserMonitorGroupLevel"] = objResult;
                return Json(FilterResult(), JsonRequestBehavior.AllowGet);  
            }

            switch(GroupName)
            {
                case"Device":
                    objUserMonitorAssgn = new MonitorAssignmentViewModel();
                    objUserMonitorAssgn.DeviceId = Id;
                    objUserMonitorAssgn.DeviceName = Text;
                    objUserMonitorAssgn.Selected = selected;
                    objUserMonitorAssgn.IsNew = isNew;
                    break;
                case "Site":
                    objUserMonitorAssgn = new MonitorAssignmentViewModel();
                    objUserMonitorAssgn.SiteId = Id;
                    objUserMonitorAssgn.SiteName = Text;
                    objUserMonitorAssgn.Selected = selected;
                    objUserMonitorAssgn.IsNew = isNew;
                    break;
                case "SecondLevelGrouping":
                    objUserMonitorAssgn = new MonitorAssignmentViewModel();
                    objUserMonitorAssgn.SecondGroupLevelId = Id;
                    objUserMonitorAssgn.SecondGroupLevelName = Text;
                    objUserMonitorAssgn.SecondLevelName = LevelName;
                    objUserMonitorAssgn.Selected = selected;
                    objUserMonitorAssgn.IsNew = isNew;
                    break;
                case "FirstLevelGrouping":
                    objUserMonitorAssgn = new MonitorAssignmentViewModel();
                    objUserMonitorAssgn.FirstGroupLevelId = Id;
                    objUserMonitorAssgn.FirstGroupLevelName = Text;
                    objUserMonitorAssgn.FirstLevelName = LevelName;
                    objUserMonitorAssgn.Selected = selected;
                    objUserMonitorAssgn.IsNew = isNew;
                    break;
            }
            objResult.Add(objUserMonitorAssgn);
            Session["UserMonitorGroupLevel"] = objResult;
            return Json(FilterResult(), JsonRequestBehavior.AllowGet);                        
        }

        public Boolean isUserMonitorGroupAlreadyExist(IList<MonitorAssignmentViewModel> objResult, String type, int Id)
        {
            foreach (MonitorAssignmentViewModel result in objResult)
            {
                switch (type)
                {
                    case "Device":
                        if (Id == result.DeviceId)
                        {
                            return true;
                        }
                        break;
                    case "Site":
                        if (Id == result.SiteId)
                        {
                            return true;
                        }
                        break;
                    case "SecondLevelGrouping":
                        if (Id == result.SecondGroupLevelId)
                        {
                            return true;
                        }
                        break;
                    case "FirstLevelGrouping":
                        if (Id == result.FirstGroupLevelId)
                        {
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        public ActionResult RemoveSeletedItem(string SItem)
        {
            IList<MonitorAssignmentViewModel> objResult;            

            if (Session["UserMonitorGroupLevel"] != null)
            {
                objResult = (List<MonitorAssignmentViewModel>)Session["UserMonitorGroupLevel"];

                foreach (var monitorItems in objResult)
                {
                    if (monitorItems.DeviceName == SItem)
                    {
                        monitorItems.Selected = false;
                        break;
                    }
                    else if (monitorItems.SiteName == SItem)
                    {
                        monitorItems.Selected = false;
                        break;
                    }
                    else if (monitorItems.SecondGroupLevelName == SItem)
                    {
                        monitorItems.Selected = false;
                        break;
                    }
                    else if (monitorItems.FirstGroupLevelName == SItem)
                    {
                        monitorItems.Selected = false;
                        break;
                    }
                }
                Session["UserMonitorGroupLevel"] = objResult;
            } 
            return Json(FilterResult(), JsonRequestBehavior.AllowGet);               
        }


        public List<MonitorAssignmentViewModel> FilterResult()
        {
            List<MonitorAssignmentViewModel>  result = (List<MonitorAssignmentViewModel>)Session["UserMonitorGroupLevel"];
            if (result != null)
            {
                // To get the records which are not deleted
                result = result.Where(p => p.Selected).ToList();
            }
            return result;
        }


        public JsonResult Monitor_Read([DataSourceRequest] DataSourceRequest request) // , string Company, string Site, string Gateway
        {
            if (Session["UserMonitorGroupLevel"] != null)
            {
                return Json(FilterResult().ToDataSourceResult(request)); 
            }
            return null;
        }

        public void AsyncMonitors(int UserId)
        {
            IList<MonitorAssignmentViewModel> objResult;
            Session["UserMonitorGroupLevel"] = null;
            if (UserId != 0)
            {
                objResult = GetMonitorAssigments(_userService.Get(Convert.ToInt32(UserId)));
                Session["UserMonitorGroupLevel"] = objResult;
            }
        }

        public JsonResult GetGroupLevelByUser(int User)
        {
            IList<MonitorAssignmentViewModel> objResult;
            Session["UserMonitorGroupLevel"] = null;
            if (User != 0)
            {
                objResult = GetMonitorAssigments(_userService.Get(Convert.ToInt32(User)));
                Session["UserMonitorGroupLevel"] = objResult;
                return Json(objResult, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult PlaceonTest(string SelectedSite, string SeletedHours, string AccountNumber)
        {
            Site objSite = new Site();
            objSite = _siteService.Get(int.Parse(SelectedSite));
            string resultSet = string.Empty;
            try
            {
                resultSet = _MonitoringService.PlaceonTest(objSite, SeletedHours, AccountNumber);
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
                resultSet = "Request has exceeded the time limit.";
            }

            return Json(resultSet, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RunReport(string fromdate, string todate, string report, string AccountNumber)
        {
            var d = fromdate;
            List<ReportsDTO> ResultSet = _MonitoringService.RunReport(Convert.ToDateTime(fromdate), Convert.ToDateTime(todate), report, AccountNumber);
            List<MonitoringViewModel> lstMonitoringViewModel = new List<MonitoringViewModel>();
           // ResultSet.ForEach(x => lstMonitoringViewModel.Add(new MonitoringViewModel(x)));
            foreach (var item in ResultSet)
            {
                MonitoringViewModel objMonitoringViewModel = new MonitoringViewModel();
                objMonitoringViewModel.sig_acct = item.sig_acct;
                objMonitoringViewModel.sig_code = item.sig_code;
                string[] datetimeformat;
                datetimeformat = item.sig_date.Split('T');
                Convert.ToDateTime(datetimeformat[0]);
                Convert.ToDateTime(datetimeformat[1]);
                objMonitoringViewModel.sig_date = (Convert.ToDateTime(datetimeformat[0]).ToShortDateString() + " " + Convert.ToDateTime(datetimeformat[1]).ToString("HH:mm:ss"));
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
                lstMonitoringViewModel.Add(objMonitoringViewModel);
            }
            return Json(lstMonitoringViewModel, JsonRequestBehavior.AllowGet);
        }

    }    
}
