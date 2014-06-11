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
    public class AccessApi : IAccessApiService
    {
        protected static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string baseResponseCallbackURL = ConfigurationManager.AppSettings["ResponseCallBackUrl"].ToString();
        RestManager obj = new RestManager();
        string callBackUrl = string.Empty;
        string commandName = string.Empty;

        #region IAccessApiService Members

        public string CardHolderAdd(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/CardHolderAdd";
            commandName = "CardHolderAdd";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string CardHolderDelete(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/CardHolderDelete";
            commandName = "CardHolderDelete";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string CardHolderModify(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/CardHolderModify";
            commandName = "CardHolderModify";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string GetCardHoldersInformation(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GetCardHoldersInformation";
            commandName = "GetCardHoldersInformation";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string GetCardHolderList(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GetCardHolderList";
            commandName = "GetCardHolderList";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string AccessGroupCreate(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/AccessGroupCreate";
            commandName = "AccessGroupCreate";
            return obj.ExecuteAPICall("/device_instance", PrepareAccessGroupRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string AccessGroupModify(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/AccessGroupModify";
            commandName = "AccessGroupModify";
            return obj.ExecuteAPICall("/device_instance", PrepareAccessGroupRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string AccessGroupDelete(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/AccessGroupDelete";
            commandName = "AccessGroupDelete";
            return obj.ExecuteAPICall("/device_instance", PrepareAccessGroupRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string GetAccessGroupInformation(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GetAccessGroupInformation";
            commandName = "GetAccessGroupInformation";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string AccessGetGroupList(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/AccessGetGroupList";
            commandName = "GetAccessGroupList";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string AccessMomentaryOpenDoor(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/AccessMomentaryOpenDoor";
            commandName = "MomentaryOpenDoor";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string AccessGetReadersList(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/AccessGetReadersList";
            commandName = "GetReadersList";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string AccessGetAccessControlStatus(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/AccessGetAccessControlStatus";
            commandName = "GetAccessControlStatus";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        public string GetPlatformAccessStatus(AccessDTO objAccessDTO)
        {
            try
            {
                logger.Debug("Get Platform Access Status started ");
                string ReponsefromPlatform = string.Empty;
                commandName = "GetPlatformAccessStatus";
                string deviceKey = objAccessDTO.ExternalDeviceKey + "&per_page=1";
                logger.Debug("Get Platform Access Status API started ");
                ReponsefromPlatform = obj.ExecutePlatformAPICall(deviceKey, commandName);
                logger.Debug("Get Platform Access API Completed with Response " + ReponsefromPlatform);
                logger.Debug("Get Platform Access Completed");
                return ReponsefromPlatform;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string AccessGetAccessControlReport(AccessDTO objAccessDTO)
        {
            callBackUrl = baseResponseCallbackURL + "/PlatformResponse/GetAccessControlReport";
            commandName = "GetAccessControlReport";
            return obj.ExecuteAPICall("/device_instance", prepareRequest(objAccessDTO, callBackUrl, commandName), commandName);
        }

        #endregion

        private string prepareRequest(AccessDTO item, string callBackUrl, string commandName)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            objDeviceReqParameters.Property = new Property();
            objDeviceReqParameters.DeviceInstnaceType = "SparkAccessControl";
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = item.ExternalDeviceKey;
            objDeviceReqParameters.DeviceType = item.DeviceType;
            objDeviceReqParameters.Type = item.DeviceType;
            PreparePropeties(objDeviceReqParameters, item, commandName);
            objDeviceReqParameters.CommandName = commandName;
            objDeviceReqParameters.CallbackUrl = callBackUrl;
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }

        private void PreparePropeties(DeviceRequestParameters objRequestParameters, AccessDTO item, string commandName)
        {
            List<Property> objLstProperties = new List<Property>();
            Property objProperty = new Property();
            switch (commandName)
            {
                case "CardHolderAdd":
                case "CardHolderModify":
                    SetCardHolderProperties(objRequestParameters, item);
                    break;
                case "CardHolderDelete":
                    objProperty = new Property();
                    objProperty.Name = "cardHolderId";
                    objProperty.Value = item.CardHolderId;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Properties = objLstProperties;
                    break;
                case "GetCardHoldersInformation":
                    objProperty = new Property();
                    objProperty.Name = "firstName";
                    objProperty.Value = item.FirstName;
                    objLstProperties.Add(objProperty);

                    objProperty = new Property();
                    objProperty.Name = "cardNumber";
                    objProperty.Value = item.CardNumber;
                    objLstProperties.Add(objProperty);

                    objProperty = new Property();
                    objProperty.Name = "lastName";
                    objProperty.Value = item.LastName;
                    objLstProperties.Add(objProperty);                    
                    objRequestParameters.Properties = objLstProperties;
                    break;
                case "AccessGroupDelete":
                case "GetAccessGroupInformation":                
                    objProperty = new Property();
                    objProperty.Name = "accessGroupId";
                    objProperty.Value = item.AccessGroupId;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Properties = objLstProperties;
                    break;
                case "AccessGroupModify":
                    objProperty = new Property();
                    objProperty.Name = "accessGroupId";
                    objProperty.Value = item.Name;
                    objLstProperties.Add(objProperty);

                    objProperty = new Property();
                    objProperty.Name = "description";
                    objProperty.Value = item.Description;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Properties = objLstProperties;
                    break;
                case "AccessGroupCreate":
                    objProperty = new Property();
                    objProperty.Name = "name";
                    objProperty.Value = item.Name;
                    objLstProperties.Add(objProperty);

                    objProperty = new Property();
                    objProperty.Name = "description";
                    objProperty.Value = item.Description;
                    objLstProperties.Add(objProperty);
                    objRequestParameters.Properties = objLstProperties;
                    break;
                case "MomentaryOpenDoor":
                    objProperty = new Property();
                    objProperty.Name = "readerId";
                    objProperty.Value = item.ReaderId;
                    objLstProperties.Add(objProperty);                    
                    objRequestParameters.Properties = objLstProperties;
                    break;
                case "GetAccessControlReport":
                    objProperty = new Property();
                    objProperty.Name = "startDateTime";
                    objProperty.Value = ConvertoJulianDate(item.StartDateTime);
                    objLstProperties.Add(objProperty);

                    objProperty = new Property();
                    objProperty.Name = "endDateTime";
                    objProperty.Value = ConvertoJulianDate(item.EndDateTime);
                    objLstProperties.Add(objProperty);                    
                    objRequestParameters.Properties = objLstProperties;
                    break;
            }
        }

        private void SetCardHolderProperties(DeviceRequestParameters objRequestParameters, AccessDTO item)
        {
            List<Property> objLstProperties = new List<Property>();
            Property objProperty = new Property();
            objRequestParameters.Property = new Property();

            objProperty.Name = "deviceIdentifier";
            objProperty.Value = item.DeviceIdentifier;
            objLstProperties.Add(objProperty);            

            objProperty = new Property();
            objProperty.Name = "cardHolderId";
            objProperty.Value = item.CardHolderId;
            objLstProperties.Add(objProperty);            

            objProperty = new Property();
            objProperty.Name = "lastName";
            objProperty.Value = item.LastName;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "firstName";
            objProperty.Value = item.FirstName;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "middleName";
            objProperty.Value = item.MiddleName;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "cardNumber";
            objProperty.Value = item.CardNumber;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "cardActive";
            objProperty.Value = item.CardActive;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "pin";
            objProperty.Value = item.Pin;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "accessGroup";
            objProperty.Value = item.AccessGroup;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "cardActivationDate";
            objProperty.Value = ConvertoJulianDate(item.CardActivationDate);
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "cardExpirationDate";
            objProperty.Value = ConvertoJulianDate(item.cardExpirationDate);
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "company";
            objProperty.Value = item.Company;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "department";
            objProperty.Value = item.Department;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "title";
            objProperty.Value = item.Title;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "officePhone";
            objProperty.Value = item.OfficePhone;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "extension";
            objProperty.Value = item.Extension;
            objLstProperties.Add(objProperty);

            objProperty = new Property();
            objProperty.Name = "mobilePhone";
            objProperty.Value = item.MobilePhone;
            objLstProperties.Add(objProperty);
            
            objRequestParameters.Property = new Property();            
            objRequestParameters.Properties = objLstProperties;
        }

        private string ConvertoJulianDate(string datetime)
        {

            if (!String.IsNullOrEmpty(datetime))
            {
                DateTime dt = Convert.ToDateTime(datetime);
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var newTime = Convert.ToInt64((dt.ToUniversalTime() - epoch).TotalSeconds);
                return Convert.ToString(newTime);
            }
            else
                return "0";

        }

        private string PrepareAccessGroupRequest(AccessDTO item, string callBackUrl, string commandName)
        {
            DeviceRequestParameters objDeviceReqParameters = new DeviceRequestParameters();
            objDeviceReqParameters.Property = new Property();
            objDeviceReqParameters.DeviceInstnaceType = "SparkAccessControl";
            objDeviceReqParameters.TimeSent = System.DateTime.UtcNow;
            objDeviceReqParameters.DeviceKey = item.ExternalDeviceKey;
            objDeviceReqParameters.DeviceType = item.DeviceType;
            objDeviceReqParameters.Type = item.DeviceType;                        
            objDeviceReqParameters.CommandName = commandName;
            objDeviceReqParameters.CallbackUrl = callBackUrl;
            if (commandName == "AccessGroupDelete")
            {
                PreparePropeties(objDeviceReqParameters, item, commandName);
            }
            else
            {
                objDeviceReqParameters.AccessGroupName = item.Name;
                objDeviceReqParameters.AccessGroupDesc = item.Description;
                if (commandName == "AccessGroupModify")
                {
                    PreparePropeties(objDeviceReqParameters, item, commandName);
                    objDeviceReqParameters.AccessGroupInformation = item.AccessGroupInformation;
                }
                else
                {
                    objDeviceReqParameters.AccessGroupReaders = item.AccessGroupReaders;
                    objDeviceReqParameters.AccessGroupTimePeriods = item.AccessGroupTimePeriods;
                }
            }
            return new UtilitiesApi().PrepareRequestBody(objDeviceReqParameters);
        }
    }
}
