using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




//using Diebold.Platform.Proxies.DTO;
using Diebold.Platform.Proxies.Contracts;
//using Diebold.Domain.Entities;
using Diebold.Platform.Proxies.REST;
//using Diebold.Platform.Proxies.REST.Enums;
using Diebold.Platform.Proxies.DTO;
//using System.Configuration;

using log4net.Core;
using log4net;


namespace Diebold.Platform.Proxies.Impl
{
    public class AlertAPI : IAlertApiService
    {
        protected static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RestManager restManager = new RestManager();
        
        public string getEMC(EMCParameters objEMC)
        {
            try
            {
                string ReponsefromPlatform = string.Empty;                               
                UtilitiesApi objUtil = new UtilitiesApi();
                ReponsefromPlatform = restManager.ExecutePlatformAPIforEMC(objUtil.PrepareRequestBodyForEMC(objEMC), "getEMC");
                logger.Debug("Get EMC API Completed with Response " + ReponsefromPlatform);
                logger.Debug("Get EMC Status Completed");
                return ReponsefromPlatform;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
