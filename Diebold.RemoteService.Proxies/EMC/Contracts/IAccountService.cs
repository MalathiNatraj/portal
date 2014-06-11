using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.RemoteService.Proxies.EMC.Contracts
{
    public interface IAccountService
    {
        bool CheckIfExists(int Id);
    }
}
