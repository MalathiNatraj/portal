using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.WebApp.Controllers.AlertHandlers
{
    public class SingleAlertHandler : BaseAlertHandler
    {
        public IDvrService _deviceService { get; set; }
        public IAlarmConfigurationService _alarmService { get; set; }
        public IAlertService _alertService { get; set; }
        private IAccessService _accessService { get; set; }
        private IIntrusionService _intrusionService { get; set; }

        public SingleAlertHandler(IDvrService deviceService, IAlarmConfigurationService alarmService, IAlertService alertService, INotificationService notificationService, IAccessService accessService,
                              IIntrusionService intrusionService)
            : base(notificationService)
        {
            _deviceService = deviceService;
            _alarmService = alarmService;
            _alertService = alertService;
            _accessService = accessService;
            _intrusionService = intrusionService;
        }

        public override List<AlertInfo> HandleAlert(Alert alert)
        {
            _logger.Debug("Entered Handle Alert Method");
            _logger.Debug("Get method started for device Id : " + alert.DeviceId);
            var device = _deviceService.Get(Convert.ToInt32(alert.DeviceId));
            _logger.Debug("Get method completed");
            var type = (AlarmType)Enum.Parse(typeof(AlarmType), alert.AlarmName, true);
            _logger.Debug("Type : " + type);  
            var alarm = _alarmService.GetByDeviceAndCapability(device.Id, type);
            _logger.Debug("Alarm : " + alarm);  
            var dateOccur = DateTime.Parse(alert.AlertDate, null, DateTimeStyles.RoundtripKind);
            _logger.Debug("date Occour : " + dateOccur);  
            var isDeviceOk = !alert.AlertActive;
            _logger.Debug("Is Device Ok : " + isDeviceOk);
            string alertValue = string.Empty;
            if (alert.Value != null && alert.Value.Count > 1)
            {
                var alertsList = new List<AlertInfo>();
                foreach (var item in alert.Value)
                {
                    if (item != null)
                    {
                        if (!string.IsNullOrEmpty(item.isNotRecording))
                        {
                            alertValue = item.isNotRecording;
                        }
                        else if (!string.IsNullOrEmpty(item.daysRecorded))
                        {
                            alertValue = item.daysRecorded;
                        }
                        else if (!string.IsNullOrEmpty(item.SMART))
                        {
                            alertValue = item.SMART;
                        }
                        else if (!string.IsNullOrEmpty(item.raidStatus))
                        {
                            alertValue = item.raidStatus;
                        }
                        else if (!string.IsNullOrEmpty(item.videoLoss))
                        {
                            alertValue = item.videoLoss;
                        }
                        else if (!string.IsNullOrEmpty(item.networkDown))
                        {
                            alertValue = item.networkDown;
                        }
                        else if (!string.IsNullOrEmpty(item.areaarmed))
                        {
                            alertValue = item.areaarmed;
                        }
                        else if (!string.IsNullOrEmpty(item.areadisarmed))
                        {
                            alertValue = item.areadisarmed;
                        }
                        else if (!string.IsNullOrEmpty(item.zonealarm))
                        {
                            alertValue = item.zonealarm;
                        }
                        else if (!string.IsNullOrEmpty(item.zonetrouble))
                        {
                            alertValue = item.zonetrouble;
                        }
                        else if (!string.IsNullOrEmpty(item.zonebypass))
                        {
                            alertValue = item.zonebypass;
                        }
                        else if (!string.IsNullOrEmpty(item.doorforced))
                        {
                            alertValue = item.doorforced;
                        }
                        else if (!string.IsNullOrEmpty(item.doorheld))
                        {
                            alertValue = item.doorheld;
                        }
                        else if (!string.IsNullOrEmpty(item.driveTemp))
                        {
                            alertValue = item.driveTemp;
                        }
                        else if (!string.IsNullOrEmpty(item.doorStatus))
                        {
                            alertValue = item.doorStatus;
                        }
                        else if (!string.IsNullOrEmpty(item.zoneStatus))
                        {
                            alertValue = item.zoneStatus;
                        }
                        else if (!string.IsNullOrEmpty(item.armed))
                        {
                            alertValue = item.armed;
                        }
                        else
                        {
                            alertValue = null;
                        }
                        _logger.Debug("alertValue : " + alertValue);

                        _logger.Debug("Typecast of alert value to string started");
                        if (!IsIgnoredValue((string)alertValue))
                        {
                            alertsList.Add(new AlertInfo { Device = device, Alarm = alarm, DateOccur = dateOccur, IsDeviceOk = isDeviceOk, Value = alertValue.ToString() });
                        }
                        _logger.Debug("Typecast of alert value to string completed");
                        _logger.Debug("Completed Handle Alert Method");
                    }
                }
                return alertsList;
            }
            else
            {
                if (!string.IsNullOrEmpty(alert.Value.First().isNotRecording))
                {
                    alertValue = alert.Value.First().isNotRecording;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().daysRecorded))
                {
                    alertValue = alert.Value.First().daysRecorded;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().SMART))
                {
                    alertValue = alert.Value.First().SMART;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().raidStatus))
                {
                    alertValue = alert.Value.First().raidStatus;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().videoLoss))
                {
                    alertValue = alert.Value.First().videoLoss;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().networkDown))
                {
                    alertValue = alert.Value.First().networkDown;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().areaarmed))
                {
                    alertValue = alert.Value.First().areaarmed;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().areadisarmed))
                {
                    alertValue = alert.Value.First().areadisarmed;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().zonealarm))
                {
                    alertValue = alert.Value.First().zonealarm;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().zonetrouble))
                {
                    alertValue = alert.Value.First().zonetrouble;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().zonebypass))
                {
                    alertValue = alert.Value.First().zonebypass;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().doorforced))
                {
                    alertValue = alert.Value.First().doorforced;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().doorheld))
                {
                    alertValue = alert.Value.First().doorheld;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().driveTemp))
                {
                    alertValue = alert.Value.First().driveTemp;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().doorStatus))
                {
                    alertValue = alert.Value.First().doorStatus;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().zoneStatus))
                {
                    alertValue = alert.Value.First().zoneStatus;
                }
                else if (!string.IsNullOrEmpty(alert.Value.First().armed))
                {
                    alertValue = alert.Value.First().armed;
                }
                _logger.Debug("alertValue : " + alertValue);
                var alertsList = new List<AlertInfo>();
                _logger.Debug("Typecast of alert value to string started");
                if (!IsIgnoredValue((string)alertValue))
                {
                    alertsList.Add(new AlertInfo { Device = device, Alarm = alarm, DateOccur = dateOccur, IsDeviceOk = isDeviceOk, Value = alertValue.ToString() });
                }
                _logger.Debug("Typecast of alert value to string completed");
                _logger.Debug("Completed Handle Alert Method");
                return alertsList;
            }
           
        }

        public override void CreateAlerts(IList<AlertInfo> alertList)
        {
            _logger.Debug("CreateAlert method started");
            Diebold.Domain.Entities.Dvr objDvr = (Diebold.Domain.Entities.Dvr)alertList[0].Device;
            IList<AlarmConfiguration> lstAllAlarmConfigByDeviceId = _alarmService.GetALLAlarmConfigByDeviceID(objDvr.Id);
            IDictionary<AlarmType, AlarmConfiguration> dctAlarmConfig = new Dictionary<AlarmType, AlarmConfiguration>();
            lstAllAlarmConfigByDeviceId.ToList().ForEach(x => { dctAlarmConfig.Add(x.AlarmType.Value, x); });
            Access objAccessResult = null;
            Intrusion objIntrusionResult = null;
            StatusPlatformDTO objstatusResult = null;
            if (objDvr.DeviceType.Equals(DeviceType.eData300) || objDvr.DeviceType.Equals(DeviceType.eData524) || objDvr.DeviceType.Equals(DeviceType.dmpXR100Access) || objDvr.DeviceType.Equals(DeviceType.dmpXR500Access))
            {
                // Need to call the platform
                _logger.Debug("GetPlatformAccessStatus started with Device Id : " + objDvr.Id);
                objAccessResult = _accessService.GetPlatformAccessStatus(objDvr.Id);
                _logger.Debug("GetPlatformAccessStatus completed");
            }
            else if (objDvr.DeviceType.Equals(DeviceType.dmpXR100) || objDvr.DeviceType.Equals(DeviceType.dmpXR500) || objDvr.DeviceType.Equals(DeviceType.bosch_D9412GV4) || objDvr.DeviceType.Equals(DeviceType.videofied01))
            {
                // Need to call the platform
                _logger.Debug("GetPlatformIntrusionDetails started with Device Id : " + objDvr.Id);
                objIntrusionResult = _intrusionService.GetPlatformIntrusionDetails(objDvr.Id);
                _logger.Debug("GetPlatformIntrusionDetails Completed");
            }
            else if (objDvr.DeviceType.Equals(DeviceType.VerintEdgeVr200) || objDvr.DeviceType.Equals(DeviceType.Costar111) || objDvr.DeviceType.Equals(DeviceType.ipConfigure530))
            {
                _logger.Debug("GetPlatformDVRDetails started with Device Id : " + objDvr.Id);
                // objstatusResult = _deviceService.GetPlatformLiveStatus(objDvr.Id, objDvr.ExternalDeviceId, false, false);
                _logger.Debug("GetPlatformDVRDetails Completed");
            }
            
            List<AlertInfo> lstAlertInfor = new List<AlertInfo>();
            List<Zone> lstZones = new List<Zone>();
            for (int i = 0; i < alertList.Count(); i++)
            {
                if (alertList[i] != null)
                {
                    bool IsLatestAlertInfo = false;
                    _logger.Debug("GetLastAlertInfoByDeviceAndIdentifier started");
                    var objLastAlertInfo = _alertService.GetLastAlertInfoByDeviceAndIdentifier(alertList[i].Device.Id, alertList[i].Alarm.Id, alertList[i].ElementIdentifier);
                    _logger.Debug("GetLastAlertInfoByDeviceAndIdentifier completed");
                    _logger.Debug("alertInfo.DateOccur" + alertList[i].DateOccur);
                    if (objLastAlertInfo != null)
                    {
                        _logger.Debug("objLastAlertInfo.DateOccur" + objLastAlertInfo.DateOccur);
                    }
                    if (objLastAlertInfo == null)
                    {
                        _logger.Debug("objLastAlertInfo is null");
                        IsLatestAlertInfo = true;
                    }
                    else if (objLastAlertInfo != null && alertList[i].DateOccur > objLastAlertInfo.DateOccur)
                    {
                        _logger.Debug("objLastAlertInfo is not null and date occour is greater than last alert info date occour");
                        IsLatestAlertInfo = true;
                    }

                    if (IsLatestAlertInfo)
                    {
                        _logger.Debug("Create method started");
                        _logger.Debug("Device Type: " + objDvr.DeviceType);
                        if (objDvr.DeviceType.Equals(DeviceType.eData300) || objDvr.DeviceType.Equals(DeviceType.eData524) || objDvr.DeviceType.Equals(DeviceType.dmpXR100Access) || objDvr.DeviceType.Equals(DeviceType.dmpXR500Access))
                        {
                            _logger.Debug("Device Type is Access Control");
                            if (alertList[i].Alarm.AlarmType.Equals(AlarmType.NetworkDown))
                            {
                                //AlertInfo alertnetworkDown = new AlertInfo();
                                //alertnetworkDown.Device = alertList[i].Device;
                                //alertnetworkDown.Alarm = alertList[i].Alarm;
                                //alertnetworkDown.DateOccur = alertList[i].DateOccur;
                                //alertnetworkDown.GroupId = alertList[i].GroupId;
                                //alertnetworkDown.IsDeviceOk = alertList[i].IsDeviceOk;
                                //alertnetworkDown.Value = alertList[i].Value;
                                //alertnetworkDown.SatisfiesAlertCondition = alertList[i].SatisfiesAlertCondition;
                                //alertnetworkDown.Id = alertList[i].Id;
                                alertList[i].ElementIdentifier = "Network Down";
                                lstAlertInfor.Add(alertList[i]);
                            }
                            else
                            {
                                _logger.Debug("Started looping through the Door List");
                                Door door = objAccessResult.DoorList[0]; // Get the first door

                                string doorStatus = door.DoorStatus.ToLower().ToString();
                                _logger.Debug("doorStatus is : " + doorStatus);

                                switch (doorStatus)
                                {
                                    case "held":
                                        // var alarmDoorHeld = _alarmService.GetByDeviceAndCapability(objDvr.Id, AlarmType.DoorHeld);
                                        var alarmDoorHeld = dctAlarmConfig[AlarmType.DoorHeld];
                                        alertList[i].Alarm = alarmDoorHeld;
                                        alertList[i].ElementIdentifier = objDvr.Site.Name + ": " + door.DoorName;
                                        _logger.Debug("Element Identifier Name is : " + alertList[i].ElementIdentifier);
                                        lstAlertInfor.Add(alertList[i]);
                                        break;
                                    case "forced":
                                        alertList[i].ElementIdentifier = objDvr.Site.Name + ": " + door.DoorName;
                                        // var AlamConfigforced = _alarmService.GetByDeviceAndCapability(objDvr.Id, AlarmType.DoorForced);
                                        var AlamConfigforced =  dctAlarmConfig[AlarmType.DoorForced];
                                        alertList[i].Alarm = AlamConfigforced;
                                        _logger.Debug("Element Identifier Name is : " + alertList[i].ElementIdentifier);
                                        lstAlertInfor.Add(alertList[i]);
                                        break;
                                    default:
                                        alertList[i].ElementIdentifier = objDvr.Site.Name + ": " + door.DoorName;
                                        lstAlertInfor.Add(alertList[i]);
                                        break;
                                }

                            }
                        }
                        else if (objDvr.DeviceType.Equals(DeviceType.dmpXR100) || objDvr.DeviceType.Equals(DeviceType.dmpXR500) || objDvr.DeviceType.Equals(DeviceType.bosch_D9412GV4) || objDvr.DeviceType.Equals(DeviceType.videofied01))
                        {
                            _logger.Debug("Device Type is Intrusion Control");
                            if (objIntrusionResult.AreaList != null && objIntrusionResult.AreaList.Count() > 0)
                            {
                                _logger.Debug("Area List is not null");

                                if (alertList[i].Alarm.AlarmType.Equals(AlarmType.AreaArmed) || alertList[i].Alarm.AlarmType.Equals(AlarmType.AreaDisarmed))
                                {
                                    Area area = objIntrusionResult.AreaList[i];
                                    if (area != null)
                                    {
                                        _logger.Debug("Area Armed status: " + area.Armed);
                                        alertList[i].ElementIdentifier = objDvr.Site.Name + ": " + area.AreaName;

                                        if (area.Armed == false)
                                        {
                                            // var alarmDisarmed = _alarmService.GetByDeviceAndCapability(objDvr.Id, AlarmType.AreaDisarmed);
                                            var alarmDisarmed = dctAlarmConfig[AlarmType.AreaDisarmed];
                                            alertList[i].Alarm = alarmDisarmed;
                                        }
                                        else
                                        {
                                            // var alarmArmed = _alarmService.GetByDeviceAndCapability(objDvr.Id, AlarmType.AreaArmed);
                                            var alarmArmed = dctAlarmConfig[AlarmType.AreaArmed];
                                            alertList[i].Alarm = alarmArmed;
                                        }
                                        _logger.Debug("Element Identifier Name - Area Name is : " + alertList[i].ElementIdentifier);
                                        lstAlertInfor.Add(alertList[i]);
                                    }
                                }
                                else if (alertList[i].Alarm.AlarmType.Equals(AlarmType.ZoneBypass) || alertList[i].Alarm.AlarmType.Equals(AlarmType.ZoneAlarm) || alertList[i].Alarm.AlarmType.Equals(AlarmType.ZoneTrouble))
                                {
                                    foreach (Area area in objIntrusionResult.AreaList)
                                    {
                                        if (area != null)
                                        {
                                            foreach (Zone zone in area.Zones)
                                            {
                                                if (zone != null)
                                                {
                                                    i++;
                                                    lstZones.Add(zone);
                                                }
                                            }
                                        }
                                    }
                                    i--;
                                    _logger.Debug("Looping through Zones completed");
                                }
                                else if (alertList[i].Alarm.AlarmType.Equals(AlarmType.NetworkDown))
                                {
                                        alertList[i].ElementIdentifier = "Network Down";
                                        lstAlertInfor.Add(alertList[i]);
                                    }
                                _logger.Debug("Looping through Area List completed");
                            }
                            else if (alertList[i].Alarm.AlarmType.Equals(AlarmType.NetworkDown)) 
                            {
                                alertList[i].ElementIdentifier = "Network Down";
                                lstAlertInfor.Add(alertList[i]);
                            }
                        }
                        else if (objDvr.DeviceType.Equals(DeviceType.Costar111) || objDvr.DeviceType.Equals(DeviceType.ipConfigure530) || objDvr.DeviceType.Equals(DeviceType.VerintEdgeVr200))
                        {
                            _logger.Debug("Device Type is Video Health Check");
                            // var DVRProperty = objstatusResult.payload.SparkDvrReport.properties.property; temprovarily commented
                            if (alertList[i].Alarm.AlarmType.Equals(AlarmType.NetworkDown))
                            {
                                alertList[i].ElementIdentifier = "Network Down";
                            }
                            lstAlertInfor.Add(alertList[i]);
                        }   
                        _logger.Debug("Create method completed");
                    }
                }
            }
            _logger.Debug("Looping through Door List completed");
            if(lstZones!=null && lstZones.Count() > 0)
            {
                for (int j = 0; j < lstZones.Count();j++)
                {
                    string zoneStatus = lstZones[j].Status.ToLower().ToString();
                    _logger.Debug("zoneStatus is : " + zoneStatus);
                    alertList[j].ElementIdentifier = objDvr.Site.Name + ": " + lstZones[j].Name;
                    _logger.Debug("Element Identifier Name - Zone Name is : " + alertList[j].ElementIdentifier + "Site Name : " + objDvr.Site.Name);
                    dynamic AlamConfig = null;

                    if (zoneStatus.ToLower().Equals("bypassed"))
                    {
                        // AlamConfig = _alarmService.GetByDeviceAndCapability(objDvr.Id, AlarmType.ZoneBypass);
                        AlamConfig = dctAlarmConfig[AlarmType.ZoneBypass];
                    }
                    else if (zoneStatus.ToLower().Equals("trouble"))
                    {
                        // AlamConfig = _alarmService.GetByDeviceAndCapability(objDvr.Id, AlarmType.ZoneTrouble);
                        AlamConfig = dctAlarmConfig[AlarmType.ZoneTrouble];
                    }
                    else if (zoneStatus.ToLower().Equals("alarm"))
                    {
                        // AlamConfig = _alarmService.GetByDeviceAndCapability(objDvr.Id, AlarmType.ZoneAlarm);
                        AlamConfig = dctAlarmConfig[AlarmType.ZoneAlarm];
                    }
                    else
                    {
                        AlamConfig = alertList[j].Alarm;
                    }
                    // var AlamConfig = _alarmService.GetByDeviceAndCapability(objDvr.Id, alertList[i].Alarm.AlarmType.Value);
                    if (AlamConfig != null)
                    {
                        alertList[j].Alarm = AlamConfig;
                        //alertInfoZone.DateOccur = alertList[i].DateOccur;
                        //alertInfoZone.GroupId = alertList[i].GroupId;
                        //alertInfoZone.IsDeviceOk = alertList[i].IsDeviceOk;
                        //alertInfoZone.Value = alertList[i].Value;
                        //alertInfoZone.SatisfiesAlertCondition = alertList[i].SatisfiesAlertCondition;
                        //alertInfoZone.Id = alertList[i].Id;
                        alertList[j].Status = zoneStatus;
                        lstAlertInfor.Add(alertList[j]);
                    }
                    else
                    {
                        _logger.Error("Alarm Config Value is Null");
                    }
                }
            }
            if (lstAlertInfor != null && lstAlertInfor.Count() > 0)
            {
                _logger.Debug("Create Multiple Items for Access Control Started");
                _alertService.CreateMultipleItems(lstAlertInfor);
                _logger.Debug("Create Multiple Items for Access Control Completed");
            }
            else
            {
                _logger.Debug("No Doors were held and hence creation did not take place.");
            }
          
            _logger.Debug("CreateAlert method completed");
        }

        public override void Notify(List<AlertInfo> alertList)
        {
            foreach (var item in alertList)
            {
                if (_alertService.ValidateSendNotification(item))
                    SendNotification(item);
            }
        }
    }
}