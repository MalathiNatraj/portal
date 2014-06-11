using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AutoMapper;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Platform.Proxies.DTO;
using Diebold.Platform.Proxies.Exceptions;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Extensions;
using Diebold.Services.Exceptions;
using Diebold.Platform.Proxies.Enums;
using System.Threading;
using System.Web.Script.Serialization;

namespace Diebold.Services.Impl
{
    public class DeviceService : BaseService, ITrackeableService<Device>, IDeviceService
    {
        private readonly IIntKeyedRepository<Device> _repository;
        private readonly ILogService _logService;
        private readonly IDeviceApiService _deviceApiService;
        private readonly IUserMonitorGroupRepository _monitorGroupRepository;
        private readonly IUserDeviceMonitorRepository _deviceMonitorGroupRepository;

        ResponseDTO response = null;
        int intResCount = 0;
        int intMaxResTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["APIMaxResponseTime"]);
        string strResponseMessage = string.Empty;

        public DeviceService(IIntKeyedRepository<Device> repository,
            IUnitOfWork unitOfWork,
            ILogService logService,
            IDeviceApiService deviceApiService,
            IUserMonitorGroupRepository monitorGroupRepository,
            IUserDeviceMonitorRepository deviceMonitorGroupRepository)
            
            : base(unitOfWork)
        {
            _repository = repository;
            _logService = logService;
            _deviceApiService = deviceApiService;
            _monitorGroupRepository = monitorGroupRepository;
            _deviceMonitorGroupRepository = deviceMonitorGroupRepository;
        }

        public bool DeviceIsEnabled(string name)
        {
            Device device;

            try
            {
                device = _repository.FindBy(u => u.Name == name);
            }
            catch (Exception e)
            {
                throw new Exception("Unknown Device", e);
            }

            return (!device.IsDisabled);
        }

        public StatusDTO GetLiveStatus(int deviceId, string macAddress, bool isGateway, bool EnableLogOperation)
        {
            try
            {
                var device = _repository.FindBy(deviceId);
                var objDeviceDTO = new DeviceDTO(DeviceTypeEnum.SparkGateway, macAddress, null);
                string ack = _deviceApiService.Status(objDeviceDTO, isGateway);
                StatusDTO response = null;               
                for (int i = 0; i <= intMaxResTime; i++)
                {
                    intResCount++;
                    if (isGateway)
                    {
                        response = (StatusDTO)(System.Web.HttpContext.Current.Application["GWStatus" + macAddress]);
                        System.Web.HttpContext.Current.Application.Remove("GWStatus" + macAddress);
                    }
                    else
                    {
                        response = (StatusDTO)(System.Web.HttpContext.Current.Application["DVStatus" + objDeviceDTO.ExternalDeviceKey]);
                        System.Web.HttpContext.Current.Application.Remove("DVStatus" + device.ExternalDeviceId);
                    }

                    if (response != null)
                        break;
                    Thread.Sleep(10000);
                }
                if (response != null && response.payload != null && response.payload.command_response != null && string.IsNullOrEmpty(response.payload.command_response.status) == false)
                {
                    if (response.payload.command_response.status == "OK")
                        return response;
                    else
                        if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                            throw new ServiceException(response.payload.messages[0].description);
                        else
                            throw new ServiceException("Error occured while while getting status.");
                }
                else
                {
                    throw new ServiceException("Timed out occurred while getting status.");
                }
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw e;
            }
        }

        public StatusPlatformDTO GetPlatformLiveStatus(int deviceId, string macAddress, bool isGateway, bool EnableLogOperation)
        {
            try
            {
                var device = _repository.FindBy(deviceId);
                var objDeviceDTO = new DeviceDTO();
                objDeviceDTO.ExternalDeviceKey = macAddress;
                string ack = _deviceApiService.StatusfrmPlatform(objDeviceDTO, isGateway);

                StatusPlatformDTO platformresponse = null;
                JavaScriptSerializer js = new JavaScriptSerializer();
                if (ack != "[]")
                {
                    IList<StatusPlatformDTO> objPlatformStatusDTO = (IList<StatusPlatformDTO>)js.Deserialize(ack, typeof(IList<StatusPlatformDTO>));

                    if (objPlatformStatusDTO != null)
                    {
                        platformresponse = objPlatformStatusDTO[0];
                    }
                    platformresponse = objPlatformStatusDTO[0];

                    if (platformresponse != null && platformresponse.payload != null)
                    {
                        if (isGateway)
                        {
                            platformresponse.isGateWay = "YES";
                        }
                        else
                        {
                            platformresponse.isGateWay = "NO";
                        }
                        return platformresponse;
                    }
                    else
                    {
                        throw new ServiceException("Timed out occurred while getting status from platform.");
                    }
                }
                else
                {
                    throw new ServiceException("No Current Status Available.");
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }        

        public IList<string> GetAllDeviceTypes()
        {
            return Enum.GetNames(typeof(DeviceType)).ToList();
        }

        public IDictionary<string, string> GetAllPollingFrequencies()
        {
            return Enum<PollingFrequency>.EnumToDictionary();
        }

        public IList<string> GetAllHealthCheckVersions()
        {
            return Enum.GetNames(typeof(HealthCheckVersion)).ToList();
        }

        public IList<TimeZoneInfo> GetAllTimeZones()
        {
            return Helpers.TimeZoneHelper.GetTimeZoneList();
        }

        public Device GetByExternalId(string externalId)
        {
            try
            {
                return _repository.FindBy(x => x.DeletedKey == null && x.ExternalDeviceId == externalId);
            }
            catch (Exception)
            {
                throw new ServiceException("Device not found.");
            }
        }

        public void Restart(int pk, string DeviceKey, string DeviceType)
        {
            try
            {
                Device deviceEntity = _repository.Load(pk);
                DeviceDTO objDeviceDTO = new DeviceDTO();
                objDeviceDTO.ExternalDeviceId = deviceEntity.ExternalDeviceId;
               // objDeviceDTO.DeviceType = "SparkGateway";
                objDeviceDTO.DeviceType = DeviceType;
                objDeviceDTO.ExternalDeviceKey = DeviceKey;
                _deviceApiService.Restart(objDeviceDTO, deviceEntity.IsGateway);

                LogOperation((deviceEntity is Gateway) ? LogAction.GatewayRestart : LogAction.DeviceRestart, deviceEntity);

                _unitOfWork.Commit();
            }
            catch (MachineshopPlatformException e)
            {
                _logger.Debug("Device API Restart Error: " + e.GetErrorMessage() + "\r\nJson Response:\r\n" + e.ResponseContent + "\r\nJson Request:\r\n" + e.RequestContent);

                throw new ServiceException("An error occurred while restarting device on platform." + e.GetErrorMessage(), e);
            }
        }

        public void Reload(int pk, string DeviceKey,string DeviceType)
        {
            try
            {
                Device deviceEntity = _repository.Load(pk);
                DeviceDTO objDeviceDTO = new DeviceDTO();
                objDeviceDTO.ExternalDeviceId = deviceEntity.ExternalDeviceId;
                //objDeviceDTO.DeviceType = "SparkGateway";
                objDeviceDTO.DeviceType = DeviceType;
                objDeviceDTO.ExternalDeviceKey = DeviceKey;
                _deviceApiService.Reload(objDeviceDTO, deviceEntity.IsGateway);
                LogOperation((deviceEntity is Gateway) ? LogAction.GatewayReload : LogAction.DeviceReload, deviceEntity);
                _unitOfWork.Commit();
            }
            catch (MachineshopPlatformException e)
            {
                _logger.Debug("Device API Reload Error: " + e.GetErrorMessage() + "\r\nJson Response:\r\n" + e.ResponseContent + "\r\nJson Request:\r\n" + e.RequestContent);

                throw new ServiceException("An error occurred while reloading device on platform." + e.GetErrorMessage(), e);
            }
        }

        public void TestConnection(int pk, string DeviceKey,string DeviceType)
        {
            try
            {
                var deviceEntity = _repository.Load(pk);

                DeviceDTO objDeviceDTO = new DeviceDTO();
                objDeviceDTO.ExternalDeviceId = DeviceKey;
                //objDeviceDTO.DeviceType = "SparkGateway";
                objDeviceDTO.DeviceType = DeviceType;
                objDeviceDTO.ExternalDeviceKey = DeviceKey;

                _deviceApiService.TestConnection(objDeviceDTO, deviceEntity.IsGateway);

                SetReponse("GWTestConnection" + DeviceKey);

                if (response != null)
                {
                    System.Web.HttpContext.Current.Application.Remove("GWTestConnection" + DeviceKey);
                    if (response.payload != null && response.payload.command_response != null)
                    {
                        strResponseMessage = response.payload.command_response.status.ToUpper();
                        if (strResponseMessage != "OK")
                        {
                            if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                                throw new ServiceException(response.payload.messages[0].description);
                            else
                                throw new ServiceException("An error occurred while testing connection.");
                        }
                    }
                    else
                    {
                        throw new ServiceException("An error occurred while testing connection.");
                    }
                }
                else
                {
                    throw new ServiceException("timed out occurred while creating gateway on platform.");
                }
                LogOperation((deviceEntity is Gateway) ? LogAction.GatewayTestConnection : LogAction.DeviceTestConnection, deviceEntity);
            }
            catch (Exception e)
            {
                throw  e;
            }
        }

        public void Audit(int id, string DeviceKey,string DeviceType)
        {
            try
            {
                _logService.Log(LogAction.DeviceAudit);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw new ServiceException("Cannot Audit Gateway", e);
            }
        }

        public IList<int> GetMonitorGroupMatches(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id)
        {
            return _monitorGroupRepository.GetMonitorGroupMatches(gatewayId, siteId, groupingLevel1Id, groupingLevel2Id);
        }

        public IList<UserMonitorGroup> GetUserMonitorGroup(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id)
        {
            return _monitorGroupRepository.GetUserMonitorGroup(gatewayId, siteId, groupingLevel1Id, groupingLevel2Id);
        }

        public void AddDeviceToUserMonitor(IList<UserMonitorGroup> matches, Device device)
        {
            IList<int> userProcessed = new List<int>();
            foreach (var group in matches)
            {
                if (userProcessed.Contains(group.User.Id)) continue;
                _deviceMonitorGroupRepository.Add(new UserDeviceMonitor
                {
                    User = group.User,
                    Device = device,
                    UserMonitorGroup = group
                });
                userProcessed.Add(group.User.Id);
            }
        }

        public Device GetDevice(int id)
        {
            return _repository.FindBy(x => x.Id == id && x.DeletedKey == null);
        }

        public void LogOperation(LogAction action, Device item)
        {
            _logService.Log(action, item.Name, null, null, null, item);
        }

        private static void UpdateStatus(Device device, IList<DeviceStatus> newDeviceStatusList)
        {
            var toUpdate = false;

            IList<DeviceStatus> DeviceStatuslist = newDeviceStatusList.FilterDuplicates<DeviceStatus>();

            if (device is Gateway)
            {
                toUpdate = true;
            }
            else if (device is Dvr)
            {
                var newdate = DeviceStatuslist.Where(x => x.Name == "timeStampAgent").FirstOrDefault();
                var currentDate = device.DeviceStatus.Where(x => x.Name == "timeStampAgent").FirstOrDefault();

                if (device.DeviceStatus == null || device.DeviceStatus.Count == 0)
                {
                    toUpdate = true;
                }
                else
                {
                    if (newdate != null && currentDate != null && (DateTime.Parse(newdate.Value)) > (DateTime.Parse(currentDate.Value)))
                        toUpdate = true;
                }
            }

            if (!toUpdate) return;

            device.DeviceStatus.Clear();
            foreach (var ds in DeviceStatuslist)
            {
                device.DeviceStatus.Add(ds);
            }
        }

        private void SetReponse(string strKey)
        {
            for (int i = 0; i <= intMaxResTime; i++)
            {
                intResCount++;
                response = (ResponseDTO)(System.Web.HttpContext.Current.Application[strKey]);
                if (response != null)
                    break;
                Thread.Sleep(10000);
            }
        }

        #region ITrackeableService<Device> Members

        public void Enable(int pk)
        {
            
        }

        public void Disable(int pk)
        {
            
        }

        #endregion
    }
}
