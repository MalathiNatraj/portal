using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.RemoteService.Proxies.EMC.DTO;

namespace Diebold.RemoteService.Proxies.EMC.Contracts
{
    public interface IEmcService
    {
        void SendAlarm(string emcAccountNumber, string zoneNumber);
        bool ValidateEmcAccount(string emcAccountNumber);
    }
}
