using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Diebold.Services.Contracts;
using Diebold.Services.Exceptions;
using DieboldMobile.Models;
using Diebold.Domain.Entities;
using AttributeRouting;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.Domain.Exceptions;
using Diebold.Domain.Contracts.Infrastructure;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace DieboldMobile.Controllers
{
    public class UserController : BaseCRUDTrackeableController<User, UserViewModel>
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ICompanyService _companyService;
        private readonly IDvrService _deviceService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IPortletService _portletService;

        public UserController(IUserService userService,
            IRoleService roleService, ICompanyService companyService,
            IDvrService deviceService, ICurrentUserProvider currentUserProvider, IPortletService portletService)
            : base(userService)
        {
            this._userService = userService;
            this._roleService = roleService;
            this._companyService = companyService;
            this._deviceService = deviceService;
            this._currentUserProvider = currentUserProvider;
            this._portletService = portletService;
        }

        protected override UserViewModel InitializeViewModel(UserViewModel item)
        {
            item.AvailableCompanyList = this._companyService.GetAll();
            item.AvailableTimeZoneList = this._deviceService.GetAllTimeZones();
            item.AvailableRoleList = _roleService.GetAll();

            return item;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public override ActionResult Index(JqGridRequest request)
        {
            var searchCriteria = request.ExtraParams["searchCriteria"];

            var page = _userService.GetPage(request.PageIndex + 1, request.RecordsCount,
                                               request.SortingName, request.SortingOrder == JqGridSortingOrders.Asc,
                                               searchCriteria);

            var response = GetJqGridResponse(page, page.Items.Select(MapEntity));
            return new JqGridJsonResult { Data = response };
        }

        public override ActionResult Create()
        {
         // Random rauserpin = new Random();
            var model = new UserViewModel
            {
                AvailableRoleList = _roleService.GetAll(),
                AvailableCompanyList = _companyService.GetAll(),
                AvailableTimeZoneList = _deviceService.GetAllTimeZones(),
                PreferredContact = PreferredContact.Mobile.ToString()
            };
           // model.UserPin = rauserpin.Next(9999).ToString();
            return View(model);
        }

        public ActionResult UserInfo()
        {
            var model = new UserViewModel
            {
                AvailableRoleList = _roleService.GetAll(),
                AvailableCompanyList = _companyService.GetAll(),
                AvailableTimeZoneList = _deviceService.GetAllTimeZones(),
                PreferredContact = PreferredContact.Mobile.ToString()
            };
            return PartialView("_UserInfo", model);
        }

        public ActionResult EditUser(int id)
        {
            var itemToEdit = MapEntity(_userService.Get(id));
            itemToEdit.AvailableRoleList = this._roleService.GetAll();
            itemToEdit.AvailableCompanyList = this._companyService.GetAll();
            itemToEdit.AvailableTimeZoneList = _deviceService.GetAllTimeZones();
            itemToEdit.OldCompanyId = itemToEdit.CompanyId;


            return View(itemToEdit);
        }

        protected override UserViewModel MapEntity(User item)
        {
            return new UserViewModel(item);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateUser(UserViewModel newItem)
        {
            UserViewModel newItem2 = new UserViewModel();
            string ErrorMsg = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    IList<string> lststrLinks = new List<string>();
                    string strLinks = System.Web.Configuration.WebConfigurationManager.AppSettings["Links"];
                    lststrLinks = strLinks.Split('~');

                    var item = newItem.MapFromViewModel();

                    // Insert Default Links to User
                    IList<Link> lstLink = new List<Link>();
                    foreach (var strLks in lststrLinks)
                    {
                        Link objLink = new Link();
                        if (strLks.Contains("linkedin"))
                        {
                            objLink.Name = "Diebold LinkedIn";
                        }
                        else if (strLks.Contains("diebold"))
                        {
                            objLink.Name = "Diebold";
                        }

                        else if (strLks.Contains("asisonline"))
                        {
                            objLink.Name = "ASIS";
                        }
                        else if (strLks.Contains("siaonline"))
                        {
                            objLink.Name = "SIA";
                        }
                        objLink.Url = strLks;
                        objLink.IsDisabled = false;
                        objLink.User = item;
                        lstLink.Add(objLink);

                    }
                    item.Links = lstLink;
                    // Insert Default RSS Feeds for User

                    IList<string> lststrRSSFeed = new List<string>();
                    string strRSSFeeds = System.Web.Configuration.WebConfigurationManager.AppSettings["RSSFeed"];
                    lststrRSSFeed = strRSSFeeds.Split('~');

                    IList<RSSFeed> lstRSSFeed = new List<RSSFeed>();
                    foreach (var strrss in lststrRSSFeed)
                    {
                        RSSFeed objRssFeed = new RSSFeed();
                        if (strrss.Contains("fbi"))
                        {
                            objRssFeed.Name = "FBI News";
                        }
                        else if (strrss.Contains("securitysystemsnews"))
                        {
                            objRssFeed.Name = "Security Systems News";
                        }

                        else if (strrss.Contains("hstoday"))
                        {
                            objRssFeed.Name = "Homeland Security News";
                        }
                        else if (strrss.Contains("securitymanagement"))
                        {
                            objRssFeed.Name = "Security Management";
                        }
                        objRssFeed.Url = strrss;
                        objRssFeed.IsDisabled = false;
                        objRssFeed.User = item;
                        lstRSSFeed.Add(objRssFeed);

                    }
                    item.RSSFeeds = lstRSSFeed;
                    IList<UserPortletsPreferences> objlstUserPortletsPreferences = new List<UserPortletsPreferences>();
                    Role objRole = _roleService.Get(item.Role.Id);

                    foreach (var roleportlet in objRole.RolePortlets)
                    {

                        UserPortletsPreferences objUserPortletsPreferences = new UserPortletsPreferences();
                        objUserPortletsPreferences.User = item;
                        objUserPortletsPreferences.PortletId = roleportlet.Portlets.Id;
                        objUserPortletsPreferences.Portlets = roleportlet.Portlets;
                        objUserPortletsPreferences.ColumnNo = roleportlet.Portlets.ColumnNo;
                        objUserPortletsPreferences.IsDisabled = false;
                        objlstUserPortletsPreferences.Add(objUserPortletsPreferences);


                        //if (roleportlet.Portlets.Name == "Live View")
                        //{
                        //    UserPortletsPreferences objUserPortletsPreferences = null;
                        //    for (int i = 0; i < 2; i++) // This is require to populate 2 Live View portlest in dashboard
                        //    {
                        //        objUserPortletsPreferences = new UserPortletsPreferences();
                        //        objUserPortletsPreferences.User = item;
                        //        objUserPortletsPreferences.PortletId = roleportlet.Portlets.Id;
                        //        objUserPortletsPreferences.Portlets = roleportlet.Portlets;
                        //        objUserPortletsPreferences.ColumnNo = roleportlet.Portlets.ColumnNo;
                        //        if (i > 0)
                        //            objUserPortletsPreferences.IsDisabled = true;
                        //        else
                        //            objUserPortletsPreferences.IsDisabled = false;
                        //    }
                        //    objlstUserPortletsPreferences.Add(objUserPortletsPreferences);
                        //}
                        //else
                        //{
                        //    UserPortletsPreferences objUserPortletsPreferences = new UserPortletsPreferences();
                        //    objUserPortletsPreferences.User = item;
                        //    objUserPortletsPreferences.PortletId = roleportlet.Portlets.Id;
                        //    objUserPortletsPreferences.Portlets = roleportlet.Portlets;
                        //    objUserPortletsPreferences.ColumnNo = roleportlet.Portlets.ColumnNo;
                        //    objUserPortletsPreferences.IsDisabled = false;
                        //    objlstUserPortletsPreferences.Add(objUserPortletsPreferences);
                        //}

                    }
                    // Dynamically Add Sequence Number based on Column
                    objlstUserPortletsPreferences.ToList().ForEach(x =>
                    {
                        if (x.ColumnNo == 1)
                        {
                            switch (x.Portlets.InternalName.ToUpper())
                            {
                                case "ACCOUNTDETAIL":
                                    x.SeqNo = 1;
                                    break;
                                case "LINKS":
                                    x.SeqNo = 2;
                                    break;
                                case "RSS":
                                    x.SeqNo = 3;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (x.ColumnNo == 2)
                        {
                            switch (x.Portlets.InternalName.ToUpper())
                            {
                                case "FEATUREDNEWS":
                                    x.SeqNo = 1;
                                    break;
                                case "INTRUSION":
                                    x.SeqNo = 2;
                                    break;
                                case "ACCESSCONTROL":
                                    x.SeqNo = 3;
                                    break;
                                case "ALERTS":
                                    x.SeqNo = 4;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (x.ColumnNo == 3)
                        {
                            switch (x.Portlets.InternalName.ToUpper())
                            {
                                case "LIVEVIEW":
                                    x.SeqNo = 1;
                                    break;
                                case "VIDEOHEALTHCHECK":
                                    x.SeqNo = 2;
                                    break;
                                case "SITEINFORMATION":
                                    x.SeqNo = 3;
                                    break;
                                case "SITEMAP":
                                    x.SeqNo = 4;
                                    break;
                                default:
                                    break;
                            }
                        }
                    });
                    item.userPortletsPreferences = objlstUserPortletsPreferences;
                    _userService.Create(item);
                    ErrorMsg = "Ok";
                    return Json(new { success = true });
                    //return RedirectToAction("UserDetails");
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        else if (serviceException.InnerException is RepositoryException)
                        {
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                            ErrorMsg = string.Format(serviceException.InnerException.Message);
                        }
                        else
                        {
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                            ErrorMsg = string.Format(serviceException.Message);
                        }
                    }
                    return Json(new { success = false, ErrorMsg = ErrorMsg });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", string.Format("Unexpected Error: [{0}]", e.Message));
                    ErrorMsg = string.Format("Unexpected Error: [{0}]", e.Message);
                    return Json(new { success = false, ErrorMsg = ErrorMsg });
                }
            }
            InitializeViewModel(newItem);

            return View(newItem);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateUserInfo(int id, UserViewModel editItem)
        {
            string ErrorMsg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {

                    if (editItem.OldCompanyId != editItem.CompanyId)
                    {
                        var userEdited = editItem.MapFromViewModel();
                        var persistentItem = _userService.GetUser(Convert.ToInt32(editItem.Id));

                        var destinationProperties = persistentItem.GetType().GetProperties();
                        foreach (var destinationPI in destinationProperties)
                        {
                            var sourcePI = userEdited.GetType().GetProperty(destinationPI.Name);
                            if (!(sourcePI.Name.Equals("Links") || sourcePI.Name.Equals("RSSFeeds")))
                            {
                                destinationPI.SetValue(persistentItem,
                                                       sourcePI.GetValue(userEdited, null),
                                                       null);
                            }
                        }

                        _userService.EditUserAndMonitoredGroupOfDevices(persistentItem, new BindingList<UserMonitorGroup>(),
                                                                        _userService.GetMonitoredGroupOfDevices(id));
                    }
                    else
                    {
                        var user = _userService.Get(editItem.Id);
                        var userEntity = editItem.MapFromViewModel();
                        userEntity.Links = user.Links;
                        userEntity.RSSFeeds = user.RSSFeeds;
                        userEntity.userPortletsPreferences = user.userPortletsPreferences;
                        _userService.Update(userEntity);
                    }

                    ErrorMsg = "Ok";
                    return Json(new { success = true });

                    if (Request.IsAjaxRequest())
                        return JsonOK();

                    return RedirectToAction("ViewUserProfile");
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        else if (serviceException.InnerException is RepositoryException)
                        {
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                            ErrorMsg = string.Format(serviceException.InnerException.Message);
                            return Json(new { success = false, ErrorMsg = ErrorMsg });
                        }
                        else
                        {
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                            ErrorMsg = string.Format(serviceException.Message);
                            return Json(new { success = false, ErrorMsg = ErrorMsg });
                        }
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", string.Format("Unexpected Error: [{0}]", e.Message));
                    ErrorMsg = string.Format("Unexpected Error: [{0}]", e.Message);
                    return Json(new { success = false, ErrorMsg = ErrorMsg });
                }
            }
            InitializeViewModel(editItem);

            return View(editItem);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public override ActionResult Create(UserViewModel newItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var item = newItem.MapFromViewModel();
                    _userService.Create(item);

                    return RedirectToAction("UserDetails");
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        else if (serviceException.InnerException is RepositoryException)
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                        else
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", string.Format("Unexpected Error: [{0}]", e.Message));
                }
            }
            InitializeViewModel(newItem);

            return View(newItem);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public override ActionResult Edit(int id, UserViewModel editItem)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (editItem.OldCompanyId != editItem.CompanyId)
                    {
                        var userEdited = editItem.MapFromViewModel();
                        var persistentItem = _userService.GetUser(Convert.ToInt32(editItem.Id));

                        var destinationProperties = persistentItem.GetType().GetProperties();
                        foreach (var destinationPI in destinationProperties)
                        {
                            var sourcePI = userEdited.GetType().GetProperty(destinationPI.Name);

                            destinationPI.SetValue(persistentItem,
                                                   sourcePI.GetValue(userEdited, null),
                                                   null);
                        }

                        _userService.EditUserAndMonitoredGroupOfDevices(persistentItem, new BindingList<UserMonitorGroup>(),
                                                                        _userService.GetMonitoredGroupOfDevices(id));
                    }
                    else
                        _userService.Update(editItem.MapFromViewModel());

                    if (Request.IsAjaxRequest())
                        return JsonOK();

                    return RedirectToAction("ViewUserProfile");
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        else if (serviceException.InnerException is RepositoryException)
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                        else
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", string.Format("Unexpected Error: [{0}]", e.Message));
                }
            }
            InitializeViewModel(editItem);

            return View(editItem);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditUserDetails(int id, UserViewModel editItem)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (editItem.OldCompanyId != editItem.CompanyId)
                    {
                        var userEdited = editItem.MapFromViewModel();
                        var persistentItem = _userService.GetUser(Convert.ToInt32(editItem.Id));

                        var destinationProperties = persistentItem.GetType().GetProperties();
                        foreach (var destinationPI in destinationProperties)
                        {
                            var sourcePI = userEdited.GetType().GetProperty(destinationPI.Name);

                            destinationPI.SetValue(persistentItem,
                                                   sourcePI.GetValue(userEdited, null),
                                                   null);
                        }

                        _userService.EditUserAndMonitoredGroupOfDevices(persistentItem, new BindingList<UserMonitorGroup>(),
                                                                        _userService.GetMonitoredGroupOfDevices(id));
                    }
                    else
                        _userService.Update(editItem.MapFromViewModel());

                    if (Request.IsAjaxRequest())
                        return JsonOK();

                    return RedirectToAction("UserDetails");
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        else if (serviceException.InnerException is RepositoryException)
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                        else
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", string.Format("Unexpected Error: [{0}]", e.Message));
                }
            }
            InitializeViewModel(editItem);

            return View(editItem);
        }

        //GET: /Dashboard/LoadViewMoreDevices
        public ActionResult LoadUserEditableInfo()
        {
            var model = new UserViewModel
            {
                AvailableRoleList = _roleService.GetAll(),
                AvailableCompanyList = _companyService.GetAll(),
                AvailableTimeZoneList = _deviceService.GetAllTimeZones(),
                PreferredContact = PreferredContact.Mobile.ToString()
            };
            return PartialView("_UserEditableInfo", model);
        }

        public ActionResult EditUserInfo()
        {
            var model = new UserViewModel
            {
                AvailableRoleList = _roleService.GetAll(),
                AvailableCompanyList = _companyService.GetAll(),
                AvailableTimeZoneList = _deviceService.GetAllTimeZones(),
                PreferredContact = PreferredContact.Mobile.ToString()
            };
            return PartialView("_UserInfo", model);
        }

        public ActionResult Users_Read([DataSourceRequest] DataSourceRequest request)
        {
            var objlstUsers = _userService.GetAll();
            var userViewModel = from objUser in objlstUsers
                                select new UserViewModel
                                {
                                    Id = objUser.Id,
                                    FirstName = objUser.FirstName,
                                    LastName = objUser.LastName,
                                    Username = objUser.Username,
                                    Email = objUser.Email,
                                    Phone = objUser.Phone,
                                    Title = objUser.Title,
                                    RoleId = objUser.Role.Id,
                                    CompanyId = objUser.Company.Id
                                };
            return Json(userViewModel.ToList().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Users_Create([DataSourceRequest] DataSourceRequest request, UserViewModel userView)
        {
            if (userView != null && ModelState.IsValid)
            {
                userView.PreferredContact = PreferredContact.Mobile.ToString();
                userView.TimeZone = "UTC";
                var item = userView.MapFromViewModel();
                _userService.Create(item);
            }

            return Json(new[] { userView }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Users_Update([DataSourceRequest] DataSourceRequest request, UserViewModel userView)
        {
            userView.PreferredContact = PreferredContact.Mobile.ToString();
            userView.TimeZone = "UTC";
            _userService.Update(userView.MapFromViewModel());
            return Json(ModelState.ToDataSourceResult());

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Users_Destroy([DataSourceRequest] DataSourceRequest request, UserViewModel userView)
        {
            return _currentUserProvider.CurrentUser.Id == userView.Id ? JsonError("You can't delete a logged user") : base.Delete(userView.Id);
        }

        [HttpPost]
        public ActionResult Create2(IEnumerable<User> perms)
        {
            var result = new List<User>();

            //Iterate all created products which are posted by the Kendo Grid
            foreach (var permission in perms)
            {
                var perm = new User
                {
                    Id = permission.Id,
                    FirstName = permission.FirstName,
                    LastName = permission.LastName,
                    Username = permission.Username,
                    Email = permission.Email,
                    Phone = permission.Phone,
                    Title = permission.Title,
                    Role = permission.Role
                };

                // store the product in the result
                result.Add(perm);
            }

            return null;

        }

        public ActionResult UserDetails()
        {
            var objlstCompanies = _companyService.GetAll();
            var companyModel = from objCompany in objlstCompanies
                               select new CompanyViewModel
                               {
                                   Id = objCompany.Id,
                                   Name = objCompany.Name,
                               };

            var objlstRoles = _roleService.GetAll();
            var roleModel = from objRole in objlstRoles
                            select new RoleViewModel
                            {
                                Id = objRole.Id,
                                Name = objRole.Name,
                            };


            ViewData["roles"] = roleModel;
            ViewData["companies"] = companyModel;
            return View("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public override ActionResult Disable(int id)
        {
            return _currentUserProvider.CurrentUser.Id == id ? JsonError("You can't disable a logged user") : base.Disable(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public override ActionResult Delete(int id)
        {
            return _currentUserProvider.CurrentUser.Id == id ? JsonError("You can't delete a logged user") : base.Delete(id);
        }
        public ActionResult ViewUserProfile()
        {
            UserViewModel objUserViewModel = new UserViewModel();
            objUserViewModel.Id = _currentUserProvider.CurrentUser.Id;
            var itemToEdit = MapEntity(_userService.Get(objUserViewModel.Id));
            objUserViewModel.FirstName = itemToEdit.FirstName;
            objUserViewModel.LastName = itemToEdit.LastName;
            objUserViewModel.Title = itemToEdit.Title;
            objUserViewModel.Username = itemToEdit.Username;
            objUserViewModel.Email = itemToEdit.Email;
            objUserViewModel.Phone = itemToEdit.Phone;
            objUserViewModel.Mobile = itemToEdit.Mobile;
            objUserViewModel.CompanyName = itemToEdit.CompanyName;
            objUserViewModel.RoleName = itemToEdit.RoleName;
            objUserViewModel.TimeZone = itemToEdit.TimeZone;
            objUserViewModel.Text1 = itemToEdit.Text1;
            objUserViewModel.Text2 = itemToEdit.Text2;
            return View(objUserViewModel);
        }

        public JsonResult GetAllCompanyDetails()
        {
            IList<SelectListItem> lstSelectedList = new List<SelectListItem>();
            IList<Company> lstCompany = new List<Company>();
            lstCompany = _companyService.GetAll();
            lstCompany.ToList().ForEach(x =>
            {
                lstSelectedList.Add(new SelectListItem { Text = x.Id.ToString(), Value = x.Name });
            });
            return Json(lstSelectedList.Select(c => new { Id = c.Value, Name = c.Text }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllRoleDetails()
        {
            IList<SelectListItem> lstSelectedList = new List<SelectListItem>();
            IList<Role> lstRole = new List<Role>();
            lstRole = _roleService.GetAll();
            lstRole.ToList().ForEach(x =>
            {
                lstSelectedList.Add(new SelectListItem { Text = x.Id.ToString(), Value = x.Name });
            });
            return Json(lstSelectedList.Select(c => new { Id = c.Value, Name = c.Text }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllTimeZones()
        {
            IList<SelectListItem> lstSelectedList = new List<SelectListItem>();
            IList<System.TimeZoneInfo> lstTimeZone = new List<System.TimeZoneInfo>();
            lstTimeZone = _deviceService.GetAllTimeZones();
            lstTimeZone.ToList().ForEach(x =>
            {
                lstSelectedList.Add(new SelectListItem { Text = x.Id.ToString(), Value = x.DisplayName });
            });
            return Json(lstSelectedList.Select(c => new { Id = c.Value, Name = c.Text }), JsonRequestBehavior.AllowGet);
        }
    }    
}
