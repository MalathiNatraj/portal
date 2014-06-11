using System;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Controllers.AlertHandlers;

namespace Diebold.WebApp.Infrastructure.Helpers
{
    public class AlertHandlerFactory : IAlertHandlerFactory
    {
        private readonly IDvrService _deviceService;
        private readonly IAlarmConfigurationService _alarmService;
        private readonly IAlertService _alertService;
        private readonly INotificationService _notificationService;
        private readonly IAccessService _accessService;
        private readonly IIntrusionService _intrusionService;

        public AlertHandlerFactory()
        {
            
        }

        public AlertHandlerFactory(IDvrService deviceService,
            IAlarmConfigurationService alarmService,
            IAlertService alertService,
            INotificationService notificationService,
            IAccessService accessService, IIntrusionService intrusionService)
        {
            _deviceService = deviceService;
            _alarmService = alarmService;
            _alertService = alertService;
            _notificationService = notificationService;
            _accessService = accessService;
            _intrusionService = intrusionService;
        }

        public IAlertHandler GetAlertHandlerByAlarmName(String alarmName)
        {
            return GetHandler(GetAlarmType(alarmName));
        }

        private static AlarmType GetAlarmType(string alarmName)
        {
            switch (alarmName.ToLower())
            {
                case "drivetemp": alarmName = "DriveTemperature"; break;
                case "daysrecorded": alarmName = "daysRecorded"; break;
                case "smart": alarmName = "SMART"; break;
                case "raidstatus": alarmName = "raidStatus"; break;
                case "videoloss": alarmName = "videoLoss"; break;
                case "isnotrecording": alarmName = "isNotRecording"; break;
                case "networkdown": alarmName = "networkDown"; break;
                case "areaarmed": alarmName = "areaarmed"; break;
                case "areadisarmed": alarmName = "areadisarmed"; break;
                case "zonealarm": alarmName = "zonealarm"; break;
                case "zonetrouble": alarmName = "zonetrouble"; break;
                case "zonebypass": alarmName = "zonebypass"; break;
                case "doorforced": alarmName = "doorforced"; break;
                case "doorheld": alarmName = "doorheld"; break;
                case "doorStatus": alarmName = "doorStatus"; break;
                case "zoneStatus": alarmName = "zoneStatus"; break;
                case "armed": alarmName = "armed"; break;

            }

            var alarmType = (AlarmType)Enum.Parse(typeof(AlarmType), alarmName, true);

            return alarmType;
        }

        private IAlertHandler GetHandler(AlarmType alarmType)
        {
            IAlertHandler alertHandler;

            switch (alarmType)
            {
                case AlarmType.DriveTemperature:
                    alertHandler = new DriveTempAlertHandler(_deviceService, _alarmService, _alertService, _notificationService);
                    break;
                case AlarmType.SMART:
                    alertHandler = new SMARTAlertHandler(_deviceService, _alarmService, _alertService, _notificationService);
                    break;
                case AlarmType.VideoLoss:
                    alertHandler = new VideoLossAlertHandler(_deviceService, _alarmService, _alertService, _notificationService);
                    break;
                case AlarmType.RaidStatus:
                    alertHandler = new RaidStatusAlertHandler(_deviceService, _alarmService, _alertService, _notificationService);
                    break;
                default:
                    alertHandler = new SingleAlertHandler(_deviceService, _alarmService, _alertService, _notificationService, _accessService, _intrusionService);
                    break;
            }

            return alertHandler;
        }
    }
}
