using System;
using DieboldMobile.Controllers.AlertHandlers;

namespace DieboldMobile.Infrastructure.Helpers
{
    public interface IAlertHandlerFactory
    {
        IAlertHandler GetAlertHandlerByAlarmName(String alarmName);
    }
}