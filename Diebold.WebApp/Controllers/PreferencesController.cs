using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Diebold.WebApp.Models;
using System.Xml;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Enums;
using Diebold.Services.Exceptions;
using Diebold.Services.Extensions;
using Diebold.WebApp.Infrastructure.Helpers;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;

namespace Diebold.WebApp.Controllers
{
    public class PreferencesController : BaseController
    {
        //
        // GET: /Preferences/

        protected readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserPortletsPreferences _userPortletsPreferences;
        private readonly IPortletService _portletService;

        public PreferencesController(IUserService userService, ICurrentUserProvider currentUserService, IUserPortletsPreferences userPortletsPreference, IPortletService portletService)
        {
            _userService = userService;
            _currentUserProvider = currentUserService;
            _userPortletsPreferences = userPortletsPreference;
            _portletService = portletService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DisplayPreferences()
        {
            try
            {
                IList<UserPortletsPreferences> objLstUserPortletsPreferences = _userPortletsPreferences.GetAllPortletsByUser(_currentUserProvider.CurrentUser.Id);
                List<PreferencesModel> lstPreferencesModel = new List<PreferencesModel>();
                PreferencesModel objPreference = null;

                foreach (var item in objLstUserPortletsPreferences)
                {
                    objPreference = new Models.PreferencesModel();
                    switch (item.Portlets.InternalName)
                    {
                        case "LIVEVIEW":
                            objPreference.PortletHeading = item.Portlets.InternalName;
                            objPreference.PortletDisplayHeader = item.Portlets.Name;
                            // Check if all the Live View Items are added or not
                            objPreference.PortletStatus = "N";
                            List<UserPortletsPreferences> lstuPreference = objLstUserPortletsPreferences.Where(x => x.Portlets.Name.Contains("Live View")).ToList();
                            if (lstuPreference.Count == 2)
                            {
                                if (lstuPreference[0].IsDisabled == true)
                                    objPreference.Id = lstuPreference[0].Id;
                                else if (lstuPreference[1].IsDisabled == true)
                                    objPreference.Id = lstuPreference[1].Id;

                                if (lstuPreference[0].IsDisabled == false && lstuPreference[1].IsDisabled == false)
                                {
                                    objPreference.PortletStatus = "Y";
                                    objPreference.Id = item.Id;
                                }
                            }
                            lstPreferencesModel.Add(objPreference);
                            break;
                        default:
                            objPreference.PortletHeading = item.Portlets.InternalName;
                            objPreference.PortletDisplayHeader = item.Portlets.Name;
                            objPreference.Id = item.Id;
                            objPreference.PortletStatus = "N";
                            if (item.IsDisabled == false)
                                objPreference.PortletStatus = "Y";
                            lstPreferencesModel.Add(objPreference);
                            break;
                    }
                }

                DashboardModel objDashboardModel = new DashboardModel();
                objDashboardModel.preferences = lstPreferencesModel.Distinct().OrderBy(x => x.PortletHeading).ToList();
                ViewBag.DashboardModel = objDashboardModel;
                return View(objDashboardModel);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult InsertData(string input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    string[] ColumnwiseItems = input.Split('~');
                    int ItemsAddedCurrently = ColumnwiseItems.Length;
                    var UserDetailsResult = _userService.Get(_currentUserProvider.CurrentUser.Id);
                    UserDetailsResult.userPortletsPreferences.ForEach(x =>
                    {
                        if (ColumnwiseItems.Contains(x.Id.ToString()))
                        {
                            x.IsDisabled = false;
                        }
                    });
                    _userService.Update(UserDetailsResult);
                    return Json(new { name = "Success" });
                }
                else
                    return JsonError("Fail to update preference.");
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError("Error:"+ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ClosePortlet(string input)
        {
            try
            {
                IList<UserPortletsPreferences> objLstUserPortletsPreferences = new List<UserPortletsPreferences>();
                objLstUserPortletsPreferences = _userPortletsPreferences.GetAllPortletsByUser(_currentUserProvider.CurrentUser.Id);
                foreach (var item in objLstUserPortletsPreferences)
                {
                    if (item.Portlets.InternalName.ToUpper().Equals(input.ToUpper()))
                    {
                        item.IsDisabled = true;
                        _userPortletsPreferences.Update(item);
                    }
                }
                return Json(new { name = "Success" });
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult Returnfunc(string input)
        {
            return RedirectToAction("DisplayPreferences");
        }

        // Add Camera from Live View Portlet context menu
        public ActionResult AddNextCamera()
        {
            try
            {
                var UserDetailsResult = _userService.Get(_currentUserProvider.CurrentUser.Id);
                List<UserPortletsPreferences> lstUserPortletPreference = _userPortletsPreferences.GetInActivePortletsByUserforLiveView(_currentUserProvider.CurrentUser.Id).OrderBy(y => y.Id).ToList();
                // Update the Next Live View
                if (lstUserPortletPreference != null && lstUserPortletPreference.Count() > 0)
                {
                    foreach (var item in UserDetailsResult.userPortletsPreferences)
                    {
                        if (item.Portlets.Id == lstUserPortletPreference.First().Portlets.Id)
                        {
                            item.IsDisabled = false;
                        }
                    }
                    _userService.Update(UserDetailsResult);
                }
                return Json("Success");
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }
    }
}
