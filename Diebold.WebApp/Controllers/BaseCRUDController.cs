using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.Domain.Entities;
using Diebold.WebApp.Infrastructure.Filters;
using Diebold.Services.Exceptions;
using Diebold.Domain.Exceptions;

namespace Diebold.WebApp.Controllers
{
    public abstract class BaseCRUDController<T, K> : BaseController where T : IntKeyedEntity where K : BaseMappeableViewModel<T>, new()
    {
        protected readonly ICRUDService<T> _service;

        public BaseCRUDController(ICRUDService<T> service)
        {
            this._service = service;
        }

        protected IEnumerable<JqGridRecord> MapEntityRecords(IEnumerable<T> items)
        {
            return from item in items
                   select new JqGridRecord<K>(Convert.ToString(item.Id),
                       this.MapEntity(item));
        }

        protected abstract K MapEntity(T item);

        protected virtual K InitializeViewModel(K item)
        {
            //on derived classes, can perform extra initialization
            //i.e.: Add items to collections used by Views.
            return item;
        }

        protected void AddModelErrors(ValidationException exception)
        {
            foreach (var error in exception.Errors)
                this.ModelState.AddModelError(error.Key, error.Message);
        }

        public virtual ActionResult Index()
        {
            return this.View();
        }

        //[Inject]
        //public ILogger Logger { get; set; }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Index(JqGridRequest request)
        {
            var pagedList = _service.GetPage(request.PageIndex + 1, request.RecordsCount,
               request.SortingName, request.SortingOrder == JqGridSortingOrders.Asc);

            var response = GetJqGridResponse(pagedList, pagedList.Select(MapEntity));

            return new JqGridJsonResult { Data = response };
        }

        public virtual ActionResult Create()
        {
            // we can call intializeviewmodel from here if needed...

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Create(K newItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    T item = newItem.MapFromViewModel();

                    _service.Create(item);    

                    return RedirectToAction("Index");
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        else if (serviceException.InnerException is RepositoryException)
                        {
                            LogError("Repository Exception occured while creating " + newItem.ToString(), serviceException);
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                        }
                        else
                        {
                            LogError("Service Exception occured while creating " + newItem.ToString(), serviceException);
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                        }
                    }
                }
                catch (Exception E)
                {
                    LogError("Exception occured while creating " + newItem.ToString(), E);
                    ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", E.Message));
                }
            }
            
            //something went wrong...
            this.InitializeViewModel(newItem);
            return View(newItem);
        }

        public virtual ActionResult AjaxCreate()
        {
            return View();
        }

        [HandleAjaxCRUDExceptionAttribute()]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult AjaxCreate(K newItem)
        {   
            ValidateModel(newItem);

            T item = newItem.MapFromViewModel();

            this._service.Create(item);        

            return new JsonResult() { Data = true };
        }



        // GET: /User/Edit/5
        public virtual ActionResult Edit(int id)
        {
            K itemToEdit = MapEntity(_service.Get(id));

            // we can call intializeviewmodel from here if needed...

            return View(itemToEdit);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Edit(int id, K editedItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    editedItem.Id = id;

                    T itemToEdit = editedItem.MapFromViewModel();

                    _service.Update(itemToEdit);

                    return RedirectToAction("Index");
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        else if (serviceException.InnerException is RepositoryException)
                        {
                            LogError("Repository Exception occured while editing " + editedItem.ToString(), serviceException);
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                        }
                        else
                        {
                            LogError("Service Exception occured while creating " + editedItem.ToString(), serviceException);
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                        }
                    }
                }
                catch (Exception E)
                {
                    LogError("Exception occured while creating " + editedItem.ToString(), E);
                    ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", E.Message));
                }
            }

            //something went wrong...
            this.InitializeViewModel(editedItem);
            return View(editedItem);
        }

        // GET: /User/Edit/5
        public virtual ActionResult AjaxEdit(int id)
        {
            K itemToEdit = MapEntity(_service.Get(id));

            // we can call intializeviewmodel from here if needed...

            return View(itemToEdit);
        }

        //
        // POST: /User/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AjaxEdit(int id, K editedItem)
        {
            ValidateModel(editedItem);

            editedItem.Id = id;
            T itemToEdit = editedItem.MapFromViewModel();

            _service.Update(itemToEdit);

            return new JqGridJsonResult() { Data = true };
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Delete(int id)
        {
            try
            {
                _service.Delete(id);
                return JsonOK();
            }
            catch (ServiceException serviceException)
            {
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                LogError("Exception occured while deleting " + id, e);
                return JsonError("An error occurred while deleting item");
            }
        }
    }
}
