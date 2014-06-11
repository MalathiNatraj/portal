using Diebold.Platform.Proxies.REST;
using System.Configuration;
using Diebold.Platform.Proxies.REST.Enums;

namespace Diebold.Platform.Proxies.Impl
{
    public abstract class BaseMachineshopAPI
    {
        protected IRestManager APIManager;

        protected BaseMachineshopAPI()
        {
            var baseUrl = ConfigurationManager.AppSettings["MachineshopPlatformURL"];
            var authorization = "Basic " + Base64Encode.Encode64(ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"].ToString() +":X");
            
            APIManager = new RestManager(baseUrl, ContentFormat.Json);
            APIManager.Headers.Add("ApplicationToken", authorization);
        }
    }
}
