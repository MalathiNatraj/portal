using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Enums;
using Diebold.Services.Exceptions;
using Diebold.Services.Extensions;
using Diebold.WebApp.Infrastructure.Helpers;
using Diebold.WebApp.Models;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using System.Data;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.WebApp.Controllers
{
    public class DashboardController : BaseController
    {
        protected readonly IAlertService _alertService;
        protected readonly IDvrService _deviceService;
        protected readonly INoteService _noteService;
        protected readonly IUserService _userService;
        protected readonly INotificationService _notificationService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserPortletsPreferences _userPortletsPreferences;
        private readonly IAccessService _accessService;
        private readonly IGatewayService _gatewayService;
        private readonly IIntrusionService _intrusionService;

        public DashboardController(IAlertService alertService,
            IDvrService deviceService,
            INoteService noteService, 
            IUserService userService,
            INotificationService notificationService,
            ICurrentUserProvider currentUserProvider,
            ILinkService linkService,
            IRSSFeedService RssFeedService,
            IUserPortletsPreferences userPortletsPreferences,
            IGatewayService gatewayService,
            IIntrusionService intrusionService,
            IAccessService accessService
            )
        {
            _alertService = alertService;
            _deviceService = deviceService;
            _noteService = noteService;
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _notificationService = notificationService;
            _userPortletsPreferences = userPortletsPreferences;
            _accessService = accessService;
            _gatewayService = gatewayService;
            _intrusionService = intrusionService;

        }
        
        public ActionResult Index() 
        {
            return View();
        }

        public ActionResult FeaturedNews()
        {
            return View();
        }

        #region Alerts/Devices List
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
                                                           (AlarmType) alertStatus.Alarm.AlarmType, alertStatus.ElementIdentifier, (Dvr) alertStatus.Device),

                                           DateOccur = (alertStatus.FirstAlertTimeStamp != null)? alertStatus.FirstAlertTimeStamp.Value.ToString(
                                                               "MMMM dd, yyyy H:mm", CultureInfo.CreateSpecificCulture("en-US")): string.Empty,

                                           AlertCleared = alertStatus.IsOk
                                       };

                if (alertStatus.Device.IsDvr)
                {
                    var dvr = (Dvr) alertStatus.Device;
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
                    EmcAccontNumber = ((Dvr) alertStatus.Device).Gateway.EMCId.ToString(),
                    EmcDevicezone = ((Dvr) alertStatus.Device).ZoneNumber
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

        #region Current Status
    
        //GET: /Dashboard/CurrentReadings
        public ActionResult CurrentReadings(int id, bool liveFromDevice = false)
        {
            try
            {
                if (id > 0)
                {
                    var device = _deviceService.Get(id);
                    ViewBag.DeviceId = id;
                    ViewBag.DeviceName = device.Name;
                    ViewBag.ShowViewMore = true;

                    //var status = device.DeviceStatus.Count > 0 ? 

                    var status = _deviceService.GetLiveStatus(id, device.Gateway.MacAddress, liveFromDevice,false);

                    IEnumerable<DeviceStatusViewModel> deviceStatusModel = null;//PredefinedDeviceStatusHelper.SortStatus(status)
                                                                           //.Select(x => new DeviceStatusViewModel(x, false)).ToList();
                    foreach (var model in deviceStatusModel)
                    {
                        if (!model.IsDictionary) continue;

                        var driveModels = ((IDictionary<string, object>)model.Value).Select(item =>
                                            PredefinedDeviceStatusHelper.GetDvrDriveViewModel(device, model, item.Key, item.Value))
                                            .Where(driveModel => driveModel != null).ToList();

                        model.Value = driveModels;
                    }

                    return PartialView("_CurrentStatus", deviceStatusModel);
                }

                ViewBag.ShowViewMore = false;
                return PartialView("_CurrentStatus", new List<DeviceStatusViewModel>());
            }
            catch (ServiceException ex)
            {
                LogError("Service Exception occured while processing current readings", ex);
                return JsonError(ex.Message);
            }
        }

        #endregion

        #region Portal code

        public ActionResult LogOn()
        {
            return RedirectToAction("Index");
        }

        private void UpdateUserPrefrences()
        {
            IList<UserPortletsPreferences> itemsToDelete = new List<UserPortletsPreferences>();
            IList<UserPortletsPreferences> UserPortlet = new List<UserPortletsPreferences>();
            IList<RolePortlets> itemsToAdd = new List<RolePortlets>();
            int Col1MaxSeqNo = 0;
            int Col2MaxSeqNo = 0;
            int Col3MaxSeqNo = 0;

            var UserDetails = _userService.Get(_currentUserProvider.CurrentUser.Id);
            var RolePortlet = UserDetails.Role.RolePortlets.ToList();
            UserPortlet = UserDetails.userPortletsPreferences;
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
            try
            {
                UpdateUserPrefrences();
                IList<UserPortletsPreferences> objLstUserPortletsPreferences = _userPortletsPreferences.GetAllActivePortletsByUser(_currentUserProvider.CurrentUser.Id);
                if (objLstUserPortletsPreferences != null && objLstUserPortletsPreferences.Count > 0)
                {
                    IList<UserPortletsPreferences> objlstDashboardPortlets = new List<UserPortletsPreferences>();
                    objlstDashboardPortlets = objLstUserPortletsPreferences.Where(x => x.IsDisabled == false).ToList();
                    return View("Home", objlstDashboardPortlets);
                }
                else
                    return JsonError("No portlets assigned to user.");
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public JsonResult GetAccountDetails()
        {
            try
            {
                AccountDetailModel objAccount = new AccountDetailModel();
                // To get company name of a user                
                objAccount.CompanyName = _currentUserProvider.CurrentUser.FirstName + " " + _currentUserProvider.CurrentUser.LastName;
                objAccount.CompanyLogo = "data:image/png;base64," + Convert.ToBase64String(_currentUserProvider.CurrentUser.Company.CompanyLogo);

                int intCurrentUserId = _currentUserProvider.CurrentUser.Id;
                // To get a site count
                objAccount.SiteCount = _userService.GetMonitoredSitesCount(intCurrentUserId);
                // To get Device count   

                IList<Dvr> objLstDeviceList = _userService.GetDevicesByParentType(intCurrentUserId, "ALL");
                int dvrCount = 0;
                int accessCount = 0;
                int intrusionCount = 0;
                objLstDeviceList.ForEach(x =>
                {
                    if (x.DeviceType.Equals(DeviceType.Costar111) || x.DeviceType.Equals(DeviceType.ipConfigure530) || x.DeviceType.Equals(DeviceType.VerintEdgeVr200))
                    {
                        objAccount.HealthDevices = ++dvrCount;
                    }
                    else if (x.DeviceType.Equals(DeviceType.eData300) || x.DeviceType.Equals(DeviceType.eData524) || x.DeviceType.Equals(DeviceType.dmpXR100Access) || x.DeviceType.Equals(DeviceType.dmpXR500Access))
                    {
                        objAccount.AccessDevices = ++accessCount;
                    }
                    else if (x.DeviceType.Equals(DeviceType.dmpXR100) || x.DeviceType.Equals(DeviceType.dmpXR500) || x.DeviceType.Equals(DeviceType.bosch_D9412GV4) || x.DeviceType.Equals(DeviceType.videofied01))
                    {
                        objAccount.IntrusionDevices = ++intrusionCount;
                    }
                });
                return Json(objAccount);
            }
            catch(Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public JsonResult InsertData(string input)
        {
            try
            {
                string[] ColumnwiseItems = input.Split('&');
                IDictionary<string, string> dctPortletPostion = new Dictionary<string, string>();
                foreach (string item in ColumnwiseItems)
                {
                    if (item.Contains("C1"))
                    {
                        UpdatePortletPosition(item, dctPortletPostion, "1~");
                    }
                    else if (item.Contains("C2"))
                    {
                        UpdatePortletPosition(item, dctPortletPostion, "2~");
                    }
                    else if (item.Contains("C3"))
                    {
                        UpdatePortletPosition(item, dctPortletPostion, "3~");
                    }
                }

                var resultSet = _userService.Get(_currentUserProvider.CurrentUser.Id);
                foreach (UserPortletsPreferences row in resultSet.userPortletsPreferences)
                {
                    string portletPosition = dctPortletPostion.Where(x => x.Key.Equals(row.Portlets.InternalName.ToUpper())).FirstOrDefault().Value;
                    if (string.IsNullOrEmpty(portletPosition) == false)
                    {
                        string[] rs = portletPosition.Split('~');
                        row.SeqNo = Convert.ToInt32(rs[1]);
                        row.ColumnNo = Convert.ToInt32(rs[0]);
                    }
                }
                _userService.Update(resultSet);
                return Json(new { name = "Success" });
            }
            catch(Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        private void UpdatePortletPosition(string item, IDictionary<string, string> dctPortletPostion, string columnNumber)
        {
            string[] temp;
            string[] RowItems;

            temp = item.Split('~');
            if (temp != null && temp.Length > 1)
            {
                RowItems = temp[1].Split(',');
                for (int i = 0; i < RowItems.Length; i++)
                {
                    dctPortletPostion.Add(RowItems[i].ToString(), columnNumber + (i + 1).ToString());
                }
            }
        }
       
        public ActionResult Returnfunc(string input)
        {
            return RedirectToAction("Index");
        }

        #endregion

        public ActionResult ShowStatusDevice(int id)
        {
            try
            {
                return CurrentReadings(id, DeviceInfo.Device, false);
            }
            catch (ServiceException serviceException)
            {
                LogError("Service Exception occured while fetching device information", serviceException);
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError("Exception occured while auditing device", e);
                return JsonError("An error occurred while fetching device information");
            }
        }

        public ActionResult CurrentReadings(int id, DeviceInfo deviceType, bool isGateway)
        {
            try
            {
                IEnumerable<DeviceStatusViewModel> deviceStatusModel;
                StatusDTO status = null;
                if (id > 0)
                {
                    ViewBag.DeviceId = id;
                    ViewBag.ShowViewMore = false;
                    ViewBag.DeviceType = deviceType;

                    Dvr device = null;

                    if (deviceType == DeviceInfo.Gateway)
                    {
                        var gateway = _gatewayService.Get(id);
                        status = _gatewayService.GetLiveStatus(id, gateway.MacAddress, true, true);
                    }
                    else
                    {
                        device = _deviceService.Get(id);
                        switch (device.DeviceType.ToString().ToLower())
                        {
                            case "costar111":
                            case "verintedgevr200":
                            case "ipconfigure530":
                                status = _deviceService.GetLiveStatus(id, device.Gateway.MacAddress + "-" + device.DeviceKey, false, true);
                                break;
                            case "edata524":
                            case "edata300":
                            case "dmpxr100Access":
                            case "dmpxr500Access":
                                status = PrepareDeviceStatus(_accessService.GetAccessDetails(device.Id).Properties);
                                break;
                            case "dmpxr100":
                            case "dmpxr500":
							case "bosch_D9412GV4":
							case "videofied01":							
                                status = PrepareDeviceStatus(_intrusionService.GetIntrusionDetails(device.Id).Properties);
                                break;
                        }
                    }
                }
                else
                {
                    deviceStatusModel = new List<DeviceStatusViewModel>();
                }

                return PartialView("_CurrentStatus", status);
            }
            catch (ServiceException ex)
            {
                LogError("Service Exception while getting status for " + deviceType, ex);
                return new ContentResult { Content = "An error occurred while executing the action. The status cannot be shown." };
            }
        }

        private StatusDTO PrepareDeviceStatus(List<DeviceProperty> objResponse)
        {
            StatusDTO objStatus = new StatusDTO();
            objStatus.payload = new StausPayload();
            objStatus.payload.SparkDvrReport = new SparkDvrReport();
            objStatus.payload.SparkDvrReport.properties = new Properties();
            ResponseProperty[] properties = new ResponseProperty[objResponse.Count];
            ResponseProperty objResponseProperty = null;
            for (int count = 0; count < objResponse.Count; count++)
            {
                objResponseProperty = new ResponseProperty();
                objResponseProperty.name = objResponse[count].name;
                objResponseProperty.value = objResponse[count].value;
                properties.SetValue(objResponseProperty, count);
            }
            objStatus.payload.SparkDvrReport.properties.property = properties;
            objStatus.isGateWay = "NO";
            return objStatus;
        }

        public JsonResult GetAllReportTypes()
        {
            string ReportName = string.Empty;
            // Bind Report Types inside Combo Box
            List<ReportModel> objReportModel = new List<ReportModel>();
            objReportModel = GetReportTypes();
            if (Session["MASReportName"] != null)
            {
                ReportName = (string)Session["MASReportName"];
                return Json(objReportModel.Select(c => new { Id = c.Id, Name = c.Name, DefaultSelectedValue = ReportName }), JsonRequestBehavior.AllowGet);
            }
            return Json(objReportModel.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public List<ReportModel> GetReportTypes()
        {
            List<ReportModel> lstReport = new List<ReportModel> 
            {
                new ReportModel{Id = 1, Name="Events"},
                new ReportModel{Id = 2, Name="Open / Close Normal"},
                new ReportModel{Id = 3, Name="Open / Close Irregular"},
                new ReportModel{Id = 4, Name="Zone List"},
                new ReportModel{Id = 5, Name="Contact List"}
            };
            return lstReport;
        }

        public JsonResult LoadViewMoreDevices(int? DeviceId)
        {
            int intDeviceId = Convert.ToInt32(DeviceId);
            // return Json(qry).ToDataSourceResult(request);
            string ElementIdentifier = string.Empty;
            string cameraName = string.Empty;
            var ResultSet = _alertService.GetAlertsByDevice(intDeviceId);
            IList<DeviceListDashboardViewModel> lstDeviceListDashboardViewModel = new List<DeviceListDashboardViewModel>();
            ResultSet.ForEach(x =>
            {
                Device objDevice = x.Device;
                ElementIdentifier = x.ElementIdentifier;
                if (objDevice != null && x.Alarm.AlarmType == AlarmType.VideoLoss)
                {
                    Diebold.Domain.Entities.Dvr objDvr = (Diebold.Domain.Entities.Dvr)objDevice;
                    if (objDvr != null)
                    {                        
                        cameraName = objDvr.Cameras.Where(y => y.Channel.Equals(ElementIdentifier)).FirstOrDefault().Name;
                    }
                    lstDeviceListDashboardViewModel.Add(new DeviceListDashboardViewModel { Id = x.Id, DeviceName = x.Device.Name, AlertName = x.Device.Name + " : " + x.Alarm.AlarmType + " : " + cameraName, IsDeviceOk = x.IsOk, Ack = x.IsAcknowledged.ToString() });
                }
                else if (x.Alarm.AlarmType != AlarmType.VideoLoss)
                {
                    lstDeviceListDashboardViewModel.Add(new DeviceListDashboardViewModel { Id = x.Id, DeviceName = x.Device.Name, AlertName = x.Device.Name + " : " + x.Alarm.AlarmType, IsDeviceOk = x.IsOk, Ack = x.IsAcknowledged.ToString() });
                }
            });          
           
            return Json(lstDeviceListDashboardViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}
