using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using DieboldMobile.Infrastructure.Response;

namespace DieboldMobile.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class HandleAjaxCRUDExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception == null) return;

            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (filterContext.Exception is InvalidOperationException)
            {
                AjaxOperationErrorResponse response = new AjaxOperationErrorResponse();
                response.ProcessModelErrors(filterContext.Controller.ViewData.ModelState);

                filterContext.Result = new JsonResult() { Data = response };
            }

            //else if (filterContext.Exception is IServiceException)
            //{
            //}
            /*
            filterContext.Result = new JsonResult
            {
                Data = new
                {
                    exceptionContext.Exception.Message,
                    exceptionContext.Exception.StackTrace
                }
            };
            */

            //ErrorSignal.FromCurrentContext().Raise(exceptionContext.Exception);
            filterContext.ExceptionHandled = true;
        }
    }
}