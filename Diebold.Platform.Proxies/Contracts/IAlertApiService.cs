using System.Collections.Generic;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Platform.Proxies.Contracts
{
    public interface IAlertApiService
    {
        string getEMC(EMCParameters objEMC);
    }
}
