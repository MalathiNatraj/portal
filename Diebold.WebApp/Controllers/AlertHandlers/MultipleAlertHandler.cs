using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;

namespace Diebold.WebApp.Controllers.AlertHandlers
{
    public class MultipleAlertHandler : BaseAlertHandler
    {
        public IDvrService _deviceService { get; set; }
        public IAlarmConfigurationService _alarmService { get; set; }
        public IAlertService _alertService { get; set; }

        public MultipleAlertHandler() { }

        public MultipleAlertHandler(IDvrService deviceService, IAlarmConfigurationService alarmService, IAlertService alertService, INotificationService notificationService)
            : base(notificationService)
        {
            _deviceService = deviceService;
            _alarmService = alarmService;
            _alertService = alertService;
        }

        protected static string GetValue(object value, int elementIdentifier)
        {
            var val = String.Empty;

            if (value is IDictionary)
            {
                val = ((IDictionary<string, Object>)value).Where(v => v.Key == elementIdentifier.ToString()).ToList().Single().Value.ToString();
            }
            if (value is IList)
            {
                val = ((IList)value)[elementIdentifier].ToString();
            }
            else if (value is string)
                val = value.ToString();

            return val;
        }
        
        public override List<AlertInfo> HandleAlert(Alert alert)
        {
            try
            {

               // var device = _deviceService.GetByExternalId(alert.DeviceId);
                var device = _deviceService.Get(Convert.ToInt32(alert.DeviceId));
                var type = (AlarmType) Enum.Parse(typeof (AlarmType), alert.AlarmName, true);
                var alarm = _alarmService.GetByDeviceAndCapability(device.Id, type);
                var dateOccur = DateTime.Parse(alert.AlertDate, null, DateTimeStyles.RoundtripKind);
                var relationalOperator =
                    (AlarmOperator) Enum.Parse(typeof (AlarmOperator), alert.RelationalOperator, true);
                
                IList<string> elements = new List<string>();
                //if (type == AlarmType.RaidStatus)
                //{
                  //  elements.Add(alert.Value.ToString());
                //}
                //else
                //{
                  //  if (alert.Value != null && alert.Value.Count() > 0)
                    //    elements = alert.Value.ToString().Split(',');
                //}
                if (type == AlarmType.SMART)
                    elements.Add(alert.AlarmName);
                else if (type == AlarmType.DriveTemperature)
                    elements.Add((Convert.ToInt32(alert.Threshold) + 1).ToString());
                else if (type == AlarmType.RaidStatus)
                    elements.Add(alert.AlarmName);
                else if (type == AlarmType.VideoLoss)
                    elements.Add(alert.AlarmName);

                //Currently active alerts
                var pendingActiveAlerts = _alertService.GetPendingAlertsByDevice(device.Id, alarm.Id).ToList();

                return(GenerateAlerts(alert, alarm, device, dateOccur, relationalOperator, elements, pendingActiveAlerts)).ToList();
            }
            catch (Exception e)
            {
                _logger.Debug(string.Format("Error Handling Multiple Alert: {0}", e.ToString()));

                throw;
            }
        }

        protected static AlertInfo GenerateAlert(Device device, AlarmConfiguration alarm, DateTime dateOccur, bool isDeviceOk, string identifier,
                                               object alertValue, bool satisfiesAlertCondition)
        {
            var alert = new AlertInfo()
            {
                Device = device,
                Alarm = alarm,
                DateOccur = dateOccur,
                IsDeviceOk = isDeviceOk,
                ElementIdentifier = identifier,
                Value = GetValue(alertValue, int.Parse(identifier)),
                SatisfiesAlertCondition = satisfiesAlertCondition
            };

            return alert;
        }

        private IEnumerable<AlertInfo> GenerateAlerts(Alert alert, AlarmConfiguration alarm, Device device, DateTime dateOccur,
                                                      AlarmOperator relationalOperator, IList<string> elements, IEnumerable<AlertStatus> pendingActiveAlerts)
        {
            var alertList = new List<AlertInfo>();

            for (var i = 0; i < elements.Count(); i++)
            {
                if (IsIgnoredValue(elements[i])) continue;

                var satisfiesAlertCondition = false;

                //Drive is OK if it doesn't satisfies rules conditions.
                var isDriveOk = !SatisfiesRule(elements[i], alert.Threshold, relationalOperator);
                //Get value of the position
                var alertValue = GetValue(elements, i);

                //...and drive is not ok...
                if (!isDriveOk)
                {
                    //Create local alert!
                    alertList.Add(GenerateAlert(device, alarm, dateOccur, false, (i + 1).ToString(), alertValue, true));
                }
                else
                {
                    if (pendingActiveAlerts != null)
                    {
                        var isCurrentlyWithActiveAlert = pendingActiveAlerts.Any(x => x.ElementIdentifier == (i + 1).ToString());

                        //Create local alert to correct status!
                        if (isCurrentlyWithActiveAlert)
                        {
                            satisfiesAlertCondition = true;
                        }
                    }

                    alertList.Add(GenerateAlert(device, alarm, dateOccur, true, (i + 1).ToString(), alertValue, satisfiesAlertCondition));
                }
            }

            return alertList;
        }
        
        public override void CreateAlerts(IList<AlertInfo> alertList)
        {
            if (alertList.Any())
            {
                IList<AlertInfo> NewAlertList = new List<AlertInfo>();
                alertList.ToList().ForEach(x =>
                {                    
                    var objLastAlertInfo = _alertService.GetLastAlertInfoByDeviceAndIdentifier(x.Device.Id, x.Alarm.Id, x.ElementIdentifier);

                    if (objLastAlertInfo == null)
                    {
                        NewAlertList.Add(x);
                    }
                    else if (objLastAlertInfo != null && x.DateOccur > objLastAlertInfo.DateOccur)
                    {
                        NewAlertList.Add(x);
                    }
                   
                });

                if (NewAlertList != null && NewAlertList.Count > 0)
                {
                    _alertService.CreateAlert(NewAlertList);
                }
            }
        }

        public override void Notify(List<AlertInfo> alertList)
        {
            foreach (var alertInfo in alertList)
            {
                if (_alertService.ValidateSendNotification(alertInfo))
                    SendNotification(alertInfo);
            }
        }
    }
}