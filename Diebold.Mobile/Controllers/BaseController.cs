using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;
using DieboldMobile.Models;
using Lib.Web.Mvc.JQuery.JqGrid;
using log4net;

namespace DieboldMobile.Controllers
{
    public abstract class BaseController : Controller
    {
        protected static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //handle exceptions and logging...
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //handle exception and logging...
            base.OnActionExecuted(filterContext);
        }

        #region Json Status

        protected enum StatusType
        {
            OK,
            Error
        }

        protected JsonResult JsonError(string message)
        {
            return JsonStatus(StatusType.Error, message);
        }

        protected JsonResult JsonOK(string message = null)
        {
            return JsonStatus(StatusType.OK, message);
        }

        protected JsonResult JsonStatus(StatusType statusType, string message = null)
        {
            return Json(new
            {
                Status = statusType.ToString(),
                Message = message
            });
        }

        protected JsonResult JsonObject(object data)
        {
            return JsonObject(StatusType.OK, data);
        }

        protected JsonResult JsonObject(StatusType statusType, object data)
        {
            return Json(new
            {
                Status = statusType.ToString(),
                Data = data
            });
        }

        #endregion

        #region JQGrid

        protected static JqGridResponse GetJqGridResponse<T, VM>(Page<T> page, IEnumerable<VM> viewModelList)
            where T : IntKeyedEntity
            where VM : BaseMappeableViewModel<T>, new()
        {
            var response = new JqGridResponse
            {
                TotalPagesCount = page.TotalPages,
                PageIndex = page.PageIndex - 1,
                TotalRecordsCount = page.TotalItems
            };

            IList<JqGridRecord<VM>> records = viewModelList.Select(item => new JqGridRecord<VM>(Convert.ToString(item.Id), item)).ToList();
            response.Records.AddRange(records);

            return response;
        }

        protected static JqGridResponse GetJqGridResponseEmpty<T, VM>()
            where T : IntKeyedEntity
            where VM : BaseMappeableViewModel<T>, new()
        {
            var response = new JqGridResponse
            {
                TotalPagesCount = 0,
                PageIndex = 0,
                TotalRecordsCount = 0,

            };

            response.Records.AddRange(Enumerable.Empty<JqGridRecord>());

            return response;
        }

        #endregion  

        public void LogDebug(object message)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug(message);
            }
        }

        public void LogError(object message)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(message);
            }
        }

        public void LogError(object message, Exception exception)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(message, exception);
            }
        }

    }
}
