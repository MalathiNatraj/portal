using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Services.Contracts
{
    public interface IImpersonateService
    {
        bool impersonateValidUser();
        void undoImpersonation();
    }
}
