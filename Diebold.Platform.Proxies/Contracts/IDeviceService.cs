using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Platform.Proxies.Contracts.Enums;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Platform.Proxies.Contracts
{
    public interface IDeviceService
    {
        IList<DeviceTypeDTO> ListDeviceTypes();
        
        /// <summary>
        /// returns the DeviceId (in platform)
        /// </summary>
        /// <param name="newDevice"></param>
        /// <returns></returns>
        string AddDevice(DeviceDTO newDevice);

        void RemoveDevice(int deviceId);

        void ModifyDevice(DeviceDTO updatedDevice);

        IList<DevicePropertyDTO> Status(int deviceId);

        //DeviceActionResultDTO ExecuteAction(DeviceActionEnum action);
    }
}
