using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using System.Linq.Dynamic;
using AutoMapper;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Platform.Proxies.DTO;
using Diebold.Platform.Proxies.Enums;
using Diebold.Platform.Proxies.Exceptions;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Extensions;
using Diebold.Services.Infrastructure;
using Diebold.Services.Exceptions;
using NHibernate;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Diebold.Services.Impl
{
    public class DvrService : BaseCRUDTrackeableService<Dvr>, IDvrService
    {
        private readonly IIntKeyedRepository<Gateway> _gatewayRepository;
        private readonly IDeviceApiService _deviceApiService;
        private readonly IAlertStatusRepository _alertStatusRepository;
        private readonly IDeviceService _deviceService;
        private readonly IAlertInfoRepository _alertInfoRepository;
        private readonly ISiteService _siteService;
        private readonly IUserMonitorGroupRepository _monitorGroupRepository;
        private readonly IAlarmConfigurationService _alarmConfigurationService;
        private readonly IIntKeyedRepository<ResolvedAlert> _resolvedAlertRepository;
        private readonly IIntKeyedRepository<AlarmConfiguration> _alarmRepository;
        protected readonly IUserDefaultsService _userDefaultService;
        private readonly ICurrentUserProvider _currentUserProvider;
        
        ResponseDTO response = null;
        int intResCount = 0;
        int intMaxResTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["APIMaxResponseTime"]);
        string strResponseMessage = string.Empty;

        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private string initVector = System.Configuration.ConfigurationManager.AppSettings["SaltValue"];

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;

        public DvrService(IIntKeyedRepository<Dvr> repository,
            IIntKeyedRepository<Gateway> gatewayRepository,
            IUnitOfWork unitOfWork,
            IValidationProvider validationProvider,
            IDeviceApiService deviceApiService,
            IAlertStatusRepository alertStatusRepository,
            IAlertInfoRepository _alertInfoRepository,
            IDeviceService deviceService,
            ILogService logService,
            ISiteService siteService,
            IUserMonitorGroupRepository monitorGroupRepository,
            IAlarmConfigurationService alarmConfigurationService,
            IIntKeyedRepository<ResolvedAlert> resolvedAlertRepository,
            IIntKeyedRepository<AlarmConfiguration> alarmRepository,
            IUserDefaultsService userDefaultService,
            ICurrentUserProvider currentUserProvider)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            this._gatewayRepository = gatewayRepository;
            this._deviceApiService = deviceApiService;
            this._alertStatusRepository = alertStatusRepository;
            this._alertInfoRepository = _alertInfoRepository;
            _deviceService = deviceService;
            _siteService = siteService;
            _monitorGroupRepository = monitorGroupRepository;
            _alarmConfigurationService = alarmConfigurationService;
            _resolvedAlertRepository = resolvedAlertRepository;
            _alarmRepository = alarmRepository;
            _userDefaultService = userDefaultService;
            _currentUserProvider = currentUserProvider;
        }

        #region IDeviceService

        public bool DeviceIsEnabled(string name)
        {
            return _deviceService.DeviceIsEnabled(name);
        }

        public StatusDTO GetLiveStatus(int deviceId, string macAddress, bool liveFromDevice, bool EnableLogOperation)
        {
            return _deviceService.GetLiveStatus(deviceId, macAddress, liveFromDevice, EnableLogOperation);
        }
        public StatusPlatformDTO GetPlatformLiveStatus(int deviceId, string macAddress, bool liveFromDevice, bool EnableLogOperation)
        {
            return _deviceService.GetPlatformLiveStatus(deviceId, macAddress, liveFromDevice, EnableLogOperation);
        }

        public Device GetByExternalId(string externalId)
        {
            return _deviceService.GetByExternalId(externalId);
        }

        public void Restart(int pk, string DeviceKey,string DeviceType)
        {
            _deviceService.Restart(pk, DeviceKey, DeviceType);
        }

        public void Reload(int pk, string DeviceKey, string DeviceType)
        {
            _deviceService.Reload(pk, DeviceKey, DeviceType);
        }

        public void TestConnection(int pk, string DeviceKey, string DeviceType)
        {
            _deviceService.TestConnection(pk, DeviceKey, DeviceType);
        }

        public void Audit(int pk, string DeviceKey, string DeviceType)
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

        #endregion

        public Page<Dvr> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);

            if (!string.IsNullOrEmpty(whereCondition))
            {
                query = query.Where(x => x.Name.Contains(whereCondition) ||
                                         x.DeviceType.ToString().Contains(whereCondition) ||
                                         x.HostName.Contains(whereCondition) ||
                                         (x.Gateway != null && (x.Gateway.Name.Contains(whereCondition) ||
                                                                x.Site.Id.ToString() == whereCondition)) ||
                                         (x.Company != null && (x.Company.Name.Contains(whereCondition))));
            }

            var orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));
            return query.OrderBy(orderBy).ToPage(pageNumber, pageSize);
        }

        public Page<Dvr> GetDiagnosticPage(int pageNumber, int pageSize, string sortBy, bool ascending, string companyId, string siteId, string gatewayId)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);

            if (!string.IsNullOrEmpty(gatewayId))
            {
                query = query.Where(x => x.Gateway.Id == int.Parse(gatewayId));
            }
            else if (!string.IsNullOrEmpty(siteId))
            {
                query = query.Where(x => x.Site.Id == int.Parse(siteId));
            }
            else if (!string.IsNullOrEmpty(companyId))
            {
                query = query.Where(x => x.Company.Id == int.Parse(companyId));
            }

            string orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));

            var page = query.OrderBy(orderBy).ToPage(pageNumber, pageSize);
            page.Items.ForEach(d => NHibernateUtil.Initialize(d.AlertStatus));

            return page;
        }

        public override void Create(Dvr item)
        {
            _logger.Debug("Device Create Method entered into service layer");
            DeviceDTO OBJDeviceDTO = new DeviceDTO();
            try
            {
                _logger.Debug("Validation of item started");
                this._validationProvider.Validate(item);
                _logger.Debug("Validation of item completed");
                _logger.Debug("Find Gateway by GatewayId stated " + item.Gateway.Id.ToString());
                var gateway = _gatewayRepository.FindBy(item.Gateway.Id);
                _logger.Debug("Find Gateway by GatewayId completed");
                _logger.Debug("Adding cameras to Dictionary started");
                var camerasStatus = item.Cameras.ToDictionary(camera => camera.Channel, camera => camera.Active);
                _logger.Debug("Adding cameras to Dictionary completed");
                
                var notificationUrl = ConfigurationManager.AppSettings["NotificationURL"];
                _logger.Debug("Notification URL " + notificationUrl.ToString());

                var alarmDTOs = item.AlarmConfigurations.Select(alarm =>
                    {
                        object threshold;

                        if (alarm.AlarmType == AlarmType.VideoLoss)
                        {
                            threshold = camerasStatus;
                        }
                        else
                        {
                            threshold = GetCustomValue(alarm.Threshold, alarm.DataType);
                        }

                        return new AlarmDTO
                                    {
                                        Name = alarm.AlarmType.ToString(),
                                        CapabilityType = GetCapability(alarm.AlarmType),
                                        RelationalOperator = GetCustomOperator(alarm.Operator),
                                        Threshold = threshold,
                                        CallbackURL = notificationUrl
                                    };
                    }).ToList();

                var configurationDTO = new ConfigurationDTO
                                           {
                                               IP = item.HostName,
                                               PortA = item.PortA ?? "0",
                                               PortB = item.PortB ?? "0",

                                               User = item.UserName ?? "",
                                               Password = item.Password ?? "",

                                               TimeZone = item.TimeZone,
                                               DaySavingTime = (item.IsInDST) ? "Yes" : "No"
                                           };
                _logger.Debug("Creating a new DeviceDTO started");
                var deviceDTO = new DeviceDTO(DeviceTypeEnum.SparkDvr1, item.DeviceType, gateway.MacAddress, item.DeviceKey,
                                              (int)item.PollingFrequency, gateway.ExternalDeviceId, configurationDTO, alarmDTOs, item.IsInDST);
                _logger.Debug("Creating a new DeviceDTO Completed");
                deviceDTO.ExternalDeviceKey = gateway.MacAddress + "-" + item.DeviceKey;
                OBJDeviceDTO = deviceDTO;
                deviceDTO.OnLine = true;
                _logger.Debug("Add Device for API Stated");
                // Store External Device Id in DB, External Device Id should be the _Id which we receive from acknowledge
                string Acknoledgement = _deviceApiService.AddDevice(deviceDTO, false);
                _logger.Debug("Recived Acknoledgement");
                _logger.Debug("Split the ack by :");
                string[] strAckArray = Acknoledgement.Split(':');
                string strExternalDeviceId = strAckArray[1];
                _logger.Debug("External Device Id " + strExternalDeviceId);
                strExternalDeviceId = strExternalDeviceId.Remove(0, 1);
                _logger.Debug("Removed the preceiding double quotes " + strExternalDeviceId);
                // strExternalDeviceId = strExternalDeviceId.Remove(strExternalDeviceId.Length - 1, 1);
                string[] tempExternalDeviceId = strExternalDeviceId.Split('"');
                strExternalDeviceId = string.Empty;
                strExternalDeviceId = tempExternalDeviceId[0];
                _logger.Debug("Removed the succeeding double quotes " + strExternalDeviceId);
                _logger.Debug("Add Device for API Completed");
                SetReponse("DVCreate" + deviceDTO.ExternalDeviceKey);

                if (response != null)
                {
                    _logger.Debug("Received a Reponse");
                    System.Web.HttpContext.Current.Application.Remove("DVCreate" + deviceDTO.ExternalDeviceKey);
                    if (response.payload != null && response.payload.command_response != null)
                    {
                        strResponseMessage = response.payload.command_response.status.ToUpper();
                        if(string.IsNullOrEmpty(strResponseMessage) ==false)
                        {
                           // item.ExternalDeviceId = response.payload.txid;
                            // External device Id need to be the _Id which is obtained from the ack
                            item.ExternalDeviceId = strExternalDeviceId;
                            int newZoneNumber = gateway.LastUsedEmcZoneNumber + 1;
                            item.ZoneNumber = newZoneNumber.ToString();
                            if (!string.IsNullOrEmpty(item.UserName))
                            item.UserName = Encrypt(item.UserName,"username"); //Encrypt the username
                            if (!string.IsNullOrEmpty(item.Password))
                            item.Password = Encrypt(item.Password,"password"); // Encrypt the password
                            gateway.LastUsedEmcZoneNumber = newZoneNumber;
                            using (var scope = new TransactionScope(TransactionScopeOption.Required))
                            {
                                this._repository.Add(item);

                                foreach (var alarmConfiguration in item.AlarmConfigurations)
                                {
                                    if (alarmConfiguration.AlarmType == AlarmType.VideoLoss)
                                    {
                                        foreach (var camera in camerasStatus)
                                        {
                                            var alertStatus = new AlertStatus
                                            {
                                                Device = item,
                                                Alarm = alarmConfiguration,
                                                ElementIdentifier = camera.Key
                                            };

                                            _alertStatusRepository.Add(alertStatus);
                                        }
                                    }
                                    else
                                    {
                                        var alertStatus = new AlertStatus { Device = item, Alarm = alarmConfiguration };
                                        _alertStatusRepository.Add(alertStatus);
                                    }
                                }

                                //Assign the device to user monitoring
                                var gatewayItem = _gatewayRepository.Load(item.Gateway.Id);
                                var siteitem = _siteService.Get(item.Site.Id);
                                var matches = GetUserMonitorGroup(gatewayItem.Id, item.Site.Id,
                                                                      siteitem.CompanyGrouping2Level.CompanyGrouping1Level.Id,
                                                                      siteitem.CompanyGrouping2Level.Id);
                                AddDeviceToUserMonitor(matches, item);

                                LogOperation(LogAction.DeviceCreate, item);

                                _unitOfWork.Commit();
                                // If the device is not created and status is not equal to OK then need to remove that device from platform
                                if (!response.payload.command_response.status.ToUpper().Equals("OK"))
                                {
                                    _deviceApiService.RemoveDevicefromPlatform(null, false, strExternalDeviceId);
                                }
                                scope.Complete();
                                _logger.Debug("Commit Operation Completed");
                            }
                        }
                        else
                        {
                            if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                                throw new ServiceException(response.payload.messages[0].description);
                            else
                                throw new ServiceException("An error occurred while creating device.");
                        }
                    }
                    else
                    {
                        throw new ServiceException("An error occurred while creating device.");
                    }
                }
                else
                {
                    throw new ServiceException("Timed out occurred while creating device.");
                }
            }
            catch (MachineshopPlatformException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();

                if (item.ExternalDeviceId != null)
                    _deviceApiService.RemoveDevice(OBJDeviceDTO, false);
                throw e;
            }
        }

        public override void Update(Dvr item)
        {
            UpdateDevice(item, true);
        }

        private void UpdateDevice(Dvr item, bool enabled)
        {
            try
            {
                item.AlarmConfigurations.ForEach(x => x.CompanyId = item.Company.Id);
                _validationProvider.Validate(item);

                if (!string.IsNullOrEmpty(item.RemovedCameras))
                {
                    string[] RemovedCameras = item.RemovedCameras.Split(',');
                    RemovedCameras.ToList().ForEach(x =>
                    {
                        var objAlertStatus = (IEnumerable<AlertStatus>)_alertStatusRepository.All().ToList().Where(alertStatus => alertStatus.Device.Id.Equals(item.Id) && alertStatus.ElementIdentifier == x).ToList();
                        if (objAlertStatus != null && objAlertStatus.Count() > 0)
                            _alertStatusRepository.Delete(objAlertStatus);

                        var lstAlertInfo = (IEnumerable<AlertInfo>)_alertInfoRepository.All().ToList().Where(alert => alert.Device.Id.Equals(item.Id) && alert.ElementIdentifier == x).ToList();
                        if (lstAlertInfo != null && lstAlertInfo.Count() > 0)
                            _alertInfoRepository.Delete(lstAlertInfo);
                    });
                }

                // Create device into platform 
                var gateway = _gatewayRepository.FindBy(item.Gateway.Id);

                var camerasStatus = item.Cameras.ToDictionary(camera => camera.Channel, camera => camera.Active);

                var notificationUrl = ConfigurationManager.AppSettings["NotificationURL"];

                var alarmDTOs = item.AlarmConfigurations.Select(alarm =>
                {
                    object threshold;

                    if (alarm.AlarmType == AlarmType.VideoLoss)
                    {
                        threshold = camerasStatus;
                    }
                    else
                    {
                        threshold =
                            GetCustomValue(alarm.Threshold,
                                           alarm.DataType);
                    }

                    return new AlarmDTO
                    {
                        Name =
                            alarm.AlarmType.ToString(),
                        CapabilityType =
                            GetCapability(
                                alarm.AlarmType),
                        RelationalOperator =
                            GetCustomOperator(
                                alarm.Operator),
                        Threshold = threshold,
                        CallbackURL = notificationUrl
                    };
                }).ToList();

                var configurationDto = new ConfigurationDTO
                {
                    IP = item.HostName,
                    PortA = item.PortA ?? "0",
                    PortB = item.PortB ?? "0",

                    User = item.UserName ?? "",
                    Password = item.Password ?? "",

                    TimeZone = item.TimeZone,
                    DaySavingTime = (item.IsInDST) ? "Yes" : "No"
                };

                var deviceDto = new DeviceDTO(DeviceTypeEnum.SparkDvr1, item.DeviceType, gateway.MacAddress,
                                              item.DeviceKey,
                                              (int)item.PollingFrequency, gateway.ExternalDeviceId,
                                              configurationDto, alarmDTOs, item.IsInDST);

                deviceDto.ExternalDeviceId = item.ExternalDeviceId;
                deviceDto.ExternalDeviceKey = gateway.MacAddress + "-" + item.DeviceKey;
                deviceDto.DeviceType = item.DeviceType.ToString();
                deviceDto.OnLine = enabled;
                _deviceApiService.ModifyDevice(deviceDto, false);

                SetReponse("DVUpdate" + deviceDto.ExternalDeviceKey);

                if (response != null)
                {
                    System.Web.HttpContext.Current.Application.Remove("DVUpdate" + deviceDto.ExternalDeviceKey);
                    if (response.payload != null && response.payload.command_response != null)
                    {
                        strResponseMessage = response.payload.command_response.status.ToUpper();
                        if (string.IsNullOrEmpty(strResponseMessage) == false)
                        {
                            using (var scope = new TransactionScope(TransactionScopeOption.Required))
                            {
                                if (!string.IsNullOrEmpty(item.UserName))
                                item.UserName = Encrypt(item.UserName, "username");  //Encrypt the username
                                if (!string.IsNullOrEmpty(item.Password))
                                item.Password = Encrypt(item.Password, "password");  // Encrypt the password
                                _repository.Update(item);
                                //Add alert status for each camara if videloss alarm exists.
                                var videoLossAlarm = item.AlarmConfigurations.Where(x => x.AlarmType == AlarmType.VideoLoss);

                                if (videoLossAlarm != null)
                                {
                                    AlarmConfiguration objConfig = videoLossAlarm.FirstOrDefault();
                                    if (objConfig != null)
                                    {
                                        foreach (var camera in camerasStatus)
                                        {
                                            var queryAlertStatus = _alertStatusRepository.FilterBy(x => x.Device.Id == item.Id && x.Alarm.Id == objConfig.Id &&
                                                                       x.ElementIdentifier == camera.Key);

                                            if (!queryAlertStatus.Any())
                                            {
                                                var alertStatus = new AlertStatus
                                                {
                                                    Device = item,
                                                    Alarm = objConfig,
                                                    ElementIdentifier = camera.Key
                                                };
                                                _alertStatusRepository.Add(alertStatus);
                                            }
                                        }
                                    }
                                }
                                LogOperation(LogAction.DeviceEdit, item);
                                _unitOfWork.Commit();
                                scope.Complete();
                            }
                        }
                        else
                        {
                            if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                                throw new ServiceException(response.payload.messages[0].description);
                            else
                                throw new ServiceException("An error occurred while updating device.");
                        }
                    }
                    else
                    {
                        throw new ServiceException("An error occurred while updating device on platform.");
                    }
                }
                else
                {
                    throw new ServiceException("Timed out occurred while updating device on platform.");
                }


            }
            catch (MachineshopPlatformException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw e;
            }
        }

        public override void Delete(int pk)
        {
            try
            {
                var itemToDelete = _repository.Load(pk);
                Dvr objDvr = itemToDelete;
                itemToDelete.LogicalDelete();

                // Create device into platform 
                var gateway = _gatewayRepository.FindBy(objDvr.Gateway.Id);

                var camerasStatus = objDvr.Cameras.ToDictionary(camera => camera.Channel, camera => camera.Active);

                var notificationUrl = ConfigurationManager.AppSettings["NotificationURL"];

                var alarmDTOs = objDvr.AlarmConfigurations.Select(alarm =>
                {
                    object threshold;

                    if (alarm.AlarmType == AlarmType.VideoLoss)
                    {
                        threshold = camerasStatus;
                    }
                    else
                    {
                        threshold =
                            GetCustomValue(alarm.Threshold,
                                           alarm.DataType);
                    }

                    return new AlarmDTO
                    {
                        Name =
                            alarm.AlarmType.ToString(),
                        CapabilityType =
                            GetCapability(
                                alarm.AlarmType),
                        RelationalOperator =
                            GetCustomOperator(
                                alarm.Operator),
                        Threshold = threshold,
                        CallbackURL = notificationUrl
                    };
                }).ToList();

                var configurationDto = new ConfigurationDTO
                {
                    IP = objDvr.HostName,
                    PortA = objDvr.PortA ?? "0",
                    PortB = objDvr.PortB ?? "0",


                    User = objDvr.UserName ?? "",
                    Password = objDvr.Password ?? "",

                    TimeZone = objDvr.TimeZone,
                    DaySavingTime = (objDvr.IsInDST) ? "Yes" : "No"
                };

                var deviceDto = new DeviceDTO(DeviceTypeEnum.SparkDvr1, objDvr.DeviceType, gateway.MacAddress,
                                              objDvr.DeviceKey,
                                              (int)objDvr.PollingFrequency, gateway.ExternalDeviceId,
                                              configurationDto, alarmDTOs, objDvr.IsInDST);

                deviceDto.ExternalDeviceId = objDvr.ExternalDeviceId;
                deviceDto.DeviceType = objDvr.DeviceType.ToString();
                deviceDto.ExternalDeviceKey = gateway.MacAddress + "-" + itemToDelete.DeviceKey;

                string strAcknoledgement = _deviceApiService.RemoveDevice(deviceDto, false);
                _logger.Debug("Acknoledgement Received for Delete");
                string[] strarrAcknoledgement = strAcknoledgement.Split(':');
                string strExternalDeviceId = strarrAcknoledgement[1];
                _logger.Debug("External Device Id " + strExternalDeviceId);
                strExternalDeviceId = strExternalDeviceId.Remove(0, 1);
                _logger.Debug("External Device Id after removing preceeding double quotes" + strExternalDeviceId);
                // strExternalDeviceId = strExternalDeviceId.Remove(strExternalDeviceId.Length - 1, 1);
                string[] tempExternalDeviceId = strExternalDeviceId.Split('"');
                strExternalDeviceId = string.Empty;
                strExternalDeviceId = tempExternalDeviceId[0];
                _logger.Debug("External Device Id after removing succeeding double quotes" + strExternalDeviceId);
                SetReponse("DVDelete" + deviceDto.ExternalDeviceKey);
                _logger.Debug("Response check started");
                if (response != null)
                {
                    System.Web.HttpContext.Current.Application.Remove("DVDelete" + deviceDto.ExternalDeviceKey);
                    _logger.Debug("Inside response");
                    if (response.payload != null && response.payload.command_response != null)
                    {
                        strResponseMessage = response.payload.command_response.status.ToUpper();
                        _logger.Debug("Inside response" + strResponseMessage);
                        _logger.Debug("Adding cameras to Dictionary completed");
                        if (strResponseMessage == "OK")
                        {
                            using (var scope = new TransactionScope(TransactionScopeOption.Required))
                            {                                                           
                                _repository.Update(itemToDelete);
                                //update device reference in user monitors group
                                UpdateUserMonitorGroup(itemToDelete);                                
                                var ActiveAlertQuery = (from alert in _alertStatusRepository.All() where alert.Device.Id == pk select alert);
                                if (ActiveAlertQuery.Count() > 0)
                                {
                                    IEnumerable<AlertStatus> ActiveAlert = (IEnumerable<AlertStatus>)ActiveAlertQuery.ToList();
                                    if (ActiveAlert != null && ActiveAlert.Count() > 0)
                                        _alertStatusRepository.Delete(ActiveAlert);
                                }

                                var lstalertinfo = (from alertinfo in _alertInfoRepository.All() where alertinfo.Device.Id == pk select alertinfo);
                                if (lstalertinfo.Count() > 0)
                                {
                                    IEnumerable<AlertInfo> lstalert = (IEnumerable<AlertInfo>)lstalertinfo.ToList();
                                    if (lstalert != null && lstalert.Count() > 0)
                                        _alertInfoRepository.Delete(lstalert);
                                }

                                var lstResolvedAlertinfo = (from resolvedalert in _resolvedAlertRepository.All() where resolvedalert.Device.Id == pk select resolvedalert);
                                if (lstResolvedAlertinfo.Count() > 0)
                                {
                                    IEnumerable<ResolvedAlert> resolvedalertinfo = (IEnumerable<ResolvedAlert>)lstResolvedAlertinfo.ToList();
                                    if (resolvedalertinfo != null && resolvedalertinfo.Count() > 0)
                                        _resolvedAlertRepository.Delete(resolvedalertinfo);
                                }
                                DeleteAlarmConfiguration(itemToDelete);
                                // Delete device from user defaults
                                string strInternalName = string.Empty;
                                if ((objDvr.DeviceType.ToString() == DeviceType.dmpXR100.ToString()) || (objDvr.DeviceType.ToString() == DeviceType.dmpXR500.ToString()) || (objDvr.DeviceType.ToString() == DeviceType.bosch_D9412GV4.ToString()) || (objDvr.DeviceType.ToString() == DeviceType.videofied01.ToString()))
                                {
                                    strInternalName = "INTRUSION";
                                }
                                else if ((objDvr.DeviceType.ToString() == DeviceType.eData300.ToString()) || (objDvr.DeviceType.ToString() == DeviceType.eData524.ToString()) || (objDvr.DeviceType.ToString() == DeviceType.dmpXR100Access.ToString()) || (objDvr.DeviceType.ToString() == DeviceType.dmpXR500Access.ToString()))
                                {
                                    strInternalName = "ACCESSCONTROL";
                                }
                                else if ((objDvr.DeviceType.ToString() == DeviceType.Costar111.ToString()) || (objDvr.DeviceType.ToString() == DeviceType.ipConfigure530.ToString()) || (objDvr.DeviceType.ToString() == DeviceType.VerintEdgeVr200.ToString()))
                                {
                                    strInternalName = "VIDEOHEALTHCHECK";
                                }
                                if (!String.IsNullOrEmpty(strInternalName))
                                {
                                    IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, strInternalName);
                                    if (lstUserDefaults.Count() > 0)
                                    {
                                        _userDefaultService.Delete(lstUserDefaults.First().Id);
                                    }
                                }
                                // some time Plateform API call take some time to execute.If API have taken more time to execute, than the Logoperation method will throw the time out error.So API call should call as last last statement.
                                LogOperation(LogAction.DeviceDelete, itemToDelete);
                                _unitOfWork.Commit(); 
                                //Remove device from platform
                                _deviceApiService.RemoveDevicefromPlatform(null, false, strExternalDeviceId);
                                scope.Complete();
                            }                            
                        }
                        else if (strResponseMessage == "ERROR")
                        {
                            if (response.payload.messages.Length > 0 && string.IsNullOrEmpty(response.payload.messages[0].description) == false)
                                throw new ServiceException(response.payload.messages[0].description);
                            else
                                throw new ServiceException("An error occurred while deleting device.");
                        }
                    }
                    else
                    {
                        throw new ServiceException("An error occurred while deleting device on platform.");
                    }
                }
                else
                {
                    throw new ServiceException("Timed out occurred while deleting device on platform.");
                }

            }
            catch (MachineshopPlatformException e)
            {
                throw  e;
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw e;
            }
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
        }

        public void DeleteDuplicateAlarmConfiguration(string deviceType, int companyId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                var lstDuplicateAlerts = (from Duplicatealert in _alarmRepository.All()
                                          where Duplicatealert.CompanyId == companyId &&
                                              Duplicatealert.Device.Id == null &&
                                              Duplicatealert.AlarmParentType.ToString().ToLower() == "access" &&
                                              Duplicatealert.DeviceType.ToString() == deviceType
                                          select Duplicatealert);

                List<int> lstint = new List<int>();
                var lstDuplicateAlertsId = lstDuplicateAlerts.ForEach(x => lstint.Add(x.Id));

                var DuplicateAlertStatusList = (from DuplicateAlertStatus in _alertStatusRepository.All()
                                                where lstint.Contains(DuplicateAlertStatus.Alarm.Id)
                                                select DuplicateAlertStatus).ToList();

                var DuplicateAlertInfoList = (from DuplicateAlertInfo in _alertInfoRepository.All()
                                              where lstint.Contains(DuplicateAlertInfo.Alarm.Id)
                                              select DuplicateAlertInfo).ToList();

                var DuplicateResolvedAlertInfoList = (from DuplicateResolvedAlerts in _resolvedAlertRepository.All()
                                                      where lstint.Contains(DuplicateResolvedAlerts.AlarmConfiguration.Id)
                                                      select DuplicateResolvedAlerts).ToList();

                if (DuplicateAlertStatusList.Count() > 0)
                {
                    _alertStatusRepository.Delete(DuplicateAlertStatusList);
                }
                if (DuplicateAlertInfoList.Count() > 0)
                {
                    _alertInfoRepository.Delete(DuplicateAlertInfoList);
                }
                if (DuplicateResolvedAlertInfoList.Count() > 0)
                {
                    _resolvedAlertRepository.Delete(DuplicateResolvedAlertInfoList);
                }

                if (lstDuplicateAlerts.Count() > 0)
                {
                    IEnumerable<AlarmConfiguration> duplicateAlertinfo = (IEnumerable<AlarmConfiguration>)lstDuplicateAlerts.ToList();
                    if (duplicateAlertinfo != null && duplicateAlertinfo.Count() > 0)
                        _alarmRepository.Delete(duplicateAlertinfo);
                }
                _unitOfWork.Commit();
                scope.Complete();
            }

        }

        public override void Enable(int pk)
        {
            try
            {
                Dvr entityToEnable = _repository.Load(pk);
                _deviceApiService.Enabled(entityToEnable.Id.ToString());
                entityToEnable.Enable();
                Update(entityToEnable);

                LogOperation(LogAction.DeviceEnable, entityToEnable);
            }
            catch (MachineshopPlatformException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw e;
            }
        }

        public override void Disable(int pk)
        {
            try
            {
                var entityToDiable = _repository.Load(pk);
                entityToDiable.Disable();

                UpdateDevice(entityToDiable, false);

                LogOperation(LogAction.DeviceEnable, entityToDiable);
            }
            catch (MachineshopPlatformException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
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

        public IList<Dvr> GetDevicesBySiteId(int siteId)
        {
            return _repository.FilterBy(x => x.DeletedKey == null && x.Site.Id == siteId).OrderBy(x => x.Name).ToList();
        }

        public IList<Dvr> GetAllDevicesBySiteList(IList<int> siteIdList)
        {
            return _repository.FilterBy(x => x.DeletedKey == null && siteIdList.Contains(x.Site.Id)).OrderBy(x => x.Name).ToList();
        }

        public IList<Dvr> GetDevicesByCompanyGrouping2Level(int companyGrouping2LevelId)
        {
            return _repository.FilterBy(x => x.DeletedKey == null && x.Site.CompanyGrouping2Level.Id == companyGrouping2LevelId).ToList();
        }

        public IList<Dvr> GetDevicesByCompanyGrouping1Level(int companyGrouping1LevelId)
        {
            return _repository.FilterBy(x => x.DeletedKey == null && x.Site.CompanyGrouping2Level.CompanyGrouping1Level.Id == companyGrouping1LevelId).ToList();
        }
        public IList<Dvr> GetDevicesByGatewayId(int gatewayId)
        {
            return _repository.FilterBy(x => x.DeletedKey == null && x.Gateway.Id == gatewayId).ToList();
        }

        public void SetCameras(int deviceId, IList<Camera> cameras)
        {
            var device = _repository.Load(deviceId);

            IList<int> itemsToDelete = new List<int>();
            foreach (var camera in device.Cameras)
            {
                if (!cameras.Contains(camera))
                    itemsToDelete.Add(camera.Id);
            }

            foreach (int cameraId in itemsToDelete)
                device.Cameras.Remove(new Camera { Id = cameraId });

            foreach (var camera in cameras)
            {
                if (camera.Id != 0)
                {
                    var cam = device.Cameras.Single(x => x.Id == camera.Id);

                    var destinationProperties = cam.GetType().GetProperties();
                    foreach (var destinationPI in destinationProperties)
                    {
                        var sourcePI = camera.GetType().GetProperty(destinationPI.Name);
                        destinationPI.SetValue(cam, sourcePI.GetValue(camera, null), null);
                    }
                }
                else
                {
                    device.Cameras.Add(camera);
                }
            }
        }

        public void SetAlarmConfigurations(int deviceId, IList<AlarmConfiguration> alarmConfigurations)
        {
            var device = _repository.Load(deviceId);

            foreach (var alarm in alarmConfigurations)
            {
                if (alarm.Id == 0) continue;

                var item = device.AlarmConfigurations.Single(x => x.Id == alarm.Id);

                var destinationProperties = item.GetType().GetProperties();
                foreach (var destinationPI in destinationProperties)
                {
                    var sourcePI = alarm.GetType().GetProperty(destinationPI.Name);
                    destinationPI.SetValue(item, sourcePI.GetValue(alarm, null), null);
                }
            }
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

        public bool ValidateDeviceKey(int gatewayId, string deviceKey)
        {
            try
            {
                var validate = _repository.FindBy(x => x.Gateway.Id == gatewayId && x.DeletedKey == null && x.DeviceKey == deviceKey);

                if (validate != null)
                {
                    var validationR = new List<ValidationResult>();
                    validationR.Add(new ValidationResult("DeviceKey", "Device Id already exists in the selected gateway."));
                    throw new ValidationException(validationR);
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return false;
        }

        public override void LogOperation(LogAction action, Dvr item)
        {
            var device = item;
            var gateway = _gatewayRepository.FindBy(item.Gateway.Id);
            // var site = device.Site;
            var site = _siteService.Get(item.Site.Id);
            var companyGrouping2 = site.CompanyGrouping2Level;
            var companyGrouping1 = companyGrouping2.CompanyGrouping1Level;

            _logService.Log(action, item.Name, companyGrouping1, companyGrouping2, site, device);
        }


        public IList<Dvr> GetAllDevicesForDisplay()
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);
            return query.OrderBy(x => x.Name).ToList();
        }

        public int GetAllDevicesforMaxId()
        {
            //var query = _repository.All();
            //IList<int> lstintDeviceKey = new List<int>();
            //query.ToList().ForEach(x =>
            //{
            //    lstintDeviceKey.Add(Convert.ToInt32(x.DeviceKey));
            //});
            //int maxDeviceId = lstintDeviceKey.ToList().OrderByDescending(x => x).First();
            //return maxDeviceId;
            
            //DB Dev Timeout
            var query = _repository.All().OrderByDescending(x => x.DeviceKey).FirstOrDefault();
            if (query == null)
            {
                return 0;
            }
            return  Convert.ToInt32(query.DeviceKey);            
        }

        public Dvr GetDeviceByInstanceName(string instanceName)
        {
            Dvr objDvr = new Dvr();
            if (string.IsNullOrEmpty(instanceName) == false)
            {
                string[] strdeviceInfo = instanceName.Split('-');
                if (strdeviceInfo.Length > 1)
                    objDvr = _repository.All().Where(x => x.Gateway.MacAddress == strdeviceInfo[0] && x.DeletedKey == null && x.DeviceKey == strdeviceInfo[1]).ToList().Single();
            }
            return objDvr;
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

        public string Encrypt(string plainText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public string Decrypt(string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }
}
