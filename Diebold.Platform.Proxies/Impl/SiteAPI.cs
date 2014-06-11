using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using Diebold.Platform.Proxies.DTO;
using System.Text;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Domain.Entities;
using Diebold.Platform.Proxies.REST;
using Diebold.Platform.Proxies.REST.Enums;
using Diebold.Platform.Proxies.DTO;
using System.Configuration;
using System;
using log4net.Core;
using log4net;

namespace Diebold.Platform.Proxies.Impl
{
    public class SiteAPI : ISiteApiService
    {
        protected static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RestManager restManager = new RestManager();

        public string getGeoCoordinates(String address)
        {
            try
            {
                string ReponsefromPlatform = string.Empty;
                var formattedAddress = URIEscape(address);
                string addressWithOtherParam = HttpUtility.UrlEncode(formattedAddress) + "&sensor=false&client=[~google_client_id]&signature=[~google_crypto_key]";
                ReponsefromPlatform = restManager.ExecuteAPICallforSite(addressWithOtherParam);
                logger.Debug("Get Platform Intrusion Status API Completed with Response " + ReponsefromPlatform);
                logger.Debug("Get Platform Intrusion Status Completed");
                return ReponsefromPlatform;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public String URIEscape(String address)
        {
            address = address.Replace("  ", "+");
            address = address.Replace(" ", "+");
            address = address.Replace(":", "%3A");
            address = address.Replace(";", "%3B");
            address = address.Replace("#", "%23");
            address = address.Replace("@", "%40");
            address = address.Replace("=", "%3D");
            address = address.Replace("&", "%26");
            address = address.Replace("?", "%3F");
            address = address.Replace("/", "%2F");
            return address;
        }

        #region Weather Alerts

        public string GetWeatherAlertbyStateandCity(string State, string City)
        {
            try
            {
                // string ReponsefromPlatform = string.Empty;
                var ReponsefromPlatform = restManager.ExecuteAPICallforWeatherAlert(State, City);
                logger.Debug("Get Platform Intrusion Status API Completed with Response " + ReponsefromPlatform);
                logger.Debug("Get Platform Intrusion Status Completed");
                return ReponsefromPlatform;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return string.Empty;
        }

        #endregion

      
    }
}
