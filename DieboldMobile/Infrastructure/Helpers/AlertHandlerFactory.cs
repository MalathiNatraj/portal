using System;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using DieboldMobile.Controllers.AlertHandlers;

namespace DieboldMobile.Infrastructure.Helpers
{
    public class AlertHandlerFactory : IAlertHandlerFactory
    {
        private readonly IDvrService _deviceService;
        private readonly IAlarmConfigurationService _alarmService;
        private readonly IAlertService _alertService;
        private readonly INotificationService _notificationService;

        public AlertHandlerFactory()
        {

        }

        public AlertHandlerFactory(IDvrService deviceService,
            IAlarmConfigurationService alarmService,
            IAlertService alertService,
            INotificationService notificationService)
        {
            _deviceService = deviceService;
            _alarmService = alarmService;
            _alertService = alertService;
            _notificationService = notificationService;
        }

        public IAlertHandler GetAlertHandlerByAlarmName(String alarmName)
        {
            return GetHandler(GetAlarmType(alarmName));
        }

        private static AlarmType GetAlarmType(string alarmName)
        {
            switch (alarmName)
            {
                case "driveTemp": alarmName = "DriveTemperature"; break;
            }

            var alarmType = (AlarmType)Enum.Parse(typeof(AlarmType), alarmName, true);

            return alarmType;
        }

        private IAlertHandler GetHandler(AlarmType alarmType)
        {
            IAlertHandler alertHandler=null;

            switch (alarmType)
            {
                case AlarmType.DriveTemperature:
                    //alertHandler = new DriveTempAlertHandler(_deviceService, _alarmService, _alertService, _notificationService);
                    break;
                case AlarmType.SMART:
                    //alertHandler = new SMARTAlertHandler(_deviceService, _alarmService, _alertService, _notificationService);
                    break;
                case AlarmType.VideoLoss:
                    //alertHandler = new VideoLossAlertHandler(_deviceService, _alarmService, _alertService, _notificationService);
                    break;
                case AlarmType.RaidStatus:
                    //alertHandler = new RaidStatusAlertHandler(_deviceService, _alarmService, _alertService, _notificationService);
                    break;
                default:
                    //alertHandler = new SingleAlertHandler(_deviceService, _alarmService, _alertService, _notificationService);
                    break;
            }

            return alertHandler;
        }
    }
}