using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Diebold.WebApp.Infrastructure.Authentication;

namespace Diebold.WebApp.Infrastructure.Helpers
{
    public static class ActionsForUserHelper
    {
        public static bool ActionAllowedForUser(string userName, Domain.Entities.Action action)
        {
            var memberShipProvider = (Membership.Provider as DieboldMembershipProvider);

            if (memberShipProvider != null)
            {
                return memberShipProvider.UserCanPerformAction(userName, action);
            }

            return false;
        }
    }
}