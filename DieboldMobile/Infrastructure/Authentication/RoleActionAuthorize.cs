using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Domain.Entities;
using System.Security.Principal;
using Diebold.Services.Contracts;
using Ninject;

namespace DieboldMobile.Infrastructure.Authentication
{
    public class RoleActionAuthorize : IAuthorizationFilter
    {
        //protected readonly IRoleService _roleService;
        protected readonly IUserService _userService;

        protected readonly Diebold.Domain.Entities.Action _action;

        /*
        public Diebold.Domain.Entities.Action Action
        {
            get { return _action; }
            set { _action = value; }
        }
        */

        public RoleActionAuthorize(IUserService userService, Diebold.Domain.Entities.Action roleAction)
            : base()
        {
            _userService = userService;
            _action = roleAction;
        }

        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        protected bool AuthorizeCore(HttpContextBase httpContext)
        {
            //if (!base.AuthorizeCore(httpContext))
            //    return false;

            //string[] roles = System.Web.Security.Roles.GetRolesForUser(httpContext.User.Identity.Name);

            if (!_userService.UserCanPerformAction(httpContext.User.Identity.Name, _action))
            {
                return false;
            }

            return true;
        }

        //protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        protected void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult(403);
            }
            else
            {
                //base.HandleUnauthorizedRequest(filterContext);
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            var user = filterContext.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                HandleUnauthorizedRequest(filterContext);
            }

            if (!AuthorizeCore(filterContext.HttpContext))
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}