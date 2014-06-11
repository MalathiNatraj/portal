using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;

namespace Diebold.WebApp.Controllers.AlertHandlers
{
    public class VideoLossAlertHandler : MultipleAlertHandler
    {
        public VideoLossAlertHandler(IDvrService deviceService, IAlarmConfigurationService alarmService, IAlertService alertService, INotificationService notificationService)
            : base(deviceService, alarmService, alertService, notificationService)
        {
        }

        public override List<AlertInfo> HandleAlert(Alert alert)
        {
            // var device = _deviceService.GetByExternalId(alert.DeviceId);
            var alertList = new List<AlertInfo>();
            var device = _deviceService.Get(Convert.ToInt32(alert.DeviceId));
            var type = (AlarmType)Enum.Parse(typeof(AlarmType), alert.AlarmName, true);
            var alarm = _alarmService.GetByDeviceAndCapability(device.Id, type);
            var dateOccur = DateTime.Parse(alert.AlertDate, null, DateTimeStyles.RoundtripKind);
            var aplitem = alert.Report.payload.SparkDvrReport.properties.propertyList.Where(x => x.name.ToLower().Equals("videoloss"));
            if (device.Cameras != null)
            {
                for (int i = 0; i < device.Cameras.Count(); i++)
                {
                    if (alert.Report.payload.SparkDvrReport.properties.propertyList != null)
                    {
                        if (aplitem.FirstOrDefault().propertyItem[i] != null)
                        {
                            alertList.Add(GenerateAlert(device, alarm, dateOccur, false, device.Cameras[i].Channel, aplitem.FirstOrDefault().propertyItem[i].ToLower(), true));
                        }
                    }
                }
            }

            //Get alerts that are currently with video loss 
            var alertStatusWithVideoLoss = _alertService.GetPendingAlertsByDevice(device.Id, alarm.Id).ToList();

            //Active Cameras
            // var activeCameras = ((IDictionary<string, object>)alert.Threshold).Where(x => x.Value.ToString().ToLower() == "vl");

            // var alertList = new List<AlertInfo>();

            //Adds alerts if it is video loss
            //alertList.AddRange((from value in ((IDictionary<string, object>) alert.Value).Where(x => x.Value.ToString().ToLower() == "vl")
            //                    let isVL = activeCameras.Where(x => x.Key == value.Key).Any()
            //                    where isVL
            //                    select GenerateAlert(device, alarm, dateOccur, false, value.Key, alert.Value, true)).ToList());
            
            //Adds alerts if it is not video loss
            //alertList.AddRange((from value in ((IDictionary<string, object>) alert.Value).Where(x => x.Value.ToString().ToLower() == "vl") 
            //                    let isCurrentlyWithVL = alertStatusWithVideoLoss.Where(x => x.ElementIdentifier == value.Key).Any()
            //                    where isCurrentlyWithVL
            //                    select GenerateAlert(device, alarm, dateOccur, true, value.Key, alert.Value, true)).ToList());
            // alertList.Add(GenerateAlert(device, alarm, dateOccur, false, "1", "vl", true));
            return alertList;
        }

        public override bool SatisfiesCapabilityRule(string element)
        {
            return (element.ToLower() == "true" || element.ToLower() == "false");
        }
    }
}