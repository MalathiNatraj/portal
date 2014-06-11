using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Services.Exceptions
{
    public class ServiceException : Exception
    {
        public ServiceException(string message)
            : base(message)
        {
        }

        public ServiceException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}
