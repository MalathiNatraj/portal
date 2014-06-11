using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Enums;
using Diebold.Services.Contracts;
using Diebold.Services.Exceptions;
using DieboldMobile.Models;
using Diebold.Domain.Entities;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace DieboldMobile.Controllers
{
    public class MonitorController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;
        private readonly IDvrService _deviceService;
        private readonly IMonitoringService _monitoringService;

        public MonitorController(IUserService userService, ICompanyService companyService, ISiteService siteService,
                                 IDvrService deviceService, ICurrentUserProvider currentUserProvider, IMonitoringService monitoringService)
        {
            this._userService = userService;
            this._currentUserProvider = currentUserProvider;
            this._userService = userService;
            this._companyService = companyService;
            this._siteService = siteService;
            this._deviceService = deviceService;
            this._monitoringService = monitoringService;
        }

        private void InitializeViewModel(MonitorViewModel monitorViewModel, IEnumerable<MonitorAssignmentViewModel> assignments)
        {
            FillItems();

            ViewBag.MonitorAssignmentViewModel = assignments ?? new List<MonitorAssignmentViewModel>();

            monitorViewModel.AvailableCompanyList = _companyService.GetAllEnabled().OrderBy(x => x.Name).ToList();
        }

        public ActionResult MAS()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            Session["UserMonitorGroupLevel"] = null;
            var monitorViewModel = new MonitorViewModel() { };

            this.InitializeViewModel(monitorViewModel, null);

            return View(monitorViewModel);
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
                ModelState.AddModelError("", string.Format("Unexpected Error: [{0}]", serviceException.Message));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", string.Format("Unexpected Error: [{0}]", e.Message));
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
            IList<SiteViewModel> objlstSiteView = new List<SiteViewModel>();
            var siteList = _siteService.GetSitesByUser(_currentUserProvider.CurrentUser.Id).Where(x => x.DeletedKey == null && x.AccountNumber != null).Distinct();
            if (siteList != null)
            {
                siteList.ToList().ForEach(x => objlstSiteView.Add(new SiteViewModel(x)));
            }
            return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip, AccountNumber = c.AccountNumber }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsyncMonitorAssigments(string id)
        {
            return PartialView("_SelectedGroupLevelForReport", (!string.IsNullOrEmpty(id)
                                                                ? GetMonitorAssigments(_userService.Get(int.Parse(id)))
                                                                : new List<MonitorAssignmentViewModel>()));
        }

        // To load company dropdown list
        public ActionResult AsyncCompany()
        {
            var monitorViewModel = new MonitorViewModel() { };
            monitorViewModel.AvailableCompanyList = _companyService.GetAllEnabled().OrderBy(x => x.Name).ToList();
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

            switch (GroupName)
            {
                case "Device":
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
            List<MonitorAssignmentViewModel> result = (List<MonitorAssignmentViewModel>)Session["UserMonitorGroupLevel"];
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

        public ActionResult GetSiteInformationDetails(string SiteId)
        {
            SiteViewModel objSiteViewModel = new SiteViewModel();
            var SiteResultSet = _siteService.Get(Convert.ToInt32(SiteId));
            if (SiteResultSet != null)
            {
                objSiteViewModel.AccountNumber =  SiteResultSet.AccountNumber;
                objSiteViewModel.Name = SiteResultSet.Name;
                objSiteViewModel.Address = SiteResultSet.Address1 + " " + SiteResultSet.Address2;
                objSiteViewModel.City = SiteResultSet.City;
                objSiteViewModel.State = SiteResultSet.State;
                objSiteViewModel.Zip = SiteResultSet.Zip;
            }
            return Json(objSiteViewModel, JsonRequestBehavior.AllowGet);
        }

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
                    resultSet = "Place on Test Started.";
                }
                else if (resultSet.Contains("element does not equal its fixed"))
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

    }    
}
