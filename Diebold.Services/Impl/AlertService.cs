using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Diebold.Domain.Enums;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Extensions;
using Diebold.Services.Infrastructure;
using Diebold.Services.Exceptions;
using Diebold.Platform.Proxies.DTO;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Platform.Proxies.Exceptions;


namespace Diebold.Services.Impl
{
    public class AlertService : BaseCRUDService<AlertInfo>, IAlertService
    {
        private readonly IAlertStatusRepository _alertStatusRepository;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserDeviceMonitorRepository _userDeviceMonitorRepository;
        private readonly IIntKeyedRepository<ResolvedAlert> _resolvedAlertRepository;
        private readonly IIntKeyedRepository<AlarmConfiguration> _alarmConfigurationRepository;
        private readonly IIntKeyedRepository<Dvr> _dvrRepository;
        private readonly IIntKeyedRepository<Gateway> _gatewayRepository;
        private readonly IUserService _userService;
        private readonly IAlertApiService _alertApiService;
        EMCParameters objEMCDTO = new EMCParameters();

        public AlertService(IAlertInfoRepository repository,
                            IUnitOfWork unitOfWork,
                            IValidationProvider validationProvider,
                            IAlertStatusRepository alertStatusRepository,
                            ICurrentUserProvider currentUserProvider,
                            IUserDeviceMonitorRepository userDeviceMonitorRepository,
                            IIntKeyedRepository<ResolvedAlert> resolvedAlertRepository,
                            IIntKeyedRepository<AlarmConfiguration> alarmConfigurationRepository,
                            IIntKeyedRepository<Dvr> dvrRepository,
                            IUserService UserService,
                            IAlertApiService alertApiService,
// ReSharper disable UnusedParameter.Local
                            IIntKeyedRepository<Gateway> gatewayRepository,
// ReSharper restore UnusedParameter.Local
                            ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            _alertStatusRepository = alertStatusRepository;
            _currentUserProvider = currentUserProvider;
            _userDeviceMonitorRepository = userDeviceMonitorRepository;
            _resolvedAlertRepository = resolvedAlertRepository;
            _alarmConfigurationRepository = alarmConfigurationRepository;
            _dvrRepository = dvrRepository;
            _gatewayRepository = gatewayRepository;
            _userService = UserService;
            _alertApiService = alertApiService;
        }
        
        public override void Create(AlertInfo item)
        {
            try
            {
                this._validationProvider.Validate(item);

                //DB Dev Timeout
                //var alertInfo = _repository.FilterBy(x => x.Device.Id == item.Device.Id && x.Alarm.Id == item.Alarm.Id)                
                //    .OrderBy(x => x.Id).LastOrDefault();
                var alertInfo = _repository.FilterBy(x => x.Device.Id == item.Device.Id && x.Alarm.Id == item.Alarm.Id)
                    .OrderByDescending(x => x.Id).FirstOrDefault();
                

                this._repository.Add(item);

                if (alertInfo != null)
                {
                    item.GroupId = !alertInfo.IsDeviceOk ? alertInfo.GroupId : item.Id;
                }
                else
                    item.GroupId = item.Id;
                
                //IList<AlertInfo> alerts = _repository.FilterBy(x => x.Device.Id == item.Device.Id && x.Alarm.Id == item.Alarm.Id)
                //    //DB Dev Timeout
                //    .OrderBy(x => x.Id).ToList();
                
                //if (alerts.Count > 0)
                //{
                //    var alertInfo = alerts.Last();

                //    item.GroupId = !alertInfo.IsDeviceOk ? alertInfo.GroupId : item.Id;
                //}
                //else
                //    item.GroupId = item.Id;

                //update alert stats
                var alertStatus =_alertStatusRepository.FindBy(x => x.Device.Id == item.Device.Id && x.Alarm.Id == item.Alarm.Id);
                // Set acknowledged to false for every time the alerts get triggered.
                alertStatus.IsAcknowledged = false;
                UpdateAlertStatus(item, alertStatus);

                this._unitOfWork.Commit();
            }
            catch (Exception E)
            {
                this._unitOfWork.Rollback();

                throw new ServiceException("Cannot Update Status", E);
            }
        }

        public void CreateMultipleItems(List<AlertInfo> lstAlertInfo)
        {
            if (lstAlertInfo != null && lstAlertInfo.Count() > 0)
            {
                //foreach (AlertInfo ai in lstAlertInfo)
                //{
                //    try
                //    {
                //          this._validationProvider.Validate(ai);
                //          IList<AlertInfo> alerts = _repository.FilterBy(x => x.Device.Id == ai.Device.Id && x.Alarm.Id == ai.Alarm.Id)
                //                .OrderBy(x => x.Id).ToList();
                //          this._repository.Add(ai);
                //          _logger.Debug("Create MultipleItems Started");                         

                //          if (alerts.Count > 0)
                //          {
                //              var alertInfo = alerts.Last();

                //              ai.GroupId = !alertInfo.IsDeviceOk ? alertInfo.GroupId : ai.Id;
                //          }
                //          else
                //              ai.GroupId = ai.Id;

                //          //update alert stats
                //          var alertStatus = _alertStatusRepository.FindBy(x => x.Device.Id == ai.Device.Id && x.Alarm.Id == ai.Alarm.Id);
                //          // Set acknowledged to false for every time the alerts get triggered.
                //          alertStatus.IsAcknowledged = false;
                //          alertStatus.ElementIdentifier = ai.ElementIdentifier;
                //          _logger.Debug("Alert ElementIdentifier: " + ai.ElementIdentifier);
                //          _logger.Debug("Alert Value: " + ai.Value);
                //          _logger.Debug("Alert Id: " + ai.Id);
                //          _logger.Debug("Alert IsDeviceOk: " + ai.IsDeviceOk);
                //          _logger.Debug("Alert SatisfiesAlertCondition: " + ai.SatisfiesAlertCondition);
                //          _logger.Debug("alertStatus: " + alertStatus);
                //          UpdateAlertStatus(ai, alertStatus);  
                //    }
                //    catch (Exception e)
                //    {
                //        this._unitOfWork.Rollback();

                //        throw new ServiceException("Cannot Update Status", e);
                //    }
                //}
                //_logger.Debug("unitOfWork Commit started");
                //this._unitOfWork.Commit();
                //_logger.Debug("unitOfWork Commit completed");
                try
                {
                    _logger.Debug("Create alert started under create multiple alerts");
                    CreateAlert(lstAlertInfo);
                    _logger.Debug("Create alert completed under create multiple alerts");
                }
                catch (Exception ex)
                {

                    throw new ServiceException("Cannot create alert info", ex);
                }
                
            }
        }

        public int GetOkDevicesCount(int userId)
        {
            return _alertStatusRepository.GetCountOKDevicesByUser(userId);
        }

        public int GetAlertedDevicesCount(int userId)
        {
            return _alertStatusRepository.GetCountAlertedDevicesByUser(userId);
        }

        private IQueryable<AlertStatus> CurrentUserAlertStatusQuery()
        {
            var query = from alert in _alertStatusRepository.All()
                        where

                            (from ud in _userDeviceMonitorRepository.All()
                             where
                                 ud.User == _currentUserProvider.CurrentUser && ud.Device == alert.Device
                                 && ud.Device.DeletedKey == null && !ud.Device.IsDisabled
                                  && !ud.Device.Company.IsDisabled && ud.Device.Company.DeletedKey == null
                                  && !((Dvr)ud.Device).Gateway.IsDisabled && ((Dvr)ud.Device).Gateway.DeletedKey == null
                                  && !((Dvr)ud.Device).Site.IsDisabled && ((Dvr)ud.Device).Site.DeletedKey == null
                             select ud).Any()
                        select alert;

            return query;
        }
        
        private IQueryable<AlarmConfiguration> CurrentUserAlarmConfigurationBySiteQuery(int siteId)
        {
            var query = from alarm in _alarmConfigurationRepository.All()
                        where
                            (from ud in _userDeviceMonitorRepository.All()
                             where
                                 ud.User == _currentUserProvider.CurrentUser && ud.Device != null &&
                                  ud.Device == alarm.Device && ((Dvr) ud.Device).Site.Id == siteId
                                  && ud.Device.DeletedKey == null && !ud.Device.IsDisabled
                             select ud).Any()

                        select alarm;

            return query;
        }

        public int GetConfiguredAlarmsBySiteAndCurrentUserCount(int siteId)
        {
            return CurrentUserAlarmConfigurationBySiteQuery(siteId).Count();
        }

        public DateTime? GetLastAlertTimeStamp(int userId)
        {
            //gets the max LastAlertTimeStamp
            //from the alertstatus 
            //filtering by devices monitored by the current user.
            var query = CurrentUserAlertStatusQuery();

            return query.Max(x => x.LastAlertTimeStamp);
        }

        public void AcknowledgeAlert(int alertId)
        {
            try
            {
                var alertStatus = _alertStatusRepository.Load(alertId);

                var resolvedAlert = new ResolvedAlert
                                        {
                                            AcknoledgeDate = DateTime.Now,
                                            AlarmConfiguration = alertStatus.Alarm,
                                            Device = alertStatus.Device,
                                            User = _currentUserProvider.CurrentUser,
                                            ElementIdentifier = alertStatus.ElementIdentifier
                                        };

                _resolvedAlertRepository.Add(resolvedAlert);

                alertStatus.IsAcknowledged = true;
                alertStatus.AlertCount = 0;
                //alertStatus.FirstAlertTimeStamp = null;
                _alertStatusRepository.Update(alertStatus);
                
                LogOperation(LogAction.AlertAcknowledge, alertStatus);

                _unitOfWork.Commit();
            }
            catch (Exception E)
            {
                this._unitOfWork.Rollback();

                throw new ServiceException("Cannot Acknoledge Alert", E);
            }
        }

        public void ModifyAlert(int alertId, AlertInfo alertInfo)
        {
            try
            {
                User PortalAdminUser =  _userService.GetUserByUserName("PortalAdmin");
                _logger.Debug("Entered Modify Alert Method");
                _logger.Debug("_alertStatusRepository.Load started");
                var alertStatus = _alertStatusRepository.Load(alertId);
                _logger.Debug("_alertStatusRepository.Load Completed");
                _logger.Debug("Convert to Resolved Alerts Started");
                var resolvedAlert = new ResolvedAlert
                {
                    AcknoledgeDate = DateTime.Now,
                    AlarmConfiguration = alertStatus.Alarm,
                    Device = alertStatus.Device,
                    User = PortalAdminUser,
                    ElementIdentifier = alertStatus.ElementIdentifier
                };
                _logger.Debug("Convert to Resolved Alerts Completed");
                _logger.Debug("Add ResolvedAlerts Started");
                _resolvedAlertRepository.Add(resolvedAlert);
                _logger.Debug("Add ResolvedAlerts Completed");
                _logger.Debug("Alert status is ack is set to true and alert count = 0 and time stamp is set to current time and isok = true");
                // alertStatus.IsAcknowledged = true;
                alertStatus.AlertCount = 0;
                alertStatus.LastAlertTimeStamp = DateTime.Now;
                alertStatus.IsOk = true;
                _logger.Debug("Update alert status started");
                _alertStatusRepository.Update(alertStatus);
                _logger.Debug("Update alert status completed");
                _logger.Debug("create alert info started");
                 Create(alertInfo);
                 _logger.Debug("create alert info completed");
                _logger.Debug("Update alert status ended");

                LogOperation(LogAction.AlertAcknowledge, alertStatus, PortalAdminUser);

               // _unitOfWork.Commit();
            }
            catch (Exception E)
            {
                this._unitOfWork.Rollback();

                throw new ServiceException("Cannot Acknoledge Alert", E);
            }
        }

        public Extensions.Page<AlertStatus> GetAlertsByStatus(int pageNumber, int pageSize, string sortBy, bool ascending, DashboardFilter filter)

        {
            var query = CurrentUserAlertStatusQuery();            

            switch (filter)
            {
                case DashboardFilter.Normal:
                    //DB Dev Timeout
                    //query = query.Where(x => x.IsOk && x.IsAcknowledged);
                    query = query.Where(x => x.IsOk && x.IsAcknowledged && IsValidAlert(x) == true);                    
                    break;

                case DashboardFilter.Alerts:
                    query = query.Where(x => !x.IsOk || !x.IsAcknowledged);

                break;
            }

            var sortBys = sortBy.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var value in sortBys)
            {
                var orderBy = string.Format("{0} {1}", value, (ascending ? string.Empty : "DESC"));
                query = query.OrderBy(orderBy);
            }
            var AlertRepository = query.ToList();
            
            //DB Dev Timeout
            //if (filter == DashboardFilter.Normal)
            //{
            //    AlertRepository = AlertRepository.Where(x => IsValidAlert(x) == true).ToList();
            //}
            var AlertQuery = AlertRepository.AsQueryable();           
            return AlertQuery.ToPage(pageNumber, pageSize);
        }

        private bool IsValidAlert(AlertStatus objAlertStatus)
        {
            Device objDevice = objAlertStatus.Device; string ElementIdentifier = objAlertStatus.ElementIdentifier;
            bool IsActiveAlert = true;
            if (objDevice != null && !string.IsNullOrEmpty(ElementIdentifier) && objAlertStatus.Alarm.AlarmType == AlarmType.VideoLoss)
            {
                Diebold.Domain.Entities.Dvr objDvr = (Diebold.Domain.Entities.Dvr)objDevice;
                if (objDvr != null && objDvr.Cameras.Where(x => x.Active == true && x.Channel.Equals(ElementIdentifier)).FirstOrDefault() == null)
                {
                    IsActiveAlert = false;
                }
            }
            return IsActiveAlert;
        }

        public IList<AlertStatus> GetPendingAlertsByDevice(int deviceId, int alarmConfigurationId)
        {
            return _alertStatusRepository.All().Where(x => !x.IsOk && x.Device.Id == deviceId && x.Alarm.Id == alarmConfigurationId).ToList();
        }

        public AlertStatus GetAlertStatusByPK(int pk)
        {
            var query = CurrentUserAlertStatusQuery().Where(x => x.Id == pk);

            return query.Single();
        }

        public Page<ResolvedAlert> GetResolvedAlertsFormDevice(int pageNumber, int pageSize, string sortBy,
                                                               bool ascending,
                                                               int deviceId)
        {
            var orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));

            var query = from lastResolvedAlert in _resolvedAlertRepository.All()
                        where lastResolvedAlert.Device.Id == deviceId
                        select lastResolvedAlert;

            return query.OrderBy(orderBy).ToPage(pageNumber, pageSize);
        }

        public IQueryable<AlertInfo> WithAlarmTypes(IQueryable<AlertInfo> query, IEnumerable<AlarmType> alarmTypes)
        {
            return query.Where(x => x.Alarm.AlarmType != null && alarmTypes.Contains(x.Alarm.AlarmType.Value));
        }
        
        public IQueryable<AlertInfo> WithDates(IQueryable<AlertInfo> query, DateTime dateFrom, DateTime dateTo)
        {
            query = query.Where(x => x.DateOccur >= dateFrom);
            query = query.Where(x => x.DateOccur <= dateTo);

            return query;
        }

        public IQueryable<AlertInfo> WithDeviceIds(IQueryable<AlertInfo> query, IList<int> deviceIds)
        {
            return query.Where(x => deviceIds.Contains(x.Device.Id));
        }

        public Page<ResultsReport> GetAlertsForReport(int pageIndex, int pageSize, string sortBy, bool ascending, 
                                                       IList<string> alarmTypes, IList<int> userIds,
                                                       string deviceStatus,string dateType, DateTime dateFrom, DateTime dateTo,
                                                       IList<int> deviceIds, bool groupLevelSelected)
        {
            int rowCount;
            var alerts = ((IAlertInfoRepository) _repository).GetAlertsForReport(alarmTypes, userIds, deviceStatus, dateType, dateFrom, dateTo, deviceIds,
                                                           pageIndex, pageSize, sortBy, ascending, groupLevelSelected, out rowCount);

            return alerts.ToPage(pageIndex, pageSize, rowCount);
        }

        public IList<ResultsReport> GetAlertsForReport(IList<string> alarmTypes, IList<int> userIds,
                                                       string deviceStatus, string dateType, DateTime dateFrom, DateTime dateTo,
                                                       IList<int> deviceIds, string sortBy, bool ascending, bool groupLevelSelected)
        {
            int rowCount;
            return ((IAlertInfoRepository)_repository).GetAlertsForReport(alarmTypes, userIds, deviceStatus, dateType, dateFrom, dateTo, deviceIds, 
                                                                          null, null, sortBy, ascending, groupLevelSelected, out rowCount);
        }

        public void CreateAlert(IList<AlertInfo> alerts)
        {
            var DeviceDetails = (Dvr)alerts[0].Device;

            if ((DeviceDetails.DeviceType == DeviceType.dmpXR100) || (DeviceDetails.DeviceType == DeviceType.dmpXR500) || (DeviceDetails.DeviceType == DeviceType.bosch_D9412GV4) || (DeviceDetails.DeviceType == DeviceType.videofied01))
            {
                CreateIntrusionAlert(alerts);
            }
            else if ((DeviceDetails.DeviceType == DeviceType.eData300) || (DeviceDetails.DeviceType == DeviceType.eData524) || (DeviceDetails.DeviceType == DeviceType.dmpXR100Access) || (DeviceDetails.DeviceType == DeviceType.dmpXR500Access))
            {
                CreateAccessAlert(alerts);
            }
            else if ((DeviceDetails.DeviceType == DeviceType.Costar111) || (DeviceDetails.DeviceType == DeviceType.ipConfigure530) || (DeviceDetails.DeviceType == DeviceType.VerintEdgeVr200))
            {
                CreateDVRAlert(alerts);
            }
            
        }

        private void UpdateDeviceStatusFromAlert(AlertInfo firstAlert)
        {
            var device = firstAlert.Device;

            //device.DeviceStatus.Clear();
            //foreach (var ds in firstAlert.Device.DeviceStatus)
            //{
            //    device.DeviceStatus.Add(ds);
            //}
        }

        private IQueryable<AlertStatus> GetAlertStatusQuery(AlertInfo alertInfo, bool hasIdentifier)
        {
            var queryAlertStatus = _alertStatusRepository.FilterBy(x => x.Device.Id == alertInfo.Device.Id && x.Alarm.Id == alertInfo.Alarm.Id);
            if (!string.IsNullOrEmpty(alertInfo.ElementIdentifier))
            {
                queryAlertStatus = queryAlertStatus.Where(x => x.ElementIdentifier == alertInfo.ElementIdentifier);
            }

            return queryAlertStatus;
        }

        private AlertStatus GetAlertStatus(AlertInfo alertInfo, bool hasIdentifier)
        {
           // return GetAlertStatusQuery(alertInfo, hasIdentifier).Single(); 09 July
           var GetAlertStatus =  GetAlertStatusQuery(alertInfo, hasIdentifier);
           List<AlertStatus> lstAlertstatus = new List<AlertStatus>();
           foreach (var item in GetAlertStatus)
           {
               lstAlertstatus.Add(item);
           }
           if (lstAlertstatus != null && lstAlertstatus.Count() == 1)
           {
               return lstAlertstatus.First();
           }
           else if (lstAlertstatus != null && lstAlertstatus.Count() > 1)
           {
               return lstAlertstatus.OrderByDescending(x => x.Id).First();
           }
           else
           {
               return lstAlertstatus.FirstOrDefault();
           }

        }

       
        private bool HasAlertStatus(AlertInfo alertInfo, bool hasIdentifier)
        {
            return GetAlertStatusQuery(alertInfo, hasIdentifier).Any();
        }

        private static void UpdateAlertStatus(AlertInfo alertInfo, AlertStatus alertStatus)
        {
            if (alertStatus != null)
            {
                _logger.Debug("Update Alert Status started for static method");
                if (!alertInfo.IsDeviceOk)
                {
                    _logger.Debug("Device is not OK");
                    alertStatus.LastAlertTimeStamp = alertInfo.DateOccur;
                    _logger.Debug("Last time stamp added");
                    if (alertStatus.AlertCount == 0 && alertStatus.IsOk)
                    {
                        //new alert group.
                        _logger.Debug("First time stamp and isacknowledged properties added");
                        alertStatus.FirstAlertTimeStamp = alertInfo.DateOccur;
                        alertStatus.IsAcknowledged = false;
                    }

                    //add alert to existing group.
                    alertStatus.AlertCount++;
                    _logger.Debug("Alert count incremented");
                }
                else
                {
                    //reset counter.
                    _logger.Debug("Device is ok and alert count is reset");
                    alertStatus.AlertCount = 0;
                    //alertStatus.FirstAlertTimeStamp = null;
                    //alertStatus.LastAlertTimeStamp = null;
                }
                 alertStatus.IsOk = alertInfo.IsDeviceOk;
                _logger.Debug("Is Ok property modified");
                _logger.Debug("Update Alert Status completed");
            }
        }

        private static void UpdateAlertStatus(AlertInfo alertInfo, List<AlertStatus> alertStatuses)
        {
            foreach (var alertStatus in alertStatuses)
            {
                if (alertStatus != null)
                {
                    _logger.Debug("Update Alert Status started for static method");
                    if (!alertInfo.IsDeviceOk)
                    {
                        _logger.Debug("Device is not OK");
                        alertStatus.LastAlertTimeStamp = alertInfo.DateOccur;
                        _logger.Debug("Last time stamp added");
                        if (alertStatus.AlertCount == 0 && alertStatus.IsOk)
                        {
                            //new alert group.
                            _logger.Debug("First time stamp and isacknowledged properties added");
                            alertStatus.FirstAlertTimeStamp = alertInfo.DateOccur;
                            alertStatus.IsAcknowledged = false;
                        }

                        //add alert to existing group.
                        alertStatus.AlertCount++;
                        _logger.Debug("Alert count incremented");
                    }
                    else
                    {
                        //reset counter.
                        _logger.Debug("Device is ok and alert count is reset");
                        alertStatus.AlertCount = 0;
                        //alertStatus.FirstAlertTimeStamp = null;
                        //alertStatus.LastAlertTimeStamp = null;
                    }
                    alertStatus.IsOk = alertInfo.IsDeviceOk;
                    _logger.Debug("Is Ok property modified");
                    _logger.Debug("Update Alert Status completed");
                }
            }
            
        }

        private AlertStatus GetLastStatus(int deviceId, int alarmId, string elementIdentifier)
        {
            var lastAlertStatus = _alertStatusRepository.All().Where(x => x.Device.Id == deviceId && x.Alarm.Id == alarmId);

            if (!String.IsNullOrEmpty(elementIdentifier))
                lastAlertStatus = lastAlertStatus.Where(x => x.ElementIdentifier == elementIdentifier);

            lastAlertStatus = lastAlertStatus.OrderByDescending(a => a.Id);

            //DB Dev Timeout
            //return lastAlertStatus.ToList().First();
            return lastAlertStatus.First();
        }

        public void LogOperation(LogAction action, AlertStatus item)
        {
            Device device;
            Site site;
            CompanyGrouping2Level companyGrouping2;
            CompanyGrouping1Level companyGrouping1;

            if (item.Device.IsDvr)
            {
                device = _dvrRepository.FindBy(x => x.Id == item.Device.Id);
                site = ((Dvr) device).Site;
                companyGrouping2 = site.CompanyGrouping2Level;
                companyGrouping1 = companyGrouping2.CompanyGrouping1Level;
            } else
            {
                device = _gatewayRepository.FindBy(x => x.Id == item.Device.Id);
                site = null;
                companyGrouping2 = null;
                companyGrouping1 = null;
            }
            
            _logService.Log(action, item.ToString(), companyGrouping1, companyGrouping2, site, device);
        }

        public void LogOperation(LogAction action, AlertStatus item, User user)
        {
            Device device;
            Site site;
            CompanyGrouping2Level companyGrouping2;
            CompanyGrouping1Level companyGrouping1;

            if (item.Device.IsDvr)
            {
                device = _dvrRepository.FindBy(x => x.Id == item.Device.Id);
                site = ((Dvr)device).Site;
                companyGrouping2 = site.CompanyGrouping2Level;
                companyGrouping1 = companyGrouping2.CompanyGrouping1Level;
            }
            else
            {
                device = _gatewayRepository.FindBy(x => x.Id == item.Device.Id);
                site = null;
                companyGrouping2 = null;
                companyGrouping1 = null;
            }

            _logService.Log(action, item.ToString(), companyGrouping1, companyGrouping2, site, device, user);
        }

        public AlertInfo GetLastAlertInfoByDeviceAndIdentifier(int deviceId, int alarmId, string elementIdentifier)
        {
            var lastAlertInfo = _repository.All().Where(x => x.Device.Id == deviceId && x.Alarm.Id == alarmId &&
                                                             x.ElementIdentifier == elementIdentifier).OrderByDescending
                (a => a.Id).ToList().FirstOrDefault();

            return lastAlertInfo;
        }

        public AlertStatus GetLastAlertStatusByDeviceAndIdentifier(int deviceId, int alarmId, string elementIdentifier)
        {
            var lastAlertStatus = _alertStatusRepository.FilterBy(x => x.Device.Id == deviceId && x.Alarm.Id == alarmId &&
                //DB Dev Timeout                                             
                //x.ElementIdentifier == elementIdentifier).OrderByDescending(a => a.Id).ToList().First();
                                                             x.ElementIdentifier == elementIdentifier).OrderByDescending(a => a.Id).First();

            return lastAlertStatus;
        }

        public bool ValidateSendNotification(AlertInfo alertInfo)
        {
            if (alertInfo.IsDeviceOk)
                return alertInfo.SatisfiesAlertCondition;

            var lastAlertStatus = GetLastStatus(alertInfo.Device.Id, alertInfo.Alarm.Id, alertInfo.ElementIdentifier);

            return !(lastAlertStatus.AlertCount >= 1);
        }

        public IList<AlertStatus> GetAlertsByDevice(int DeviceId)
        {
            return _alertStatusRepository.FilterBy(x => x.Device.Id == DeviceId).ToList();
        }

         private IQueryable<AlertStatus> CurrentUserAlertStatusQuery(string strparentType)
        {
            try
            {
                
                var qryResultSet = (from alert in _alertStatusRepository.All()
                                   where alert.Alarm.DeviceType.HasValue
                                    select alert);
                IQueryable<AlertStatus> qry = null;// = qryResultSet.AsQueryable();
                DeviceType VER200 = DeviceType.VerintEdgeVr200;
                switch (strparentType)
                {
                    case "VerintEdgeVr200":
                       //  qry.Where(x => x.Alarm.DeviceType.Value == VER200).Select(y => y).ToList();
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.VerintEdgeVr200
                              select a;
                        break;
                    case "ipConfigure530": // ipConfigure530
                        //qry.Where(x => x.Alarm.DeviceType.Equals("ipConfigure530"));
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.ipConfigure530
                              select a;
                        break;
                    case "Costar111": // Costar111
                        //qry.Where(x => x.Alarm.DeviceType.Equals("Costar111"));
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.Costar111
                              select a;
                        break;
                    case "eData524":
                        //qry.Where(x => x.Alarm.DeviceType.Equals("eData524"));
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.eData524
                              select a;
                        break;
                    case "eData300":
                        //qry.Where(x => x.Alarm.DeviceType.Equals("eData300"));
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.eData300
                              select a;
                        break;
                    case "dmpXR100Access":
                        //qry.Where(x => x.Alarm.DeviceType.Equals("eData300"));
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.dmpXR100Access
                              select a;
                        break;
                    case "dmpXR500Access":
                        //qry.Where(x => x.Alarm.DeviceType.Equals("eData300"));
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.dmpXR500Access
                              select a;
                        break;
                    case "dmpXR100":
                        //qry.Where(x => x.Alarm.DeviceType.Equals("dmpXR100"));
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.dmpXR100
                              select a;
                        break;
                    case "dmpXR500":
                        //qry.Where(x => x.Alarm.DeviceType.Equals("dmpXR500"));
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.dmpXR500
                              select a;
                        break;
					case "bosch_D9412GV4":
                        //qry.Where(x => x.Alarm.DeviceType.Equals("dmpXR500"));
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.bosch_D9412GV4
                              select a;
                        break;
                    case "videofied01":
                        //qry.Where(x => x.Alarm.DeviceType.Equals("dmpXR500"));
                        qry = from a in qryResultSet
                              where a.Alarm.DeviceType == DeviceType.videofied01
                              select a;
                        break;
                    default:
                        qry = from a in qryResultSet
                              select a;
                        break;
                }
                var query = from alert in qry.AsQueryable()
                            where

                                (from ud in _userDeviceMonitorRepository.All()
                                 where
                                     ud.User == _currentUserProvider.CurrentUser && ud.Device == alert.Device
                                     && ud.Device.DeletedKey == null && !ud.Device.IsDisabled
                                      && !ud.Device.Company.IsDisabled && ud.Device.Company.DeletedKey == null
                                      && !((Dvr)ud.Device).Gateway.IsDisabled && ((Dvr)ud.Device).Gateway.DeletedKey == null
                                      && !((Dvr)ud.Device).Site.IsDisabled && ((Dvr)ud.Device).Site.DeletedKey == null
                                 select ud).Any()
                            select alert;
                return query;
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
                      

           
        }
        
        public IList<AlertStatus> GetAlertDetailsByDeviceType(string strdeviceType)
        {
            var query = CurrentUserAlertStatusQuery(strdeviceType);
           // query.Where(x=>x.Device.dev
            return query.ToList();
        }

        public IList<AlertStatus> GetAlertDetailsByDeviceId(int deviceId)
        {
            var qryResultSet = CurrentUserAlertStatusQuery().Where(x => x.Device.Id == deviceId).OrderByDescending(x => x.LastAlertTimeStamp);
            return qryResultSet.ToList();
        }

        public IList<ResolvedAlert> getPreviouslyAcknoledgedAlets(int deviceId)
        {
               var query = from lastResolvedAlert in _resolvedAlertRepository.All()
                            where lastResolvedAlert.Device.Id == deviceId
                            select lastResolvedAlert;
            return query.ToList();
        }

        public IList<AlertStatus> GetAllAlertDetails()
        {
            var qryResultSet = CurrentUserAlertStatusQuery().OrderByDescending(x => x.LastAlertTimeStamp);
            return qryResultSet.ToList();
        }

        public IList<AlertStatus> GetAllAlertDetailsByParentType(String ParentType)
        {
            var qryResultSet = CurrentUserAlertStatusQuery().OrderByDescending(x => x.LastAlertTimeStamp);
            IList<AlertStatus> alerts = qryResultSet.ToList();

            if (ParentType == "ALL")
            {
                return alerts;
            }
            IList<AlertStatus> parentTypeAlert = new List<AlertStatus>();
            alerts.ForEach(x =>
            {
                Dvr dvr = (Dvr)x.Device;
                if (ParentType == AlarmParentType.DVR.ToString() && (dvr.DeviceType.Equals(DeviceType.Costar111) || dvr.DeviceType.Equals(DeviceType.ipConfigure530) || dvr.DeviceType.Equals(DeviceType.VerintEdgeVr200)))
                {
                    parentTypeAlert.Add(x);
                }
                else if (ParentType == AlarmParentType.Access.ToString() && (dvr.DeviceType.Equals(DeviceType.eData300) || dvr.DeviceType.Equals(DeviceType.eData524) || dvr.DeviceType.Equals(DeviceType.dmpXR100Access) || dvr.DeviceType.Equals(DeviceType.dmpXR500Access)))
                {
                    parentTypeAlert.Add(x);
                }
                else if (ParentType == AlarmParentType.Intrusion.ToString() && (dvr.DeviceType.Equals(DeviceType.dmpXR100) || dvr.DeviceType.Equals(DeviceType.dmpXR500) || dvr.DeviceType.Equals(DeviceType.bosch_D9412GV4) || dvr.DeviceType.Equals(DeviceType.videofied01)))
                {
                    parentTypeAlert.Add(x);
                }
            });
            return parentTypeAlert;
        }

        public AlertStatus GetAlertStatusByDeviceIdandAlarmConfigId(int DeviceId, int AlarmConfigId)
        {
           // var qryResultSet = CurrentUserAlertStatusQuery().Where(x => x.Device.Id == DeviceId && x.Alarm.Id == AlarmConfigId);
           var qryResultSet = _alertStatusRepository.All().Where(x => x.Device.Id == DeviceId && x.Alarm.Id == AlarmConfigId);
            return ((AlertStatus)qryResultSet.FirstOrDefault());
        }
        public string getEMC(string alarmType, int deviceId)
        {
            EMCParameters objEMCParam = new EMCParameters();
            Dvr device;
            device = _dvrRepository.FindBy(x => x.Id == deviceId);
            objEMCParam.siteId = device.Gateway.EMCId.ToString();
            objEMCParam.alarmType = "271";
            objEMCParam.zone = device.ZoneNumber;
            string alarm = alarmType.ToLower();
            if (alarm == AlarmType.NetworkDown.ToString().ToLower())
            {
                objEMCParam.status = "003";
            }
            else if (alarm == "alertclear")
            {
                objEMCParam.status = "002";
            }
            else
            {
                objEMCParam.status = "001";
            }
            string strResult = _alertApiService.getEMC(objEMCParam);
            return strResult;
        }

        public List<AlertStatus> ContainsAnyAlertStatus(List<AlertStatus> AllAlertStatus, string values)
        {
            List<AlertStatus> lstAlertStatus = new List<AlertStatus>();

            if (values != null)
            {
                lstAlertStatus = (from a in AllAlertStatus
                                  where a.ElementIdentifier.Equals(values)
                                  select a).ToList();
            }
            else
            {
                lstAlertStatus = (from a in AllAlertStatus
                                  where a.ElementIdentifier == null
                                  select a).ToList();
            }        
                                    
                                    
            //if (AlertStatusResult != null && AlertStatusResult.Count() > 0)
            //{
            //    // return AlertStatusResult.OrderByDescending(x => x.Id).First();
            //    AlertStatusResult.ForEach(x => lstAlertStatus.Add(x));
            //}
            return lstAlertStatus;
            
        }

        public List<AlertStatus> ContainsAnyAlertStatus(List<AlertStatus> AllAlertStatus, string values, int[] alarmId)
        {
            List<AlertStatus> lstAlertStatus = new List<AlertStatus>();

            var AlertStatusResult = from a in AllAlertStatus
                                    where alarmId.Contains(a.Alarm.Id)
                                    select a;
            if (values != null)
            {
                AlertStatusResult = from a in AlertStatusResult
                                    where a.ElementIdentifier.Contains(values)
                                    select a;
            }



            if (AlertStatusResult != null && AlertStatusResult.Count() > 0)
            {
                // return AlertStatusResult.OrderByDescending(x => x.Id).First();
                AlertStatusResult.ForEach(x => lstAlertStatus.Add(x));
            }
            return lstAlertStatus;

        }

        public List<AlertStatus> EqualsAnyAlertStatus(List<AlertStatus> AllAlertStatus, string values, int alarmId)
        {
            List<AlertStatus> lstAlertStatus = new List<AlertStatus>();
            // var AllAlertStatus = _alertStatusRepository.All();
            // Hard coded to check whether the element identifier is equal if the value is Data Room

            var AlertStatusResult = from a in AllAlertStatus
                                    where a.ElementIdentifier.Equals(values)
                                     && a.Alarm.Id == alarmId
                                    select a;
            if (AlertStatusResult != null && AlertStatusResult.Count() > 0)
            {
                // return AlertStatusResult.OrderByDescending(x => x.Id).First();
                AlertStatusResult.ForEach(x => lstAlertStatus.Add(x));
            }
            return lstAlertStatus;
        }

        /*
        private void AssignDefaultValues(AlertInfo alertInfo)
        {
            if (alertInfo.SatisfiesAlertCondition)
            {
                var queryAlertInfo = _repository.FilterBy(x => x.Device.Id == alertInfo.Device.Id && x.Alarm.Id == alertInfo.Alarm.Id
                                                                    && x.ElementIdentifier == alertInfo.ElementIdentifier);
                var alertInfoList = queryAlertInfo.ToList();
                _repository.Add(alertInfo);
                if (alertInfoList.Count > 0)
                {

                    var alert = alertInfoList.Last();
                    alertInfo.GroupId = !alert.IsDeviceOk ? alert.GroupId : alertInfo.Id;
                }
                else
                    alertInfo.GroupId = alertInfo.Id;
            }
        }*/

        
        private void AssignDefaultValues(AlertInfo alertInfo)
        {
            if (alertInfo.SatisfiesAlertCondition)
            {
                var queryAlertInfo = _repository.FilterBy(x => x.Device.Id == alertInfo.Device.Id && x.Alarm.Id == alertInfo.Alarm.Id
                                                                    && x.ElementIdentifier == alertInfo.ElementIdentifier).OrderByDescending(x => x.Id);
                //var alertInfoList = queryAlertInfo.ToList();
                
                _repository.Add(alertInfo);
                try
                {
                    var lastAlert = queryAlertInfo.First();
                    if (lastAlert != null)
                    {
                        alertInfo.GroupId = !lastAlert.IsDeviceOk ? lastAlert.GroupId : alertInfo.Id;
                    }
                    else
                        alertInfo.GroupId = alertInfo.Id;
                }
                catch (Exception e)
                {
                    alertInfo.GroupId = alertInfo.Id;
                }

                
            }
        }
        


        private void  CreateIntrusionAlert(IList<AlertInfo> alerts)
        {
            try
            {
                var Zoneconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.ZoneTrouble || x.AlarmType == AlarmType.ZoneBypass || x.AlarmType == AlarmType.ZoneAlarm)).Select(y => y.Id).ToList();
                var areaconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.AreaArmed || x.AlarmType == AlarmType.AreaDisarmed)).Select(y => y.Id).ToList();
                var networkDownconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.NetworkDown)).Select(y => y.Id).ToList();
                List<int> alarmconflist = new List<int>();
                if (Zoneconfiguration.Contains(alerts[0].Alarm.Id))
                {
                    alarmconflist.AddRange(Zoneconfiguration);
                }
                else if (areaconfiguration.Contains(alerts[0].Alarm.Id))
                {
                    alarmconflist.AddRange(areaconfiguration);
                }
                else
                {
                    alarmconflist.AddRange(networkDownconfiguration);
                }

                var AllAlertStatus = _alertStatusRepository.All().Where(x => x.IsOk == false && x.ElementIdentifier != null &&x.Device.Id == alerts[0].Device.Id && alarmconflist.Contains(x.Alarm.Id)).ToList();
                foreach (var alertInfo in alerts)
                {
                    var firstAlert = alertInfo;
                    var standardAlertStatus = _alertStatusRepository.FilterBy(x => x.Alarm == firstAlert.Alarm && x.Device == firstAlert.Device && x.ElementIdentifier == null).SingleOrDefault();
                    if (standardAlertStatus != null)
                    {
                        _alertStatusRepository.Delete(standardAlertStatus);
                    }
                   _validationProvider.Validate(alertInfo);
                    //If it should create a new alert info.
                    AssignDefaultValues(alertInfo);
                    List<AlertStatus> alcontainstatus = ContainsAnyAlertStatus(AllAlertStatus, alertInfo.ElementIdentifier);
                    List<AlertStatus> alequalstatus = EqualsAnyAlertStatus(AllAlertStatus, alertInfo.ElementIdentifier, alertInfo.Alarm.Id);
                    //Create alert status if it doesn't exist for this element.
                    if (alertInfo.ElementIdentifier != null)
                    {
                        if(alcontainstatus.Count() == 0)
                        {
                            if ((alertInfo.Alarm.AlarmType == AlarmType.ZoneBypass || alertInfo.Alarm.AlarmType == AlarmType.ZoneAlarm || alertInfo.Alarm.AlarmType == AlarmType.ZoneTrouble) && (alertInfo.Status == null || alertInfo.Status.ToLower().Equals("normal")))
                            {
                                alertInfo.IsDeviceOk = true;
                            }
                            else
                            {
                                var alertStatusElementIdenfifier = new AlertStatus
                                {
                                    Device = alertInfo.Device,
                                    Alarm = alertInfo.Alarm,
                                    ElementIdentifier = alertInfo.ElementIdentifier,
                                    LastAlertTimeStamp = alertInfo.DateOccur
                                };
                                _logger.Debug("Alert Info Intrusion Element Identifier : " + alertInfo.ElementIdentifier);
                                _alertStatusRepository.Add(alertStatusElementIdenfifier);
                            }
                        }
                        else if (alequalstatus != null && alequalstatus.Count() >= 1)
                        {
                            if ((alertInfo.Alarm.AlarmType == AlarmType.ZoneBypass || alertInfo.Alarm.AlarmType == AlarmType.ZoneAlarm || alertInfo.Alarm.AlarmType == AlarmType.ZoneTrouble) && (alertInfo.Status == null || alertInfo.Status.ToLower().Equals("normal")))
                            {
                                alertInfo.IsDeviceOk = true;
                            }
                            _alertStatusRepository.Update(alequalstatus.First());
                        }

                        // Check if the alert is already present, if so then update the previous entry
                        else if (alertInfo.Alarm.AlarmType == AlarmType.AreaArmed || alertInfo.Alarm.AlarmType == AlarmType.AreaDisarmed)
                        {
                            AlertStatus firstAlertStatus = alcontainstatus.First();
                            // update the alertstatus of previously recived alert
                            _alertStatusRepository.Update(firstAlertStatus);
                            alertInfo.IsDeviceOk = true;

                            // Insert a new record to be done 
                            var alertStatusElementIdenfifier = new AlertStatus
                            {
                                Device = alertInfo.Device,
                                Alarm = alertInfo.Alarm,
                                ElementIdentifier = alertInfo.ElementIdentifier,
                                LastAlertTimeStamp = alertInfo.DateOccur
                            };
                            _logger.Debug("Alert Info Intrusion 1 Element Identifier : " + alertInfo.ElementIdentifier);
                            _alertStatusRepository.Add(alertStatusElementIdenfifier);
                            
                        }
                        else if ((alertInfo.Alarm.AlarmType == AlarmType.ZoneBypass || alertInfo.Alarm.AlarmType == AlarmType.ZoneAlarm || alertInfo.Alarm.AlarmType == AlarmType.ZoneTrouble))
                        {
                            if (alertInfo.Status == null || alertInfo.Status.ToLower().Equals("normal"))
                            {
                                AlertStatus firstAlertStatus = alcontainstatus.First();
                                // update the alertstatus of previously recived alert
                                _alertStatusRepository.Update(firstAlertStatus);
                                alertInfo.IsDeviceOk = true;
                            }
                            else
                            {
                                AlertStatus firstAlertStatus = alcontainstatus.First();
                                alertInfo.IsDeviceOk = true;
                                _alertStatusRepository.Update(firstAlertStatus);
                                
                                // Insert a new record to be done 
                                var alertStatusElementIdenfifier = new AlertStatus
                                {
                                    Device = alertInfo.Device,
                                    Alarm = alertInfo.Alarm,
                                    ElementIdentifier = alertInfo.ElementIdentifier,
                                    LastAlertTimeStamp = alertInfo.DateOccur
                                };
                                _logger.Debug("Alert Info Intrusion 1 Element Identifier : " + alertInfo.ElementIdentifier);
                                _alertStatusRepository.Add(alertStatusElementIdenfifier);
                            }
                        }
                        else if (alertInfo.Alarm.AlarmType == AlarmType.NetworkDown)
                        {
                            if (alcontainstatus != null && alcontainstatus.Count() > 0)
                            {
                                AlertStatus firstAlertStatus = alcontainstatus.First();
                                if (alertInfo.Value.ToLower().Equals("false"))
                                {
                                    firstAlertStatus.IsOk = true;
                                    alertInfo.IsDeviceOk = true;
                                }
                                _alertStatusRepository.Update(firstAlertStatus);
                            }
                        }
                    }
                    if (alertInfo.SatisfiesAlertCondition)
                    {
                        //Update status for this element.
                        UpdateAlertStatus(alertInfo, alcontainstatus);
                    }
                }

                // UpdateDeviceStatusFromAlert(firstAlert);
                 this._unitOfWork.Commit();
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("could not execute query"))
                {
                    this._unitOfWork.Rollback();
                    throw new ServiceException("Cannot Update Status", e);
                }
                
            }
        }

        public void CreateClearAlert(IList<AlertInfo> alerts)
        {
            try
            {
                var AllAlertStatus = _alertStatusRepository.All().Where(x => x.IsOk == false && x.Device.Id == alerts[0].Device.Id && x.Alarm.Id == alerts[0].Alarm.Id).ToList();
                var AllAlertInfo = _repository.All().Where(x => x.IsDeviceOk == false && x.Device.Id == alerts[0].Device.Id && x.Alarm.Id == alerts[0].Alarm.Id).ToList();
                if (AllAlertStatus != null && AllAlertStatus.Count() > 0)
                {
                    foreach (var item in AllAlertStatus)
                    {
                        item.IsOk = true;
                        _alertStatusRepository.Update(item);
                    }
                }
                if (AllAlertInfo != null && AllAlertInfo.Count() > 0)
                {
                    foreach (var item in AllAlertInfo)
                    {
                        item.IsDeviceOk = true;
                        _repository.Update(item);
                    }
                }
                this._unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                this._unitOfWork.Rollback();
                throw new ServiceException("Cannot Update Status", ex);
            }
            
            
        }

        private void CreateAccessAlert(IList<AlertInfo> alerts)
        {
            try
            {
                var Doorconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.DoorHeld || x.AlarmType == AlarmType.DoorForced)).Select(y => y.Id).ToList();
                var networkDownconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.NetworkDown)).Select(y => y.Id).ToList();
                List<int> alarmconflist = new List<int>();
                if (Doorconfiguration.Contains(alerts[0].Alarm.Id))
                {
                    alarmconflist.AddRange(Doorconfiguration);
                }
                else
                {
                    alarmconflist.AddRange(networkDownconfiguration);
                }

                var AllAlertStatus = _alertStatusRepository.All().Where(x => x.IsOk == false && x.ElementIdentifier != null && x.Device.Id == alerts[0].Device.Id && alarmconflist.Contains(x.Alarm.Id)).ToList();
                foreach (var alertInfo in alerts)
                {
                    var standardAlertStatus = _alertStatusRepository.FilterBy(x => x.Alarm == alertInfo.Alarm && x.Device == alertInfo.Device && x.ElementIdentifier == null).SingleOrDefault();
                    if (standardAlertStatus != null)
                    {
                        _alertStatusRepository.Delete(standardAlertStatus);
                    }
                    _validationProvider.Validate(alertInfo);
                    //If it should create a new alert info.
                    AssignDefaultValues(alertInfo);
                    //Create alert status if it doesn't exist for this element.
                    if (alertInfo.ElementIdentifier != null || alertInfo.ElementIdentifier != "Network Down")
                    {
                        if (ContainsAnyAlertStatus(AllAlertStatus, alertInfo.ElementIdentifier).Count() == 0)
                        {
                            var alertStatusElementIdenfifier = new AlertStatus
                            {
                                Device = alertInfo.Device,
                                Alarm = alertInfo.Alarm,
                                ElementIdentifier = alertInfo.ElementIdentifier,
                                LastAlertTimeStamp = alertInfo.DateOccur
                            };
                            _logger.Debug("Alert Info Acess Element Identifier : " + alertInfo.ElementIdentifier);
                            _alertStatusRepository.Add(alertStatusElementIdenfifier);
                        }

                        // Check if the alert is already present, if so then update the previous entry
                        else if (alertInfo.Alarm.AlarmType == AlarmType.DoorHeld || alertInfo.Alarm.AlarmType == AlarmType.DoorForced)
                        {
                            List<AlertStatus> alstatus = EqualsAnyAlertStatus(AllAlertStatus, alertInfo.ElementIdentifier, alertInfo.Alarm.Id);
                            if (alstatus != null && alstatus.Count() >= 1)
                            {
                                // Check if any changes has happened in alert if so then update and insert else skip this statement
                                AlertStatus firstAlertStatus = alstatus.First();
                                // update the alertstatus of previously recived alert
                                
                                // firstAlertStatus.LastAlertTimeStamp = DateTime.Now;
                                _alertStatusRepository.Update(firstAlertStatus);
                            }
                            else
                            {
                                List<AlertStatus> alContainsstatus = ContainsAnyAlertStatus(AllAlertStatus, alertInfo.ElementIdentifier);
                                // Check if any changes has happened in alert if so then update and insert else skip this statement
                                AlertStatus firstAlertStatus = alContainsstatus.First(); // 15 July
                                // update the alertstatus of previously recived alert
                                firstAlertStatus.IsOk = true;
                                alertInfo.IsDeviceOk = true;
                                _alertStatusRepository.Update(firstAlertStatus);
                                
                                // Insert a new record to be done 
                                var alertStatusElementIdenfifier = new AlertStatus
                                {
                                    Device = alertInfo.Device,
                                    Alarm = alertInfo.Alarm,
                                    ElementIdentifier = alertInfo.ElementIdentifier,
                                    LastAlertTimeStamp = alertInfo.DateOccur
                                };
                                _logger.Debug("Alert Info Acess 1 Element Identifier : " + alertInfo.ElementIdentifier);
                                _alertStatusRepository.Add(alertStatusElementIdenfifier);
                            }
                        }
                    }
                    else
                    {
                        List<AlertStatus> alContainsnwstatus = ContainsAnyAlertStatus(AllAlertStatus, "Network Down", networkDownconfiguration.ToArray());
                        if (alContainsnwstatus != null && alContainsnwstatus.Count() > 0)
                        {
                            if (alertInfo.Value.ToLower().Equals("false"))
                            {
                                alertInfo.IsDeviceOk = true;
                            }
                            else
                            {
                                AlertStatus firstAlertStatus = alContainsnwstatus.First();
                                _alertStatusRepository.Update(firstAlertStatus);
                            }
                        }
                        else
                        {
                            var alertStatusElementIdenfifier = new AlertStatus
                            {
                                Device = alertInfo.Device,
                                Alarm = alertInfo.Alarm,
                                ElementIdentifier = alertInfo.ElementIdentifier,
                                LastAlertTimeStamp = alertInfo.DateOccur
                            };
                            _alertStatusRepository.Add(alertStatusElementIdenfifier);
                        }
                    }

                    if (alertInfo.SatisfiesAlertCondition)
                    {
                        //Update status for this element.
                        UpdateAlertStatus(alertInfo, GetAlertStatus(alertInfo, true));
                        //UpdateAlertStatus(alertInfo, listToupdate);
                    }
                }

                // UpdateDeviceStatusFromAlert(firstAlert);
                this._unitOfWork.Commit();
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("could not execute query"))
                {
                    this._unitOfWork.Rollback();
                    throw new ServiceException("Cannot Update Status", e);
                }
            }
        }

        private void CreateDVRAlert(IList<AlertInfo> alerts)
        {
            try
            {
                //var DaysRecordedconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.DaysRecorded)).Select(y => y.Id).ToList();
                //var networkDownconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.NetworkDown)).Select(y => y.Id).ToList();
                //var IsNotRecordingconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.IsNotRecording)).Select(y => y.Id).ToList();
                //var VideoLossconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.VideoLoss)).Select(y => y.Id).ToList();
                //var SMARTconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.SMART)).Select(y => y.Id).ToList();
                //var DriveTempconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.DriveTemperature)).Select(y => y.Id).ToList();
                //var RaidStatusconfiguration = _alarmConfigurationRepository.All().Where(x => x.Device.Id == alerts[0].Device.Id && (x.AlarmType == AlarmType.RaidStatus)).Select(y => y.Id).ToList();
                //List<int> alarmconflist = new List<int>();
                //if (DaysRecordedconfiguration.Contains(alerts[0].Alarm.Id))
                //{
                //    alarmconflist.AddRange(DaysRecordedconfiguration);
                //}
                //else if (IsNotRecordingconfiguration.Contains(alerts[0].Alarm.Id))
                //{
                //    alarmconflist.AddRange(IsNotRecordingconfiguration);
                //}
                //else
                //{
                //    alarmconflist.AddRange(networkDownconfiguration);
                //}

                var AllAlertStatus = _alertStatusRepository.All().Where(x => x.IsOk == false && x.Device.Id == alerts[0].Device.Id && x.Alarm.Id == alerts[0].Alarm.Id).ToList();
                foreach (var alertInfo in alerts)
                {

                    var firstAlert = alertInfo;

                    _validationProvider.Validate(alertInfo);
                    //If it should create a new alert info.
                    AssignDefaultValues(alertInfo);
                    //Create alert status if it doesn't exist for this element.
                   // alertInfo.ElementIdentifier = null; // Made null because only one alert needs to be inserted into the DB even when we receive mote number of alerts
                    //if (alertInfo.ElementIdentifier != null)
                    //{
                        
                    //}
                    //else
                    //{
                    List<AlertStatus> alEqualsnwstatus = new List<AlertStatus>();
                    List<AlertStatus> alContainsnwstatus = ContainsAnyAlertStatus(AllAlertStatus, null, new int[] { alertInfo.Alarm.Id });
                    if (alertInfo.Alarm.AlarmType.Equals(AlarmType.VideoLoss))
                    {
                        alEqualsnwstatus = EqualsAnyAlertStatus(AllAlertStatus, alertInfo.ElementIdentifier, alertInfo.Alarm.Id);
                        if (alEqualsnwstatus != null && alEqualsnwstatus.Count() > 0)
                        {
                            if (alertInfo.Value.Equals(""))
                            {
                                alertInfo.IsDeviceOk = true;
                            }
                            else
                            {
                                AlertStatus firstAlertStatus = alEqualsnwstatus.First();
                                _alertStatusRepository.Update(firstAlertStatus);
                            }
                        }
                        else
                        {
                            if (alertInfo.Value.Equals(""))
                            {

                            }
                            else
                            {
                                var alertStatusElementIdenfifier = new AlertStatus
                                {
                                    Device = alertInfo.Device,
                                    Alarm = alertInfo.Alarm,
                                    ElementIdentifier = alertInfo.ElementIdentifier,
                                    LastAlertTimeStamp = alertInfo.DateOccur
                                };
                                _alertStatusRepository.Add(alertStatusElementIdenfifier);
                            }
                        }
                    }
                    else
                    {
                        
                        if (alContainsnwstatus != null && alContainsnwstatus.Count() > 0)
                        {
                            AlertStatus firstAlertStatus = alContainsnwstatus.First();
                            _alertStatusRepository.Update(firstAlertStatus);
                        }
                        else
                        {
                            var alertStatusElementIdenfifier = new AlertStatus
                            {
                                Device = alertInfo.Device,
                                Alarm = alertInfo.Alarm,
                                ElementIdentifier = alertInfo.ElementIdentifier,
                                LastAlertTimeStamp = alertInfo.DateOccur
                            };
                            _alertStatusRepository.Add(alertStatusElementIdenfifier);
                        }
                    }
                   // }

                    if (alertInfo.SatisfiesAlertCondition)
                    {
                        _logger.Debug("Alert Info Satisfies Alert Condition");

                        if (alertInfo.Alarm.AlarmType.Equals(AlarmType.VideoLoss))
                        {
                            UpdateAlertStatus(alertInfo, alEqualsnwstatus);
                        }
                        else
                        {
                            UpdateAlertStatus(alertInfo, alContainsnwstatus);
                        }
                        //UpdateAlertStatus(alertInfo, listToupdate);
                    }
                }

                // UpdateDeviceStatusFromAlert(firstAlert);
                this._unitOfWork.Commit();
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("could not execute query"))
                {
                    this._unitOfWork.Rollback();
                    throw new ServiceException("Cannot Update Status", e);
                }
            }
        }

    }

    
}