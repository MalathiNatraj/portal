using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Infrastructure;
using Diebold.Platform.Proxies.DTO;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Services.Exceptions;
using System.Threading;
using Ninject;
using Ninject.Activation.Blocks;
using System.Configuration;
using Diebold.Platform.Proxies.Exceptions;
using System.Web.Script.Serialization;


namespace Diebold.Services.Impl
{
    public class AccessService : IAccessService
    {
        private readonly IDvrService _dvrService;
        private readonly IAccessApiService _accessApiService;
        AccessDTO objAccessDTO = new AccessDTO();        
        AccessResponseDTO response = null;
        int intMaxResTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["APIMaxResponseTime"]);
        int intResCount = 0;
        private string responseMessage = string.Empty;
        AccessPlatformResponseDTO platformresponse = null;

        public AccessService() { }
        [Inject]
        public AccessService(
            IDvrService dvrservice,
            IAccessApiService accessapiservice
            )
        {
            _dvrService = dvrservice;
            _accessApiService = accessapiservice;
        }

        public Access GetAccessDetails(int deviceId)
        {
            SetDeviceInformation(objAccessDTO, deviceId);
            Access objResult = new Access();
            _accessApiService.AccessGetAccessControlStatus(objAccessDTO);
            AccessStatusResponseDTO response = null;

            for (int i = 0; i <= intMaxResTime; i++)
            {
                intResCount++;
                response = (AccessStatusResponseDTO)(System.Web.HttpContext.Current.Application["AccessGetAccessControlStatus" + objAccessDTO.ExternalDeviceKey]);

                if (response != null)
                    break;
                Thread.Sleep(1000);
            }
            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AccessGetAccessControlStatus" + objAccessDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage == "OK")
                    {
                        string networkdown = response.payload.SparkAccessControlReport.properties.property.Where(x => x.name.ToLower() == "networkdown").Select(y => y.value).FirstOrDefault();
                        if (response.payload.SparkAccessControlReport.properties.property != null)
                        {
                            if (networkdown == "false")
                            {
                                objResult.Status = "Connected";
                            }
                            else
                            {
                                objResult.Status = "Not Connected";
                            }
                        }
                        List<DeviceProperty> objLst = new List<DeviceProperty>();
                        foreach (ResponseProperty objProp in response.payload.SparkAccessControlReport.properties.property)
                        {
                            DeviceProperty objDeviceProperty = new DeviceProperty();
                            objDeviceProperty.name = objProp.name;
                            objDeviceProperty.value = objProp.value;
                            objLst.Add(objDeviceProperty);
                        }
                        objResult.Properties = objLst;

                        List<Door> objLstDoor = new List<Door>();
                        Door objDoor = new Door();

                        if (response.payload.SparkAccessControlReport.DoorStatusDataList != null && response.payload.SparkAccessControlReport.DoorStatusDataList.DoorStatusData != null)
                        {
                            foreach (AccessStatusDoorStatusData objDoorStatusData in response.payload.SparkAccessControlReport.DoorStatusDataList.DoorStatusData)
                            {
                                objDoor = new Door();
                                foreach (ResponseProperty objProperty in objDoorStatusData.properties.property)
                                {
                                    if (!String.IsNullOrEmpty(objProperty.name))
                                    {
                                        switch (objProperty.name.ToLower())
                                        {
                                            case "doorname":
                                                objDoor.DoorName = Convert.ToString(objProperty.value);
                                                break;
                                            case "readeronline":
                                                objDoor.Online = Convert.ToString(objProperty.value);
                                                break;
                                            case "doorstatus":
                                                objDoor.DoorStatus = Convert.ToString(objProperty.value);
                                                break;
                                            case "readerid":
                                                objDoor.ReaderId = Convert.ToString(objProperty.value);
                                                break;
                                        }
                                    }
                                }
                                if (string.IsNullOrEmpty(objDoor.DoorName) == false)
                                    objLstDoor.Add(objDoor);
                            }
                        }
                        objResult.DoorList = objLstDoor;
                    }
                    else
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while getting the Access details.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while getting the Access details.");
            }
            return objResult;
        }
        public Access GetPlatformAccessStatus(int deviceId)
        {
            SetPlatformDeviceInformation(objAccessDTO, deviceId);
            Access objResult = new Access();
            string accessPlatformResponse = _accessApiService.GetPlatformAccessStatus(objAccessDTO);
            //SetPlatformAccessResponse("GetPlatformAccessStatus" + objAccessDTO.ExternalDeviceKey);
            if (accessPlatformResponse != "[]")
            {

                JavaScriptSerializer js = new JavaScriptSerializer();
                IList<AccessPlatformResponseDTO> objAccessPlatformResponseDTO = (IList<AccessPlatformResponseDTO>)js.Deserialize(accessPlatformResponse, typeof(IList<AccessPlatformResponseDTO>));
                if (objAccessPlatformResponseDTO != null)
                {
                    platformresponse = objAccessPlatformResponseDTO[0];
                }

                if (platformresponse != null)
                {
                    if (platformresponse.payload != null)
                    {
                        if (platformresponse.payload.SparkAccessControlReport.properties.property != null)
                        {
                            string networkdown = platformresponse.payload.SparkAccessControlReport.properties.property.networkDown;
                            if (networkdown == "false")
                            {
                                objResult.Status = "Connected";
                            }
                            else
                            {
                                objResult.Status = "Not Connected";
                            }
                        }

                        List<DeviceProperty> objLst = new List<DeviceProperty>();
                        DeviceProperty objDeviceProperty = new DeviceProperty();
                        objDeviceProperty.name = "deviceIdentifier";
                        objDeviceProperty.value = platformresponse.payload.SparkAccessControlReport.properties.property.deviceIdentifier;
                        objLst.Add(objDeviceProperty);

                        DeviceProperty objDeviceProperty1 = new DeviceProperty();
                        objDeviceProperty1.name = "networkDown";
                        objDeviceProperty1.value = platformresponse.payload.SparkAccessControlReport.properties.property.networkDown;
                        objLst.Add(objDeviceProperty1);

                        DeviceProperty objDeviceProperty2 = new DeviceProperty();
                        objDeviceProperty2.name = "softwareVersion";
                        objDeviceProperty2.value = platformresponse.payload.SparkAccessControlReport.properties.property.softwareVersion;
                        objLst.Add(objDeviceProperty2);

                        DeviceProperty objDeviceProperty3 = new DeviceProperty();
                        objDeviceProperty3.name = "string1";
                        objDeviceProperty3.value = platformresponse.payload.SparkAccessControlReport.properties.property.string1;
                        objLst.Add(objDeviceProperty3);

                        DeviceProperty objDeviceProperty4 = new DeviceProperty();
                        objDeviceProperty4.name = "string2";
                        objDeviceProperty4.value = platformresponse.payload.SparkAccessControlReport.properties.property.string2;
                        objLst.Add(objDeviceProperty4);

                        DeviceProperty objDeviceProperty5 = new DeviceProperty();
                        objDeviceProperty5.name = "int1";
                        objDeviceProperty5.value = platformresponse.payload.SparkAccessControlReport.properties.property.int1;
                        objLst.Add(objDeviceProperty5);

                        DeviceProperty objDeviceProperty6 = new DeviceProperty();
                        objDeviceProperty6.name = "int2";
                        objDeviceProperty6.value = platformresponse.payload.SparkAccessControlReport.properties.property.int2;
                        objLst.Add(objDeviceProperty6);

                        DeviceProperty objDeviceProperty7 = new DeviceProperty();
                        objDeviceProperty7.name = "bool1";
                        objDeviceProperty7.value = platformresponse.payload.SparkAccessControlReport.properties.property.bool1;
                        objLst.Add(objDeviceProperty7);

                        DeviceProperty objDeviceProperty8 = new DeviceProperty();
                        objDeviceProperty8.name = "bool2";
                        objDeviceProperty8.value = platformresponse.payload.SparkAccessControlReport.properties.property.bool2;
                        objLst.Add(objDeviceProperty8);

                        DeviceProperty objDeviceProperty9 = new DeviceProperty();
                        objDeviceProperty9.name = "ACErrorCode";
                        objDeviceProperty9.value = platformresponse.payload.SparkAccessControlReport.properties.property.ACErrorCode;
                        objLst.Add(objDeviceProperty9);

                        objResult.Properties = objLst;

                        List<Door> objLstDoor = new List<Door>();
                        Door objDoor = new Door();

                        if (platformresponse.payload.SparkAccessControlReport.DoorStatusDataList != null && platformresponse.payload.SparkAccessControlReport.DoorStatusDataList.DoorStatusData != null)
                        {
                            foreach (PlatformAccessStatusDoorStatusData objDoorStatusData in platformresponse.payload.SparkAccessControlReport.DoorStatusDataList.DoorStatusData)
                            {
                                objDoor = new Door();

                                if (objDoorStatusData.properties.property != null)
                                {
                                    if (!String.IsNullOrEmpty(objDoorStatusData.properties.property.doorName))
                                    {
                                        objDoor.DoorName = Convert.ToString(objDoorStatusData.properties.property.doorName);
                                    }
                                    if (!String.IsNullOrEmpty(objDoorStatusData.properties.property.readerOnline))
                                    {
                                        objDoor.Online = Convert.ToString(objDoorStatusData.properties.property.readerOnline);
                                    }
                                    if (!String.IsNullOrEmpty(objDoorStatusData.properties.property.doorStatus))
                                    {
                                        objDoor.DoorStatus = Convert.ToString(objDoorStatusData.properties.property.doorStatus);
                                    }
                                    if (!String.IsNullOrEmpty(objDoorStatusData.properties.property.readerID))
                                    {
                                        objDoor.ReaderId = Convert.ToString(objDoorStatusData.properties.property.readerID);
                                    }
                                }

                                if (string.IsNullOrEmpty(objDoor.DoorName) == false)
                                    objLstDoor.Add(objDoor);
                            }
                        }
                        objResult.DoorList = objLstDoor;
                    }
                }
                else
                {
                    throw new ServiceException("Timed out occurred while getting the Access details.");
                }
            }
            else
            {
                throw new ServiceException("No Current Status Available.");
            }
            return objResult;
        }
        private void SetPlatformAccessResponse(string keyName)
        {
            for (int i = 0; i <= intMaxResTime; i++)
            {
                intResCount++;
                IList<AccessPlatformResponseDTO> lstplatformresponse = (IList<AccessPlatformResponseDTO>)(System.Web.HttpContext.Current.Application[keyName]);
                if (lstplatformresponse != null)
                {
                    platformresponse = lstplatformresponse[0];
                }
                if (platformresponse != null)
                    break;
                Thread.Sleep(10000);
            }
        }
        #region CardHolderAdd
        public string CardHolderAdd(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            objAccessDTO.FirstName = Item.firstName;
            objAccessDTO.LastName = Item.lastName;
            objAccessDTO.CardNumber = Item.cardNumber;
            objAccessDTO.Pin = Item.pin;
            objAccessDTO.AccessGroupId = Item.accessGroupId;
            objAccessDTO.AccessGroup = Item.accessGroupId;
            objAccessDTO.CardActivationDate = Item.cardActivationDate;
            objAccessDTO.cardExpirationDate = Item.cardExpirationDate;
            objAccessDTO.CardActive = Item.isActive;
            _accessApiService.CardHolderAdd(objAccessDTO);

            SetResponse("CardHolderAdd" + objAccessDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("CardHolderAdd" + objAccessDTO.ExternalDeviceKey);
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
                throw new ServiceException("Timed out occurred while adding user code.");
            }
            return responseMessage;
        }
        #endregion

        public IList<AccessGroupList> AccessGetGroupList(int deviceId)
        {
            IList<AccessGroupList> objlstprofile = new List<AccessGroupList>();

            SetDeviceInformation(objAccessDTO, deviceId);
            _accessApiService.AccessGetGroupList(objAccessDTO);
            AccessGroupListResponseDTO response = null;

            for (int i = 0; i <= intMaxResTime; i++)
            {
                intResCount++;
                response = (AccessGroupListResponseDTO)(System.Web.HttpContext.Current.Application["AccessGetGroupList" + objAccessDTO.ExternalDeviceKey]);
                if (response != null)
                    break;
                Thread.Sleep(10000);
            }

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AccessGetGroupList" + objAccessDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage == "OK")
                    {
                        List<AccessGroupList> lstAccessList = new List<AccessGroupList>();
                        foreach (var accessGroupValue in response.payload.SparkAccessControlResponse.properties.propertyList.propertyItem)
                        {
                            AccessGroupList objaccess = new AccessGroupList();
                            objaccess.AccessGroupValue = accessGroupValue.value;
                            lstAccessList.Add(objaccess);
                        }
                        objlstprofile = lstAccessList;
                    }
                    else
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while getting the access group list.");
                    }
                }
            }
            else
            {

                throw new ServiceException("timed out occurred while getting profile number list on platform.");
            }
            return objlstprofile;
        }
              

        #region AccessGroupCreate
        public string AccessGroupCreate(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            objAccessDTO.Name = Item.Name;
            objAccessDTO.Description = Item.Description;
            objAccessDTO.BeginTime = Item.BeginTime;
            objAccessDTO.EndTime = Item.EndTime;
            objAccessDTO.Day = Item.Day;
            objAccessDTO.ReaderId = Item.Reader;
            PrepareAccessGroupReaderObject(objAccessDTO);
            PrepareAccessGroupTimePerioads(objAccessDTO);
            _accessApiService.AccessGroupCreate(objAccessDTO);

            SetResponse("AccessGroupCreate" + objAccessDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AccessGroupCreate" + objAccessDTO.ExternalDeviceKey);

                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while adding access group.");
                    }
                }
            }
            else
            {
                throw new ServiceException("timed out occurred while dding access group.");

            }
            return responseMessage;
        }
        #endregion AccessGroupCreate

        #region dmpXRAccessGroupCreate
        public string dmpXRAccessGroupCreate(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            objAccessDTO.Name = Item.Name;
            objAccessDTO.Description = Item.Description;
            objAccessDTO.BeginTime = Item.BeginTime;
            objAccessDTO.EndTime = Item.EndTime;
            objAccessDTO.Day = Item.Day;
            objAccessDTO.ReaderId = Item.Reader;            
            List<AccessGroupReader> objLstReaders = new List<AccessGroupReader>();
            AccessGroupReader objReader = null;            
            objReader = new AccessGroupReader();
            objReader.ReaderName = "0";
            objReader.ReaderId = "0";
            objLstReaders.Add(objReader);            
            objAccessDTO.AccessGroupReaders = objLstReaders;            
            dmpXRPrepareAccessGroupTimePerioads(objAccessDTO);
            _accessApiService.AccessGroupCreate(objAccessDTO);

            SetResponse("AccessGroupCreate" + objAccessDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AccessGroupCreate" + objAccessDTO.ExternalDeviceKey);

                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while adding access group.");
                    }
                }
            }
            else
            {
                throw new ServiceException("timed out occurred while dding access group.");

            }
            return responseMessage;
        }
        #endregion dmpXRAccessGroupCreate

        #region dmpXRAccessGroupCreate        
        private void dmpXRPrepareAccessGroupTimePerioads(AccessDTO objAccessDTO)
        {
            objAccessDTO.AccessGroupTimePeriods = new List<AccecGroupTimePeriod>();

            if (string.IsNullOrEmpty(objAccessDTO.Day) == false)
            {
                string[] timePerioads = objAccessDTO.Day.Split(',');
                objAccessDTO.Day = objAccessDTO.Day.Replace(",", "");

                AccecGroupTimePeriod objAccessGroupTimePeriod = new AccecGroupTimePeriod();
                objAccessGroupTimePeriod.BeginTime = "0";
                objAccessGroupTimePeriod.EndTime = "0";
                objAccessGroupTimePeriod.Days = objAccessDTO.Day;
                objAccessDTO.AccessGroupTimePeriods.Add(objAccessGroupTimePeriod);

            }
        }
        #endregion dmpXRAccessGroupCreate

        #region dmpXRPrepareAccessGroupReaderObject
        private void dmpXRPrepareAccessGroupReaderObject(AccessDTO objAccessDTO)
        {
            List<AccessGroupReader> objLstReaders = new List<AccessGroupReader>();
            AccessGroupReader objReader = null;

            objReader = new AccessGroupReader();
            objReader.ReaderName = "0";
            objReader.ReaderId = "0";
            objLstReaders.Add(objReader);
            objAccessDTO.AccessGroupReaders = objLstReaders;
        }
        #endregion dmpXRPrepareAccessGroupReaderObject        

        #region dmpXRAccessGroupModify
        public string dmpXRAccessGroupModify(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            objAccessDTO.Name = Item.Name;
            objAccessDTO.Description = Item.Description;
            objAccessDTO.BeginTime = Item.BeginTime;
            objAccessDTO.EndTime = Item.EndTime;
            objAccessDTO.Day = Item.Day;

            objAccessDTO.AccessGroupInformation = new List<AccessDTO>();
            AccessDTO objAccessGroupInfo = null;
            objAccessGroupInfo = new AccessDTO();

            objAccessGroupInfo.Name = Item.Name;
            objAccessGroupInfo.Description = Item.Description;
            objAccessGroupInfo.BeginTime = Item.BeginTime;
            objAccessGroupInfo.EndTime = Item.EndTime;
            objAccessGroupInfo.Day = Item.Day;
            objAccessGroupInfo.ReaderId = Item.Reader;
            objAccessGroupInfo.AccessGroupId = Item.accessGroupId;
            
            dmpXRPrepareAccessGroupReaderObject(objAccessGroupInfo);
            dmpXRPrepareAccessGroupTimePerioads(objAccessGroupInfo);
            objAccessDTO.AccessGroupInformation.Add(objAccessGroupInfo);
            
            _accessApiService.AccessGroupModify(objAccessDTO);

            SetResponse("AccessGroupModify" + objAccessDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AccessGroupModify" + objAccessDTO.ExternalDeviceKey);

                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while modifying access group.");
                    }
                }
            }
            else
            {
                throw new ServiceException("timed out occurred while modifying access group.");

            }
            return responseMessage;
        }
        #endregion dmpXRAccessGroupModify

        private void PrepareAccessGroupTimePerioads(AccessDTO objAccessDTO)
        {
            objAccessDTO.AccessGroupTimePeriods = new List<AccecGroupTimePeriod>();

            if (string.IsNullOrEmpty(objAccessDTO.Day) == false)
            {
                string[] timePerioads = objAccessDTO.Day.Split(',');
                objAccessDTO.Day = objAccessDTO.Day.Replace(",", "");
                if (timePerioads.Length == 10)
                {
                    //if (timePerioads[7] == "1")
                    //    FillAccessGroupTimePriaod(objAccessDTO);
                    //if (timePerioads[8] == "1")
                    //    FillAccessGroupTimePriaod(objAccessDTO);
                    //if (timePerioads[9] == "1")
                    FillAccessGroupTimePriaod(objAccessDTO);
                }
                else
                    throw new ServiceException("Cycle information required.");
            }
        }

        private void FillAccessGroupTimePriaod(AccessDTO objAccessDTO)
        {
            DateTime dt = DateTime.Now;
            string newDate = dt.ToShortDateString();
            string beginDate = ConvertoJulianTime(newDate + " " + objAccessDTO.BeginTime);
            string endDate = ConvertoJulianTime(newDate + " " + objAccessDTO.EndTime);

            AccecGroupTimePeriod objAccessGroupTimePeriod = new AccecGroupTimePeriod();
            objAccessGroupTimePeriod.BeginTime = beginDate;
            objAccessGroupTimePeriod.EndTime = endDate;
            objAccessGroupTimePeriod.Days = objAccessDTO.Day;
            objAccessDTO.AccessGroupTimePeriods.Add(objAccessGroupTimePeriod);            
        }
        
        private string ConvertoJulianTime(string datetime)
        {
            DateTime dt = Convert.ToDateTime(datetime);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var newTime = Convert.ToInt64((dt.ToUniversalTime() - epoch).TotalSeconds);
            return Convert.ToString(newTime);
        }

        private string ConvertFromJulianTime(string dateTime)
        {
            long unixTime = Convert.ToInt64(dateTime);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var newTime = epoch.AddSeconds(unixTime);
            DateTime dt = newTime.ToLocalTime();
            string convertedTime = dt.ToShortTimeString();

            return Convert.ToString(convertedTime);
        }

        private void PrepareAccessGroupReaderObject(AccessDTO objAccessDTO)
        {
            List<AccessGroupReader> objLstReaders = new List<AccessGroupReader>();
            AccessGroupReader objReader = null;
            if (string.IsNullOrEmpty(objAccessDTO.ReaderId) == false)
            {
                string[] readerInformation = objAccessDTO.ReaderId.Split(',');
                if (readerInformation !=null && readerInformation.Length > 0)
                {
                    foreach (string strReaderName in readerInformation)
                    {
                        objReader = new AccessGroupReader();                        
                        string[] readerValues = strReaderName.Split('_');
                        if (readerValues != null && readerValues.Length > 1)
                        {
                            objReader.ReaderName = readerValues[0];
                            objReader.ReaderId = readerValues[1];
                        }
                        objLstReaders.Add(objReader);
                    }
                }
            }
            else
                throw new ServiceException("Reader information required.");

            objAccessDTO.AccessGroupReaders = objLstReaders;
        }

        #region AccessGroupModify

        public string AccessGroupModify(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            objAccessDTO.Name = Item.Name;
            objAccessDTO.Description = Item.Description;
            objAccessDTO.BeginTime = Item.BeginTime;
            objAccessDTO.EndDateTime = Item.EndTime;
            objAccessDTO.Day = Item.Day;
            
            if (string.IsNullOrEmpty(Item.BeginTime) == false)
            {
                string[] lstBeginTime = Item.BeginTime.Split('#');
                string[] lstEndTime = Item.EndTime.Split('#');
                string[] lstDay = Item.Day.Split('#');
                string[] lstReaderId = Item.Reader.Split('#');
                if (lstBeginTime != null && lstBeginTime.Length > 0)
                {
                    objAccessDTO.AccessGroupInformation = new List<AccessDTO>();
                    AccessDTO objAccessGroupInfo = null;
                    for (int i = 0; i < lstBeginTime.Length; i++)
                    {
                        objAccessGroupInfo = new AccessDTO();
                        objAccessGroupInfo.Name = Item.Name;
                        objAccessGroupInfo.Description = Item.Description;
                        objAccessGroupInfo.BeginTime = lstBeginTime[i]; //Item.BeginTime;
                        objAccessGroupInfo.EndTime = lstEndTime[i];
                        objAccessGroupInfo.Day = lstDay[i];
                        objAccessGroupInfo.ReaderId = lstReaderId[i];
                        objAccessGroupInfo.AccessGroupId = Item.accessGroupId;

                        PrepareAccessGroupReaderObject(objAccessGroupInfo);
                        PrepareAccessGroupTimePerioads(objAccessGroupInfo);

                        objAccessDTO.AccessGroupInformation.Add(objAccessGroupInfo);
                    }
                }
            }
            
            _accessApiService.AccessGroupModify(objAccessDTO);

            SetResponse("AccessGroupModify" + objAccessDTO.ExternalDeviceKey);
           
            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AccessGroupModify" + objAccessDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while modifying access group.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while modifying access group.");
            }

            return responseMessage;

        }
        #endregion AccessGroupModify


        #region AccessGroupDelete

        public string AccessGroupDelete(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            objAccessDTO.AccessGroupId = Item.accessGroupId;
            _accessApiService.AccessGroupDelete(objAccessDTO);

            SetResponse("AccessGroupDelete" + objAccessDTO.ExternalDeviceKey);
           
            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AccessGroupDelete" + objAccessDTO.ExternalDeviceKey);

                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("An error occurred while deleteting the access group.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while deleteting the access group.");
            }

            return responseMessage;

        }
        #endregion AccessGroupDelete

        #region GetCardHoldersInformation
        public Access GetCardHoldersInformation(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            Access objAccess = new Access();
            objAccessDTO.FirstName = Item.firstName;
            objAccessDTO.LastName = Item.lastName;
            objAccessDTO.CardNumber = Item.cardNumber;
            _accessApiService.GetCardHoldersInformation(objAccessDTO);
            AccessCardHolderResponseDTO response = null;
            for (int i = 0; i <= intMaxResTime; i++)
            {
                intResCount++;
                response = (AccessCardHolderResponseDTO)(System.Web.HttpContext.Current.Application["GetCardHoldersInformation" + objAccessDTO.ExternalDeviceKey]);
                if (response != null)
                    break;
                Thread.Sleep(10000);
            }

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("GetCardHoldersInformation" + objAccessDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage == "OK")
                    {
                        AccCHList objCardHolder = null;
                        List<AccCHList> objlstCardHolder = new List<AccCHList>();
                        if (response.payload.SparkAccessControlResponse != null && response.payload.SparkAccessControlResponse.cardholderslist != null
                            && response.payload.SparkAccessControlResponse.cardholderslist.CardHolderInformation != null)
                        {
                            foreach (AccessCardHolderInformation cardHollderInfo in response.payload.SparkAccessControlResponse.cardholderslist.CardHolderInformation)
                            {
                                objCardHolder = new AccCHList();

                                foreach (AccessCHProperty cardHollderProperties in cardHollderInfo.properties.property)
                                {
                                    switch (cardHollderProperties.name.ToLower())
                                    {
                                        case "cardholderid":
                                            objCardHolder.CardHolderId = cardHollderProperties.value;
                                            break;
                                        case "lastname":
                                            objCardHolder.lastName = cardHollderProperties.value;
                                            break;
                                        case "firstname":
                                            objCardHolder.firstName = cardHollderProperties.value;
                                            break;
                                        case "middlename":
                                            objCardHolder.middleName = cardHollderProperties.value;
                                            break;
                                        case "cardnumber":
                                            objCardHolder.cardNumber = cardHollderProperties.value;
                                            break;
                                        case "cardactive":
                                            objCardHolder.isActive = cardHollderProperties.value;
                                            break;
                                        case "pin":
                                            objCardHolder.pin = cardHollderProperties.value;
                                            break;
                                        case "accessgroup":
                                            objCardHolder.accessGroupId = cardHollderProperties.value;
                                            break;
                                        case "cardactivationdate":
                                            objCardHolder.cardActivationDate = ConverFromJulianDate(cardHollderProperties.value);
                                            break;
                                        case "cardexpirationdate":
                                            objCardHolder.cardExpirationDate = ConverFromJulianDate(cardHollderProperties.value);
                                            break;
                                        case "company":
                                            objCardHolder.Company = cardHollderProperties.value;
                                            break;
                                        case "department":
                                            objCardHolder.Department = cardHollderProperties.value;
                                            break;
                                        case "title":
                                            objCardHolder.Title = cardHollderProperties.value;
                                            break;
                                        case "officephone":
                                            objCardHolder.OfficePhone = cardHollderProperties.value;
                                            break;
                                        case "extension":
                                            objCardHolder.Extension = cardHollderProperties.value;
                                            break;
                                        case "mobilephone":
                                            objCardHolder.MobilePhone = cardHollderProperties.value;
                                            break;
                                    }
                                }
                                objlstCardHolder.Add(objCardHolder);
                            }
                        }
                        objAccess.AccessCardHolderList = objlstCardHolder;
                    }
                    else
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while getting the card holder information.");
                    }
                }                
            }
            else
            {
                throw new ServiceException("Timed out occurred while getting the card holder information.");
            }
            return objAccess;
        }
        #endregion

        public string CardHolderModify(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            objAccessDTO.CardHolderId = Item.CardHolderId;
            objAccessDTO.LastName = Item.lastName;
            objAccessDTO.FirstName = Item.firstName;
            objAccessDTO.MiddleName = Item.middleName;
            objAccessDTO.CardNumber = Item.cardNumber;
            objAccessDTO.CardActive = Item.isActive;
            objAccessDTO.Pin = Item.pin;
            objAccessDTO.AccessGroup = Item.accessGroupId;
            objAccessDTO.CardActivationDate = Item.cardActivationDate;
            objAccessDTO.cardExpirationDate = Item.cardExpirationDate;
            objAccessDTO.Company = Item.Company;
            objAccessDTO.Department = Item.Department;
            objAccessDTO.Title = Item.Title;
            objAccessDTO.OfficePhone = Item.OfficePhone;
            objAccessDTO.Extension = Item.Extension;
            objAccessDTO.MobilePhone = Item.MobilePhone;

            _accessApiService.CardHolderModify(objAccessDTO);

            SetResponse("CardHolderModify" + objAccessDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("CardHolderModify" + objAccessDTO);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("An error occurred while modifying card holder.");
                    }
                }
            }
            else
            {
                throw new ServiceException("timed out occurred while modifying card holder.");
            }
            return responseMessage;
        }

        #region CardHolderDelete
        public string CardHolderDelete(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            objAccessDTO.FirstName = Item.firstName;
            objAccessDTO.LastName = Item.lastName;
            objAccessDTO.CardNumber = Item.cardNumber;
            objAccessDTO.CardHolderId = Item.CardHolderId;
            _accessApiService.CardHolderDelete(objAccessDTO);
            SetResponse("CardHolderDelete" + objAccessDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("CardHolderDelete" + objAccessDTO.ExternalDeviceKey);

                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("An error occurred while deleting card holder.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while deleting card holder.");
            }
            return responseMessage;
        }
        #endregion

        #region GetAccessReports
        public Access GetAccessControlReport(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            string result = string.Empty;
            List<DeviceProperty> lstIntrusionReport = new List<DeviceProperty>();
            objAccessDTO.StartDateTime = Item.startDateTime;
            objAccessDTO.EndDateTime = Item.endDateTime;
            Access objResult = new Access();
            _accessApiService.AccessGetAccessControlReport(objAccessDTO);

            SetResponse("GetAccessControlReport" + objAccessDTO.ExternalDeviceKey);
            
            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("GetAccessControlReport" + objAccessDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage == "OK")
                    {
                        AccReportList objReport = null;
                        List<AccReportList> objlstReport = new List<AccReportList>();

                        if (response.payload.SparkAccessControlResponse != null && response.payload.SparkAccessControlResponse.AccessControlReportsList != null && response.payload.SparkAccessControlResponse.AccessControlReportsList.AccessControlReport != null)
                        {
                            foreach (AccControlReport objReportItems in response.payload.SparkAccessControlResponse.AccessControlReportsList.AccessControlReport)
                            {
                                objReport = new AccReportList();
                                foreach (ResponseProperty objProperty in objReportItems.properties.property)
                                {
                                    switch (objProperty.name.ToLower())
                                    {
                                        case "type":
                                            objReport.Acctype = objProperty.value;
                                            break;
                                        case "datetime":
                                            objReport.Accdatetime = objProperty.value;
                                            break;
                                        case "user":
                                            objReport.Accuser = objProperty.value;
                                            break;
                                        case "message":
                                            objReport.Accmessage = objProperty.value;
                                            break;
                                    }
                                }
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
                            throw new ServiceException("Error occured while getting access details report.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while getting the access details report.");
            }
            return objResult;

        }
        #endregion
        private string ConverFromJulianDate(string dateTime)
        {
            long unixTime = Convert.ToInt64(dateTime);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            var newTime = epoch.AddSeconds(unixTime);
            DateTime dt = newTime.ToLocalTime();
            return Convert.ToString(dt);
        }

        #region GetReadersList        
        public IList<DeviceProperty> GetReadersList(int deviceId)
        {
            IList<DeviceProperty> objlstprofile = new List<DeviceProperty>();
            SetDeviceInformation(objAccessDTO, deviceId);

            _accessApiService.AccessGetReadersList(objAccessDTO);
            SetResponse("AccessGetReadersList" + objAccessDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AccessGetReadersList" + objAccessDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage == "OK")
                    {
                        List<DeviceProperty> lstAccessList = new List<DeviceProperty>();
                       foreach (AccessNameReaderPropertyCollection AccessNamePropertyitem in response.payload.SparkAccessControlResponse.readerslist.readerinformation)                        
                        {
                            DeviceProperty objreader = new DeviceProperty();
                            foreach (AccessProperty objProperty in AccessNamePropertyitem.properties.property)
                            {

                                if (!String.IsNullOrEmpty(objProperty.name))
                                {
                                    switch (objProperty.name.ToLower())
                                    {
                                        case "readerid":
                                            objreader.value = objProperty.value;
                                            break;
                                        case "readername":
                                            objreader.name = objProperty.value;
                                            break;
                                    }
                                }
                            }
                            if (objreader.name != null && objreader.value != null)
                            {
                                lstAccessList.Add(objreader);
                            }
                        }
                        objlstprofile = lstAccessList;
                    }
                    else
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while getting readers list.");
                    }
                }
            }
            else
            {
                throw new ServiceException("Timed out occurred while getting the readers list.");
            }
            return objlstprofile;
        }
        #endregion

        #region AccessMomentaryOpenDoor
        public string AccessMomentaryOpenDoor(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            objAccessDTO.ReaderId = Item.Reader;
            _accessApiService.AccessMomentaryOpenDoor(objAccessDTO);
            SetResponse("AccessMomentaryOpenDoor" + objAccessDTO.ExternalDeviceKey);

            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("AccessMomentaryOpenDoor" + objAccessDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage != "OK")
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("An error occurred while momentary open doorr.");
                    }
                }
            }
            else
            {
                throw new ServiceException("timed out occurred while momentary open door.");
            }
            return responseMessage;
        }
        #endregion

        #region GetAccessGroupInformation
        public Access GetAccessGroupInformation(Access Item)
        {
            SetDeviceInformation(objAccessDTO, Item.DeviceId);
            Access objResult = new Access();
            objAccessDTO.AccessGroupId = Item.accessGroupId;
            _accessApiService.GetAccessGroupInformation(objAccessDTO);

            AccessGroupResponseDTO response = null;
            for (int i = 0; i <= intMaxResTime; i++)
            {
                intResCount++;
                response = (AccessGroupResponseDTO)(System.Web.HttpContext.Current.Application["GetAccessGroupInformation" + objAccessDTO.ExternalDeviceKey]);
                if (response != null)
                    break;
                Thread.Sleep(10000);
            }
            List<Access> objlstAccess = new List<Access>();
            List<AccGrpList> objLstAG = new List<AccGrpList>();
            AccGrpList objAccG = new AccGrpList();
            if (response != null)
            {
                System.Web.HttpContext.Current.Application.Remove("GetAccessGroupInformation" + objAccessDTO.ExternalDeviceKey);
                if (response.payload != null && response.payload.command_response != null)
                {
                    responseMessage = response.payload.command_response.status.ToUpper();
                    if (responseMessage == "OK")
                    {
                        foreach (AccessProperty objDesc in response.payload.SparkAccessControlResponse.AccessGroupInformation.properties.property)
                        {
                            if (objDesc.name.ToLower() == "description")
                            {
                                objResult.Description = objDesc.value;
                            }
                        }
                        foreach (AccessGrpTimePeriod objTimePeriod in response.payload.SparkAccessControlResponse.AccessGroupInformation.accessgrouptimeperiodlist.accessgrouptimeperiod)
                        {
                            objAccG = new AccGrpList();
                            foreach (AccessProperty objTime in objTimePeriod.properties.property)
                            {
                                switch (objTime.name.ToLower())
                                {
                                    case "begintime":
                                        objAccG.beginTime = ConvertFromJulianTime(objTime.value);
                                        break;
                                    case "endtime":
                                        objAccG.endTime = ConvertFromJulianTime(objTime.value);
                                        break;
                                    case "days":
                                        objAccG.days = objTime.value;
                                        break;
                                }
                            }
                            objAccG.readerId = string.Empty;
                            objAccG.readerName = string.Empty;
                            if (objTimePeriod.properties != null && objTimePeriod.properties.readerslist != null && objTimePeriod.properties.readerslist.readerinformation != null)
                            {
                                foreach (AccessGroupNamePropertyCollection objReaders in objTimePeriod.properties.readerslist.readerinformation)
                                {
                                    //Reader Id = 1,2,3,4
                                    foreach (AccessProperty objReaderValues in objReaders.properties.property)
                                    {
                                        switch (objReaderValues.name.ToLower())
                                        {
                                            case "readerid":
                                                objAccG.readerId = objAccG.readerId + "," + objReaderValues.value;
                                                break;
                                            case "readername":
                                                objAccG.readerName = objAccG.readerName + "," + objReaderValues.value;
                                                break;
                                        }

                                    }
                                }
                            }
                            if (objAccessDTO.DeviceType.ToLower() != "dmpxr100access" && objAccessDTO.DeviceType.ToLower() != "dmpxr500access")
                            {

                                if (!String.IsNullOrEmpty(objAccG.readerId) && !String.IsNullOrEmpty(objAccG.readerId))
                                {
                                    objAccG.readerId = objAccG.readerId.Remove(0, 1);
                                    objAccG.readerName = objAccG.readerName.Remove(0, 1);
                                    objLstAG.Add(objAccG);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(objAccG.days))
                                {
                                    objAccG.readerId = "";
                                    objAccG.readerName = "";
                                    objLstAG.Add(objAccG);
                                }
                            }
                        }
                        objResult.AccessGroupInformation = objLstAG;
                    }
                    else
                    {
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while getting Access Group Information.");
                    }
                }                
            }
            else
            {
                throw new ServiceException("Timed out occured while getting Access Group Information.");
            }
            return objResult;
        }
        #endregion

        private void SetDeviceInformation(AccessDTO objAccessDTO, int deviceId)
        {
            Dvr objDvr = _dvrService.Get(deviceId);
            objAccessDTO.DeviceType = objDvr.DeviceType.ToString();
            objAccessDTO.ExternalDeviceKey = objDvr.Gateway.MacAddress + "-" + objDvr.DeviceKey;
        }
        private void SetPlatformDeviceInformation(AccessDTO objAccessDTO, int deviceId)
        {
            Dvr objDvr = _dvrService.Get(deviceId);
            objAccessDTO.DeviceType = objDvr.DeviceType.ToString();
            objAccessDTO.ExternalDeviceKey = objDvr.ExternalDeviceId;
        }
        private void SetResponse(string keyName)
        {
            for (int i = 0; i <= intMaxResTime; i++)
            {
                intResCount++;
                response = (AccessResponseDTO)(System.Web.HttpContext.Current.Application[keyName]);
                if (response != null)
                    break;
                Thread.Sleep(10000);
            }
        }
    }
}
