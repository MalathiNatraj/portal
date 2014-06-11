using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Contracts;
using Diebold.Services.Impl;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface IUserDefaultsService : ICRUDService<UserDefaults>
    {
        IList<UserDefaults> GetUserDefaultsUserandPortlet(int UserId, string PortletName);

        IList<UserDefaults> GetUserDefaultsUserandPortlet(int UserId, string PortletName, string filterName);
    }
}
