using System;
using System.Web.Mvc;
using Diebold.WebApp.Infrastructure.Authentication;
using Diebold.WebApp.Infrastructure.Helpers;
using Diebold.WebApp.Models;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Diebold.WebApp.Controllers
{
    public class AlertController : BaseController
    {
        private readonly IAlertHandlerFactory _alertHandlerFactory;
        private readonly IDvrService _dvrService;
        private readonly IAlertService _alertService;

        public AlertController(IAlertHandlerFactory alertHandlerFactory, IDvrService dvrService, IAlertService alertService)
        {
            _alertHandlerFactory = alertHandlerFactory;
            _dvrService = dvrService;
            _alertService = alertService;
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult Notify()
        {
            logger.Debug("Reached  Notification Response");
            string responseText = string.Empty;

            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    responseText = readStream.ReadToEnd();
                    logger.Debug("Received JSON @ Callback for Alert -->:" + responseText);
                }
            }

            if (string.IsNullOrEmpty(responseText) == true)
            {
                return Json("Invalid POST");
            }
            else
            {
                NotifyAlert(responseText);
                return Json("Processed alert successfully.");
            }
        }

        private void NotifyAlert( String responseText)
        {
            try
            {
                NotificationViewModel message = new NotificationViewModel();
                if (responseText.ToLower().Contains("alert_clear"))
                {
                    message.AlertClear = getAlertClear(responseText);
                    if (ModelState.IsValid)
                    {
                        var alertHandler = _alertHandlerFactory.GetAlertHandlerByAlarmName(message.AlertClear.AlarmName);
                        //Typecast AlertClear to Alert
                        AlertClear objAlertClear = new AlertClear();
                        Alert objAlert = new Alert();
                        objAlertClear = message.AlertClear;
                        objAlert.AlarmName = objAlertClear.AlarmName;
                        objAlert.AlertActive = objAlertClear.AlertActive;
                        objAlert.AlertDate = objAlertClear.AlertDate;
                        objAlert.DeviceId = objAlertClear.DeviceId;
                        objAlert.RelationalOperator = objAlertClear.RelationalOperator;
                        objAlert.Threshold = objAlertClear.Threshold;
                        objAlert.Value = objAlertClear.Value;
                        //Get alerts from platform alert.
                        //var alertList = alertHandler.HandleAlert(message.AlertClear);
                        var alertList = alertHandler.HandleAlert(objAlert);
                        // Update IsOk property in AlertStatus Table
                       // To get alertstatus details pass device Id and alarmconfiguration Id
                        var DeviceDetails = ((Dvr)alertList[0].Device).DeviceType;
                        _alertService.CreateClearAlert(alertList);
                        string strEMC = getEMC("alertclear", objAlert);
                    }
                }
                else
                {
                    message.Alert = getAlert(responseText);

                    if (ModelState.IsValid)
                    {

                        var alertHandler = _alertHandlerFactory.GetAlertHandlerByAlarmName(message.Alert.AlarmName);

                        //Get alerts from platform alert.
                        var alertList = alertHandler.HandleAlert(message.Alert);

                        //Create one or more alerts.
                        alertHandler.CreateAlerts(alertList);
                        //Notificate for each alert.
                        alertHandler.Notify(alertList);

                        Alert objAlertEMC = new Alert();
                        objAlertEMC = message.Alert;

                        string strEMC = getEMC("alert", objAlertEMC);


                        //return new EmptyResult();
                    }
                }

                
            }
            catch(Exception ex)
            {
                logger.Debug("Error process alerts"+ex); 
            }
        }

      
        private Alert getAlert(String responseText)
        {
            logger.Debug("Entered the Get Alert Method");
            JavaScriptSerializer js = new JavaScriptSerializer();
            logger.Debug(" Deseialization of response object started" + responseText);
            var objAlertTemp = (AlertRequestObject)js.Deserialize(responseText, typeof(AlertRequestObject));
            logger.Debug("Desialization of response object completed" + objAlertTemp);
            Alert alert = new Alert();
            if (objAlertTemp.alert != null)
            {
                logger.Debug("Alert is not null" + objAlertTemp.alert);
            }
            else
            {
                logger.Debug("Alert is null");
            }
            logger.Debug("Get device by Instance Name started " + objAlertTemp.alert.device_instance_name);
            var DVR = _dvrService.GetDeviceByInstanceName(objAlertTemp.alert.device_instance_name);
            logger.Debug("Get device by Instance Name completed with DVR ID : " + DVR.Id);
            alert.DeviceId = DVR.Id.ToString();
            alert.AlarmName = objAlertTemp.alert.rule_name;
            alert.AlertDate = objAlertTemp.alert.alert_date;
            alert.Threshold = objAlertTemp.alert.threshold;
            alert.Value = objAlertTemp.alert.value;
            alert.AlertActive = true;
            alert.Report = objAlertTemp.alert.Report;
            string relationString = objAlertTemp.alert.rule_condition_type;
            var dieboldOperator = string.Empty;
            logger.Debug("Relation String : " + relationString);
            switch (relationString)
            {
                case "GreaterThanRuleCondition": dieboldOperator = AlarmOperator.GreaterThan.ToString(); break;
                case "GreaterThanEqualRuleCondition": dieboldOperator = AlarmOperator.GreaterThanOrEquals.ToString(); break;
                case "LessThanRuleCondition": dieboldOperator = AlarmOperator.LessThan.ToString(); break;
                case "LessThanEqualRuleCondition": dieboldOperator = AlarmOperator.LessThanOrEquals.ToString(); break;
                case "EqualRuleCondition": dieboldOperator = AlarmOperator.Equals.ToString(); break;
                case "NotEqualRuleCondition": dieboldOperator = AlarmOperator.NotEquals.ToString(); break;
                case "NotInRuleCondition": dieboldOperator = AlarmOperator.NotInRuleCondition.ToString(); break;
            }
            alert.RelationalOperator = dieboldOperator;
            logger.Debug("Get Alert method Completed");
            return alert;
        }

        private AlertClear getAlertClear(String responseText)
        {
            logger.Debug("Alert Clear Method Started");
            JavaScriptSerializer js = new JavaScriptSerializer();
            logger.Debug("Deserialization of response text started");
            logger.Debug(responseText);
            var objAlertTemp = (AlertRequestObject)js.Deserialize(responseText, typeof(AlertRequestObject));
            logger.Debug("Deserialization of response text completed");
            AlertClear alertClear = new AlertClear();
            logger.Debug("Get Device by Instance name started");
            var DVR = _dvrService.GetDeviceByInstanceName(objAlertTemp.alert_clear.device_instance_name);
            logger.Debug("Get Device by instance name completed");
            alertClear.DeviceId = DVR.Id.ToString();
            alertClear.AlarmName = objAlertTemp.alert_clear.rule_name;
            alertClear.AlertDate = objAlertTemp.alert_clear.alert_date;
            alertClear.Threshold = objAlertTemp.alert_clear.threshold;
            alertClear.Value = objAlertTemp.alert_clear.value;
            alertClear.AlertActive = true;

            string relationString = objAlertTemp.alert_clear.rule_condition_type;
            var dieboldOperator = string.Empty;
            switch (relationString)
            {
                case "GreaterThanRuleCondition": dieboldOperator = AlarmOperator.GreaterThan.ToString(); break;
                case "GreaterThanEqualRuleCondition": dieboldOperator = AlarmOperator.GreaterThanOrEquals.ToString(); break;
                case "LessThanRuleCondition": dieboldOperator = AlarmOperator.LessThan.ToString(); break;
                case "LessThanEqualRuleCondition": dieboldOperator = AlarmOperator.LessThanOrEquals.ToString(); break;
                case "EqualRuleCondition": dieboldOperator = AlarmOperator.Equals.ToString(); break;
                case "NotEqualRuleCondition": dieboldOperator = AlarmOperator.NotEquals.ToString(); break;
                case "NotInRuleCondition": dieboldOperator = AlarmOperator.NotInRuleCondition.ToString(); break;
            }
            alertClear.RelationalOperator = dieboldOperator;
            logger.Debug("Alert Clear Method Completed");
            return alertClear;
        }
        private string getEMC(string alert, Alert objAlertEMC)
        {
            logger.Debug("getEMC Method Started");
            string alarmName = string.Empty;
            if (alert == "alertclear")
            {
                alarmName = "alertclear";

            }
            else
            {
                if (objAlertEMC.AlarmName.ToLower() != AlarmType.NetworkDown.ToString().ToLower())
                {
                    alarmName = "alert";
                }
                else
                {
                    alarmName = objAlertEMC.AlarmName;
                }
            }

            string strEMC = _alertService.getEMC(alarmName, Convert.ToInt32(objAlertEMC.DeviceId));
            logger.Debug("getEMC Method Completed " + strEMC);
            return strEMC;
        }      
        
    }
}
