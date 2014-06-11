using System;
using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Platform.Proxies.REST;
using Diebold.Platform.Proxies.REST.Enums;
using Diebold.Platform.Proxies.DTO;
using System.Configuration;
using log4net.Core;
using log4net;

namespace Diebold.Platform.Proxies.Impl
{
    public class SystemSummaryAPI : ISystemSummaryAPIService
    {
        protected static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string baseResponseCallbackURL = ConfigurationManager.AppSettings["ResponseCallBackUrl"].ToString();
        RestManager obj = new RestManager();
        string commandName = string.Empty;

        public string GetSystemSummaryAPI(string strSystemSummary)
        {
            try
            {
                logger.Debug("Get System Summary Started with SystemSummary : " + strSystemSummary);
                string ReponsefromAPI = string.Empty;
                commandName = string.Empty;
                logger.Debug("Get System Summary for API started with System Summary " + strSystemSummary);
                ReponsefromAPI = obj.ExecuteAPICallforSystemSummary("", strSystemSummary, commandName);
                logger.Debug("Get System Summary for API Completed with Response " + ReponsefromAPI);
                logger.Debug("Get System Summary Completed");
                return ReponsefromAPI;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
