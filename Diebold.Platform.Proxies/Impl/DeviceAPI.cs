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
    public class DeviceApi : BaseMachineshopAPI, IDeviceApiService
    {
        protected static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string baseResponseCallbackURL = ConfigurationManager.AppSettings["ResponseCallBackUrl"].ToString();
        public IList<DeviceTypeDTO> ListDeviceTypes()
        {
            return APIManager.GETRequestCollection<DeviceTypeDTO>("deviceTypes");
        }

        public string AddDevice(DeviceDTO newDevice, bool isGateway)
        {
            RestManager restManager = new RestManager();
            string callBackUrl = string.Empty;
            string commandName = string.Empty;
            
            if (isGateway)
            {
                callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GateWayCreateResponse";
                commandName = "Configure";
            }
            else
            {
                callBackUrl = baseResponseCallbackURL + "/PlatformResponse/DeviceCreateResponse";
                commandName = "AddDevice";
            }

            return restManager.ExecuteAPICall("/device_instance", prepareRequest(newDevice, isGateway, callBackUrl, commandName), commandName);
        }

        private string prepareRequest(DeviceDTO newDevice, bool isGateWay, string callBackUrl, string commandName)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceType = newDevice.DeviceType;
            objDeviceReqParameters.DeviceKey = newDevice.ExternalDeviceKey;
            if (isGateWay == false)
            {
                switch (commandName)
                {
                    case "GetStatus":
                    case "Reboot":
                    case "RestartAgent":
                    case "Ping":
                        objDeviceReqParameters.DeviceInstnaceType = "SparkDvr";
                        break;
                    case "GetAccessControlStatus":
                        objDeviceReqParameters.DeviceInstnaceType = "SparkAccessControl";
                        break;
                    case "GetIntrusionStatus":
                        objDeviceReqParameters.DeviceInstnaceType = "SparkIntrusion";
                        break;
                    default:
                        objDeviceReqParameters.DeviceInstnaceType = "SparkGateway";
                        break;
                }
            }
            else
            {
                objDeviceReqParameters.DeviceInstnaceType = "SparkGateway";
            }
            PrepareDVRPropeties(objDeviceReqParameters, newDevice, isGateWay, commandName);
            objDeviceReqParameters.Alarms = newDevice.Alarms;
            objDeviceReqParameters.CommandName = commandName;
            objDeviceReqParameters.CallbackUrl = callBackUrl;
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }

        private void PrepareDVRPropeties(DeviceRequestParameters objRequestParameters, DeviceDTO newDevice, bool isGateway, string commandName)
        {
            List<Property> objLstProperties = new List<Property>();
            Property objProperty = new Property();
            objProperty.Name = "type";
            if (isGateway == false)
            {
                switch (commandName)
                {
                    case "GetStatus":
                    case "Reboot":
                    case "RestartAgent":
                    case "Ping":
                        objProperty.Value = "SparkDvr";
                        break;
                    case "GetAccessControlStatus":
                        objProperty.Value = "SparkAccessControl";
                        break;
                    case "GetIntrusionStatus":
                        objProperty.Value = "SparkIntrusion";
                        break;
                    default:
                        objProperty.Value = "SparkGateway";
                        break;
                }
            }

            //
            objProperty.Value = "SparkGateway";
            objProperty.Value = newDevice.DeviceType;
            objLstProperties.Add(objProperty);
            if (newDevice.Configuration != null)
            {
                objProperty = new Property();
                objProperty.Name = "host";
                objProperty.Value = newDevice.Configuration.IP;
                objLstProperties.Add(objProperty);

                objProperty = new Property();
                objProperty.Name = "user";
                objProperty.Value = newDevice.Configuration.User;
                objLstProperties.Add(objProperty);

                objProperty = new Property();
                objProperty.Name = "pass";
                objProperty.Value = newDevice.Configuration.Password;
                objLstProperties.Add(objProperty);

                objProperty = new Property();
                objProperty.Name = "portA";
                objProperty.Value = newDevice.Configuration.PortA;
                objLstProperties.Add(objProperty);

                objProperty = new Property();
                objProperty.Name = "portB";
                objProperty.Value = newDevice.Configuration.PortB;
                objLstProperties.Add(objProperty);

                objProperty = new Property();
                objProperty.Name = "timezone";
                objProperty.Value = newDevice.Configuration.TimeZone;
                objLstProperties.Add(objProperty);

                objProperty = new Property();
                objProperty.Name = "dst";
                objProperty.Value = newDevice.IsInDst.ToString().ToLower();
                objLstProperties.Add(objProperty);

                objProperty = new Property();
                objProperty.Name = "reportFrequency";
                objProperty.Value = newDevice.PollingFrequency.ToString();
                objLstProperties.Add(objProperty);

                objProperty = new Property();
                objProperty.Name = "reportBufferSize";
                objProperty.Value = "1";
                objLstProperties.Add(objProperty);

                objProperty = new Property();
                objProperty.Name = "onLine";
                objProperty.Value = newDevice.OnLine.ToString().ToLower();
                objLstProperties.Add(objProperty);
                objRequestParameters.Properties = objLstProperties;
            }
        }

        public string GetDevicetype(string strType)
        {
            string strDeviceType = string.Empty;
            switch (strType)
            {
                case "Costar111":
                    strDeviceType = "Costar111";
                    break;
                case "ipConfigure530":
                    strDeviceType = "ipConfigure530";

                    break;
                case "VerintEdgeVr200":
                    strDeviceType = "VerintEdgeVr200";

                    break;
                case "eData524":
                    strDeviceType = "eData524";
                    break;
                case "eData300":
                    strDeviceType = "eData300";
                    break;
                case "dmpXR100Access":
                    strDeviceType = "dmpXR100Access";
                    break;
                case "dmpXR500Access":
                    strDeviceType = "dmpXR500Access";
                    break;
                case "dmpXR100":
                    strDeviceType = "dmpXR100";
                    break;
                case "dmpXR500":
                    strDeviceType = "dmpXR500";
                    break;
                case "bosch_D9412GV4":
                    strDeviceType = "bosch_D9412GV4";
                    break;
                case "videofied01":
                    strDeviceType = "videofied01";
                    break;
            }
            return strDeviceType;
        }

        private string prepareRequestforDelete(DeviceDTO newDevice, bool isGateWay)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            string commandName = string.Empty;
            if (isGateWay)
            {
                commandName = "Revert";
            }
            else
            {
                commandName = "RemoveDevice";
            }
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = newDevice.ExternalDeviceKey;
            objDeviceReqParameters.DeviceInstnaceType = "SparkGateway";
            objDeviceReqParameters.DeviceType = newDevice.DeviceType;
            objDeviceReqParameters.CommandName = commandName;
            PrepareDVRPropeties(objDeviceReqParameters, newDevice, isGateWay, commandName);
            if (isGateWay)
                objDeviceReqParameters.CallbackUrl = baseResponseCallbackURL+"/PlatformResponse/GateWayDeleteResponse";
            else
                objDeviceReqParameters.CallbackUrl = baseResponseCallbackURL + "/PlatformResponse/DeviceDeleteResponse";
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }


        private string prepareRequestforTestConnectionDevice(DeviceDTO device)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            objDeviceReqParameters.CommandName = "Ping";
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = device.ExternalDeviceKey;
            objDeviceReqParameters.DeviceType = device.DeviceType;
            objDeviceReqParameters.Type = device.DeviceType;
            objDeviceReqParameters.DeviceInstnaceType = device.DeviceType;
            PrepareDVRPropeties(objDeviceReqParameters, device, false, "Ping");

            objDeviceReqParameters.CallbackUrl = baseResponseCallbackURL + "/PlatformResponse/GateWayTestConnectionResponse";
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }
        private string prepareRequestforMediaCapture(DeviceDTO device, DeviceMediaType deviceMediaType)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();

            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = device.ExternalDeviceKey;
            objDeviceReqParameters.DeviceType = device.DeviceType;
            objDeviceReqParameters.Type = device.DeviceType;
            objDeviceReqParameters.DeviceInstnaceType = device.DeviceType;
            switch (deviceMediaType)
            {
                case DeviceMediaType.Video:
                    objDeviceReqParameters.CommandName = "GetIntrusionImage";
                    break;
                case DeviceMediaType.Image:
                    objDeviceReqParameters.CommandName = "GetIntrusionVideo";
                    break;
                default:
                    break;
            }
            PrepareDVRPropeties(objDeviceReqParameters, device, false, objDeviceReqParameters.CommandName);
            objDeviceReqParameters.CallbackUrl = baseResponseCallbackURL + "/PlatformResponse/DeviceMediaCapture";
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        
        }
        private string prepareRequestforTestConnectionGateway(DeviceDTO device)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            objDeviceReqParameters.CommandName = "Ping";
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = device.ExternalDeviceKey;
            objDeviceReqParameters.DeviceType = device.DeviceType;
            objDeviceReqParameters.Type = device.DeviceType;
            objDeviceReqParameters.DeviceInstnaceType = device.DeviceType;

            if (device.DeviceType.ToLower() == "dmpxr100" || device.DeviceType.ToLower() == "dmpxr500" || device.DeviceType.ToLower() == "bosch_d9412gv4" || device.DeviceType.ToLower() == "videofied01")
            {
                objDeviceReqParameters.DeviceInstnaceType = "SparkIntrusion";
            }
            else if (device.DeviceType.ToLower() == "edata300" || device.DeviceType.ToLower() == "edata524" || device.DeviceType.ToLower() == "dmpxr100access" || device.DeviceType.ToLower() == "dmpxr500access")
            {
                objDeviceReqParameters.DeviceInstnaceType = "SparkAccessControl";
            }
            else if (device.DeviceType.ToLower() == "verintedgevr200" || device.DeviceType.ToLower() == "ipconfigure530" ||  device.DeviceType.ToLower() == "costar111")
            {
                objDeviceReqParameters.DeviceInstnaceType = "SparkDvr";
            }            

            PrepareDVRPropeties(objDeviceReqParameters, device, true, "Ping");
            objDeviceReqParameters.CallbackUrl = baseResponseCallbackURL + "/PlatformResponse/GateWayTestConnectionResponse";
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }

        private string prepareRequestforRebootDevice(DeviceDTO device, string commandName)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            objDeviceReqParameters.CommandName = commandName;
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = device.ExternalDeviceKey;
            objDeviceReqParameters.DeviceType = device.DeviceType;
            objDeviceReqParameters.Type = device.DeviceType;
            objDeviceReqParameters.DeviceInstnaceType = device.DeviceType;
            if (device.DeviceType.ToLower() == "dmpxr100" || device.DeviceType.ToLower() == "dmpxr500" || device.DeviceType.ToLower() == "bosch_d9412gv4" || device.DeviceType.ToLower() == "videofied01")
            {
                objDeviceReqParameters.DeviceInstnaceType = "SparkIntrusion";
            }
            else if (device.DeviceType.ToLower() == "edata300" || device.DeviceType.ToLower() == "edata524" || device.DeviceType.ToLower() == "dmpxr100access" || device.DeviceType.ToLower() == "dmpxr500access")
            {
                objDeviceReqParameters.DeviceInstnaceType = "SparkAccessControl";
            }
            else if (device.DeviceType.ToLower() == "verintedgevr200" || device.DeviceType.ToLower() == "ipconfigure530" || device.DeviceType.ToLower() == "costar111")
            {
                objDeviceReqParameters.DeviceInstnaceType = "SparkDvr";
            } 
            PrepareDVRPropeties(objDeviceReqParameters, device, false, "Ping");
            objDeviceReqParameters.CallbackUrl = baseResponseCallbackURL + "/PlatformResponse/RebootDeviceResponse";
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }

        private string prepareRequestforRebootGateway(DeviceDTO device, string commandName)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            objDeviceReqParameters.CommandName = commandName;
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = device.ExternalDeviceKey;
            objDeviceReqParameters.DeviceType = device.DeviceType;
            objDeviceReqParameters.Type = device.DeviceType;
            objDeviceReqParameters.DeviceInstnaceType = device.DeviceType;           
            PrepareDVRPropeties(objDeviceReqParameters, device, true, "Ping");
            objDeviceReqParameters.CallbackUrl = baseResponseCallbackURL + "/PlatformResponse/RebootGatewayResponse";
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }

        private string prepareRequestforReStartGateway(DeviceDTO device, string commandName)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            objDeviceReqParameters.CommandName = commandName;
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = device.ExternalDeviceKey;
            objDeviceReqParameters.DeviceType = device.DeviceType;
            objDeviceReqParameters.Type = device.DeviceType;
            objDeviceReqParameters.DeviceInstnaceType = device.DeviceType;
            if (device.DeviceType.ToLower() == "dmpxr100" || device.DeviceType.ToLower() == "dmpxr500" || device.DeviceType.ToLower() == "bosch_d9412gv4" || device.DeviceType.ToLower() == "videofied01")
            {
                objDeviceReqParameters.DeviceInstnaceType = "SparkIntrusion";
            }
            else if (device.DeviceType.ToLower() == "edata300" || device.DeviceType.ToLower() == "edata524" || device.DeviceType.ToLower() == "dmpxr100access" || device.DeviceType.ToLower() == "dmpxr500access")
            {
                objDeviceReqParameters.DeviceInstnaceType = "SparkAccessControl";
            }
            else if (device.DeviceType.ToLower() == "verintedgevr200" || device.DeviceType.ToLower() == "ipconfigure530" || device.DeviceType.ToLower() == "costar111")
            {
                objDeviceReqParameters.DeviceInstnaceType = "SparkDvr";
            }
            PrepareDVRPropeties(objDeviceReqParameters, device, true, "Ping");
            objDeviceReqParameters.CallbackUrl = baseResponseCallbackURL + "/PlatformResponse/RestartAgent";
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }

        public string RemoveDevice(DeviceDTO deletedDevice, bool isGateWay)
        {
            RestManager restManager = new RestManager();
            return restManager.ExecuteAPICall("/device_instance", prepareRequestforDelete(deletedDevice, isGateWay), "Remove");
        }

        public string RemoveDevicefromPlatform(DeviceDTO deletedDevice, bool isGateWay, string ExternalDeviceId)
        {
            RestManager restManager = new RestManager();
            return restManager.ExecuteAPICallforDelete("/device_instance/" + ExternalDeviceId, "null");
        }

        public void ModifyDevice(DeviceDTO updatedDevice, bool isGateWay)
        {
            RestManager restManager = new RestManager();
            string callBackUrl = string.Empty;
            string commandName = string.Empty;
            if (isGateWay)
            {
                callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GateWayUpdateResponse";
                commandName = "Configure";
            }
            else
            {
                callBackUrl = baseResponseCallbackURL + "/PlatformResponse/DeviceUpdateResponse";
                commandName = "UpdateDevice";
            }
            restManager.ExecuteAPICall("/device_instance", prepareRequest(updatedDevice, isGateWay, callBackUrl, commandName), commandName);
        }

        public string Status(DeviceDTO device, bool isGateway)
        {
            RestManager restManager = new RestManager();
            if (isGateway)
            {
                device.DeviceType = "SparkGateway";
                return restManager.ExecuteAPICall("/device_instance", prepareRequest(device, isGateway, baseResponseCallbackURL + "/PlatformResponse/GatewayStatusResponse", "GetStatus"),"Gateway Status");
            }
            else
            {
                device.DeviceType = "SparkDvr";
                return restManager.ExecuteAPICall("/device_instance", prepareRequest(device, isGateway, baseResponseCallbackURL + "/PlatformResponse/DeviceStatusResponse", "GetStatus"), "Device STatus");
            }
        }

        public string StatusfrmPlatform(DeviceDTO device, bool isGateway)
        {
            RestManager restManager = new RestManager();
            if (isGateway)
            {
                device.DeviceType = "SparkGateway";              
            }
            else
            {
                device.DeviceType = "SparkDvr";            
            }

            logger.Debug("Get Status from Platform Started ");
            string ReponsefromPlatform = string.Empty;
            string commandName = string.Empty;
            commandName = "GetPlatformDevicePollingStatus";
            string deviceKey = device.ExternalDeviceKey + "&per_page=1";
            logger.Debug("Get Platform Status API started ");
            ReponsefromPlatform = restManager.ExecutePlatformAPICall(deviceKey, commandName);
            logger.Debug("Get Platform Status API Completed with Response " + ReponsefromPlatform);
            logger.Debug("Get Platform Status Completed");
            return ReponsefromPlatform;
        }

        private static object ParseValue(string dataType, object value)
        {
            return value;
        }

        public void Enabled(string deviceId)
        {
            APIManager.Request("device_instance", new RequestParameters(new[] { deviceId }), RequestMethod.PUT);
        }

        public void Disabled(string deviceId)
        {
            APIManager.Request("device_instance", new RequestParameters(new[] { deviceId }), RequestMethod.PUT);
        }

        public void Restart(DeviceDTO newDevice, bool isGateway)
        {
            RestManager obj = new RestManager();
            if (isGateway)
            {
                obj.ExecuteAPICall("/device_instance", prepareRequestforReStartGateway(newDevice, "RestartAgent"), "Restart Gateway");
            }
            else
            {
                obj.ExecuteAPICall("/device_instance", prepareRequestforReStartGateway(newDevice, "RestartAgent"), "Restart Device");
            }
        }

        /// <summary>
        ///  Need to change the method that is being called.
        /// </summary>
        /// <param name="newDevice"></param>
        /// <param name="isGateway"></param>
        public void Reload(DeviceDTO newDevice, bool isGateway)
        {
            RestManager obj = new RestManager();
            
            if (isGateway)
            {
                newDevice.DeviceType = "SparkGateway";
                obj.ExecuteAPICall("/device_instance", prepareRequestforRebootGateway(newDevice, "Reboot"), "Reload Gateway");
            }
            else
            {
                obj.ExecuteAPICall("/device_instance", prepareRequestforRebootDevice(newDevice, "Reboot"), "Reload Device");
            }
        }

        public void TestConnection(DeviceDTO newDevice, bool isGateway)
        {
            RestManager restManager = new RestManager();
            if (isGateway)
            {
                newDevice.DeviceType = "SparkGateway";
                restManager.ExecuteAPICall("/device_instance", prepareRequestforTestConnectionGateway(newDevice), "Gateway TestConnection");
            }
            else
            {               
                if (string.IsNullOrEmpty(newDevice.DeviceType))
                {
                    newDevice.DeviceType = "SparkDvr";
                }

                restManager.ExecuteAPICall("/device_instance", prepareRequestforTestConnectionGateway(newDevice), "Device TestConnection");
            }
        }
    }
}
