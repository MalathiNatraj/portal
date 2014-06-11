using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;
using Diebold.WebApp.Infrastructure.Authentication;

namespace Diebold.WebApp.Controllers
{
    public class MyAccessPointsController : BaseController
    {
        //
        // GET: /MyAccessPoints/
        // GET: /Access/
        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;
        protected readonly IUserDefaultsService _userDefaultService;
        private readonly IDvrService _dvrService;
        private readonly IAccessService _accessService;
        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;

        public MyAccessPointsController(IUserService userService, ICurrentUserProvider currentUserProvider, IUserDefaultsService userDefaultService, IDvrService dvrService, IAccessService accessService, ICompanyService companyService, ISiteService siteService)
        {
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _userDefaultService = userDefaultService;
            _dvrService = dvrService;
            _accessService = accessService;
            _companyService = companyService;
            _siteService = siteService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetCardholderView(string action, int deviceId)
        {
            try
            {
                var device = _dvrService.Get(deviceId);
                var model = new AccessCardholderViewModel()
                {
                    DeviceId = deviceId,
                    DeviceName = device.Name,
                    DeviceType = device.DeviceType
                };


                switch (model.DeviceType)
                {
                    case DeviceType.dmpXR100Access:
                        try
                        {
                            if (string.Compare(action, "add", true) == 0 || string.Compare(action, "deleteAccessGroup", true) == 0)
                            {                                
                                model.AccessGroupList = _accessService.AccessGetGroupList(deviceId).Select(c => new dmpXRAccessGroupModel { Id = c.AccessGroupValue, Name = c.AccessGroupValue }).ToList();
                            }
                            else if (string.Compare(action, "updateAccessGroup", true) == 0)
                            {
                                model.AccessGroupList = _accessService.AccessGetGroupList(deviceId).Select(c => new dmpXRAccessGroupModel { Id = c.AccessGroupValue, Name = c.AccessGroupValue }).ToList();
                                var profileNumList = _accessService.GetReadersList(deviceId).Select(c => new dmpXRAccessGroupModel { Id = c.value, Name = c.name }).ToList();
                                string strReaderName = "";
                                for (int i = 0; i < profileNumList.Count(); i++)
                                {
                                    strReaderName = strReaderName + "," + profileNumList[i].Name;
                                }
                                model.readerName = strReaderName;
                            }
                            else if (string.Compare(action, "addAccessGroup", true) == 0)
                            {
                                var AddProfileNumList = _accessService.GetReadersList(deviceId).Select(c => new dmpXRAccessGroupModel { Id = c.value, Name = c.name }).ToList();
                                string strReaderName = "";
                                for (int i = 0; i < AddProfileNumList.Count(); i++)
                                {
                                    strReaderName = strReaderName + "," + AddProfileNumList[i].Name;
                                }
                                model.readerName = strReaderName;
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }

                        if (string.Compare(action, "add", true) == 0)
                        {                            
                            return PartialView("DMPXR/CardholderAdd", model);
                        }
                        else if (string.Compare(action, "modify", true) == 0)
                        {
                            return PartialView("DMPXR/CardholderModify", model);
                        }
                        else if (string.Compare(action, "delete", true) == 0)
                        {
                            return PartialView("DMPXR/Delete", model);
                        }
                        else if (string.Compare(action, "deleteAccessGroup", true) == 0)
                        {
                            return PartialView("DMPXR/AccessGroupDelete", model);
                        }
                        else if (string.Compare(action, "addAccessGroup", true) == 0)
                        {
                            return PartialView("DMPXR/AddAccessGroupTimePeriods", model);
                        }
                        else if (string.Compare(action, "updateAccessGroup", true) == 0)
                        {
                            return PartialView("DMPXR/AccessGroupModify", model);
                        }
                        throw new Exception("No view found for action: " + action);

                    case DeviceType.dmpXR500Access:
                        try
                        {
                            if (string.Compare(action, "add", true) == 0 || string.Compare(action, "deleteAccessGroup", true) == 0)
                            {
                                model.AccessGroupList = _accessService.AccessGetGroupList(deviceId).Select(c => new dmpXRAccessGroupModel { Id = c.AccessGroupValue, Name = c.AccessGroupValue }).ToList();
                            }
                            else if (string.Compare(action, "updateAccessGroup", true) == 0)
                            {
                                model.AccessGroupList = _accessService.AccessGetGroupList(deviceId).Select(c => new dmpXRAccessGroupModel { Id = c.AccessGroupValue, Name = c.AccessGroupValue }).ToList();
                                var UpdateprofileNumList500 = _accessService.GetReadersList(deviceId).Select(c => new dmpXRAccessGroupModel { Id = c.value, Name = c.name }).ToList();
                                string strReaderName = "";
                                for (int i = 0; i < UpdateprofileNumList500.Count(); i++)
                                {
                                    strReaderName = strReaderName + "," + UpdateprofileNumList500[i].Name;
                                }
                                model.readerName = strReaderName;
                            }
                            else if (string.Compare(action, "addAccessGroup", true) == 0)
                            {
                                var profileNumListXR500 = _accessService.GetReadersList(deviceId).Select(c => new dmpXRAccessGroupModel { Id = c.value, Name = c.name }).ToList();
                                string strReaderName = "";
                                for (int i = 0; i < profileNumListXR500.Count(); i++)
                                {
                                    strReaderName = strReaderName + "," + profileNumListXR500[i].Name;
                                }
                                model.readerName = strReaderName;
                            }
                        }
                        catch (Exception e)
                        {

                            throw new Exception(e.Message);
                        }

                        if (string.Compare(action, "add", true) == 0)
                        {
                            return PartialView("DMPXR/CardholderAdd", model);
                        }
                        else if (string.Compare(action, "modify", true) == 0)
                        {
                            return PartialView("DMPXR/CardholderModify", model);
                        }
                        else if (string.Compare(action, "delete", true) == 0)
                        {
                            return PartialView("DMPXR/Delete", model);
                        }
                        else if (string.Compare(action, "deleteAccessGroup", true) == 0)
                        {
                            return PartialView("DMPXR/AccessGroupDelete", model);
                        }
                        else if (string.Compare(action, "addAccessGroup", true) == 0)
                        {
                            return PartialView("DMPXR/AddAccessGroupTimePeriods", model);
                        }
                        else if (string.Compare(action, "updateAccessGroup", true) == 0)
                        {
                            return PartialView("DMPXR/AccessGroupModify", model);
                        }
                        throw new Exception("No view found for action: " + action);

                    default:
                        try
                        {
                            if (string.Compare(action, "add", true) == 0 || string.Compare(action, "deleteAccessGroup", true) == 0 || string.Compare(action, "updateAccessGroup", true) == 0)
                            {
                                model.AccessGroupList = _accessService.AccessGetGroupList(deviceId).Select(c => new dmpXRAccessGroupModel { Id = c.AccessGroupValue, Name = c.AccessGroupValue }).ToList();                               
                            }
                        }
                        catch (Exception e)
                        {

                            throw new Exception(e.Message);
                        }
                        if (string.Compare(action, "add", true) == 0)
                        {
                            return PartialView("CardholderAdd", model);
                        }
                        else if (string.Compare(action, "modify", true) == 0)
                        {
                            return PartialView("CardholderModify", model);
                        }
                        else if (string.Compare(action, "delete", true) == 0)
                        {
                            return PartialView("Delete", model);
                        }
                        else if (string.Compare(action, "deleteAccessGroup", true) == 0)
                        {
                            return PartialView("AccessGroupDelete", model);
                        }
                        else if (string.Compare(action, "addAccessGroup", true) == 0)
                        {
                            return PartialView("AddAccessGroupTimePeriods", model);
                        }
                        else if (string.Compare(action, "updateAccessGroup", true) == 0)
                        {
                            return PartialView("AccessGroupModify", model);
                        }
                        throw new Exception("No view found for action: " + action);
                }

            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

    }
}
