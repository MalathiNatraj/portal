using Diebold.Domain.Entities;
using Diebold.Services.Contracts;

namespace Diebold.WebApp.Controllers.AlertHandlers
{
    public class RaidStatusAlertHandler : MultipleAlertHandler
    {
        public RaidStatusAlertHandler(IDvrService deviceService, IAlarmConfigurationService alarmService, IAlertService alertService, INotificationService notificationService)
            : base(deviceService, alarmService, alertService, notificationService)
        {
        }
        
        public override bool SatisfiesRule(string element, object thresholdValue, AlarmOperator relationalOperator)
        {
            return element.ToLower() != "clean";
        }
    }
}