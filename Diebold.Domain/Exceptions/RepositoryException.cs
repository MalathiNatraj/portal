using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Diebold.Domain.Exceptions
{
    public class RepositoryException : SystemException
    {
        public RepositoryException(string message)
            : base(message)
        {
        }
    }
}