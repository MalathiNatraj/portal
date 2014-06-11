using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using DieboldMobile.Infrastructure.Authentication;

namespace DieboldMobile.Infrastructure.Helpers
{
    public class ActionsForUserHelper
    {
        public static bool ActionAllowedForUser(string userName, Diebold.Domain.Entities.Action action)
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