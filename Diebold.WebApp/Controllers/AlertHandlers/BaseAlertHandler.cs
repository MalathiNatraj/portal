using System;
using System.Collections.Generic;
using System.Globalization;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Services.Extensions;
using Diebold.WebApp.Infrastructure.Helpers;
using Diebold.WebApp.Models;
using log4net;

namespace Diebold.WebApp.Controllers.AlertHandlers
{
    public abstract class BaseAlertHandler : IAlertHandler
    {
        protected static ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        protected INotificationService _notificationService;

        protected BaseAlertHandler() {}

        protected BaseAlertHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        protected void SendNotification(AlertInfo alertInfo)
        {
            var notification = new Notification
                                   {
                SendToEmail = alertInfo.Alarm.Email,
                SendToEmc = alertInfo.Alarm.Emc,
                SendToLog = alertInfo.Alarm.Log,
                DeviceId = alertInfo.Device.Id,
                DeviceName = alertInfo.Device.Name,
                AlertCleared = alertInfo.IsDeviceOk,

                AlarmName = alertInfo.Alarm.AlarmType.Value.GetDescription() + " " +
                            AlarmHelper.GetAlertDescriptionForAlert((AlarmType)alertInfo.Alarm.AlarmType, alertInfo.ElementIdentifier, (Dvr)alertInfo.Device),

                DateOccur = alertInfo.DateOccur.ToString("MMMM dd, yyyy H:mm", CultureInfo.CreateSpecificCulture("en-US")),
            };

            if (alertInfo.Device.IsDvr)
            {
                var dvr = (Dvr) alertInfo.Device;

                notification.TimeZone = dvr.TimeZone;
                notification.SiteName = dvr.Site.Name;
                notification.SiteAddress1 = dvr.Site.Address1;
                notification.SiteAddress2 = dvr.Site.Address2;
                notification.EmcAccontNumber = dvr.Gateway.EMCId.ToString();
                notification.EmcDevicezone = dvr.ZoneNumber;
            }

            _notificationService.Notify(notification);
        }
        
        public virtual bool IsIgnoredValue(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public virtual bool SatisfiesRule(string value, object thresholdValue, AlarmOperator relationalOperator)
        {
            var element = Convert.ToDecimal(value);
            var threshold = Convert.ToDecimal(thresholdValue);
            
            switch (relationalOperator)
            {
                case AlarmOperator.Equals:
                    {
                        return (element.Equals(threshold));
                    }
                case AlarmOperator.GreaterThan:
                    {
                        return (element > threshold);
                    }
                case AlarmOperator.GreaterThanOrEquals:
                    {
                        return (element >= threshold);
                    }
                case AlarmOperator.LessThan:
                    {
                        return (element < threshold);
                    }
                case AlarmOperator.LessThanOrEquals:
                    {
                        return (element <= threshold);
                    }
                case AlarmOperator.NotEquals:
                    {
                        return (!element.Equals(threshold));
                    }
            }

            return true;
        }

        public virtual bool SatisfiesCapabilityRule(string element)
        {
            return element != string.Empty;
        }

        public abstract List<AlertInfo> HandleAlert(Alert alert);
        public abstract void CreateAlerts(IList<AlertInfo> alertList);
        public abstract void Notify(List<AlertInfo> alertList);
    }
}