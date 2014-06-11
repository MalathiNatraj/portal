using Diebold.Domain.Entities;
using Diebold.Services.Contracts;

namespace Diebold.WebApp.Controllers.AlertHandlers
{
    public class SMARTAlertHandler : MultipleAlertHandler
    {
        public SMARTAlertHandler(IDvrService deviceService, IAlarmConfigurationService alarmService, IAlertService alertService, INotificationService notificationService)
            : base(deviceService, alarmService, alertService, notificationService)
        {
        }
        
        public override bool SatisfiesRule(string element, object threshold, AlarmOperator relationalOperator)
        {
            return element.ToLower() != "passed" && element.ToLower() != "unsupported";
        }

        public override bool SatisfiesCapabilityRule(string element)
        {
            return element != "unsupported";
        }
    }
}