using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Contracts;

namespace Diebold.WebApp.Infrastructure.Authentication
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IMembershipService service;

        public CurrentUserProvider(IMembershipService service)
        {
            this.service = service;
        }
        
        /// <summary>
        /// Brings Current Logged User (Uses current Session).
        /// 
        /// TO DO: Research if a fresh DB Session is convenient (i.e. 
        /// when editing permissions may receive a stale permission list?)
        /// </summary>
        public Domain.Entities.User CurrentUser
        {
            get
            {
                return service.GetUserByUserName(this.CurrentUserName);
            }
        }

        /// <summary>
        /// Validates if there's a user with the username from SecureAuth
        /// </summary>
        public bool UsernameExists
        {
            get
            {
                return service.UsernameExists(this.CurrentUserName);
            }
        }

        private string CurrentUserName
        {
            get
            {
                if (HostingEnvironment.IsHosted)
                {
                    var cur = HttpContext.Current;
                    if (cur != null)
                        return cur.User.Identity.Name;
                }
                var user = Thread.CurrentPrincipal;
                
                if (user == null || user.Identity == null)
                    return String.Empty;
                
                return user.Identity.Name;
            }
        }
    }
}