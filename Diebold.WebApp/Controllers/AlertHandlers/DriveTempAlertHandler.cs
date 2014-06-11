using System;
using Diebold.Services.Contracts;

namespace Diebold.WebApp.Controllers.AlertHandlers
{
    class DriveTempAlertHandler : MultipleAlertHandler
    {
        public DriveTempAlertHandler(IDvrService deviceService, IAlarmConfigurationService alarmService, IAlertService alertService, INotificationService notificationService) : base(deviceService, alarmService, alertService, notificationService)
        {
        }

        public override bool IsIgnoredValue(string value)
        {
            return Convert.ToInt32(value) == 0;
        }

        public override bool  SatisfiesCapabilityRule(string element)
        {
            return element != "0";
        }
    }
}