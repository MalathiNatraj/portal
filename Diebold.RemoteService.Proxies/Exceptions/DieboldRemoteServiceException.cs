using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.RemoteService.Proxies.Exceptions
{
    public class DieboldRemoteServiceException : Exception
    {
        public DieboldRemoteServiceException(string message)
            : base(message)
        {
        }

        public DieboldRemoteServiceException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}
