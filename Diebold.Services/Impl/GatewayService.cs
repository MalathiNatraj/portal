using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Platform.Proxies.DTO;
using Diebold.Platform.Proxies.Enums;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts;
using System.Linq.Dynamic;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Exceptions;
using Diebold.Services.Extensions;
using Diebold.Services.Infrastructure;
using Diebold.Platform.Proxies.Exceptions;
using System.Timers;
using System.Threading;
using System.Web;
using log4net;
using System.Configuration;


namespace Diebold.Services.Impl
{
    public class GatewayService : BaseCRUDTrackeableService<Gateway>, IGatewayService
    {
        private readonly ISiteRepository _siteRepository;
        private readonly IDeviceApiService _deviceApiService;
        private readonly IDeviceService _deviceService;
        private readonly IAlertStatusRepository _alertStatusRepository;
        private readonly IGatewayApiService _gatewayApiService;
        private readonly IDvrService _dvrService;
        private readonly IIntKeyedRepository<Dvr> _dvrRepository;
        private readonly IAlertInfoRepository _alertInfoRepository;
        private readonly IIntKeyedRepository<ResolvedAlert> _resolvedAlertRepository;
        private readonly IUserMonitorGroupRepository _monitorGroupRepository;
        private readonly IIntKeyedRepository<AlarmConfiguration> _alarmRepository;
        public GatewayService(IIntKeyedRepository<Gateway> repository,
            IIntKeyedRepository<Dvr> dvrRepository,
            ISiteRepository siteRepository,
            IUnitOfWork unitOfWork, 
            IValidationProvider validationProvider,
            IGatewayApiService gatewayApiService,
            IDeviceApiService deviceApiService,
            IDeviceService deviceService,
            IAlertStatusRepository alertStatusRepository,
            ILogService logService,
            IDvrService dvrService,
            IAlertInfoRepository alertInfoRepository,
            IIntKeyedRepository<ResolvedAlert> resolvedAlertRepository,
            IUserMonitorGroupRepository monitorGroupRepository,
            IIntKeyedRepository<AlarmConfiguration> alarmRepository)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            _siteRepository = siteRepository;
            this._deviceApiService = deviceApiService;
            this._deviceService = deviceService;
            _alertStatusRepository = alertStatusRepository;
            this._gatewayApiService = gatewayApiService;
            this._dvrService = dvrService;
            _dvrRepository = dvrRepository;
            _alertInfoRepository = alertInfoRepository;
            _resolvedAlertRepository = resolvedAlertRepository;
            _monitorGroupRepository = monitorGroupRepository;
            _alarmRepository = alarmRepository;
        }

        #region IDeviceService

        public bool DeviceIsEnabled(string name)
        {
            return _deviceService.DeviceIsEnabled(name);
        }
        
        public Device GetByExternalId(string externalId)
        {
            return _deviceService.GetByExternalId(externalId);
        }

        public void Restart(int pk, string DeviceKey, string DeviceType)
        {
            _deviceService.Restart(pk, DeviceKey, DeviceType);
        }

        public void Reload(int pk, string DeviceKey, string DeviceType)
        {
            _deviceService.Reload(pk, DeviceKey, DeviceType);
        }

        public void TestConnection(int pk, string DeviceKey,string DeviceType)
        {
            _deviceService.TestConnection(pk, DeviceKey,DeviceType);
        }

        public void Audit(int pk, string DeviceKey,string DeviceType)
        {
            _deviceService.TestConnection(pk, DeviceKey, DeviceType);
        }

        public IList<int> GetMonitorGroupMatches(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id)
        {
            return _deviceService.GetMonitorGroupMatches(gatewayId, siteId, groupingLevel1Id, groupingLevel2Id);
        }

        public IList<UserMonitorGroup> GetUserMonitorGroup(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id)
        {
            return _deviceService.GetUserMonitorGroup(gatewayId, siteId, groupingLevel1Id, groupingLevel2Id);
        }

        public void AddDeviceToUserMonitor(IList<UserMonitorGroup> matches, Device device)
        {
            _deviceService.AddDeviceToUserMonitor(matches, device);
        }

        public Device GetDevice(int id)
        {
            return _deviceService.GetDevice(id);
        }

        public StatusDTO GetLiveStatus(int deviceId, string macAddress, bool liveFromDevice, bool EnableLogOperation)
        {
            return _deviceService.GetLiveStatus(deviceId,macAddress, liveFromDevice,EnableLogOperation);
        }
        public StatusPlatformDTO GetPlatformLiveStatus(int deviceId, string macAddress, bool liveFromDevice, bool EnableLogOperation)
        {
            return _deviceService.GetPlatformLiveStatus(deviceId, macAddress, liveFromDevice, EnableLogOperation);
        }
        public IList<string> GetAllDeviceTypes()
        {
            return _deviceService.GetAllDeviceTypes();
        }

        public IDictionary<string, string> GetAllPollingFrequencies()
        {
            return _deviceService.GetAllPollingFrequencies();
        }

        #endregion
        
        #region Get

        public IList<Protocol> GetProtocols()
        {
            var enabledProtocols = new List<Protocol> { 
                new Protocol { Id = 1, Name = "Gateway Version 1" },
                new Protocol { Id = 2, Name = "Gateway Version 2" }
            }.OrderBy(x => x.Id).ToList();
            return enabledProtocols;
        }

        public IList<TimeZoneInfo> GetAllTimeZones()
        {
            return Helpers.TimeZoneHelper.GetTimeZoneList().OrderBy(x => x.DisplayName).ToList();
        }

        public IList<Gateway> GetAll(int pageNumber, int pageSize, string sortBy, bool ascending, out int recordCount, string whereCondition)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);

                query = query.Where(x => x.Name.Contains(whereCondition)
                    || x.Company.Name.Contains(whereCondition));

            recordCount = query.Count();

            string orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));

            return query.OrderBy(orderBy).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public Page<Gateway> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition, string status = null)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);

            if (!string.IsNullOrEmpty(whereCondition))
            {
                query = query.Where(x => x.Name.Contains(whereCondition)
                                         || x.Company.Name.Contains(whereCondition));
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.IsDisabled == bool.Parse(status));
            }

            var orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));
            return query.OrderBy(orderBy).ToPage(pageNumber, pageSize);
        }


        //public IList<string> GetEnabledMacAddress(int pageNumber, int pageSize, string sortBy, bool ascending, out int recordCount)
        //{
        //    var macList = _gatewayApiService.GetUnassignedMACAddresses();

        //    recordCount = macList.Count();

        //    return macList;
        //}

        public IList<Gateway> GetAllByStatus(string whereCondition)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);

            if (!string.IsNullOrEmpty(whereCondition))
            {
                query = query.Where(x => x.IsDisabled == bool.Parse(whereCondition));
            }
            return query.ToList();
        }

        public IList<string> GetEnabledMacAddress()
        {
            return _gatewayApiService.GetUnassignedMACAddresses();
        }

        #endregion

        #region CRUD

        public override void Create(Gateway item)
        {
            DeviceDTO objDeviceDto = new DeviceDTO();
            try
            {
                // Validate whether the Mac address is already Used or not
                bool IsMacAddressUsed = ValidateMacAddress(item.MacAddress);
                if (IsMacAddressUsed == false)
                {
                    _logger.Debug("Inside call");
                    var configurationDto = new ConfigurationDTO();

                    var deviceDto = new DeviceDTO(DeviceTypeEnum.SparkGateway, item.MacAddress, configurationDto);
                    objDeviceDto = deviceDto;
                    objDeviceDto.OnLine = true;
                    string strAck = _deviceApiService.AddDevice(deviceDto, true);
                    _logger.Debug("Ack------------->: " + strAck);

                    string[] strAckArray = strAck.Split(':');
                    string strExternalDeviceId = strAckArray[1];
                    _logger.Debug("External Device Id " + strExternalDeviceId);
                    strExternalDeviceId = strExternalDeviceId.Remove(0, 1);
                    _logger.Debug("Removed the preceiding double quotes " + strExternalDeviceId);
                    // strExternalDeviceId = strExternalDeviceId.Remove(strExternalDeviceId.Length - 1, 1);
                    string[] tempExternalDeviceId = strExternalDeviceId.Split('"');
                    strExternalDeviceId = string.Empty;
                    strExternalDeviceId = tempExternalDeviceId[0];
                    _logger.Debug("Removed the succeeding double quotes " + strExternalDeviceId);

                    ResponseDTO response = null;
                    int intResCount = 0;
                    int intMaxResTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["APIMaxResponseTime"]);
                    for (int i = 0; i <= intMaxResTime; i++)
                    {
                        intResCount++;
                        response = (ResponseDTO)(System.Web.HttpContext.Current.Application["GWCreate" + item.MacAddress]);

                        if (response != null)
                            break;
                        Thread.Sleep(10000);
                    }

                    if (response != null)
                    {
                        _logger.Debug("Reached call from callback");
                        System.Web.HttpContext.Current.Application.Remove("GWCreate" + item.MacAddress);
                        if (response.payload != null && response.payload.command_response != null && response.payload.command_response.status.ToUpper() == "OK")
                        {
                           // item.ExternalDeviceId = response.payload.txid;
                            // External device Id need to be the _Id which is obtained from the ack
                            item.ExternalDeviceId = strExternalDeviceId;
                        }
                        else
                        {
                            _logger.Debug("Error creating gateway in API: " + item.Id);
                            throw new ServiceException("An error occurred while creating gateway on platform.");
                        }
                    }
                    else
                    {
                        _logger.Debug("API Gateway creation timed out exception " + item.Id);
                        throw new ServiceException("timed out occurred while creating gateway on platform.");
                    }

                    item.LastUsedEmcZoneNumber = 1;

                    //Add new Alarm Configuration.
                    var alarmConfig = new AlarmConfiguration
                                          {
                                              AlarmType = AlarmType.NetworkDown,
                                              DataType = DataType.Boolean,
                                              Device = item,
                                              DeviceType = null,
                                              Email = false,
                                              Emc = false,
                                              Log = false,
                                              Operator = AlarmOperator.Equals,
                                              Severity = AlarmSeverity.Warning,
                                              Threshold = "true"
                                          };

                    item.AlarmConfigurations.Add(alarmConfig);
                    using (var scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        _repository.Add(item);
                        var alertStatus = new AlertStatus
                        {
                            Device = item,
                            Alarm = alarmConfig
                        };
                        _alertStatusRepository.Add(alertStatus);
                        LogOperation(LogAction.GatewayCreate, item);
                        _unitOfWork.Commit();

                        scope.Complete();
                    }
                }
                else
                {
                    throw new Exception("Mac Address Already Used");
                }
            }
            catch (MachineshopPlatformException e)
            {
                _logger.Error("Gateway API Create Error: " + e);

                throw new ServiceException("An error occurred while creating gateway on platform." + e.GetErrorMessage(), e);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();

                if (item.ExternalDeviceId != null)
                    _deviceApiService.RemoveDevice(objDeviceDto, true);
                _logger.Error("Gateway Local Create Error: " + e);

                throw new ServiceException("An error occurred while creating gateway.", e);
            }
        }

        private bool ValidateMacAddress(string strMacAddress)
        {
           IList<Gateway> lstGateway = _repository.All().Where(x => x.MacAddress.Equals(strMacAddress) && x.DeletedKey == null).ToList();
           if (lstGateway != null && lstGateway.Count() > 0)
           {
               return true;
           }
           else
           {
               return false;
           }
        }
        public override void Update(Gateway item)
        {
            updateGateway(item, true);
        }


        private void updateGateway(Gateway item, bool enable)
        {
            try
            {
                item.Touch();
                _validationProvider.Validate(item);


                var configurationDto = new ConfigurationDTO();
                
                
                // Update device into platform 
                var deviceDto = new DeviceDTO(DeviceTypeEnum.SparkGateway, item.MacAddress, configurationDto)
                {
                    ExternalDeviceId = item.ExternalDeviceId
                };

                deviceDto.OnLine = enable;

                _deviceApiService.ModifyDevice(deviceDto, true);

                ResponseDTO response = null;
                int intResCount = 0;
                int intMaxResTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["APIMaxResponseTime"]);
                _logger.Debug("Processed Request");
                for (int i = 0; i <= intMaxResTime; i++)
                {
                    intResCount++;
                    response = (ResponseDTO)(System.Web.HttpContext.Current.Application["GWUpdate" + item.MacAddress]);

                    if (response != null)
                        break;
                    Thread.Sleep(10000);
                }

                if (response != null)
                {
                    System.Web.HttpContext.Current.Application.Remove("GWUpdate" + item.MacAddress);
                    if (response.payload != null && response.payload.command_response != null && response.payload.command_response.status.ToUpper() == "OK")
                    {

                        using (var scope = new TransactionScope(TransactionScopeOption.Required))
                        {
                            _repository.Update(item);
                            LogOperation(LogAction.GatewayEdit, item);
                            _unitOfWork.Commit();
                            scope.Complete();
                        }
                    }
                    else
                    {
                        _logger.Debug("Error updating gateway in API: " + item.Id);
                        throw new ServiceException("An error occurred while editing gateway on platform.");
                    }
                }
                else
                {
                    _logger.Debug("API Gateway update timed out exception " + item.Id);
                    throw new ServiceException("Timedout occurred while editing gateway on platform.");
                }
            }
            catch (MachineshopPlatformException e)
            {
                _logger.Debug("Gateway API Update Error: " + e.GetErrorMessage() + "\r\nJson Response:\r\n" + e.ResponseContent + "\r\nJson Request:\r\n" + e.RequestContent);

                throw new ServiceException("An error occurred while editing gateway on platform." + e.GetErrorMessage(), e);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _logger.Debug("Gateway Local Update Error: " + e.Message);

                throw new ServiceException("An error occurred while editing gateway.", e);
            }
        }

        public override void Delete(int pk)
        {
            try
            {
                var itemToDelete = _repository.Load(pk);
                var configurationDto = new ConfigurationDTO();
                var deviceDto = new DeviceDTO(DeviceTypeEnum.SparkGateway, itemToDelete.MacAddress, configurationDto);
                deviceDto.ExternalDeviceKey=itemToDelete.MacAddress;
                _deviceApiService.RemoveDevice(deviceDto, true);

                ResponseDTO response = null;
                int intResCount = 0;
                int intMaxResTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["APIMaxResponseTime"]);
                _logger.Debug("Processed Request");
                for (int i = 0; i <= intMaxResTime; i++)
                {
                    intResCount++;
                    response = (ResponseDTO)(System.Web.HttpContext.Current.Application["GWDelete" + itemToDelete.MacAddress]);

                    if (response != null)
                        break;
                    Thread.Sleep(10000);
                }
                if (response != null)
                {
                    System.Web.HttpContext.Current.Application.Remove("GWDelete" + itemToDelete.MacAddress);
                    if (response.payload != null && response.payload.command_response != null && response.payload.command_response.status.ToUpper() == "OK")
                    {
                        using (var scope = new TransactionScope(TransactionScopeOption.Required))
                        {
                            DeleteDevicesfrmPlatform(pk);
                            _logger.Debug("Delete Gateway from platform Started" + itemToDelete.ExternalDeviceId);
                            string strGatewayAck = _deviceApiService.RemoveDevicefromPlatform(null, true, itemToDelete.ExternalDeviceId);
                            _logger.Debug("Completed Delete Gateway from platform" + strGatewayAck);
                            itemToDelete.LogicalDelete();
                            _repository.Update(itemToDelete);
                            LogOperation(LogAction.GatewayDelete, itemToDelete);

                            _unitOfWork.Commit();
                            scope.Complete();
                        }
                    }
                    else
                    {
                        _logger.Debug("Error deleting gateway in API: " + itemToDelete.Id);
                        throw new ServiceException("An error occurred while deleting gateway on platform.");
                    }
                }
                else
                {
                    _logger.Debug("API Gateway delete timed out exception: " + itemToDelete.Id);
                    throw new ServiceException("Timedout error occurred while deleting gateway on platform.");
                }
            }
            catch (MachineshopPlatformException e)
            {
                _logger.Debug("Gateway API Delete Error: " + e.GetErrorMessage() + "\r\nJson Response:\r\n" + e.ResponseContent + "\r\nJson Request:\r\n" + e.RequestContent);

                throw new ServiceException("An error occurred while deleting gateway on platform." + e.GetErrorMessage(), e);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();

                _logger.Debug("Gateway Local Delete Error: " + e.Message);

                throw new ServiceException("An error occurred while deleting gateway.", e);
            }

        }
        private void DeleteDevicesfrmPlatform(int pk)
        {
            _logger.Debug("Inside Delete Devices from Platform");
            IList<Dvr> objlstDvr = _dvrService.GetDevicesByGatewayId(pk);
            if (objlstDvr.Count > 0)
            {
                _logger.Debug("Delete device loop started");
                foreach (Dvr deviceItem in objlstDvr)
                {
                    Dvr objDvr = deviceItem;
                    deviceItem.LogicalDelete();
                    _dvrRepository.Update(deviceItem);
                    _logger.Debug("User Monitor Group update started");
                    UpdateUserMonitorGroup(deviceItem);
                    _logger.Debug("User Monitor Group update completed");
                    var ActiveAlertQuery = (from alert in _alertStatusRepository.All() where alert.Device.Id == deviceItem.Id select alert);
                    _logger.Debug("Delete Alerts started");
                    if (ActiveAlertQuery.Count() > 0)
                    {
                        IEnumerable<AlertStatus> ActiveAlert = (IEnumerable<AlertStatus>)ActiveAlertQuery.ToList();
                        if (ActiveAlert != null && ActiveAlert.Count() > 0)
                            _alertStatusRepository.Delete(ActiveAlert);
                    }
                    _logger.Debug("Delete Alerts completed");
                    var lstalertinfo = (from alertinfo in _alertInfoRepository.All() where alertinfo.Device.Id == deviceItem.Id select alertinfo);
                    _logger.Debug("Delete Alert Info started");
                    if (lstalertinfo.Count() > 0)
                    {
                        IEnumerable<AlertInfo> lstalert = (IEnumerable<AlertInfo>)lstalertinfo.ToList();
                        if (lstalert != null && lstalert.Count() > 0)
                            _alertInfoRepository.Delete(lstalert);
                    }
                    _logger.Debug("Delete Alert Info completed");

                    var lstResolvedAlertinfo = (from resolvedalert in _resolvedAlertRepository.All() where resolvedalert.Device.Id == deviceItem.Id select resolvedalert);
                    _logger.Debug("Delete Resolved Alert started");
                    if (lstResolvedAlertinfo.Count() > 0)
                    {
                        IEnumerable<ResolvedAlert> resolvedalertinfo = (IEnumerable<ResolvedAlert>)lstResolvedAlertinfo.ToList();
                        if (resolvedalertinfo != null && resolvedalertinfo.Count() > 0)
                            _resolvedAlertRepository.Delete(resolvedalertinfo);
                    }
                    _logger.Debug("Delete Resolved Alert completed");
                    DeleteAlarmConfiguration(deviceItem);
                    // some time Plateform API call take some time to execute.If API have taken more time to execute, than the Logoperation method will throw the time out error.So API call should call as last last statement.
                    // LogOperation(LogAction.DeviceDelete, itemToDelete);

                    // Deleting the device from platform
                    _logger.Debug("Delete device from platform Started" + deviceItem.ExternalDeviceId);
                    string strDvAck = _deviceApiService.RemoveDevicefromPlatform(null, false, deviceItem.ExternalDeviceId);
                    _logger.Debug("Delete device from platform completed" + strDvAck);
                }
                _logger.Debug("Delete device loop completed");
            }
            _logger.Debug("Completed Delete Devices from Platform");
        }

        public int GetDeviceCountByGatewayId(int gatewayId)
        {
            IList<Dvr> objlstDvr = _dvrService.GetDevicesByGatewayId(gatewayId);
            return objlstDvr.Count();
        }
        private void UpdateUserMonitorGroup(Dvr item)
        {
            IList<UserMonitorGroup> lstGroup = _monitorGroupRepository.GetByDeviceId(item.Id);
            if (lstGroup.Count() > 0)
            {
                foreach (UserMonitorGroup grp in lstGroup)
                {
                    grp.Device = null;
                    grp.Site = item.Site;
                    //TODO: This should be deleted - as tem fix provided as an update due to transaction error.
                    _monitorGroupRepository.Update(grp);
                }
            }
        }
        private void DeleteAlarmConfiguration(Dvr item)
        {
            _logger.Debug("Delete Alarm Configuration started in Gateway Service.(Alarm parent type updated as Deleted)");
            IList<AlarmConfiguration> lstAlarms = item.AlarmConfigurations;
            if (lstAlarms.Count() > 0)
            {
                foreach (AlarmConfiguration AlarmConfig in lstAlarms)
                {
                    AlarmConfig.AlarmParentType = AlarmParentType.Deleted;
                    //TODO: This should be deleted - as tem fix provided as an update due to transaction error.                    
                    _alarmRepository.Update(AlarmConfig);
                }
            }
            _logger.Debug("Delete Alarm Configuration completed in Gateway Service.(Alarm parent type updated as Deleted)");
        }
        public override void Enable(int pk)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    Gateway entityToEnable = _repository.Load(pk);
                    entityToEnable.Enable();
                    entityToEnable.Touch();
                    updateGateway(entityToEnable, true);
                    LogOperation(LogAction.GatewayEnable, entityToEnable);
                    _unitOfWork.Commit();
                }
                catch (MachineshopPlatformException e)
                {
                    _logger.Debug("Gateway API Enable Error: " + e.GetErrorMessage() + "\r\nJson Response:\r\n" + e.ResponseContent + "\r\nJson Request:\r\n" + e.RequestContent);
                    
                    throw new ServiceException("An error occurred while enabling gateway on platform." + e.GetErrorMessage(), e);
                }
                catch(Exception e)
                {
                    _unitOfWork.Rollback();
                    
                    _logger.Debug("Gateway Local Enable Error: " + e.Message);

                    throw new ServiceException("An error occurred while enabling gateway.", e);
                }

                scope.Complete();
            }
        }

        public override void Disable(int pk)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    var entityToEnable = _repository.Load(pk);
                    entityToEnable.Disable();
                    entityToEnable.Touch();
                    updateGateway(entityToEnable, false);
                    LogOperation(LogAction.GatewayDisable, entityToEnable);

                    _unitOfWork.Commit();
                }
                catch (MachineshopPlatformException e)
                {
                    _logger.Debug("Gateway API Disable Error: " + e.GetErrorMessage() + "\r\nJson Response:\r\n" + e.ResponseContent + "\r\nJson Request:\r\n" + e.RequestContent);
                    
                    throw new ServiceException("An error occurred while disabling gateway on platform." + e.GetErrorMessage(), e);
                }
                catch(Exception e)
                {
                    _unitOfWork.Rollback();
                    
                    _logger.Debug("Gateway Local Disable Error: " + e.Message);

                    throw new ServiceException("An error occurred while disabling gateway.", e);
                }

                scope.Complete();
            }
        }

        #endregion

        public override void LogOperation(LogAction action, Gateway item)
        {
            //Force loading of site.
            //var site = item.Site = _siteRepository.FindBy(item.Site.Id);
            //var companyGrouping2 = site.CompanyGrouping2Level;
            //var companyGrouping1 = companyGrouping2.CompanyGrouping1Level;
            //_logService.Log(action, item.Name, companyGrouping1, companyGrouping2, site);
        }

        public void RevokeCerificate(int pk)
        {
            try
            {
                var itemToRevoke = _repository.Load(pk);                
                DeviceDTO objDeviceDTAO = new DeviceDTO();
                objDeviceDTAO.ExternalDeviceKey = itemToRevoke.MacAddress;
                _gatewayApiService.RevokeDevice(objDeviceDTAO);
                ResponseDTO response = null;
                int intResCount = 0;
                int intMaxResTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["APIMaxResponseTime"]);
                _logger.Debug("Processed Request");
                for (int i = 0; i <= intMaxResTime; i++)
                {
                    intResCount++;
                    response = (ResponseDTO)(System.Web.HttpContext.Current.Application["GWRevoke" + itemToRevoke.MacAddress]);

                    if (response != null)
                        break;
                    Thread.Sleep(10000);
                }

                if (response != null)
                {
                    System.Web.HttpContext.Current.Application.Remove("GWRevoke" + itemToRevoke.MacAddress);
                    if (response.payload != null && response.payload.command_response != null && response.payload.command_response.status.ToUpper() == "OK")
                    {
                        using (var scope = new TransactionScope(TransactionScopeOption.Required))
                        {
                            DeleteDevicesfrmPlatform(pk);
                            _logger.Debug("Delete Gateway from platform Started" + itemToRevoke.ExternalDeviceId);
                            string strGatewayAck = _deviceApiService.RemoveDevicefromPlatform(null, true, itemToRevoke.ExternalDeviceId);
                            _logger.Debug("Completed Delete Gateway from platform" + strGatewayAck);
                            itemToRevoke.LogicalDelete();
                            _repository.Update(itemToRevoke);
                            LogOperation(LogAction.GatewayDelete, itemToRevoke);
                            _unitOfWork.Commit();

                            scope.Complete();
                        }
                    }
                    else
                    {
                        _logger.Debug("Error revoke gateway in API: " + itemToRevoke.Id);
                        throw new ServiceException("An error occurred while deleting gateway on platform.");
                    }
                }
                else
                {
                    _logger.Debug("API Gateway revoke timed out exception: " + itemToRevoke.Id);
                    throw new ServiceException("Timedout error occurred while deleting gateway on platform.");
                }
            }
            catch (MachineshopPlatformException e)
            {
                _logger.Debug("Gateway API Revoke Error: " + e.GetErrorMessage() + "\r\nJson Response:\r\n" +
                              e.ResponseContent + "\r\nJson Request:\r\n" + e.RequestContent);

                throw new ServiceException(
                    "An error occurred while deleting gateway on platform." + e.GetErrorMessage(), e);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();

                _logger.Debug("Gateway Local Revoke Error: " + e.Message);

                throw new ServiceException("An error occurred while deleting gateway.", e);
            }
        }

        public IList<Gateway> GetAllActiveGateway()
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);
            return query.ToList();
        }

        public IList<Gateway> GetGatewaysByCompanyId(int companyId, bool showDisabled)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null && x.Company.Id == companyId);

            if (!showDisabled)
                query = query.Where(x => x.IsDisabled == false);

            return query.ToList();
        }
        private static string GetCustomOperator(AlarmOperator alarmOperator)
        {
            var customOperator = string.Empty;
            switch (alarmOperator)
            {
                case AlarmOperator.GreaterThan: customOperator = ">"; break;
                case AlarmOperator.GreaterThanOrEquals: customOperator = ">="; break;
                case AlarmOperator.LessThan: customOperator = "<"; break;
                case AlarmOperator.LessThanOrEquals: customOperator = "<="; break;
                case AlarmOperator.Equals: customOperator = "=="; break;
                case AlarmOperator.NotEquals: customOperator = "!="; break;
            }

            return customOperator;
        }

        private static string GetCapability(AlarmType? alarmType)
        {
            var customAlert = string.Empty;
            switch (alarmType)
            {
                case AlarmType.DaysRecorded: customAlert = "daysRecorded"; break;
                case AlarmType.IsNotRecording: customAlert = "isNotRecording"; break;
                case AlarmType.NetworkDown: customAlert = "networkDown"; break;
                case AlarmType.SMART: customAlert = "SMART"; break;
                case AlarmType.DriveTemperature: customAlert = "driveTemp"; break;
                case AlarmType.RaidStatus: customAlert = "raidStatus"; break;
                case AlarmType.VideoLoss: customAlert = "videoLoss"; break;
                case AlarmType.ZoneAlarm: customAlert = "zoneAlarm"; break;
                case AlarmType.ZoneBypass: customAlert = "zoneBypass"; break;
                case AlarmType.ZoneTrouble: customAlert = "zoneTrouble"; break;
                case AlarmType.AreaArmed: customAlert = "areaArmed"; break;
                case AlarmType.AreaDisarmed: customAlert = "areaDisarmed"; break;
                case AlarmType.DoorForced: customAlert = "doorForced"; break;
                case AlarmType.DoorHeld: customAlert = "doorHeld"; break;
            }

            return customAlert;
        }

        private static object GetCustomValue(string value, DataType dataType)
        {
            object customValue;
            switch (dataType.ToString().ToLower())
            {
                case "boolean": customValue = (value == "0") ? "false" : "true"; break;
                case "string": customValue = value.ToLower(); break;
                case "integer": customValue = int.Parse(value); break;
                default: customValue = value; break;
            }

            return customValue;
        }        
    }
}
