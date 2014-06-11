using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;
using Diebold.Domain.Enums;

namespace Diebold.Services.Contracts
{
    public interface IUserService : ICRUDTrackeableService<User>
    {
        bool UserCanPerformAction(string userName, Diebold.Domain.Entities.Action action);

        bool UserIsEnabled(string userName);

        bool IsUserMonitoringParent(int userId, int groupingLevel1Id, int groupingLevel2Id, int siteId, int gatewayId);

        User GetUser(int id);

        int GetMonitoredDevicesCount(int userId);
        int GetMonitoredGatewaysCount(int userId);
        int GetMonitoredSitesCount(int userId);
        int GetMonitoredDevicesCountByType(int userId, string deviceType);

        int GetMonitoredDevicesCountBySite(int userId, int siteId);

        User GetUserByUserName(string userName);

        IList<UserMonitorGroup> GetMonitoredGroupOfDevices(int userId);

        IList<User> GetUsersMonitoringDevice(int deviceId);

        IList<int> GetMonitorGroupMatches(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id);

        void EditUserAndMonitoredGroupOfDevices(User user, IList<UserMonitorGroup> newMonitoredGroupOfDevices,
                                         IList<UserMonitorGroup> deletedMonitoredGroupOfDevices);

        Page<User> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition);
        
        void AddDeviceToUserMonitorGroup(UserMonitorGroup userMonitorGroup);

        void AddDeviceToUserMonitor(IList<int> matches, Dvr device);

        bool UsernameExists(string currentUserName);

        IList<User> GetUsersByCompany(int companyId, UserStatus userStatus);

        bool IsGroupingLevel1MonitoringByUsers(int groupingLevel1Id);

        bool IsGroupingLevel2MonitoringByUsers(int groupingLevel2Id);
        
        IList<CompanyGrouping1Level> GetMonitoringGrouping1LevelsByUser(int userId);

        IList<CompanyGrouping2Level> GetMonitoringGrouping2LevelsByUser(int userId, int? firstGroupLevelId);

        IList<Site> GetMonitoringSitesByUser(int userId, int? seconGroupLevelId);

        IList<Device> GetSitesByUser(int userId, int? seconGroupLevelId);

        IList<Dvr> GetMonitoringDevicesByUser(int userId, int? siteId);

        IList<Dvr> GetDevicesByParentType(int userId, String parentType);
        int GetDevicesCountByParentType(int UserId, string ParentType);
        //int GetDevicesCountByParentTypeAndSite(int UserId, int SiteId, string ParentType);
        IDictionary<String, int> GetDevicesCount(IList<Dvr> objlstDevice);
        IList<DeviceCounts> GetDeviceCountsByUser(int userId);
        IList<Dvr> GetDevicesByUserId(int userId);
        User GetUserByName(string userName);
    }
}
