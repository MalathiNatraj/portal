using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface IUserPortletsPreferences : ICRUDTrackeableService<UserPortletsPreferences>
    {
        IList<UserPortletsPreferences> GetAllPortletsByUser(int UserId);
        IList<UserPortletsPreferences> GetAllActivePortletsByUser(int UserId);
        IList<UserPortletsPreferences> GetInActivePortletsByUserforLiveView(int UserId);
    }
}
