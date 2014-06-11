using System;
using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Services.Impl;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Services.Contracts
{
    public interface IDeviceService
    {
        bool DeviceIsEnabled(string name);
        StatusDTO GetLiveStatus(int deviceId, string macAddress, bool liveFromDevice, bool EnableLogOperation);
        StatusPlatformDTO GetPlatformLiveStatus(int deviceId, string macAddress, bool liveFromDevice, bool EnableLogOperation);

        IList<string> GetAllDeviceTypes();
        IDictionary<string, string> GetAllPollingFrequencies();
        IList<TimeZoneInfo> GetAllTimeZones();

        Device GetByExternalId(string externalId);

        void Restart(int pk, string DeviceKey, string DeviceType);
        void Reload(int pk, string DeviceKey,string DeviceType);
        void TestConnection(int pk, string DeviceKey,string deviceType);
        void Audit(int id, string DeviceKey,string deviceType);

        IList<int> GetMonitorGroupMatches(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id);

        IList<UserMonitorGroup> GetUserMonitorGroup(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id);

        void AddDeviceToUserMonitor(IList<UserMonitorGroup> matches, Device device);

        Device GetDevice(int id);
    }
}