using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Contracts;
using Diebold.Services.Impl;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface IRolePortletService : ICRUDService<RolePortlets>
    {
        IList<RolePortlets> GetAllRolePortletByroleId(int RoleId);
    }
}
