using System;
using System.Web.Mvc;
using System.Web.Security;

namespace DieboldMobile.Infrastructure.Authentication
{
    public class SSOAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            //MakeReturnUriAbsolute(filterContext);

            AllowAnonymousAccessToSpecificActions(filterContext);
        }

        //private void MakeReturnUriAbsolute(AuthorizationContext filterContext)
        //{
        //    if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        string loginUrl = FormsAuthentication.LoginUrl;

        //        if (loginUrl.StartsWith("http") || loginUrl.StartsWith("https"))
        //        {
        //            if (filterContext.HttpContext.Request != null)
        //            {
        //                loginUrl += "?ReturnUrl=" + filterContext.HttpContext.Request.Url.AbsoluteUri;
        //            }
        //            filterContext.Result = new RedirectResult(loginUrl);
        //        }
        //    }
        //}

        private void AllowAnonymousAccessToSpecificActions(AuthorizationContext filterContext)
        {
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                                     ||
                                     filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(
                                         typeof(AllowAnonymousAttribute), true);

            if (!skipAuthorization)
            {
                base.OnAuthorization(filterContext);
            }
        }

    }


    public static class MyExtensions
    {
        public static bool IsAbsoluteUrl(this String str)
        {
            return ( str.StartsWith("http") || str.StartsWith("https") );
        }
    }
    
}