using System;
using System.Web.Mvc;
using DieboldMobile.Models;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Diebold.Services.Exceptions;

namespace DieboldMobile.Controllers
{
    public abstract class BaseCRUDTrackeableController<T, K> : BaseCRUDController<T, K>
        where T : TrackeableEntity
        where K : BaseMappeableViewModel<T>, new()
    {
        protected new readonly ICRUDTrackeableService<T> _service;

        protected BaseCRUDTrackeableController(ICRUDTrackeableService<T> service)
            : base(service)
        {
            this._service = service;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Enable(int id)
        {
            try
            {
                _service.Enable(id);

                return JsonOK();
            }
            catch (ServiceException serviceException)
            {
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                return JsonError("An error occurred while enabling item");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Disable(int id)
        {
            try
            {
                _service.Disable(id);
                return JsonOK();
            }
            catch (ServiceException serviceException)
            {
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                return JsonError("An error occurred while disabling item");
            }
        }
    }
}
