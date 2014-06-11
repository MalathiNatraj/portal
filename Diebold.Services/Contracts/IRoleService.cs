using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;

namespace Diebold.Services.Contracts
{
    public interface IRoleService : ICRUDTrackeableService<Role>
    {
        IList<string> GetAllActions();

        IList<string> GetActionsByRole(string roleName);

        bool RoleHasAction(string roleName, Domain.Entities.Action action);

        Page<Role> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition);
        void SetRolePortlets(int RoleId, IList<RolePortlets> RolePortlets);
    }
}
