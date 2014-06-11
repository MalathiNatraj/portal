using System.Collections.Generic;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Platform.Proxies.Contracts
{
    public interface IGatewayApiService
    {
        IList<string> GetUnassignedMACAddresses();
        void RevokeDevice(DeviceDTO item);
    }
}
