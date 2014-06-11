using System;
using System.Web.Mvc;
using DieboldMobile.Infrastructure.Authentication;
using DieboldMobile.Infrastructure.Helpers;
using DieboldMobile.Models;

namespace DieboldMobile.Controllers
{
    public class AlertController : BaseController
    {
        private readonly IAlertHandlerFactory _alertHandlerFactory;

        public AlertController(IAlertHandlerFactory alertHandlerFactory)
        {
            _alertHandlerFactory = alertHandlerFactory;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Notify(NotificationViewModel message)
        {
            if (ModelState.IsValid)
            {
                var alertHandler = _alertHandlerFactory.GetAlertHandlerByAlarmName(message.Alert.AlarmName);

                //Get alerts from platform alert.
                var alertList = alertHandler.HandleAlert(message.Alert);

                //Create one or more alerts.
                alertHandler.CreateAlerts(alertList);

                //Notificate for each alert.
                alertHandler.Notify(alertList);

                return new EmptyResult();
            }

            throw new Exception("Invalid POST");
        }
    }
}
