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

namespace Diebold.Services.Impl
{
    public class UserService : BaseCRUDTrackeableService<User>, IUserService
    {
        private readonly IUserMonitorGroupRepository _monitorGroupRepository;
        private readonly IDvrService _deviceService;
        private readonly IUserDeviceMonitorRepository _userDeviceMonitorRepository;
        private readonly IUserMonitorGroupRepository _userMonitorGroupRepository;
        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;
        private readonly IDvrService _dvrService;
        
        public UserService(IUserRepository repository, IUnitOfWork unitOfWork,
            IValidationProvider validationProvider, 
            IUserMonitorGroupRepository monitorGroupRepository,
            IDvrService deviceService,
            IUserDeviceMonitorRepository userDeviceMonitorRepository,
            ILogService logService,
            IUserMonitorGroupRepository userMonitorGroupRepository,
            ISiteService siteService,
            ICompanyService companyService, IDvrService dvrService)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            _monitorGroupRepository = monitorGroupRepository;
            _deviceService = deviceService;
            _userDeviceMonitorRepository = userDeviceMonitorRepository;
            _userMonitorGroupRepository = userMonitorGroupRepository;
            _siteService = siteService;
            _companyService = companyService;
            _dvrService = dvrService;
        }

        public bool UserCanPerformAction(string userName, Domain.Entities.Action action)
        {
            try
            {
                var user = _repository.FindBy(u => u.Username == userName.Split('@')[0] && u.DeletedKey == null && u.IsDisabled == false);
                if (user.IsDisabled || user.DeletedKey != null)
                    return false;
                return user.Role.Actions.Contains(action);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public bool UserIsEnabled(string userName)
        {
            User user;

            try
            {
                user = _repository.FindBy(u => u.Username == userName.Split('@')[0] && u.DeletedKey == null && u.IsDisabled == false);
            }
            catch (Exception e)
            {
                throw new Exception("Unknown User", e);
            }
            
            return (!user.IsDisabled && user.DeletedKey == null);
        }

        public int GetMonitoredDevicesCount(int userId)
        {
            return _userDeviceMonitorRepository.GetCountOfMonitoredDevicesByUser(userId);
        }

        public int GetMonitoredGatewaysCount(int userId)
        {
            return _userDeviceMonitorRepository.GetCountOfMonitoredGatewaysByUser(userId);
        }

        public int GetMonitoredDevicesCountBySite(int userId, int siteId)
        {
            return _userDeviceMonitorRepository.GetCountOfMonitoredDevicesByUserAndSite(userId, siteId);
        }
        public int GetMonitoredDevicesCountByType(int userId,string deviceType)
        {
            return _userDeviceMonitorRepository.GetCountOfMonitoredDevicesByUserAndDeviceType(userId, deviceType);
        }

        public int GetMonitoredSitesCount(int userId)
        {
            var userMonitorGroups = _monitorGroupRepository.FilterBy(x => x.User.Id == userId);

            var sitesToCount = new List<Site>();

            foreach (var userMonitorGroup in userMonitorGroups)
            {
                if (userMonitorGroup.Device != null)
                {
                    Dvr objDvr = _deviceService.Get(userMonitorGroup.Device.Id);
                    sitesToCount.Add(objDvr.Site);
                }
                else if (userMonitorGroup.Site != null)
                {
                    sitesToCount.Add(userMonitorGroup.Site);
                }
                else if (userMonitorGroup.FirstGroupLevel != null)
                {
                    sitesToCount.AddRange(userMonitorGroup.FirstGroupLevel.CompanyGrouping2Levels.SelectMany(x => x.Sites));
                }
                else if (userMonitorGroup.SecondGroupLevel != null)
                {
                    sitesToCount.AddRange(userMonitorGroup.SecondGroupLevel.Sites);
                }
            }
            return sitesToCount.Where(x => x.DeletedKey == null && x.IsDisabled == false).Distinct().Count();
        }

        public IList<User> GetUsersMonitoringDevice(int deviceId)
        {
            return _userDeviceMonitorRepository.GetUsersMonitoringDevice(deviceId);
        }

        public IList<int> GetMonitorGroupMatches(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id)
        {
            return _monitorGroupRepository.GetMonitorGroupMatches(gatewayId, siteId, groupingLevel1Id, groupingLevel2Id);
        }

        public bool IsUserMonitoringParent(int userId, int groupingLevel1Id, int groupingLevel2Id, int siteId, int gatewayId)
        {
            if (_monitorGroupRepository.IsUserMonitoringGroupingLevel1(userId, groupingLevel1Id) ||
                _monitorGroupRepository.IsUserMonitoringGroupingLevel2(userId, groupingLevel2Id) ||
                _monitorGroupRepository.IsUserMonitoringSite(userId, siteId))
            {
                return true;
            }

            return false;
        }

        public bool IsGroupingLevel2MonitoringByUsers(int groupingLevel2Id)
        {
            return _monitorGroupRepository.IsGroupingLevel2MonitoringByUsers(groupingLevel2Id);
        }

        public IList<CompanyGrouping1Level> GetMonitoringGrouping1LevelsByUser(int userId)
        {
            return _monitorGroupRepository.GetMonitoringGrouping1LevelsByUser(userId);
        }

        public IList<CompanyGrouping2Level> GetMonitoringGrouping2LevelsByUser(int userId, int? firstGroupLevelId)
        {
            return _monitorGroupRepository.GetMonitoringGrouping2LevelsByUser(userId, firstGroupLevelId).OrderBy(x=>x.Name).ToList();
        }

        public IList<Site> GetMonitoringSitesByUser(int userId, int? seconGroupLevelId)
        {
            return _monitorGroupRepository.GetMonitoringSitesByUser(userId, seconGroupLevelId).OrderBy(x=>x.Name).ToList();
        }

        public IList<Dvr> GetMonitoringDevicesByUser(int userId, int? siteId)
        {
            return _monitorGroupRepository.GetMonitoringDevicesByUser(userId, siteId).OrderBy(x=>x.Name).ToList();
        }

        public bool IsGroupingLevel1MonitoringByUsers(int groupingLevel1Id)
        {
            return _monitorGroupRepository.IsGroupingLevel1MonitoringByUsers(groupingLevel1Id);
        }

        public User GetUserByUserName(string userName)
        {
            User user;

            try
            {
                user = _repository.FindBy(u => u.Username == userName.Split('@')[0] && u.DeletedKey == null && u.IsDisabled == false);
            }
            catch (Exception e)
            {
                throw new Exception("Unknown User", e);
            }

            return user;
        }

        public IList<UserMonitorGroup> GetMonitoredGroupOfDevices(int userId)
        {
            return _monitorGroupRepository.FilterBy(x => x.User.Id == userId)
                .ToList();
        }

        public User GetUser(int id)
        {
            return _repository.FindBy(id);
        }

        public void EditUserAndMonitoredGroupOfDevices(User user, IList<UserMonitorGroup> newMonitoredGroupOfDevices, IList<UserMonitorGroup> deletedMonitoredGroupOfDevices)
        {
            try
            {
                //User user = _repository.Load(userItem.Id);
                
                _repository.Update(user);

                this._validationProvider.ValidateAll(newMonitoredGroupOfDevices);

                //TO DO: check redundancy between groups...
                IList<UserDeviceMonitor> userDeviceMonitorList = new List<UserDeviceMonitor>();

                foreach (UserMonitorGroup group in newMonitoredGroupOfDevices)
                {
                    group.User = user;

                    if (group.Device != null)
                    {
                        userDeviceMonitorList.Add(new UserDeviceMonitor
                                                      {
                                                          Device = _deviceService.Get(group.Device.Id),
                                                          User = user,
                                                          UserMonitorGroup = group
                                                      });
                    }
                    else if (group.Site != null)
                    {
                        //newMonitoredDevices.AddRange(_deviceService.GetDevicesBySiteId(group.Site.Id));
                        foreach (var device in _deviceService.GetDevicesBySiteId(group.Site.Id))
                        {
                            userDeviceMonitorList.Add(new UserDeviceMonitor
                                                          {
                                Device = device,
                                User = user,
                                UserMonitorGroup = group
                            });
                        }
                    }
                    else if (group.SecondGroupLevel != null)
                    {
                        foreach (var device in _deviceService.GetDevicesByCompanyGrouping2Level(group.SecondGroupLevel.Id))
                        {
                            userDeviceMonitorList.Add(new UserDeviceMonitor()
                            {
                                Device = device,
                                User = user,
                                UserMonitorGroup = group
                            });
                        }
                    }
                    else if (group.FirstGroupLevel != null)
                    {
                        foreach (var device in _deviceService.GetDevicesByCompanyGrouping1Level(group.FirstGroupLevel.Id))
                        {
                            userDeviceMonitorList.Add(new UserDeviceMonitor()
                            {
                                Device = device,
                                User = user,
                                UserMonitorGroup = group
                            });
                        }
                    }

                    LogOperation(LogAction.UserAssignMonitor, user);
                }

                _monitorGroupRepository.Add(newMonitoredGroupOfDevices);
                _userDeviceMonitorRepository.Add(userDeviceMonitorList);

                foreach (UserMonitorGroup userMonitorGroup in deletedMonitoredGroupOfDevices)
                {
                    userMonitorGroup.User = user;

                    foreach (UserDeviceMonitor deviceMonitor in _userDeviceMonitorRepository.FilterBy(x => x.UserMonitorGroup.Id == userMonitorGroup.Id))
                    {
                        _userDeviceMonitorRepository.Delete(deviceMonitor);
                    }

                    var userMonitorGroupToDelete = _monitorGroupRepository.FindBy(x => x.Id == userMonitorGroup.Id);
                    _monitorGroupRepository.Delete(userMonitorGroupToDelete);
                }

                this._unitOfWork.Commit();
            }
            catch (Exception E)
            {
                this._unitOfWork.Rollback();

                throw new ServiceException("Entity Create Exception", E);
            }
        }

        public void AddDeviceToUserMonitorGroup(UserMonitorGroup userMonitorGroup)
        {
            _monitorGroupRepository.Add(userMonitorGroup);
        }

        public void AddDeviceToUserMonitor(IList<int> matches, Dvr device)
        {
            IList<int> userProcessed = new List<int>();
            foreach (var groupId in matches)
            {
                var umg = _monitorGroupRepository.Load(groupId);

                if (!userProcessed.Contains(umg.User.Id))
                {
                    _userDeviceMonitorRepository.Add(new UserDeviceMonitor { User = umg.User, Device = device, UserMonitorGroup = umg });
                    userProcessed.Add(umg.User.Id);
                }
            }
        }

        public IList<Dvr> GetDevicesByParentType(int userId, String ParentType)
        {
            IList<UserMonitorGroup> usetMonitorGroup = _userMonitorGroupRepository.GetUserMonitorGroupByUser(userId);
            IDictionary<int, Dvr> deviceMap = new Dictionary<int, Dvr>();
            foreach (UserMonitorGroup group in usetMonitorGroup)
            {
                if (group.Device != null)
                {
                    deviceMap[group.Device.Id] = (Dvr)group.Device;
                }
                else if (group.Site != null)
                {
                    foreach (var device in _deviceService.GetDevicesBySiteId(group.Site.Id))
                    {
                        deviceMap[device.Id] = device;
                    }
                }
                else if (group.SecondGroupLevel != null)
                {
                    foreach (var device in _deviceService.GetDevicesByCompanyGrouping2Level(group.SecondGroupLevel.Id))
                    {
                        deviceMap[device.Id] = device;
                    }
                }
                else if (group.FirstGroupLevel != null)
                {
                    foreach (var device in _deviceService.GetDevicesByCompanyGrouping1Level(group.FirstGroupLevel.Id))
                    {
                        deviceMap[device.Id] = device;
                    }
                }
            }
            var objlstDevice = deviceMap.Values.ToList();
            if (ParentType == "ALL")
            {
                return objlstDevice;
            }
            IList<Dvr> parentTypeDevice = new List<Dvr>();
            objlstDevice.ForEach(x =>
            {
                if (ParentType == AlarmParentType.DVR.ToString() && (x.DeviceType.Equals(DeviceType.Costar111) || x.DeviceType.Equals(DeviceType.ipConfigure530) || x.DeviceType.Equals(DeviceType.VerintEdgeVr200)))
                {
                    parentTypeDevice.Add(x);
                }
                else if (ParentType == AlarmParentType.Access.ToString() && (x.DeviceType.Equals(DeviceType.eData300) || x.DeviceType.Equals(DeviceType.eData524) || x.DeviceType.Equals(DeviceType.dmpXR100Access) || x.DeviceType.Equals(DeviceType.dmpXR500Access)))
                {
                    parentTypeDevice.Add(x);
                }
                else if (ParentType == AlarmParentType.Intrusion.ToString() && (x.DeviceType.Equals(DeviceType.dmpXR100) || x.DeviceType.Equals(DeviceType.dmpXR500) || x.DeviceType.Equals(DeviceType.videofied01) || x.DeviceType.Equals(DeviceType.bosch_D9412GV4)))
                {
                    parentTypeDevice.Add(x);
                }
            });
            return parentTypeDevice;
        }

        public bool UsernameExists(string userName)
        {
            bool usernameExists;

            try
            {
                _repository.FindBy(u => u.Username == userName.Split('@')[0] && u.DeletedKey == null && u.IsDisabled == false);
                usernameExists = true;
            }
            catch (Exception)
            {
                usernameExists = false;
            }

            return usernameExists;
        }

        public Page<User> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);

            if (!string.IsNullOrEmpty(whereCondition))
            {
                query = query.Where(x => x.FirstName.Contains(whereCondition) ||
                                         x.LastName.Contains(whereCondition) ||
                                         x.Email.ToString().Contains(whereCondition) ||
                                         x.Phone.Contains(whereCondition) ||
                                         (x.Role != null && (x.Role.Name.Contains(whereCondition))) ||
                                         (x.Company != null && (x.Company.Name.Contains(whereCondition))));
            }

            string orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));

            return query.OrderBy(orderBy).ToPage(pageNumber, pageSize);
        }

        public IList<User> GetUsersByCompany(int companyId, UserStatus userStatus)
        {
            var query = _repository.All().Where(x => x.Company.Id == companyId);

            switch (userStatus)
            {
                case UserStatus.ActiveUsers: query = query.Where(x => /*x.IsDisabled == false &&*/ x.DeletedKey == null); break;
                case UserStatus.DeletedUsers: query = query.Where(x => x.DeletedKey != null); break;
            }

            return query.ToList().OrderBy(x => x.Name).ToList();
        }

        public IList<Device> GetSitesByUser(int userId, int? seconGroupLevelId)
        {
            return _monitorGroupRepository.GetSitesByUser(userId, seconGroupLevelId);
        }

        public int GetDevicesCountByParentType(int UserId,string ParentType)
        {
            IList<Dvr> objlstDevice = _monitorGroupRepository.GetMonitoringDevicesByUser(UserId, null);
            IList<Dvr> parentTypeDevice = new List<Dvr>();
            objlstDevice.ForEach(x =>
            {
                if (ParentType == AlarmParentType.DVR.ToString() && (x.DeviceType.Equals(DeviceType.Costar111) || x.DeviceType.Equals(DeviceType.ipConfigure530) || x.DeviceType.Equals(DeviceType.VerintEdgeVr200)))
                {
                    parentTypeDevice.Add(x);
                }
                else if (ParentType == AlarmParentType.Access.ToString() && (x.DeviceType.Equals(DeviceType.eData300) || x.DeviceType.Equals(DeviceType.eData524) || x.DeviceType.Equals(DeviceType.dmpXR100Access) || x.DeviceType.Equals(DeviceType.dmpXR500Access)))
                {
                    parentTypeDevice.Add(x);
                }
                else if (ParentType == AlarmParentType.Intrusion.ToString() && (x.DeviceType.Equals(DeviceType.dmpXR100) || x.DeviceType.Equals(DeviceType.dmpXR500) || x.DeviceType.Equals(DeviceType.bosch_D9412GV4) || x.DeviceType.Equals(DeviceType.videofied01)))
                {
                    parentTypeDevice.Add(x);
                }
            });

            return parentTypeDevice.Count();
        }

        public IDictionary<String, int> GetDevicesCount(IList<Dvr> objlstDevice)
        {
            int dvrCount = 0;
            int intrusionCount = 0;
            int accessCount = 0;
            objlstDevice.ForEach(x =>
            {
                if (x.DeviceType.Equals(DeviceType.Costar111) || x.DeviceType.Equals(DeviceType.ipConfigure530) || x.DeviceType.Equals(DeviceType.VerintEdgeVr200))
                {
                    dvrCount++;
                }
                else if (x.DeviceType.Equals(DeviceType.eData300) || x.DeviceType.Equals(DeviceType.eData524) || x.DeviceType.Equals(DeviceType.dmpXR100Access) || x.DeviceType.Equals(DeviceType.dmpXR500Access))
                {
                    accessCount++;
                }
                else if (x.DeviceType.Equals(DeviceType.dmpXR100) || x.DeviceType.Equals(DeviceType.dmpXR500) || x.DeviceType.Equals(DeviceType.bosch_D9412GV4) || x.DeviceType.Equals(DeviceType.videofied01))
                {
                    intrusionCount++;
                }
            });
            IDictionary<String, int> parentTypeDeviceCountMap = new Dictionary<String, int>();
            parentTypeDeviceCountMap.Add("DVR", dvrCount);
            parentTypeDeviceCountMap.Add("ACCESS", accessCount);
            parentTypeDeviceCountMap.Add("INTRUSION", intrusionCount);
            return parentTypeDeviceCountMap;
        }

        public IList<Dvr> GetDevicesByUserId(int userId)
        {
            IList<Dvr> objlstDevice = _monitorGroupRepository.GetMonitoringDevicesByUser(userId, null);
            return objlstDevice;
        }

        public IList<DeviceCounts> GetDeviceCountsByUser(int userId)
        {
            IList<DeviceCounts> DeviceListCountDetails = _userMonitorGroupRepository.GetDeviceCountsByUser(userId);
            return DeviceListCountDetails;
        }

        public User GetUserByName(string userName)
        {
            User user = new User();
            //DB Dev Timeout
            //List<User> lstUser = new List<User>();
            try
            {
                //lstUser = _repository.All().Where(u => u.Username == userName.Split('@')[0] && u.DeletedKey == null && u.IsDisabled == false).ToList();                
                user = _repository.All().Where(u => u.Username == userName.Split('@')[0] && u.DeletedKey == null && u.IsDisabled == false).FirstOrDefault();                
                //if (lstUser != null && lstUser.Count() > 0)
                //{
                //    user = lstUser.First();
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (Exception e)
            {
                _logger.Debug("Error occurred in GetUserByName in UserService" + e.Message);
                throw new Exception("Unknown User", e);
            }

            return user;
        }
    }
}
