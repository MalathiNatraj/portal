using System.Collections.Generic;
using Diebold.Domain.Entities;

namespace Diebold.Domain.Contracts
{
    public interface IUserMonitorGroupRepository : IIntKeyedRepository<UserMonitorGroup>
    {
        bool IsUserMonitoringGroupingLevel1(int userId, int groupingLevel1Id);
        bool IsUserMonitoringGroupingLevel2(int userId, int groupingLevel2Id);
        bool IsUserMonitoringSite(int userId, int siteId);
        bool IsUserMonitoringGateway(int userId, int gatewayId);
        bool IsGroupingLevel1MonitoringByUsers(int groupingLevel1);
        bool IsGroupingLevel2MonitoringByUsers(int groupingLevel2);
        IList<int> GetMonitorGroupMatches(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id);
        IList<CompanyGrouping1Level> GetMonitoringGrouping1LevelsByUser(int userId);
        IList<CompanyGrouping2Level> GetMonitoringGrouping2LevelsByUser(int userId, int? firstGroupLevelId);
        IList<Site> GetMonitoringSitesByUser(int userId, int? secondGroupLevelId);

        IList<Device> GetSitesByUser(int userId, int? secondGroupLevelId);

        IList<Dvr> GetMonitoringDevicesByUser(int userId, int? siteId);
        IList<UserMonitorGroup> GetUserMonitorGroup(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id);
        IList<UserMonitorGroup> GetByDeviceId(int deviceId);
        IList<DeviceCounts> GetDeviceCountsByUser(int userId);
        IList<UserMonitorGroup> GetUserMonitorGroupByUser(int userId);

    }
}
