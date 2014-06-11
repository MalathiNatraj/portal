using System.Collections.Generic;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Platform.Proxies.DTO;
using Diebold.Platform.Proxies.Enums;
using Diebold.Platform.Proxies.REST;
using Diebold.Platform.Proxies.REST.Enums;
using System.Configuration;

namespace Diebold.Platform.Proxies.Impl
{
    public class GatewayApi : BaseMachineshopAPI, IGatewayApiService
    {
        string baseResponseCallbackURL = ConfigurationManager.AppSettings["ResponseCallBackUrl"].ToString();
        public IList<string> GetUnassignedMACAddresses()
        {
            var macs = APIManager.GETRequest<UnassignedMacsDTO>("unassignedSlots/{0}", DeviceTypeEnum.SparkGateway);
            return macs.Items;
        }

        public void RevokeDevice(DeviceDTO item)
        {
            RestManager restManager = new RestManager();
            restManager.ExecuteAPICall("/device_instance/", prepareRequestforDelete(item), "RevokeDevice");
        }

        private string prepareRequestforDelete(DeviceDTO item)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            objDeviceReqParameters.CommandName = "Delete";
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = item.ExternalDeviceKey;
            objDeviceReqParameters.DeviceType = "";
            objDeviceReqParameters.DeviceInstnaceType = "SparkGateway";
            objDeviceReqParameters.CallbackUrl = baseResponseCallbackURL + "/PlatformResponse/GateWayRevokeResponse";
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }
    }
}
