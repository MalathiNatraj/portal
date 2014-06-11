using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Platform.Proxies.DTO;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Services.Exceptions;
using System.Web.Script.Serialization;

namespace Diebold.Services.Impl
{
    public class IntrusionService : IIntrusionService
    {
        private readonly IDvrService _dvrService;
        private readonly IIntrusionApiService _intrusionApiService;
        private string responseMessage = string.Empty;

        int intMaxResTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["APIMaxResponseTime"]);
        int intResCount = 0;
        IntrusionResponseDTO response = null;
        IntrusionDTO objIntrusionDTO = new IntrusionDTO();
        IntrusionPlatformResponseDTO platformresponse = null;

        public IntrusionService() { }
        public IntrusionService(IDvrService dvrService, IIntrusionApiService intrusionApiService)
        {
            _dvrService = dvrService;
            _intrusionApiService = intrusionApiService;
        }

        public string UserCodeAdd(Intrusion Item)
        {
            SetDeviceInformation(objIntrusionDTO, Item.DeviceId);
            objIntrusionDTO.UserCode = Item.UserCode;
            objIntrusionDTO.UserName = Item.UserName;
            objIntrusionDTO.ProfileNumber = Item.ProfileNumber;
            objIntrusionDTO.UserNumber = Item.UserNumber;
            objIntrusionDTO.DeviceInstanceId = Item.DeviceInstanceId;

            if (Item.AreasAuthorityLevel != null)
            {
                objIntrusionDTO.AccessLevels = new Dictionary<string, string>();
                foreach (var item in Item.AreasAuthorityLevel)
                {
                    objIntrusionDTO.AccessLevels.Add(item.Key, item.Value);
                }
            }
            _intrusionApiService.UserCodeAdd(objIntrusionDTO);

            var userCodeAdd = (objIntrusionDTO.DeviceType == DeviceType.bosch_D9412GV4.ToString()) ? "UserCodeAdd2" : "UserCodeAdd";
            SetResponse(userCodeAdd + objIntrusionDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove(userCodeAdd + objIntrusionDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while adding used code.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while creating user code on platform.");
            }
            return responseMessage;
        }

        public int MediaCapture(Intrusion Item, DeviceMediaType deviceMediaType)
        {
            SetDeviceInformation(objIntrusionDTO, Item.DeviceId);
            objIntrusionDTO.UserCode = Item.UserCode;
            objIntrusionDTO.UserName = Item.UserName;
            objIntrusionDTO.ProfileNumber = Item.ProfileNumber;
            objIntrusionDTO.UserNumber = Item.UserNumber;
            objIntrusionDTO.DeviceInstanceId = Item.DeviceInstanceId;
            objIntrusionDTO.ZoneNumber = Item.zoneNumber;


            _intrusionApiService.CaptureMedia(objIntrusionDTO, deviceMediaType);

            SetResponse("DeviceMediaCapture" + objIntrusionDTO.ExternalDeviceKey);
            var id = -1;
            if (response != null)
            {
                if (response.payload != null && response.payload.command_response != null)
                {
                    System.Web.HttpContext.Current.Application.Remove("DeviceMediaCapture" + objIntrusionDTO.ExternalDeviceKey);
                    responseMessage = response.payload.command_response.status.ToUpper();

                    if (responseMessage == "OK")
                    {
                        switch (deviceMediaType)
                        {
                            case DeviceMediaType.Video:
                                int.TryParse(response.payload.SparkIntrusionResponse.IntrusionVideoData.properties.property.value, out id);
                                break;
                            case DeviceMediaType.Image:
                                int.TryParse(response.payload.SparkIntrusionResponse.IntrusionImageData.properties.property.value, out id);
                                break;
                            default:
                                break;
                        }
                        //id = response.payload.SparkIntrusionResponse.;
                    }
                    else
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while getting media file.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Error occured while getting media file.");
            }
            return id;

        }

        public IList<ProfileNumberListModel> GetProfileNumberList(int deviceId)
        {
            IList<ProfileNumberListModel> objlstprofile = new List<ProfileNumberListModel>();
            SetDeviceInformation(objIntrusionDTO, deviceId);
            _intrusionApiService.GetProfileNumberList(objIntrusionDTO);

            SetResponse("GetProfileNumberList" + objIntrusionDTO.ExternalDeviceKey);

            if (response != null)
            {
                if (response.payload != null && response.payload.command_response != null)
                {
                    System.Web.HttpContext.Current.Application.Remove("GetProfileNumberList" + objIntrusionDTO.ExternalDeviceKey);
                    responseMessage = response.payload.command_response.status.ToUpper();

                    if (responseMessage == "OK")
                    {
                        List<ProfileNumberListModel> lstProfileNumList = new List<ProfileNumberListModel>();
                        foreach (var ProfileNumber in response.payload.SparkIntrusionResponse.properties.propertyList.propertyitem)
                        {
                            ProfileNumberListModel objIntrusion = new ProfileNumberListModel();
                            objIntrusion.profileNum = ProfileNumber.value;
                            lstProfileNumList.Add(objIntrusion);
                        }
                        objlstprofile = lstProfileNumList;
                    }
                    else
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while getting profile number list.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while getting profile number list.");
            }
            return objlstprofile;
        }

        #region GetUserCodesInformation
        public Intrusion GetUserCodesInformation(Intrusion Item)
        {
            SetDeviceInformation(objIntrusionDTO, Item.DeviceId);
            Intrusion objintrusion = new Intrusion();
            objIntrusionDTO.UserName = Item.UserName;
            objIntrusionDTO.UserCode = Item.UserCode;
            _intrusionApiService.GetUserCodesInformation(objIntrusionDTO);

            //SetResponse("GetUserCodesInformation" + objIntrusionDTO.ExternalDeviceKey);
            IntrusionUserCodeResponseDTO response = null;
            var responseType = (objIntrusionDTO.DeviceType == DeviceType.bosch_D9412GV4.ToString()) ? "GetUserCodesInformation2" : "GetUserCodesInformation";
            string currentid = string.Format("{0}{1}", responseType, objIntrusionDTO.ExternalDeviceKey);
            for (int i = 0; i <= intMaxResTime; i++)
            {
                intResCount++;

                response = (IntrusionUserCodeResponseDTO)(System.Web.HttpContext.Current.Application[currentid]);
                if (response != null)
                    break;
                Thread.Sleep(10000);
            }

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove(currentid);
                if (response.payload != null && response.payload.command_response != null && response.payload.command_response.status.ToUpper() == "OK")
                {
                    UserCodeList objUserCode = null;
                    List<UserCodeList> objlstUserCode = new List<UserCodeList>();
                    if (response.payload.SparkIntrusionResponse != null && ((response.payload.SparkIntrusionResponse.UserCodeInformationList != null 
                        && response.payload.SparkIntrusionResponse.UserCodeInformationList.UserCodeInformation != null) || (response.payload.SparkIntrusionResponse.UserCodeInformationList2 != null 
                        && response.payload.SparkIntrusionResponse.UserCodeInformationList2.UserCodeInformation2 != null)))
                    {
                        if (responseType == "GetUserCodesInformation")
                        {
                            foreach (IntruUserCodeInformation userCodeInfo in response.payload.SparkIntrusionResponse.UserCodeInformationList.UserCodeInformation)
                            {
                                objUserCode = new UserCodeList();
                                foreach (ResponseProperty userCodeProperties in userCodeInfo.properties.property)
                                {
                                    switch (userCodeProperties.name.ToLower())
                                    {
                                        case "usernumber":
                                            objUserCode.UserNumber = userCodeProperties.value;
                                            break;
                                        case "username":
                                            objUserCode.UserName = userCodeProperties.value;
                                            break;
                                        case "usercode":
                                            objUserCode.UserCode = userCodeProperties.value;
                                            break;
                                        case "pincode":
                                            objUserCode.Zip = userCodeProperties.value;
                                            break;
                                        case "profilenumber":
                                            objUserCode.ProfileNumber = userCodeProperties.value;
                                            break;
                                        case "tempdate":
                                            objUserCode.tempDate = userCodeProperties.value;
                                            break;
                                    }
                                }
                                objlstUserCode.Add(objUserCode);
                            }
                        }
                        else if(responseType == "GetUserCodesInformation2") {
                            IntruUserCodeInformation userCodeInfo = response.payload.SparkIntrusionResponse.UserCodeInformationList2.UserCodeInformation2;
                            
                                objUserCode = new UserCodeList();
                                objUserCode.Areas = new Dictionary<string, string>();
                                
                                foreach (ResponseProperty userCodeProperties in userCodeInfo.properties.property)
                                {
                                    switch (userCodeProperties.name.ToLower())
                                    {
                                        case "usernumber":
                                            objUserCode.UserNumber = userCodeProperties.value;
                                            break;
                                        case "username":
                                            objUserCode.UserName = userCodeProperties.value;
                                            break;
                                        case "usercode":
                                            objUserCode.UserCode = userCodeProperties.value;
                                            break;
                                        case "pincode":
                                            objUserCode.Zip = userCodeProperties.value;
                                            break;
                                        case "profilenumber":
                                            objUserCode.ProfileNumber = userCodeProperties.value;
                                            break;
                                        case "tempdate":
                                            objUserCode.tempDate = userCodeProperties.value;
                                            break;
                                    }
                                }
                                foreach (ResponseProperty userCodeProperties in userCodeInfo.AreasAuthorityLevel.properties.property)
                                {
                                    objUserCode.Areas.Add(userCodeProperties.name, userCodeProperties.value);
                                }
                                objlstUserCode.Add(objUserCode);
                            
                        }
                    }
                    objintrusion.UserCodeList = objlstUserCode;
                }
                else
                {
                    if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                        throw new ServiceException(response.payload.messages[0].description);
                    else
                        throw new ServiceException("Error occured while getting user code information.");
                }
            }
            else
            {
                throw new ServiceException("timed out occurred while getting user code information.");
            }
            return objintrusion;
        }
        #endregion

        public string UserCodeModify(Intrusion Item)
        {
            SetDeviceInformation(objIntrusionDTO, Item.DeviceId);
            objIntrusionDTO.UserCode = Item.UserCode;
            objIntrusionDTO.UserName = Item.UserName;
            objIntrusionDTO.ProfileNumber = Item.ProfileNumber;
            objIntrusionDTO.UserNumber = Item.UserNumber;
            objIntrusionDTO.DeviceInstanceId = Item.DeviceInstanceId;

            if (Item.AreasAuthorityLevel != null)
            {
                objIntrusionDTO.AccessLevels = new Dictionary<string, string>();
                foreach (var item in Item.AreasAuthorityLevel)
                {
                    objIntrusionDTO.AccessLevels.Add(item.Key, item.Value);
                }
            }
            _intrusionApiService.UserCodeModify(objIntrusionDTO);

            var userCodeModify = (objIntrusionDTO.DeviceType == DeviceType.bosch_D9412GV4.ToString()) ? "UserCodeModify2" : "UserCodeModify";
            SetResponse(userCodeModify + objIntrusionDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove(userCodeModify + objIntrusionDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while modifying used code.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while modifying user code on platform.");
            }
            return responseMessage;
        }

        public string UserCodeDelete(Intrusion Item)
        {
            SetDeviceInformation(objIntrusionDTO, Item.DeviceId);
            objIntrusionDTO.UserCode = Item.UserCode;
            objIntrusionDTO.UserName = Item.UserName;
            objIntrusionDTO.ProfileNumber = Item.ProfileNumber;
            objIntrusionDTO.UserNumber = Item.UserNumber;
            objIntrusionDTO.DeviceInstanceId = Item.DeviceInstanceId;

            if (Item.AreasAuthorityLevel != null)
            {
                objIntrusionDTO.AccessLevels = new Dictionary<string, string>();
                foreach (var item in Item.AreasAuthorityLevel)
                {
                    objIntrusionDTO.AccessLevels.Add(item.Key, item.Value);
                }
            }
            _intrusionApiService.UserCodeDelete(objIntrusionDTO);

            var userCodeDelete = (objIntrusionDTO.DeviceType == DeviceType.bosch_D9412GV4.ToString()) ? "UserCodeDelete" : "UserCodeDelete";
            SetResponse(userCodeDelete + objIntrusionDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove(userCodeDelete + objIntrusionDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while deleting used code.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while deleting user code on platform.");
            }
            return responseMessage;
        }


        public Intrusion GetIntrusionDetails(int deviceId)
        {
            SetDeviceInformation(objIntrusionDTO, deviceId);
            Intrusion objResult = new Intrusion();
            _intrusionApiService.GetIntrusionStatus(objIntrusionDTO);

            SetResponse("GetIntrusionStatus" + objIntrusionDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("GetIntrusionStatus" + objIntrusionDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage == "OK")
                    {
                        Area objArea = null;
                        Zone objZone = null;
                        string networkdown = response.payload.SparkIntrusionReport.properties.property.Where(x => x.name.ToLower() == "networkdown").Select(y => y.value).FirstOrDefault();

                        if (networkdown.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                        {
                            objResult.Status = "Connected";
                        }
                        else
                        {
                            objResult.Status = "Not Connected";
                        }
                        List<DeviceProperty> objLst = new List<DeviceProperty>();
                        foreach (ResponseProperty objProp in response.payload.SparkIntrusionReport.properties.property)
                        {
                            DeviceProperty objDeviceProperty = new DeviceProperty();
                            objDeviceProperty.name = objProp.name;
                            objDeviceProperty.value = objProp.value;
                            objLst.Add(objDeviceProperty);
                        }
                        objResult.Properties = objLst;
                        List<Area> objLstArea = new List<Area>();
                        if (response.payload.SparkIntrusionReport != null && response.payload.SparkIntrusionReport.AreasStatusList != null && response.payload.SparkIntrusionReport.AreasStatusList.AreaStatus != null)
                        {
                            foreach (IntrusionAreasStatuslist objAreaStatus in response.payload.SparkIntrusionReport.AreasStatusList.AreaStatus)
                            {
                                objArea = new Area();
                                foreach (ResponseProperty objProperty in objAreaStatus.properties.property)
                                {
                                    switch (objProperty.name)
                                    {
                                        case "areaNumber":
                                            objArea.AreaNumber = Convert.ToInt32(objProperty.value);
                                            break;
                                        case "armed":
                                            objArea.Armed = Convert.ToBoolean(objProperty.value);
                                            break;
                                        case "scheduleStatus":
                                            objArea.ScheduleStatus = Convert.ToBoolean(objProperty.value);
                                            break;
                                        case "lateStatus":
                                            objArea.LateStatus = Convert.ToBoolean(objProperty.value);
                                            break;
                                        case "areaName":
                                            objArea.AreaName = objProperty.value;
                                            break;
                                    }
                                }
                                List<Zone> objLstZone = new List<Zone>();

                                if (objAreaStatus.ZonesStatusList != null && objAreaStatus.ZonesStatusList.ZoneStatus != null)
                                {

                                    foreach (Zonestatus objZoneStatus in objAreaStatus.ZonesStatusList.ZoneStatus)
                                    {
                                        objZone = new Zone();
                                        foreach (ResponseProperty objProperty in objZoneStatus.properties.property)
                                        {
                                            switch (objProperty.name)
                                            {
                                                case "zoneNumber":
                                                    objZone.Number = Convert.ToInt32(objProperty.value);
                                                    break;
                                                case "zoneStatus":
                                                    objZone.Status = objProperty.value;
                                                    break;
                                                case "zoneName":
                                                    objZone.Name = objProperty.value;
                                                    break;
                                            }
                                        }
                                        if (string.IsNullOrEmpty(objZone.Name) == false && string.IsNullOrEmpty(objZone.Status) == false)
                                            objLstZone.Add(objZone);
                                    }
                                }
                                objArea.Zones = objLstZone;
                                if (!String.IsNullOrEmpty(objArea.AreaName))
                                    objLstArea.Add(objArea);
                            }
                        }
                        objResult.AreaList = objLstArea;
                    }
                    else
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while getting the intrusion details.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while getting the Intrusion details from platform.");
            }
            return objResult;
        }

        public Intrusion GetPlatformIntrusionDetails(int deviceId)
        {
            SetPlatformDeviceInformation(objIntrusionDTO, deviceId);
            Intrusion objResult = new Intrusion();
            var device = _dvrService.GetDevice(deviceId);
            if (device != null && device is Dvr)
            {
                var dvr = (Dvr)device;
                objResult.DeviceName = dvr.Name;
                objResult.DeviceType = dvr.DeviceType;
                objResult.DeviceId = dvr.Id;

            }
            string intrusionPlatformResponse = _intrusionApiService.GetPlatformIntrusionStatus(objIntrusionDTO);

            // SetPlatformIntrusionResponse("GetPlatformIntrusionStatus" + objIntrusionDTO.ExternalDeviceKey);
            if (intrusionPlatformResponse != "[]")
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                IList<IntrusionPlatformResponseDTO> objIntrusionPlatformResponseDTO = (IList<IntrusionPlatformResponseDTO>)js.Deserialize(intrusionPlatformResponse, typeof(IList<IntrusionPlatformResponseDTO>));
                if (objIntrusionPlatformResponseDTO != null)
                {
                    platformresponse = objIntrusionPlatformResponseDTO[0];
                }

                if (platformresponse != null)
                {
                    System.Web.HttpContext.Current.Application.Remove("GetPlatformIntrusionStatus" + objIntrusionDTO.ExternalDeviceKey);
                    if (platformresponse.payload != null)
                    {
                        Area objArea = null;
                        Zone objZone = null;
                        string networkdown = platformresponse.payload.SparkIntrusionReport.properties.property.networkDown;
                        if (networkdown.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                        {
                            objResult.Status = "Connected";
                        }
                        else
                        {
                            objResult.Status = "Not Connected";
                        }
                        List<DeviceProperty> objLst = new List<DeviceProperty>();

                        DeviceProperty objDeviceProperty = new DeviceProperty();
                        objDeviceProperty.name = "deviceIdentifier";
                        objDeviceProperty.value = platformresponse.payload.SparkIntrusionReport.properties.property.deviceIdentifier;
                        objLst.Add(objDeviceProperty);

                        DeviceProperty objDeviceProperty1 = new DeviceProperty();
                        objDeviceProperty1.name = "networkDown";
                        objDeviceProperty1.value = platformresponse.payload.SparkIntrusionReport.properties.property.networkDown;
                        objLst.Add(objDeviceProperty1);

                        DeviceProperty objDeviceProperty2 = new DeviceProperty();
                        objDeviceProperty2.name = "softwareVersion";
                        objDeviceProperty2.value = platformresponse.payload.SparkIntrusionReport.properties.property.softwareVersion;
                        objLst.Add(objDeviceProperty2);

                        DeviceProperty objDeviceProperty3 = new DeviceProperty();
                        objDeviceProperty3.name = "string1";
                        objDeviceProperty3.value = platformresponse.payload.SparkIntrusionReport.properties.property.string1;
                        objLst.Add(objDeviceProperty3);

                        DeviceProperty objDeviceProperty4 = new DeviceProperty();
                        objDeviceProperty4.name = "string2";
                        objDeviceProperty4.value = platformresponse.payload.SparkIntrusionReport.properties.property.string2;
                        objLst.Add(objDeviceProperty4);

                        DeviceProperty objDeviceProperty5 = new DeviceProperty();
                        objDeviceProperty5.name = "int1";
                        objDeviceProperty5.value = platformresponse.payload.SparkIntrusionReport.properties.property.int1;
                        objLst.Add(objDeviceProperty5);

                        DeviceProperty objDeviceProperty6 = new DeviceProperty();
                        objDeviceProperty6.name = "int2";
                        objDeviceProperty6.value = platformresponse.payload.SparkIntrusionReport.properties.property.int2;
                        objLst.Add(objDeviceProperty6);

                        DeviceProperty objDeviceProperty7 = new DeviceProperty();
                        objDeviceProperty7.name = "bool1";
                        objDeviceProperty7.value = platformresponse.payload.SparkIntrusionReport.properties.property.bool1;
                        objLst.Add(objDeviceProperty7);

                        DeviceProperty objDeviceProperty8 = new DeviceProperty();
                        objDeviceProperty8.name = "bool2";
                        objDeviceProperty8.value = platformresponse.payload.SparkIntrusionReport.properties.property.bool2;
                        objLst.Add(objDeviceProperty8);

                        DeviceProperty objDeviceProperty9 = new DeviceProperty();
                        objDeviceProperty9.name = "intrusionErrorCode";
                        objDeviceProperty9.value = platformresponse.payload.SparkIntrusionReport.properties.property.intrusionErrorCode;
                        objLst.Add(objDeviceProperty9);

                        objResult.Properties = objLst;

                        List<Area> objLstArea = new List<Area>();
                        if (platformresponse.payload.SparkIntrusionReport != null && platformresponse.payload.SparkIntrusionReport.AreasStatusList != null && platformresponse.payload.SparkIntrusionReport.AreasStatusList.AreaStatus != null)
                        {
                            foreach (IntrusionPlatformAreasStatuslist objAreaStatus in platformresponse.payload.SparkIntrusionReport.AreasStatusList.AreaStatus)
                            {
                                objArea = new Area();
                                if (!String.IsNullOrEmpty(objAreaStatus.properties.property.areaNumber))
                                {
                                    objArea.AreaNumber = Convert.ToInt32(objAreaStatus.properties.property.areaNumber);
                                }
                                if (!String.IsNullOrEmpty(objAreaStatus.properties.property.areaNumber))
                                {
                                    objArea.Armed = Convert.ToBoolean(objAreaStatus.properties.property.armed);
                                }
                                if (!String.IsNullOrEmpty(objAreaStatus.properties.property.scheduleStatus))
                                {
                                    objArea.ScheduleStatus = Convert.ToBoolean(objAreaStatus.properties.property.scheduleStatus);
                                }
                                if (!String.IsNullOrEmpty(objAreaStatus.properties.property.lateStatus))
                                {
                                    objArea.LateStatus = Convert.ToBoolean(objAreaStatus.properties.property.lateStatus);
                                }
                                if (!String.IsNullOrEmpty(objAreaStatus.properties.property.areaName))
                                {
                                    objArea.AreaName = objAreaStatus.properties.property.areaName;
                                }

                                List<Zone> objLstZone = new List<Zone>();

                                if (objAreaStatus.ZonesStatusList != null && objAreaStatus.ZonesStatusList.ZoneStatus != null)
                                {

                                    foreach (PlatformZonestatus objZoneStatus in objAreaStatus.ZonesStatusList.ZoneStatus)
                                    {
                                        objZone = new Zone();
                                        ZoneResponseProperty objZoneReponse = objZoneStatus.properties.property;
                                        if (!String.IsNullOrEmpty(objZoneReponse.zoneNumber))
                                        {
                                            objZone.Number = Convert.ToInt32(objZoneReponse.zoneNumber);
                                        }
                                        if (objZoneReponse.zoneStatus != null)
                                        {
                                            objZone.Status = objZoneReponse.zoneStatus;
                                        }
                                        if (objZoneReponse.zoneName != null)
                                        {
                                            objZone.Name = objZoneReponse.zoneName;
                                        }

                                        if (string.IsNullOrEmpty(objZone.Name) == false && string.IsNullOrEmpty(objZone.Status) == false)
                                            objLstZone.Add(objZone);
                                    }
                                }
                                objArea.Zones = objLstZone;
                                if (!String.IsNullOrEmpty(objArea.AreaName))
                                    objLstArea.Add(objArea);
                            }
                        }
                        objResult.AreaList = objLstArea;
                    }
                }
                else
                {
                    throw new ServiceException("Timed out occurred while getting the Intrusion details from platform.");
                }
            }
            else
            {
                throw new ServiceException("No Current Status Available.");
            }
            return objResult;
        }
        public string AreaArm(Intrusion Item)
        {
            SetDeviceInformation(objIntrusionDTO, Item.DeviceId);
            objIntrusionDTO.AreaNumber = Item.AreaNumber;
            _intrusionApiService.AreaArm(objIntrusionDTO);

            SetResponse("AreaArm" + objIntrusionDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AreaArm" + objIntrusionDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while area arm");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while area arm.");
            }
            return responseMessage;
        }

        public string AreaDisArm(Intrusion Item)
        {
            SetDeviceInformation(objIntrusionDTO, Item.DeviceId);
            objIntrusionDTO.AreaNumber = Item.AreaNumber;
            _intrusionApiService.AreaDisarm(objIntrusionDTO);

            SetResponse("AreaDisArm" + objIntrusionDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AreaDisArm" + objIntrusionDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while area disarm");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timeout error occured while area disarm.");
            }
            return responseMessage;
        }

        public string ZoneByPass(Intrusion Item)
        {
            SetDeviceInformation(objIntrusionDTO, Item.DeviceId);
            objIntrusionDTO.ZoneNumber = Item.zoneNumber;
            _intrusionApiService.UserZoneBypass(objIntrusionDTO);

            SetResponse("ZoneBypass" + objIntrusionDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("ZoneBypass" + objIntrusionDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while zone bypass");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while zone bypass.");
            }
            return responseMessage;
        }

        public string ZoneResetByPass(Intrusion Item)
        {
            SetDeviceInformation(objIntrusionDTO, Item.DeviceId);
            objIntrusionDTO.ZoneNumber = Item.zoneNumber;
            _intrusionApiService.UserZoneResetBypass(objIntrusionDTO);

            SetResponse("ZoneResetByPass" + objIntrusionDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("ZoneResetByPass" + objIntrusionDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while zone bypass reset.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while  while zone bypass reset.");
            }
            return responseMessage;
        }

        public Intrusion GetIntrusionReport(Intrusion Item)
        {
            List<DeviceProperty> lstIntrusionReport = new List<DeviceProperty>();
            Intrusion objResult = new Intrusion();
            SetDeviceInformation(objIntrusionDTO, Item.DeviceId);
            objIntrusionDTO.StartDateTime = Item.startDateTime;
            objIntrusionDTO.StartEndTime = Item.endDateTime;
            _intrusionApiService.GetIntrusionReport(objIntrusionDTO);

            SetResponse("GetIntrusionReport" + objIntrusionDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("GetIntrusionReport" + objIntrusionDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage == "OK")
                    {
                        ReportList objReport = null;
                        List<ReportList> objlstReport = new List<ReportList>();

                        if (response.payload.SparkIntrusionResponse != null && response.payload.SparkIntrusionResponse.IntrusionReportsList != null && response.payload.SparkIntrusionResponse.IntrusionReportsList.IntrusionReport != null)
                        {
                            foreach (IntrusionReport objReportItems in response.payload.SparkIntrusionResponse.IntrusionReportsList.IntrusionReport)
                            {
                                objReport = new ReportList();
                                foreach (ResponseProperty objProperty in objReportItems.properties.property)
                                {
                                    switch (objProperty.name.ToLower())
                                    {
                                        case "type":
                                            objReport.type = objProperty.value;
                                            break;
                                        case "datetime":
                                            objReport.datetime = objProperty.value;
                                            break;
                                        case "user":
                                            objReport.user = objProperty.value;
                                            break;
                                        case "message":
                                            objReport.message = objProperty.value;
                                            break;
                                    }
                                }
                                if (objReport.type != null && objReport.user != null)
                                    objlstReport.Add(objReport);
                            }
                        }
                        objResult.ReportList = objlstReport;
                    }
                    else
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occurred while getting intrusion report.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while getting intrusion report.");
            }
            return objResult;
        }

        private void SetDeviceInformation(IntrusionDTO objIntrusionDTO, int deviceId)
        {
            Dvr objDvr = _dvrService.Get(deviceId);
            objIntrusionDTO.DeviceType = objDvr.DeviceType.ToString();
            objIntrusionDTO.ExternalDeviceKey = objDvr.Gateway.MacAddress + "-" + objDvr.DeviceKey;

        }

        private void SetResponse(string keyName)
        {
            for (int i = 0; i <= intMaxResTime; i++)
            {
                intResCount++;
                response = (IntrusionResponseDTO)(System.Web.HttpContext.Current.Application[keyName]);
                if (response != null)
                    break;
                Thread.Sleep(10000);
            }
        }
        private void SetPlatformDeviceInformation(IntrusionDTO objIntrusionDTO, int deviceId)
        {
            Dvr objDvr = _dvrService.Get(deviceId);
            objIntrusionDTO.DeviceType = objDvr.DeviceType.ToString();
            objIntrusionDTO.ExternalDeviceKey = objDvr.ExternalDeviceId;//objDvr.Gateway.MacAddress + "-" + objDvr.DeviceKey;

        }
        private void SetPlatformIntrusionResponse(string keyName)
        {
            for (int i = 0; i <= intMaxResTime; i++)
            {
                intResCount++;
                IList<IntrusionPlatformResponseDTO> lstplatformresponse = (IList<IntrusionPlatformResponseDTO>)(System.Web.HttpContext.Current.Application[keyName]);
                if (lstplatformresponse != null)
                {
                    platformresponse = lstplatformresponse[0];
                }
                if (platformresponse != null)
                    break;
                Thread.Sleep(10000);
            }
        }
    }
}
