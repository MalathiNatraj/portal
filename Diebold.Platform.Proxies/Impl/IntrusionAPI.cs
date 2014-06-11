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
using Diebold.Platform.Proxies.Utilities;


namespace Diebold.Platform.Proxies.Impl
{
    public class IntrusionApi : IIntrusionApiService
    {
        protected static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string baseResponseCallbackURL = ConfigurationManager.AppSettings["ResponseCallBackUrl"].ToString();
        RestManager restManager = new RestManager();
        string callBackUrl = string.Empty;
        string commandName = string.Empty;
        #region IIntrusionApiService Members

        public string AreaArm(IntrusionDTO objAccess)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/AreaArm";
            commandName = "AreaArm";
            return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
        }

        public string AreaDisarm(IntrusionDTO objAccess)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/AreaDisarm";
            commandName = "AreaDisarm";
            return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
        }

        public string GetProfileNumberList(IntrusionDTO objAccess)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GetProfileNumberList";
            commandName = "GetProfileNumberList";
            return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
        }

        public string GetIntrusionReport(IntrusionDTO objAccess)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GetIntrusionReport";
            commandName = "GetIntrusionReport";
            return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
        }

        public string GetIntrusionStatus(IntrusionDTO objAccess)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GetIntrusionStatus";
            commandName = "GetIntrusionStatus";
            return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
        }
        public string CaptureMedia(IntrusionDTO objAccess, DeviceMediaType deviceMediaType)
        {
            switch (deviceMediaType)
            {
                case DeviceMediaType.Image:
                    commandName = "GetIntrusionImage";
                    break;
                case DeviceMediaType.Video:
                    commandName = "GetIntrusionVideo";
                    break;
                default:
                    break;
            }
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/DeviceMediaCapture";
            return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
           
        }
        public string GetPlatformIntrusionStatus(IntrusionDTO objAccess)
        {
            try
            {
                logger.Debug("Get Platform Intrusion Status started ");
                string ReponsefromPlatform = string.Empty;
                commandName = "GetPlatformIntrusionStatus";                
                string deviceKey = objAccess.ExternalDeviceKey + "&per_page=1";
                logger.Debug("Get Platform Intrusion Status API started " );
                ReponsefromPlatform = restManager.ExecuteAPICallforIntrusion(deviceKey, commandName);
                logger.Debug("Get Platform Intrusion Status API Completed with Response " + ReponsefromPlatform);
                logger.Debug("Get Platform Intrusion Status Completed");
                return ReponsefromPlatform;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public string GetUserCodesInformation(IntrusionDTO objAccess)
        {
            if (objAccess.DeviceType == DeviceType.bosch_D9412GV4.ToString())
            {
                callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GetUserCodesInformation2";
                commandName = "GetUserCodesInformation2";
                return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
            }
            else {
                callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GetUserCodesInformation";
                commandName = "GetUserCodesInformation";
                return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
            }
        }

        public string GetUsersCodeList(IntrusionDTO objAccess)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GetUsersCodeList";
            commandName = "GetUsersCodeList";
            return restManager.ExecuteAPICall(string.Format("/device_instance/{0}/device_command", objAccess.DeviceInstanceId), prepareRequest(objAccess, callBackUrl, commandName), commandName);
        }

        public string UserCodeAdd(IntrusionDTO objAccess)
        {

            
            if(objAccess.DeviceType == DeviceType.bosch_D9412GV4.ToString())
                return restManager.ExecuteAPICall("/device_instance/" + objAccess.DeviceInstanceId + "/device_command", objAccess.toAddUserCode2Request(), "UserCodeAdd2");

            commandName = "UserCodeAdd";
                callBackUrl = baseResponseCallbackURL + "/PlatformResponse/UserCodeAdd";
                return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), "UserCodeAdd");
        }

        public string UserCodeDelete(IntrusionDTO objAccess)
        {
            //if (objAccess.DeviceType == DeviceType.bosch_D9412GV4.ToString())
                //return restManager.ExecuteAPICall("/device_instance/" + objAccess.DeviceInstanceId + "/device_command", objAccess.toDeleteUserCode2Request(), "UserCodeDelete2");
            //else
                callBackUrl = baseResponseCallbackURL + "/PlatformResponse/UserCodeDelete";
                commandName = "UserCodeDelete";
                return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), "UserCodeDelete");
        }

        public string UserCodeModify(IntrusionDTO objAccess)
        {
            if (objAccess.DeviceType == DeviceType.bosch_D9412GV4.ToString())
                return restManager.ExecuteAPICall("/device_instance/" + objAccess.DeviceInstanceId + "/device_command", objAccess.toModifyUserCode2Request(), "UserCodeModify2");

            commandName = "UserCodeModify";
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/UserCodeModify";
            return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), "UserCodeModify");
        }

        public string UserZoneBypass(IntrusionDTO objAccess)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/UserZoneBypass";
            commandName = "ZoneBypass";
            return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
        }

        public string UserZoneResetBypass(IntrusionDTO objAccess)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/UserZoneResetBypass";
            commandName = "ZoneResetBypass";
            return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
        }

        public string RestartAgent(IntrusionDTO objAccess)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/RestartAgent";
            commandName = "RestartAgent";
            return restManager.ExecuteAPICall("/device_instance", prepareRequest(objAccess, callBackUrl, commandName), commandName);
        }
        #endregion
       
        private string prepareRequest(IntrusionDTO item, string callBackUrl, string commandName)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            objDeviceReqParameters.DeviceInstnaceType = "SparkIntrusion";
            objDeviceReqParameters.DeviceType = item.DeviceType;
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = item.ExternalDeviceKey;
            objDeviceReqParameters.DeviceType = item.DeviceType;
            objDeviceReqParameters.Type = item.DeviceType;
            PreparePropeties(objDeviceReqParameters, item, commandName);
            objDeviceReqParameters.CommandName = commandName;
            objDeviceReqParameters.CallbackUrl = callBackUrl;
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }

        private void PreparePropeties(DeviceRequestParameters objRequestParameters, IntrusionDTO item, string commandName)
        {
            List<Property> objLstProperties = new List<Property>();
            Property objProperty = new Property();
            switch(commandName)
            {
                case "AreaArm":
                case "AreaDisarm":
                    objProperty = new Property();
                    objProperty.Name = "areaNumber";
                    objProperty.Value = item.AreaNumber;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;
                    break;
                case "GetIntrusionImage":
                case "GetIntrusionVideo":
                    objProperty = new Property();
                    objProperty.Name = "zoneNumber";
                    objProperty.Value = item.ZoneNumber;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;
                    break;
                case "GetIntrusionReport":
                   
                    objProperty = new Property();
                    objProperty.Name = "startDateTime";
                    objProperty.Value = ConvertoJulianDate(item.StartDateTime);
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;
                    
                    objProperty = new Property();
                    objProperty.Name = "endDateTime";
                    objProperty.Value = ConvertoJulianDate(item.StartEndTime);
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;
                    break;
                case "GetUserCodesInformation":
                case "GetUserCodesInformation2":
                    objProperty = new Property();
                    objProperty.Name = "userName";
                    objProperty.Value = item.UserName;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;

                    objProperty = new Property();
                    objProperty.Name = "userCode";
                    objProperty.Value = item.UserCode;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;
                    break;
                case "UserCodeAdd":
                case "UserCodeAdd2":
                case "UserCodeModify":
                case "UserCodeModify2":
               
                    objProperty = new Property();
                    objProperty.Name = "userNumber";
                    objProperty.Value = item.UserNumber;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;

                    objProperty = new Property();
                    objProperty.Name = "userCode";
                    objProperty.Value = item.UserCode;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;

                    objProperty = new Property();
                    objProperty.Name = "pinCode";
                    objProperty.Value = item.PinCode;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;

                    objProperty = new Property();
                    objProperty.Name = "profileNumber";
                    objProperty.Value = item.ProfileNumber;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;

                    objProperty = new Property();
                    objProperty.Name = "tempDate";
                    objProperty.Value = item.TempDate;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;

                    objProperty = new Property();
                    objProperty.Name = "userName";
                    objProperty.Value = item.UserName;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;
                    objRequestParameters.Property.UserCodeInformation = new UserCodeInformationPropery();
                    objRequestParameters.Property.UserCodeInformation.Properties = objLstProperties;

                    if (item.AccessLevels != null && item.AccessLevels.Count > 0)
                    {
                        var listProp = new List<Property>();
                        foreach (var al in item.AccessLevels)
                        {
                            var acprop = new Property();
                            acprop.Name = al.Key;
                            acprop.Value = al.Value;
                            listProp.Add(acprop);
                        }

                        objRequestParameters.Property.UserCodeInformation.AreasAuthorityLevels = listProp;
                    }
                    break;
                  
                case "UserCodeDelete":
                case "UserCodeDelete2":
                    objProperty = new Property();
                    objProperty.Name = "userNumber";
                    objProperty.Value = item.UserNumber;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;
                    break;
                case "ZoneBypass":
                case "ZoneResetBypass":
                    objProperty.Name = "zoneNumber";
                    objProperty.Value = item.ZoneNumber;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Property = objProperty;
                    break;
            }
            objRequestParameters.Properties = objLstProperties;
            
        }

        private string ConvertoJulianDate(string datetime)
        {
            DateTime dt = Convert.ToDateTime(datetime);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var newTime = Convert.ToInt64((dt.ToUniversalTime() - epoch).TotalSeconds);
            return Convert.ToString(newTime);
        }
    }
}
