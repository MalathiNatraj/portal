using System;
using Diebold.WebApp.Controllers.AlertHandlers;

namespace Diebold.WebApp.Infrastructure.Helpers
{
    public interface IAlertHandlerFactory
    {
        IAlertHandler GetAlertHandlerByAlarmName(String alarmName);
    }
}