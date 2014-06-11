using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Platform.Proxies.Contracts
{
    public interface IGatewayService
    {
        IList<string> GetUnassignedMACAddresses();
    }
}
