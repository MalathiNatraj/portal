using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.Domain.Contracts
{
    public interface IUserDeviceMonitorRepository : IIntKeyedRepository<UserDeviceMonitor>
    {
        int GetCountOfMonitoredDevicesByUser(int userId);

        int GetCountOfMonitoredDevicesByUserAndSite(int userId, int siteId);

        IList<User> GetUsersMonitoringDevice(int deviceId);

        int GetCountOfMonitoredGatewaysByUser(int userId);

        int GetCountOfMonitoredDevicesByUserAndDeviceType(int userId,string deviceType);

        IList<Device> GetDevicesByUser(int userId);
    }
}
