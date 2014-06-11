using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Platform.Proxies.REST;
using Diebold.Platform.Proxies.REST.Enums;
using Diebold.Platform.Proxies.DTO;
using System.Configuration;
using System;
using log4net.Core;
using log4net;

namespace Diebold.Platform.Proxies.Impl
{
    public class MonitoringAPI : IMonitoringAPIService
    {
        protected static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string baseResponseCallbackURL = ConfigurationManager.AppSettings["ResponseCallBackUrl"].ToString();
        RestManager obj = new RestManager();
        string callBackUrl = string.Empty;
        string commandName = string.Empty;
        public string PlaceonTestAPI(string Monitoring)
        {
            try
            {
                logger.Debug("Place on Test Started with content body" + Monitoring);
                string ReponsefromAPI = string.Empty;
                commandName = "Place on Test";
                logger.Debug("Place on Test for API started with Content body " + Monitoring + " with Command Name " + commandName);
                ReponsefromAPI = obj.ExecuteAPICallforMAS("", Monitoring, commandName);
                logger.Debug("Place on Test for API Completed with Response " + ReponsefromAPI);
                logger.Debug("Place on Test Completed");
                return ReponsefromAPI;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
           
        }
        public string PlaceonTestAPIforDDChange(string Monitoring)
        {
            try
            {
                logger.Debug("Place on Test for DD Change Started with content body" + Monitoring);
                string ReponsefromAPI = string.Empty;
                commandName = "Place on Test";
                logger.Debug("Place on Test for DD Change for API started with Content body " + Monitoring + " with Command Name " + commandName);
                ReponsefromAPI = obj.ExecuteAPICallforMAS("", Monitoring, commandName);
                logger.Debug("Place on Test for DD Change for API Completed with Response " + ReponsefromAPI);
                logger.Debug("Place on Test for DD Change Completed");
                return ReponsefromAPI;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public string RunReport(string ReportName, DateTime dtFromDate, DateTime dtToDate, string strReportParams)
        {
            logger.Debug("Run Report Started for Report Name: " + ReportName + " From Date: " + dtFromDate.ToString() + " To Date: " + dtToDate.ToString() + "Command Name: " + commandName);
            string ReponsefromAPI = string.Empty;
            switch (ReportName)
            {
                case "Events":
                    commandName = "Event";
                    break;
                case "Open/Close":
                    commandName = "Open/Close Normal";
                    break;
                case "Call":
                    commandName = "Open/Close Irregular";
                    break;
                case "Zone List":
                    commandName = "Zone List";
                    break;
                default:
                    break;
            }
            logger.Debug("Execution of API Call for MAS Started with Report Params " + strReportParams + " and Command Name " + commandName);
            ReponsefromAPI =  obj.ExecuteAPICallforMAS("", strReportParams, commandName);
            logger.Debug("Execution of API Call for MAS Completed with Response " + ReponsefromAPI);
            logger.Debug("Run Report Completed"); 
            return ReponsefromAPI;
        }

        public string FireWidgetReport(string strReportParams)
        {
            logger.Debug("Fire Widget Report Started with reportParms " + strReportParams);
            string ReponsefromAPI = string.Empty;
            logger.Debug("Execution of API Call for Fire Widget Started with Report Params " + strReportParams);
            ReponsefromAPI = obj.ExecuteAPICallforFireWidget("", strReportParams);
            logger.Debug("Execution of API Call for Fire Widget Completed with Response " + ReponsefromAPI);
            logger.Debug("Run Report Completed");
            return ReponsefromAPI;
        }
    }
}
