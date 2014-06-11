using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface ILinkService : ICRUDTrackeableService<Link>
    {
        IList<Link> GetAllLinksByUser(int UserId);
        IList<Link> GetAllActiveLinksByUser(int UserId);
    }
}
