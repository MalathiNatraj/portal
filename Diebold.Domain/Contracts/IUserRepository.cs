using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.Domain.Contracts
{
    public interface IUserRepository : IIntKeyedRepository<User>
    {
    }
}
