using System.Linq;
using System.Web.Mvc;
using Diebold.Services.Contracts;
using DieboldMobile.Models;
using Diebold.Domain.Entities;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Collections.Generic;
using Kendo.Mvc.UI;
using Diebold.Services.Exceptions;
using Diebold.Domain.Exceptions;
using Kendo.Mvc.Extensions;
using System;

namespace DieboldMobile.Controllers
{
    public class RoleController : BaseCRUDTrackeableController<Role, RoleViewModel>
    {
        private readonly IRoleService _roleService;
        private readonly IPortletService _portletService;
        private readonly IRolePortletService _rolePortletService;

        public RoleController(IRoleService roleService, IPortletService portletService, IRolePortletService rolePortletService)
            : base(roleService)
        {
            this._roleService = roleService;
            this._portletService = portletService;
            this._rolePortletService = rolePortletService;
        }

        protected override RoleViewModel InitializeViewModel(RoleViewModel item)
        {
            item.AvailableActionsList = this._roleService.GetAllActions();

            return item;
        }

        public ActionResult List()
        {
            var RoleViewModel = _roleService.GetAll().Select(role => new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                ActionColumn = "Role Management"
            }).OrderByDescending(x => x.Id).ToList();
            return View(RoleViewModel);
        }

        public ActionResult CreateRole()
        {
            //now this override can be avoided by using the InitializeViewModel 
            //call from the base class Create() method...

            Dictionary<int, string> AllActionList = new Dictionary<int, string>();
            List<string> objlstAvailableActionsList = new List<string>();
            objlstAvailableActionsList = _roleService.GetAllActions().ToList();

            int intIndex = 0;
            objlstAvailableActionsList.ToList().ForEach(x =>
            {
                intIndex += 1;
                AllActionList.Add(intIndex, x.ToString());
            });

            List<SelectListItem> SelectedItems = new List<SelectListItem>();
            List<SelectListItem> AllActionsItems = new List<SelectListItem>();

            objlstAvailableActionsList.ToList().ForEach(x =>
            {
                Dictionary<int, string> CurrentItem = new Dictionary<int, string>();
                string ItemVal = AllActionList.Where(y => y.Value.Equals(x.ToString())).FirstOrDefault().Value;
                string ItemId = AllActionList.Where(y => y.Value.Equals(x.ToString())).FirstOrDefault().Key.ToString();
                AllActionsItems.Add(new SelectListItem() { Text = ItemVal, Value = ItemId, Selected = false });
            });

            SelectList sl = new SelectList(SelectedItems, "Value", "Text");
            SelectList AllActions = new SelectList(AllActionsItems, "Value", "Text");

            var model = new RoleViewModel
            {
                AvailableActionsList = _roleService.GetAllActions(),
                OverallActionList = AllActions,
                SelectedActionsList = sl,
                AvailablePortlets = GetAvailablePortlets(),
                SelectedPortlets = new List<Portlets>(),
            };
            TempData["CreateRole_AvailableActionsList"] = AllActionList;

            ViewBag.CreateRoleInfo = model;
            return View(model);
        }

        private IList<Portlets> GetAvailablePortlets()
        {
            IList<Portlets> objLstPortlets = _portletService.GetAll().Where(x => x.InternalName != "LIVEVIEW_TWO").OrderBy(x => x.Name).ToList();
            TempData["CreateRole_Portlets"] = objLstPortlets;
            return objLstPortlets;
        }

        private Portlets getPortletByInternalName(string strInternalName)
        {
            return _portletService.GetByInternalName(strInternalName);
        }

        private List<RolePortlets> preparePortletObject(string strName, Role objRoleDetails)
        {
            string[] strPortlets = strName.Split(',');
            List<Portlets> objLsrPortlets = _portletService.GetAll().ToList();
            List<RolePortlets> objLstRolePortlet = new List<RolePortlets>();
            RolePortlets objRolePortlet = null;
            Portlets objPortlets = null;

            foreach (string strPortletName in strPortlets)
            {
                objRolePortlet = new RolePortlets();
                objPortlets = new Portlets();
                objPortlets = objLsrPortlets.Where(x => x.InternalName == strPortletName).FirstOrDefault();
                objRolePortlet.Portlets = objPortlets;
                objRolePortlet.Role = objRoleDetails;
                objLstRolePortlet.Add(objRolePortlet);
            }

            /*Populating default Live View2 portlet if user has LiveView Access*/
            if (strPortlets.Contains("LIVEVIEW"))
            {
                objRolePortlet = new RolePortlets();
                objRolePortlet.Portlets = _portletService.GetByInternalName("LIVEVIEW_TWO");
                objRolePortlet.Role = objRoleDetails;
                objLstRolePortlet.Add(objRolePortlet);
            }

            return objLstRolePortlet;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateRole(FormCollection formValue, List<string> SelectedActionsList)
        {
            RoleViewModel objRoleDetails = new RoleViewModel();
            IList<RolePortlets> SelectedPortlet = new List<RolePortlets>();
            try
            {
                if (formValue != null)
                {
                    objRoleDetails.Name = formValue.Get("Name").ToString();
                    List<string> objlstActions = new List<string>();
                    Dictionary<int, string> AllActionList = TempData["CreateRole_AvailableActionsList"] as Dictionary<int, string>;
                    SelectedActionsList.ForEach(x =>
                    {
                        string strRole = AllActionList.Where(y => y.Key.Equals(Convert.ToInt16(x))).FirstOrDefault().Value;
                        objlstActions.Add(strRole);
                    });
                    objRoleDetails.Actions = objlstActions;
                    var itemToCreate = objRoleDetails.MapFromViewModel();
                    itemToCreate.RolePortlets = preparePortletObject(formValue.Get("SelectedPortlets").ToString(), itemToCreate);
                    SelectedPortlet = itemToCreate.RolePortlets;
                    _roleService.Create(itemToCreate);
                    return RedirectToAction("List");
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
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, string.Format("Unexpected Error: [{0}]", e.Message));
            }
            objRoleDetails.SelectedPortlets = SelectedPortlet.Select(x => x.Portlets).ToList();
            objRoleDetails.AvailableActionsList = _roleService.GetAllActions();
            objRoleDetails.AvailablePortlets = GetAvailablePortlets();

            objRoleDetails = PrepareRoleViewModel(objRoleDetails);
            return View("CreateRole", objRoleDetails);
        }

        private RoleViewModel PrepareRoleViewModel(RoleViewModel itemToEdit)
        {
            Dictionary<int, string> AllActionList = new Dictionary<int, string>();
            List<string> objlstAvailableActionsList = new List<string>();
            SelectList objAllActions = new SelectList(_roleService.GetAllActions());
            objlstAvailableActionsList = _roleService.GetAllActions().ToList();

            int intIndex = 0;
            objlstAvailableActionsList.ToList().ForEach(x =>
            {
                intIndex += 1;
                AllActionList.Add(intIndex, x.ToString());
            });

            List<SelectListItem> SelectedItems = new List<SelectListItem>();
            List<SelectListItem> AllActionsItems = new List<SelectListItem>();

            AllActionList.ToList().ForEach(x =>
            {
                if (itemToEdit.Actions.ToList().Where(y => y.Equals(x.Value)).Count() > 0)
                {
                    Dictionary<int, string> CurrentItem = new Dictionary<int, string>();
                    string ItemVal = AllActionList.Where(y => y.Value.Equals(x.Value)).FirstOrDefault().Value;
                    string ItemId = AllActionList.Where(y => y.Value.Equals(x.Value)).FirstOrDefault().Key.ToString();
                    SelectedItems.Add(new SelectListItem() { Text = ItemVal, Value = ItemId, Selected = false });
                }
                else
                {
                    Dictionary<int, string> CurrentItem = new Dictionary<int, string>();
                    string ItemVal = AllActionList.Where(y => y.Value.Equals(x.Value)).FirstOrDefault().Value;
                    string ItemId = AllActionList.Where(y => y.Value.Equals(x.Value)).FirstOrDefault().Key.ToString();
                    AllActionsItems.Add(new SelectListItem() { Text = ItemVal, Value = ItemId, Selected = false });
                }

            });

            SelectList sl = new SelectList(SelectedItems, "Value", "Text");
            SelectList AllActions = new SelectList(AllActionsItems, "Value", "Text");
            itemToEdit.SelectedActionsList = sl;
            itemToEdit.OverallActionList = AllActions;
            itemToEdit.AvailableActionsList = objlstAvailableActionsList;
            IList<Portlets> objLsrPortlets = itemToEdit.AvailablePortlets;



            if (itemToEdit.SelectedPortlets != null)
            {
                foreach (Portlets objPortlet in itemToEdit.SelectedPortlets)
                {
                    objLsrPortlets.Remove(objLsrPortlets.Where(x => x.Id == objPortlet.Id).FirstOrDefault());
                }
            }
            itemToEdit.AvailablePortlets = objLsrPortlets;
            return itemToEdit;
        }

        public ActionResult RoleEdit(int id)
        {
            Role objrole = _roleService.Get(id);
            var SelectedPortlet = objrole.RolePortlets.Select(x => x.Portlets).Where(x => x.InternalName != "LIVEVIEW_TWO").OrderBy(x => x.Name).ToList();
            RoleViewModel itemToEdit = MapEntity(_roleService.Get(id));
            itemToEdit.AvailableActionsList = _roleService.GetAllActions();
            itemToEdit.AvailablePortlets = GetAvailablePortlets();
            itemToEdit.SelectedPortlets = SelectedPortlet;
            itemToEdit = PrepareRoleViewModel(itemToEdit);

            Dictionary<int, string> AllActionList = new Dictionary<int, string>();
            List<string> objlstAvailableActionsList = new List<string>();
            objlstAvailableActionsList = _roleService.GetAllActions().ToList();

            int intIndex = 0;
            objlstAvailableActionsList.ToList().ForEach(x =>
            {
                intIndex += 1;
                AllActionList.Add(intIndex, x.ToString());
            });
            TempData["EditRole_AvailableActionsList"] = AllActionList;

            return View(itemToEdit);
        }

        public ActionResult RoleEditItem(FormCollection formValue, List<string> SelectedActionsList)
        {
            RoleViewModel objRoleDetails = new RoleViewModel();
            List<Portlets> objlstSelectedPortlets = new List<Portlets>();
            Role objRole = null;
            try
            {
                IList<Diebold.Domain.Entities.Action> objlstEnumActions = new List<Diebold.Domain.Entities.Action>();
                List<string> objlstActions = new List<string>();
                Dictionary<int, string> AllActionList = TempData["EditRole_AvailableActionsList"] as Dictionary<int, string>;
                SelectedActionsList.ForEach(x =>
                {
                    string strRole = AllActionList.Where(y => y.Key.Equals(Convert.ToInt16(x))).FirstOrDefault().Value;
                    objlstEnumActions.Add((Diebold.Domain.Entities.Action)Enum.Parse(typeof(Diebold.Domain.Entities.Action), strRole, true));
                });

                var SelectedRoleId = Convert.ToInt32(formValue.Get("Id").ToString());
                objRoleDetails.Id = SelectedRoleId;
                objRole = _roleService.Get(SelectedRoleId);
                objRole.Name = formValue.Get("Name").ToString();
                objRole.Actions = objlstEnumActions;
                objRole.RolePortlets = GetUpdatedRolePortlets(formValue.Get("SelectedPortlets").ToString(), objRole).ToList();
                objlstSelectedPortlets = objRole.RolePortlets.Select(x => x.Portlets).ToList();
                _roleService.Update(objRole);

                return RedirectToAction("List");
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
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, string.Format("Unexpected Error: [{0}]", e.Message));
            }
            objRoleDetails = MapEntity(objRole);
            objRoleDetails.SelectedPortlets = objlstSelectedPortlets;
            objRoleDetails = PrepareRoleViewModel(objRoleDetails);
            return View("RoleEdit", objRoleDetails);
        }

        public IList<RolePortlets> GetUpdatedRolePortlets(string strName, Role role)
        {
            string[] strPortlets = strName.Split(',');
            List<Portlets> objLsrPortlets = _portletService.GetAll().ToList();
            List<RolePortlets> objLstRolePortlet = new List<RolePortlets>();
            RolePortlets objRolePortlet = null;
            Portlets objPortlets = null;
            foreach (string strPortletName in strPortlets)
            {
                objRolePortlet = new RolePortlets();
                objPortlets = new Portlets();
                if (role.RolePortlets.Where(x => x.Portlets.InternalName.Equals(strPortletName)).ToList().Count > 0)
                {
                    objRolePortlet = role.RolePortlets.Where(x => x.Portlets.InternalName.Equals(strPortletName)).FirstOrDefault();
                }
                else
                {
                    objPortlets = objLsrPortlets.Where(x => x.InternalName == strPortletName).FirstOrDefault();
                    objRolePortlet.Portlets = objPortlets;
                    objRolePortlet.Role = role;
                }
                objLstRolePortlet.Add(objRolePortlet);
            }

            /*Populating default Live View2 portlet if user has LiveView Access*/
            if (strPortlets.Contains("LIVEVIEW"))
            {
                objRolePortlet = new RolePortlets();
                objRolePortlet.Portlets = _portletService.GetByInternalName("LIVEVIEW_TWO");
                objRolePortlet.Role = role;
                objLstRolePortlet.Add(objRolePortlet);
            }

            return objLstRolePortlet;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Destroy(int RoleId)
        {
            try
            {
                _roleService.Delete(RoleId);
                return JsonOK();
            }
            catch (Exception ex)
            {
                string ErrorDetails = string.Empty;
                if (ex.InnerException != null)
                {
                    ErrorDetails = ex.Message;
                    return Json(ErrorDetails);
                }
                else
                {
                    ErrorDetails = string.Format("Unexpected Error: [{0}]", ex.Message);
                    return Json(ErrorDetails);
                }
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public override ActionResult Index(JqGridRequest request)
        {
            var page = _roleService.GetPage(request.PageIndex + 1, request.RecordsCount,
                                               request.SortingName, request.SortingOrder == JqGridSortingOrders.Asc, null);

            var response = GetJqGridResponse(page, page.Items.Select(MapEntity));
            return new JqGridJsonResult { Data = response };
        }

        public override ActionResult Create()
        {
            //now this override can be avoided by using the InitializeViewModel 
            //call from the base class Create() method...

            var model = new RoleViewModel
            {
                AvailableActionsList = _roleService.GetAllActions()
            };

            return View(model);
        }

        public override ActionResult Edit(int id)
        {
            //now this override can be avoided by using the InitializeViewModel 
            //call from the base class Create() method...

            RoleViewModel itemToEdit = MapEntity(_roleService.Get(id));
            itemToEdit.AvailableActionsList = _roleService.GetAllActions();

            return View(itemToEdit);
        }

        protected override RoleViewModel MapEntity(Role item)
        {
            return new RoleViewModel(item);
        }
    }
}
