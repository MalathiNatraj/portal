﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;
using Diebold.WebApp.Infrastructure.Authentication;

namespace Diebold.WebApp.Controllers
{
    public class AlertsController : BaseController
    {
        //
        // GET: /Alert/
        private readonly IDeviceService _deviceService;
        private readonly IAlertService _alertService;
        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ISiteService _siteService;
        private readonly IDvrService _dvrService;
        private readonly ICompanyService _companyService;

        public AlertsController(IDeviceService deviceService, IAlertService alertService, IUserService userService, ICurrentUserProvider currentUserProvider, ISiteService siteService, IDvrService dvrService, ICompanyService companyService)
        {
            _deviceService = deviceService;
            _alertService = alertService;
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _siteService = siteService;
            _dvrService = dvrService;
            _companyService = companyService;
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
        public ActionResult getHealthDetails()
        {
            return View("HealthCheckDetails");
        }

        public JsonResult GetAllDevicebyParentType(string parentType)
        {
            Session["parentType"] = parentType;
            var user = _currentUserProvider.CurrentUser;
            IList<Dvr> objLstDevices = _userService.GetDevicesByParentType(_currentUserProvider.CurrentUser.Id, parentType);

            IList<DeviceModel> objlstDevice = new List<DeviceModel>();
            objLstDevices.ToList().ForEach(x =>
            {
                DeviceModel objdevice = new DeviceModel();
                objdevice.Id = x.Id;
                objdevice.Name = x.Name;
                objdevice.SiteId = x.Site.Id;
                objdevice.SiteName = x.Site.Name;
                objdevice.Address1 = x.Site.Address1;
                if (!string.IsNullOrEmpty(x.Site.Address2))
                    objdevice.Address2 = x.Site.Address2;
                else
                    objdevice.Address2 = string.Empty;
                objdevice.City = x.Site.City;
                objdevice.State = x.Site.State;
                objdevice.Zip = x.Site.Zip;
                objlstDevice.Add(objdevice);
            });

            objLstDevices = null;

            return Json(objlstDevice.Select(c => new { Id = c.Id, Name = c.Name, Location = c.SiteId, SiteName = c.SiteName, Address1 = c.Address1, Address2 = c.Address2, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllDeviceSiteDetailsbyParentType(int id)
        {
            AlertDetailViewModel viewmodel;
            DeviceListDashboardViewModel DLviewmodel = new DeviceListDashboardViewModel();
            if (id > 0)
            {
                ViewBag.NotesEnabled = true;

                viewmodel = new AlertDetailViewModel(_alertService.GetAlertStatusByPK(id));

                viewmodel.MonitoredDevicesCount =
                    _userService.GetMonitoredDevicesCountBySite(_currentUserProvider.CurrentUser.Id,
                                                                viewmodel.SiteId).ToString();
                
                viewmodel.MonitoredDevicesAlarmsCount =
                    _alertService.GetConfiguredAlarmsBySiteAndCurrentUserCount(viewmodel.SiteId).ToString();

                Site objSite = new Site();
                objSite = _siteService.Get(viewmodel.SiteId);
                //viewmodel.ContactPersonName = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactName;
                //viewmodel.ContactPersonEmail = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactEmail;
                //viewmodel.ContactPersonPhone = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactOffice;
                // DeviceListDashboardViewModel restManager = new DeviceListDashboardViewModel();
                
                //DLviewmodel.ContactPersonName = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactName;
                DLviewmodel.ContactPersonName = objSite.ContactName;
                //DLviewmodel.ContactPersonEmail = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactEmail;
                DLviewmodel.ContactPersonEmail = objSite.ContactEmail;
                //DLviewmodel.ContactPersonPhone = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactOffice;
                DLviewmodel.ContactPersonPhone = objSite.ContactNumber;
                DLviewmodel.Location = viewmodel.SiteName;
                DLviewmodel.LocationAddress = viewmodel.Address;
                ViewBag.AlertDetails = viewmodel;
            }
            else
            {
                ViewBag.NotesEnabled = false;
                viewmodel = GetInitializedViewModel();
            }

          //  return Json(viewmodel, JsonRequestBehavior.AllowGet);
            return Json(DLviewmodel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActiveAlertsbyDeviceId(int deviceId)
        {
            var ResultSet = _alertService.GetAlertDetailsByDeviceId(deviceId);
            IList<DeviceListDashboardViewModel> lstDeviceListDashboardViewModel = new List<DeviceListDashboardViewModel>();
            lstDeviceListDashboardViewModel = PopulateAlerts(ResultSet);
            return Json(lstDeviceListDashboardViewModel, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetALLActiveAlerts(String parentType)
        {
            Session["parentType"] = parentType;
            var ResultSet = _alertService.GetAllAlertDetailsByParentType(parentType);
            IList<DeviceListDashboardViewModel> lstDeviceListDashboardViewModel = new List<DeviceListDashboardViewModel>();
            lstDeviceListDashboardViewModel = PopulateAlerts(ResultSet);
            return Json(lstDeviceListDashboardViewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAlertStatus(int currentRow, string deviceType, string deviceId)
        {
            var ResultSet = _alertService.GetAlertDetailsByDeviceType(deviceType);
            var alertStatusResultSet = ResultSet.Where(x => x.Id == currentRow).Select(y => y);
            if (alertStatusResultSet != null)
            {
                alertStatusResultSet.First().Alarm.Ack = true; // need to change to true
                // AlertInfo objAlertInfo = _alertService.Get(alertStatusResultSet.First().Alarm.Id);
                _alertService.AcknowledgeAlert(currentRow);
            }

           // Rebind Grid
            var Results = new List<AlertStatus>();
            if (!deviceType.Equals((string)Session["parentType"]))
            {
                var ResultSet1 = _alertService.GetAllAlertDetailsByParentType((string)Session["parentType"]);
                IList<DeviceListDashboardViewModel> lstDeviceListDashboardViewModel1 = new List<DeviceListDashboardViewModel>();
                lstDeviceListDashboardViewModel1 = PopulateAlerts(ResultSet1);
                return Json(lstDeviceListDashboardViewModel1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (deviceType.Equals("Device List"))
                {
                    Results = _alertService.GetAllAlertDetails().ToList();
                }
                else
                {
                    Results = _alertService.GetAlertDetailsByDeviceId(Convert.ToInt32(deviceId)).ToList();
                }
            }
            IList<DeviceListDashboardViewModel> lstDeviceListDashboardViewModel = new List<DeviceListDashboardViewModel>();
            lstDeviceListDashboardViewModel = PopulateAlerts(Results);
            return Json(lstDeviceListDashboardViewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPreviouslyAcknoledgedAlerts(int DeviceId)
        {
            var ResultSet = _alertService.getPreviouslyAcknoledgedAlets(DeviceId);
            // return Json(ResultSet.ToList(), JsonRequestBehavior.AllowGet);
            IList<ResolvedAlertListDashboardViewModel> lstResolvedAlertListDashboardViewModel = new List<ResolvedAlertListDashboardViewModel>();
            ResolvedAlertListDashboardViewModel objResolvedAlertListDashboardViewModel = new ResolvedAlertListDashboardViewModel();
            lstResolvedAlertListDashboardViewModel = ResultSet.Select(x => new ResolvedAlertListDashboardViewModel(x)).ToList(); 
                          
            //lstResolvedAlertListDashboardViewModel = (ResultSet,
            return PartialView("_PreviouslyAcknoledgedAlerts", lstResolvedAlertListDashboardViewModel);
        }

        private IList<DeviceListDashboardViewModel> PopulateAlerts(IList<AlertStatus> lstAlertStatus)
        {
            IList<DeviceListDashboardViewModel> lstDeviceListDashboardViewModel = new List<DeviceListDashboardViewModel>();
            string ElementIdentifier = string.Empty;
            string cameraName = string.Empty;
            lstAlertStatus.Where(y => y.LastAlertTimeStamp.HasValue).Select(z => z).ToList().ForEach(x =>
            {
                if ((x.IsAcknowledged != true && x.IsOk != true) || (x.IsAcknowledged != false && x.IsOk != true))
                {
                    Device objDevice = x.Device;
                    ElementIdentifier = x.ElementIdentifier;
                    if (objDevice != null && x.Alarm.AlarmType == AlarmType.VideoLoss)
                    {
                        Diebold.Domain.Entities.Dvr objDvr = (Diebold.Domain.Entities.Dvr)objDevice;
                        if (objDvr != null)
                        {
                            if (ElementIdentifier != null)
                                cameraName = objDvr.Cameras.Where(y => y.Channel.Equals(ElementIdentifier)).FirstOrDefault().Name;
                            else
                                cameraName = string.Empty;
                        }
                        lstDeviceListDashboardViewModel.Add(new DeviceListDashboardViewModel
                        {
                            Id = x.Id,
                            DeviceName = x.Device.Name,
                            AlertName = x.Device.Name + ": " + x.Alarm.AlarmType + ": " + cameraName,
                            // AlertName = x.Device.Name + ": " + x.Alarm.AlarmType,
                            IsDeviceOk = x.IsOk,                            
                            Ack = x.Alarm.Ack.ToString(),
                            LastAlert = x.LastAlertTimeStamp,
                            DeviceId = x.Device.Id,
                            AlarmConfigId = x.Alarm.Id,
                            IsAcknowledged = x.IsAcknowledged
                        });                      
                    }
                    else if ((x.Alarm.AlarmType == AlarmType.DoorHeld) || (x.Alarm.AlarmType == AlarmType.DoorForced) || (x.Alarm.AlarmType == AlarmType.ZoneBypass) ||
                             (x.Alarm.AlarmType == AlarmType.ZoneTrouble) || (x.Alarm.AlarmType == AlarmType.ZoneAlarm) || (x.Alarm.AlarmType == AlarmType.AreaArmed) || (x.Alarm.AlarmType == AlarmType.AreaDisarmed))
                    {
                        lstDeviceListDashboardViewModel.Add(new DeviceListDashboardViewModel
                        {
                            Id = x.Id,
                            DeviceName = x.Device.Name,
                            AlertName = x.Device.Name + ": " + x.Alarm.AlarmType + ": " + x.ElementIdentifier,
                            IsDeviceOk = x.IsOk,
                            Ack = x.Alarm.Ack.ToString(),
                            LastAlert = x.LastAlertTimeStamp,
                            DeviceId = x.Device.Id,
                            AlarmConfigId = x.Alarm.Id,
                            IsAcknowledged = x.IsAcknowledged
                        });
                        
                    }
                    else if (x.Alarm.AlarmType != AlarmType.VideoLoss && x.Alarm.AlarmType != AlarmType.DoorHeld && x.Alarm.AlarmType != AlarmType.DoorForced && x.Alarm.AlarmType != AlarmType.ZoneBypass && x.Alarm.AlarmType != AlarmType.ZoneTrouble && x.Alarm.AlarmType != AlarmType.ZoneAlarm && x.Alarm.AlarmType != AlarmType.AreaArmed && x.Alarm.AlarmType != AlarmType.AreaDisarmed)
                    {
                        lstDeviceListDashboardViewModel.Add(new DeviceListDashboardViewModel
                        {
                            Id = x.Id,
                            DeviceName = x.Device.Name,
                            AlertName = x.Device.Name + ": " + x.Alarm.AlarmType,
                            IsDeviceOk = x.IsOk,                            
                            Ack = x.Alarm.Ack.ToString(),
                            LastAlert = x.LastAlertTimeStamp,
                            DeviceId = x.Device.Id,
                            AlarmConfigId = x.Alarm.Id,
                            IsAcknowledged = x.IsAcknowledged
                        });                        
                    }
                }                
                else if ((x.IsAcknowledged == false && x.IsOk == true))
                {
                    if (x.Alarm.Ack == true)
                    {
                        Device objDevice = x.Device;
                        ElementIdentifier = x.ElementIdentifier;
                        if (objDevice != null && x.Alarm.AlarmType == AlarmType.VideoLoss)
                        {
                            Diebold.Domain.Entities.Dvr objDvr = (Diebold.Domain.Entities.Dvr)objDevice;
                            if (objDvr != null)
                            {
                               if(ElementIdentifier!=null)
                                cameraName = objDvr.Cameras.Where(y => y.Channel.Equals(ElementIdentifier)).FirstOrDefault().Name;
                               else
                                   cameraName = string.Empty;
                            }
                            lstDeviceListDashboardViewModel.Add(new DeviceListDashboardViewModel
                            {
                                Id = x.Id,
                                DeviceName = x.Device.Name,
                                AlertName = x.Device.Name + ": " + x.Alarm.AlarmType + ": " + cameraName,
                                // AlertName = x.Device.Name + ": " + x.Alarm.AlarmType,
                                IsDeviceOk = x.IsOk,                                
                                Ack = x.Alarm.Ack.ToString(),
                                LastAlert = x.LastAlertTimeStamp,
                                DeviceId = x.Device.Id,
                                AlarmConfigId = x.Alarm.Id,
                                IsAcknowledged = x.IsAcknowledged
                            });

                        }
                        else if ((x.Alarm.AlarmType == AlarmType.DoorHeld) || (x.Alarm.AlarmType == AlarmType.DoorForced) || (x.Alarm.AlarmType == AlarmType.ZoneBypass) || (x.Alarm.AlarmType == AlarmType.ZoneTrouble) || (x.Alarm.AlarmType == AlarmType.ZoneAlarm) || (x.Alarm.AlarmType == AlarmType.AreaArmed) || (x.Alarm.AlarmType == AlarmType.AreaDisarmed))
                        {
                            
                            lstDeviceListDashboardViewModel.Add(new DeviceListDashboardViewModel
                            {
                                Id = x.Id,
                                DeviceName = x.Device.Name,
                                AlertName = x.Device.Name + ": " + x.Alarm.AlarmType + ": " + x.ElementIdentifier,
                                IsDeviceOk = x.IsOk,
                                Ack = x.Alarm.Ack.ToString(),
                                LastAlert = x.LastAlertTimeStamp,
                                DeviceId = x.Device.Id,
                                AlarmConfigId = x.Alarm.Id,
                                IsAcknowledged = x.IsAcknowledged
                            });
                            
                        }
                        else if (x.Alarm.AlarmType != AlarmType.VideoLoss && x.Alarm.AlarmType != AlarmType.DoorHeld && x.Alarm.AlarmType != AlarmType.DoorForced && x.Alarm.AlarmType != AlarmType.ZoneBypass && x.Alarm.AlarmType != AlarmType.ZoneTrouble && x.Alarm.AlarmType != AlarmType.ZoneAlarm && x.Alarm.AlarmType != AlarmType.AreaArmed && x.Alarm.AlarmType != AlarmType.AreaDisarmed)
                        {
                            lstDeviceListDashboardViewModel.Add(new DeviceListDashboardViewModel
                            {

                                Id = x.Id,
                                DeviceName = x.Device.Name,
                                AlertName = x.Device.Name + ": " + x.Alarm.AlarmType,
                                IsDeviceOk = x.IsOk,                                
                                Ack = x.Alarm.Ack.ToString(),
                                LastAlert = x.LastAlertTimeStamp,
                                DeviceId = x.Device.Id,
                                AlarmConfigId = x.Alarm.Id,
                                IsAcknowledged = x.IsAcknowledged
                            });
                        }
                    }
                }
            });
            lstDeviceListDashboardViewModel.ToList().ForEach(x =>
            {
                if ((x.IsAcknowledged == false && x.IsDeviceOk == false) || (x.IsAcknowledged == true && x.IsDeviceOk == false))
                {
                    x.Status = "Alert";
                }
                else if (x.IsAcknowledged == false && x.IsDeviceOk == true)
                {
                    x.Status = "OK";
                }
                if (x.LastAlert.HasValue == true)
                {
                    x.LastAlertS = x.LastAlert.Value.ToString("MM-dd-yyyy HH:mm:ss");
                }
            });
            return lstDeviceListDashboardViewModel;
        }
    }
}
