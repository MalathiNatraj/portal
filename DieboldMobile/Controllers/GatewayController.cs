using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Diebold.Services.Extensions;
using DieboldMobile.Models;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.Services.Exceptions;
using Diebold.Domain.Exceptions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace DieboldMobile.Controllers
{
    public class GatewayController : BaseCRUDTrackeableController<Gateway, GatewayViewModel>
    {
        private readonly IGatewayService _gatewayService;
        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;
        private readonly INotificationService _notificationService;
        private IList<string> UnassignedMacs
        {
            get { return (IList<string>)Session["UnassignedMacs"]; }
            set { Session["UnassignedMacs"] = value; }
        }

        public GatewayController(IGatewayService service, ISiteService siteService, ICompanyService companyService, INotificationService notificationService)
            : base(service)
        {
            this._gatewayService = service;
            this._companyService = companyService;
            this._siteService = siteService;
            this._notificationService = notificationService;
        }

        #region Get

        public override ActionResult Index(JqGridRequest request)
        {
            var searchCriteria = request.ExtraParams["searchCriteria"];

            var page = _gatewayService.GetPage(request.PageIndex + 1, request.RecordsCount,
                                               request.SortingName, request.SortingOrder == JqGridSortingOrders.Asc,
                                               searchCriteria);


            var response = GetJqGridResponse(page, page.Items.Select(MapEntity));
            return new JqGridJsonResult { Data = response };
        }


        //
        //GET: /Gateway/Create

        public override ActionResult Create()
        {
            //Forces to refresh list of MACs.
            UnassignedMacs = null;

            var model = new GatewayViewModel
            {
                AvailableSitesList = new List<Site>(),
                AvailableCompanyList = this._companyService.GetAllEnabled(),
                AvailableProtocolsList = this._gatewayService.GetProtocols(),
                AvailableTimeZoneList = this._gatewayService.GetAllTimeZones()
            };

            return View(model);
        }


        //
        //GET: /Gateway/Edit

        public ActionResult EditWithFieldsDisabled(int id)
        {
            var gatewayItem = _gatewayService.Get(id);
            var model = MapEntityForEdit(gatewayItem);

            //Forces to refresh list of MACs.
            UnassignedMacs = null;

            model.AvailableSitesList = _siteService.GetSitesByCompany(gatewayItem.Company.Id, true);
            model.AvailableCompanyList = this._companyService.GetAllEnabled();
            model.AvailableTimeZoneList = this._gatewayService.GetAllTimeZones();
            model.AvailableProtocolsList = this._gatewayService.GetProtocols();

            var firstOrDefault = (Protocol)this._gatewayService.GetProtocols().FirstOrDefault(p => p.Id == gatewayItem.Protocol);
            if (firstOrDefault != null)
                model.ProtocolName = firstOrDefault.Name;

            return View("Edit", model);
        }

        #endregion

        #region Post

        [AcceptVerbs(HttpVerbs.Post)]
        public override ActionResult Create(GatewayViewModel newItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var item = newItem.MapFromViewModel();

                    try
                    {
                        _notificationService.ValidateEmcAccount(newItem.EMCId.ToString());
                        ViewBag.EMCAccountIsInvalid = false;
                    }
                    catch (Exception)
                    {
                        ViewBag.EMCAccountIsInvalid = true;
                    }

                    if (newItem.CreateIfEMCFail || !ViewBag.EMCAccountIsInvalid)
                    {
                        _service.Create(item);
                        return RedirectToAction("Index");
                    }
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException.Message.Equals("Mac Address Already Used"))
                            ModelState.AddModelError("ServiceError", string.Format("Mac address has already been used."));
                        else if (serviceException.InnerException is ValidationException)
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        else if (serviceException.InnerException is RepositoryException)
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                        else
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                    }
                }
                catch (ValidationException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unexpected Error: [{0}]", ex.Message));
                }
            }

            if (!ModelState.IsValid)
                ViewBag.EMCAccountIsInvalid = false;

            this.InitializeViewModel(newItem);

            return View(newItem);
        }

        //
        //POST: /Gateway/FillGrid
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FillGrid(JqGridRequest request)
        {
            if (UnassignedMacs == null)
            {
                UnassignedMacs = _gatewayService.GetEnabledMacAddress();
            }

            var unassignedMacs = UnassignedMacs.AsQueryable();
            unassignedMacs = request.SortingOrder == JqGridSortingOrders.Desc ? unassignedMacs.OrderByDescending(x => x) : unassignedMacs.OrderBy(x => x);
            var unassignedMacsPage = unassignedMacs.ToPage(request.PageIndex + 1, request.RecordsCount);

            var response = new JqGridResponse
            {
                TotalPagesCount = unassignedMacsPage.TotalPages,
                PageIndex = unassignedMacsPage.PageIndex - 1,
                TotalRecordsCount = unassignedMacsPage.TotalItems
            };

            response.Records.AddRange(MapEntityRecords(unassignedMacsPage));

            return new JqGridJsonResult { Data = response };
        }

        //
        //POST: /Gateway/EditWithFieldsDisabled
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditWithFieldsDisabled(int id, GatewayViewModelForEdit editedItem)
        {
            Gateway itemToEdit = null;

            if (ModelState.IsValid)
            {
                try
                {
                    itemToEdit = _gatewayService.Get(id);
                    var itemFromView = editedItem.MapFromViewModel();

                    MergeGatewayItemWithNewGatewayItem(itemToEdit, itemFromView);

                    editedItem.AvailableSitesList = _siteService.GetSitesByCompany(itemToEdit.Company.Id, true);
                    editedItem.AvailableCompanyList = this._companyService.GetAllEnabled();
                    editedItem.AvailableTimeZoneList = this._gatewayService.GetAllTimeZones();
                    editedItem.AvailableProtocolsList = this._gatewayService.GetProtocols();

                    try
                    {
                        _notificationService.ValidateEmcAccount(itemFromView.EMCId.ToString());
                        ViewBag.EMCAccountIsInvalid = false;
                    }
                    catch (Exception)
                    {
                        ViewBag.EMCAccountIsInvalid = true;
                    }

                    if (editedItem.CreateIfEMCFail || !ViewBag.EMCAccountIsInvalid)
                    {
                        _service.Update(itemToEdit);
                        return RedirectToAction("Index");
                    }
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        else if (serviceException.InnerException is RepositoryException)
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                        else
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                    }
                }
                catch (ValidationException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
                catch (Exception E)
                {
                    ModelState.AddModelError("", string.Format("Unexpected Error: [{0}]", E.Message));
                }
            }

            editedItem.MacAddress = editedItem.MacAddressName;

            if (itemToEdit != null)
            {
                editedItem.CompanyId = itemToEdit.Company.Id;
                editedItem.SiteId = itemToEdit.Site.Id;

                var firstOrDefault = (Protocol)this._gatewayService.GetProtocols().FirstOrDefault(p => p.Id == itemToEdit.Protocol);
                if (firstOrDefault != null)
                    editedItem.ProtocolName = firstOrDefault.Name;
            }

            if (!ModelState.IsValid)
                ViewBag.EMCAccountIsInvalid = false;

            return View("Edit", editedItem);
        }

        #endregion

        #region override

        protected override GatewayViewModel InitializeViewModel(GatewayViewModel item)
        {
            item.AvailableSitesList = this._siteService.GetAll();
            item.AvailableCompanyList = this._companyService.GetAll();
            item.AvailableTimeZoneList = this._gatewayService.GetAllTimeZones();
            item.AvailableProtocolsList = this._gatewayService.GetProtocols();

            return item;
        }

        protected override GatewayViewModel MapEntity(Gateway item)
        {
            return new GatewayViewModel(item);
        }

        #endregion

        private IEnumerable<JqGridRecord> MapEntityRecords(IEnumerable<string> retList)
        {
            return from item in retList
                   select new JqGridRecord<GatewayViewModel.MacAddressViewModel>(item,
                       this.MapEntity(item));
        }

        protected GatewayViewModel.MacAddressViewModel MapEntity(string item)
        {
            return new GatewayViewModel.MacAddressViewModel(item);
        }

        protected GatewayViewModelForEdit MapEntityForEdit(Gateway item)
        {
            return new GatewayViewModelForEdit(item);
        }

        private static void MergeGatewayItemWithNewGatewayItem(Gateway itemToEdit, Gateway itemFromView)
        {
            itemToEdit.EMCId = itemFromView.EMCId;
            itemToEdit.Name = itemFromView.Name;
            itemToEdit.TimeZone = itemFromView.TimeZone;
            itemToEdit.MacAddress = itemFromView.MacAddress;
        }


        #region Async

        public ActionResult AsyncGateways(int id)
        {
            var item = new GatewayViewModel
            {
                AvailableCompanyList = this._companyService.GetAll()
            };

            return Json(item.AvailableCompanies);
        }

        public ActionResult AsyncSites(int id)
        {
            var items = _siteService.GetSitesByCompany(id, false);
            var select = new SelectList(items, "Id", "Name");
            return Json(select);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Revoke(int id)
        {
            try
            {
                _gatewayService.RevokeCerificate(id);
                return JsonOK();
            }
            catch (ServiceException serviceException)
            {
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                return JsonError("An error occurred while revoke certificate item");
            }
        }

        #endregion

        #region Kendo UI Changes
        public ActionResult Gateway_Read([DataSourceRequest] DataSourceRequest request)
        {
            var gatewayResultSet = _gatewayService.GetAllActiveGateway();
            return Json(gatewayResultSet.Select(gw => new GatewayViewModel(gw)).ToList().ToDataSourceResult(request));
        }

        public ActionResult Getmac_Address([DataSourceRequest] DataSourceRequest request)
        {
            if (UnassignedMacs == null)
            {
                UnassignedMacs = _gatewayService.GetEnabledMacAddress();
            }

            var unassignedMacs = UnassignedMacs.AsQueryable();
            var response = MapEntityRecords(unassignedMacs);
            return Json(response.ToList().ToDataSourceResult(request));
        }

        public ActionResult GetGatewayDetails()
        {
            var gatewayResultSet = _gatewayService.GetAllActiveGateway();
            return Json(gatewayResultSet.Select(gw => new GatewayViewModel(gw)).ToList(), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public override ActionResult Delete(int id)
        {
            try
            {
                this._gatewayService.Delete(id);
                return JsonOK();
            }
            catch (ServiceException serviceException)
            {
                return JsonError(serviceException.Message);
            }
            catch (Exception e)
            {
                return JsonError("An error occurred while deleting item");
            }
        }
        #endregion

    }
}
