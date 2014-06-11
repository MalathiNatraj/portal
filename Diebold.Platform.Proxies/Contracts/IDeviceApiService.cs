using System.Collections.Generic;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Platform.Proxies.Contracts
{
    public interface IDeviceApiService
    {
        IList<DeviceTypeDTO> ListDeviceTypes();
        
        /// <summary>
        /// returns the DeviceId (in platform)
        /// </summary>
        /// <param name="newDevice"></param>
        /// <returns></returns>
        string AddDevice(DeviceDTO newDevice, bool isGateway);

        string RemoveDevice(DeviceDTO deviceId, bool isGateway);

        void ModifyDevice(DeviceDTO updatedDevice, bool isGateway);

        string Status(DeviceDTO device, bool isGateway);

        string StatusfrmPlatform(DeviceDTO device, bool isGateway);

        void Enabled(string deviceId);

        void Disabled(string deviceId);

        void Restart(DeviceDTO Device, bool isGateway);

        void Reload(DeviceDTO device, bool isGateway);

        void TestConnection(DeviceDTO device, bool isGateway);

        string RemoveDevicefromPlatform(DeviceDTO deletedDevice, bool isGateWay, string ExternalDeviceId);
    }
}
