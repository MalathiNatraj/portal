using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using Diebold.Services.Contracts;
using Diebold.Services.Infrastructure;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.WebApp.Models;
using Diebold.Domain.Entities;
using Diebold.Services.Exceptions;
using Diebold.Domain.Exceptions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Web;
using System.IO;

namespace Diebold.WebApp.Controllers
{
    public class CompanyController : BaseCRUDTrackeableController<Company, CompanyViewModel>
    {
        #region Properties

        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;
        private readonly IUserService _userService;
        private readonly ICompanyInventoryService _companyInventoryService;
        List<CompanyInventory> lstCompanyInventory = new List<CompanyInventory>();
        #endregion

        #region Constructor

        public CompanyController(ICompanyService companyService, ISiteService siteService, IUserService userService, ICompanyInventoryService companyInventoryService)
            : base(companyService)
        {
            this._companyService = companyService;
            this._siteService = siteService;
            this._userService = userService;
            this._companyInventoryService = companyInventoryService;
        }

        #endregion

        #region Public Methods

        protected override CompanyViewModel InitializeViewModel(CompanyViewModel item)
        {
            item.AvailableReportingFrequencyList = _companyService.GetAllReportingFrequencies();
            item.AvailableSubscriptionList = _companyService.GetAllSubscriptions();
            item.ThirdLevelGrouping = CompanyLevelGrouping.Site.ToString();
            item.FourthLevelGrouping = CompanyLevelGrouping.Device.ToString();

            if (item.GroupingRelations != null)
            {
                var groupingRelations = item.GroupingRelations.Select(
                    cg => new CompanyGrouping() { Grouping1Id = cg.Grouping1Id, Grouping1Name = cg.Grouping1Name }).ToList();

                item.AvailableGrouping1LevelList = groupingRelations;
            }
            else
            {
                item.AvailableGrouping1LevelList = new List<CompanyGrouping>();
            }

            return item;
        }

        protected override CompanyViewModel MapEntity(Company item)
        {
            return new CompanyViewModel(item);
        }

        public CompanyViewModel MapGroupEntity(Company item)
        {
            var model = new CompanyViewModel(item)
            {
                Grouping1SelectedItems = new List<string>(),
                Grouping2SelectedItems = new List<string>(),
                Sites = new List<string>()
            };

            foreach (var level in item.CompanyGrouping1Levels)
                model.Grouping1SelectedItems.Add(level.Id.ToString());

            foreach (var level in item.CompanyGrouping1Levels.First().CompanyGrouping2Levels)
                model.Grouping2SelectedItems.Add(level.Id.ToString());

            if (item.CompanyGrouping1Levels.First().CompanyGrouping2Levels.Count > 0)
            {
                foreach (var site in item.CompanyGrouping1Levels.First().CompanyGrouping2Levels.First().Sites)
                    model.Sites.Add(site.Id.ToString());
            }

            return model;
        }

        #endregion

        #region Private Methods

        private void InitializeCollections(CompanyViewModel item, Company company)
        {
            item.AvailableReportingFrequencyList = _companyService.GetAllReportingFrequencies();
            item.AvailableSubscriptionList = _companyService.GetAllSubscriptions();
            item.AvailableGrouping2AllItemList = _companyService.GetGrouping2LevelsByCompanyId(company.Id);
            item.AvailableGrouping1LevelItemList = company.CompanyGrouping1Levels;
            item.AvailableGrouping2LevelItemList = company.CompanyGrouping1Levels.First().CompanyGrouping2Levels;

            var GroupLevel2 = company.CompanyGrouping1Levels.First().CompanyGrouping2Levels.FirstOrDefault();
            if (GroupLevel2 != null)
            {
                item.SelectedSiteList = _siteService.GetSitesByCompanyGrouping2Level(GroupLevel2.Id);
                var AvailableSites = _companyService.GetSitesByCompanyId(company.Id);
                item.AvailableSiteList = AvailableSites.Where(x => !x.CompanyGrouping2Level.Id.Equals(Convert.ToInt32(GroupLevel2.Id))).ToList();
            }
            else
            {
                item.SelectedSiteList = new List<Site>();
                item.AvailableSiteList = new List<Site>();
            }
            //TODO
            item.AvailableGrouping1LevelList = item.GroupingRelations ?? new List<CompanyGrouping>();


            var Grouplevel2Items = company.CompanyGrouping1Levels.Where(x => x.Id == Convert.ToInt32(company.CompanyGrouping1Levels.First().Id)).FirstOrDefault();
            IList<int> SelectedGrouplevel2Items = new List<int>();

            if (Grouplevel2Items.CompanyGrouping2Levels != null && Grouplevel2Items.CompanyGrouping2Levels.Any())
            {
                SelectedGrouplevel2Items = Grouplevel2Items.CompanyGrouping2Levels.Select(x => x.Id).ToList();
            }

            var Grouping2Levels = _companyService.GetGrouping2LevelsByCompanyId(company.Id).Where(x => !SelectedGrouplevel2Items.Contains(x.Id)).ToList();
            var Select = Grouping2Levels.Select(ListItem => new SelectListItem
            {
                Text = ListItem.CompanyGrouping1Level.Name + " / " + ListItem.Name,
                Value = ListItem.Id.ToString()
            }).ToList();

            item.AvailableGrouping2AllItemList = Grouping2Levels;


            @ViewBag.CompanyGroupingModel = new List<CompanyGroupingViewModel>();
        }

        private static void SetDefault(Company item)
        {
            item.ThirdLevelGrouping = CompanyLevelGrouping.Site.ToString();
            item.FourthLevelGrouping = CompanyLevelGrouping.Device.ToString();
        }

        private static void MapGroupingLevels(Company item, CompanyViewModel model)
        {
            foreach (var companyGrouping in model.GroupingRelations)
            {
                var companyG1 = new CompanyGrouping1Level { Company = item, Name = companyGrouping.Grouping1Name };

                foreach (var companyG2 in companyGrouping.Grouping2List)
                {
                    companyG1.CompanyGrouping2Levels.Add(new CompanyGrouping2Level() { CompanyGrouping1Level = companyG1, Name = companyG2.Grouping2Name });
                }

                item.CompanyGrouping1Levels.Add(companyG1);
            }
        }

        private static bool CheckIfLevel1NameExists(string name, int level1Id, IEnumerable<CompanyGrouping1Level> level1List)
        {
            return level1List.Where(x => x.Name.ToLower() == name.ToLower() && x.Id != level1Id).ToList().Count() > 0;
        }

        private static bool CheckIfLevel2NameExists(string name, int level2Id, IEnumerable<CompanyGrouping2Level> level2List)
        {
            return level2List.Where(x => x.Name.ToLower() == name.ToLower() && x.Id != level2Id).ToList().Count() > 0;
        }

        #endregion

        #region Get Methods

        public override ActionResult Create()
        {
            IList<CompanyDefaultSubscription> lstCompanySubscriptions = GetCompanyDefaultSubscription(null);
            lstCompanySubscriptions.ToList().ForEach(x =>
            {
                x.isSelected = true;
            });
            var model = new CompanyViewModel
            {
                AvailableReportingFrequencyList = this._companyService.GetAllReportingFrequencies(),
                AvailableSubscriptionList = this._companyService.GetAllSubscriptions(),
                ThirdLevelGrouping = CompanyLevelGrouping.Site.ToString(),
                FourthLevelGrouping = CompanyLevelGrouping.Device.ToString(),
                FirstLevelGrouping = string.Empty,
                SecondLevelGrouping = string.Empty,
                Grouping1LevelName = string.Empty,
                Grouping1Selected = string.Empty,
                AvailableGrouping1LevelItemList = new List<CompanyGrouping1Level>(),
                AvailableGrouping1LevelList = new List<CompanyGrouping>(),
                Subscriptions = _companyService.GetAllSubscriptions().ToList(),
                CompanyDefaultSubscription = lstCompanySubscriptions
            };
            Session["CreateCompany_DefaultSubscription"] = null;
            Session["CreateCompany_DefaultSubscription"] = lstCompanySubscriptions;
            return View(model);
        }

        private IList<CompanyDefaultSubscription> GetCompanyDefaultSubscription(IList<String> Subscriptions)
        {
            IList<CompanyDefaultSubscription> lstCompanySubscriptions = new List<CompanyDefaultSubscription>();
            var AvailableSubscriptionList = this._companyService.GetAllSubscriptions();
            int i = 0;
            AvailableSubscriptionList.ToList().ForEach(x =>
            {
                i++;
                string strTypeName = x.ToString();
                CompanyDefaultSubscription Alarm = new CompanyDefaultSubscription();
                Alarm.Name = x;
                Alarm.Id = i;
                if (Subscriptions != null && Subscriptions.Where(y => y.Equals(strTypeName)).ToList().Count > 0)
                    Alarm.isSelected = true;
                else
                    Alarm.isSelected = false;

                lstCompanySubscriptions.Add(Alarm);
            });

            return lstCompanySubscriptions;
        }

        public override ActionResult Edit(int id)
        {
            var itemToEdit = MapEntity(_companyService.Get(id));
            itemToEdit.AvailableReportingFrequencyList = this._companyService.GetAllReportingFrequencies();
            itemToEdit.AvailableSubscriptionList = this._companyService.GetAllSubscriptions();
            itemToEdit.ThirdLevelGrouping = CompanyLevelGrouping.Site.ToString();
            itemToEdit.FourthLevelGrouping = CompanyLevelGrouping.Device.ToString();
            itemToEdit.CompanyDefaultSubscription = GetCompanyDefaultSubscription(itemToEdit.Subscriptions);
            Session["EditCompany_DefaultSubscription"] = null;
            Session["EditCompany_DefaultSubscription"] = GetCompanyDefaultSubscription(itemToEdit.Subscriptions);
            return View(itemToEdit);
        }

        public ActionResult EditCompanyGrouping(int id)
        {
            ViewBag.IsEditGrouping = true;

            var company = _companyService.Get(id);
            var itemToEdit = MapGroupEntity(company);

            ViewBag.SecondLevelGrouping = company.SecondLevelGrouping;

            InitializeCollections(itemToEdit, company);

            return View(itemToEdit);
        }

        #endregion

        #region Post Methods

        public override ActionResult Index()
        {
            var objlstCompany = _companyService.GetAll();
            var CompanyViewModel = from objCompany in objlstCompany
                                   select new CompanyViewModel
                                   {
                                       Id = objCompany.Id,
                                       Name = objCompany.Name,
                                       PrimaryContactEmail = objCompany.PrimaryContactEmail,
                                       PrimaryContactMobile = objCompany.PrimaryContactMobile,
                                       PrimaryContactMobilePreferred = objCompany.PrimaryContactMobilePreferred,
                                       PrimaryContactName = objCompany.PrimaryContactName,
                                       PrimaryContactOffice = objCompany.PrimaryContactOffice,
                                       PrimaryContactOfficePreferred = objCompany.PrimaryContactOfficePreferred,
                                       IsDisabled = objCompany.IsDisabled
                                   };
            TempData["Companies"] = objlstCompany;
            return View(CompanyViewModel);
        }

        public ActionResult Company_Read([DataSourceRequest] DataSourceRequest request)
        {
            var objlstCompany = _companyService.GetAll();
            var CompanyViewModel = from objCompany in objlstCompany
                                   select new CompanyViewModel
                                   {
                                       Id = objCompany.Id,
                                       Name = objCompany.Name,
                                       PrimaryContactEmail = objCompany.PrimaryContactEmail,
                                       PrimaryContactMobile = objCompany.PrimaryContactMobile,
                                       PrimaryContactMobilePreferred = objCompany.PrimaryContactMobilePreferred,
                                       PrimaryContactName = objCompany.PrimaryContactName,
                                       PrimaryContactOffice = objCompany.PrimaryContactOffice,
                                       PrimaryContactOfficePreferred = objCompany.PrimaryContactOfficePreferred
                                   };
            return Json(CompanyViewModel.ToList().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateCompany(CompanyViewModel newItem, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["CreateCompany_DefaultSubscription"] != null)
                    {
                        IList<CompanyDefaultSubscription> lstCompanyDefaultSubscription = Session["CreateCompany_DefaultSubscription"] as IList<CompanyDefaultSubscription>;
                        IList<string> objlstSubscription = new List<string>();
                        lstCompanyDefaultSubscription.ToList().ForEach(x =>
                        {
                            if (x.isSelected == true)
                                objlstSubscription.Add(x.Name);
                        });

                        newItem.Subscriptions = objlstSubscription.ToList();
                    }

                    Company item = newItem.MapFromViewModel();
                    if (files != null && files.Count() > 0)
                    {
                        string strFileName = files.ToList()[0].FileName;
                        byte[] content;//= new byte[str.Length];
                        using (Stream str = files.ToList()[0].InputStream)
                        {
                            content = new byte[str.Length];
                            str.Read(content, 0, content.Length);
                        }

                        item.FileName = strFileName;
                        item.CompanyLogo = content;
                    }

                    SetDefault(item);
                    MapGroupingLevels(item, newItem);
                    this._service.Create(item);
                    Session["EditCompany_DefaultSubscription"] = null;
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
                            LogError("Repository Exception occured while creating companay", serviceException);
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                        }
                        else
                        {
                            LogError("Service Exception occured while creating companay", serviceException);
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                        }
                    }
                }
                catch (Exception e)
                {
                    LogError("Exception occured while creating companay", e);
                    ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));
                }
            }

            this.InitializeViewModel(newItem);
            return View("Create", newItem);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditCompanyDetails(int id, CompanyViewModel editedItem, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["EditCompany_DefaultSubscription"] != null)
                    {
                        IList<CompanyDefaultSubscription> lstCompanyDefaultSubscription = Session["EditCompany_DefaultSubscription"] as IList<CompanyDefaultSubscription>;
                        IList<string> objlstSubscription = new List<string>();
                        lstCompanyDefaultSubscription.ToList().ForEach(x =>
                        {
                            if (x.isSelected == true)
                                objlstSubscription.Add(x.Name);
                        });

                        editedItem.Subscriptions = objlstSubscription.ToList();
                    }

                    var itemFromView = editedItem.MapFromViewModel();
                    if (files != null && files.Count() > 0)
                    {
                        string strFileName = files.ToList()[0].FileName;
                        byte[] content;
                        using (Stream str = files.ToList()[0].InputStream)
                        {
                            content = new byte[str.Length];
                            str.Read(content, 0, content.Length);
                        }

                        itemFromView.FileName = strFileName;
                        itemFromView.CompanyLogo = content;
                    }
                    else
                    {
                        var EditcompanyDetails = _service.Get(editedItem.Id);
                        itemFromView.FileName = EditcompanyDetails.FileName;
                        itemFromView.CompanyLogo = EditcompanyDetails.CompanyLogo;
                    }
                    SetDefault(itemFromView);
                    _service.Update(itemFromView);
                    Session["EditCompany_DefaultSubscription"] = null;
                    return RedirectToAction("Index");
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                        {
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        }
                        else
                        {
                            LogError("Service Exception occured while editing companay", serviceException);
                            ModelState.AddModelError("ServiceError", serviceException.InnerException.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogError("Exception occured while editing companay", e);
                    ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));
                }
            }

            editedItem.AvailableReportingFrequencyList = this._companyService.GetAllReportingFrequencies();
            editedItem.AvailableSubscriptionList = this._companyService.GetAllSubscriptions();
            editedItem.ThirdLevelGrouping = CompanyLevelGrouping.Site.ToString();
            editedItem.FourthLevelGrouping = CompanyLevelGrouping.Device.ToString();

            return View("Edit", editedItem);

        }

        public ActionResult Disable(int id)
        {
            try
            {
                _service.Disable(id);
            }
            catch (ServiceException serviceException)
            {
                if (serviceException.InnerException != null)
                {
                    if (serviceException.InnerException is ValidationException)
                        AddModelErrors((ValidationException)serviceException.InnerException);
                    else if (serviceException.InnerException is RepositoryException)
                    {
                        LogError("Repository Exception occured while disabling companay", serviceException);
                        ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                    }
                    else
                    {
                        LogError("Service Exception occured while disabling companay", serviceException);
                        ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                    }
                }
            }
            catch (Exception e)
            {
                LogError("Exception occured while disabling companay", e);
                ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));
            }

            return RedirectToAction("Index");
        }

        public ActionResult Enable(int id)
        {
            try
            {
                _service.Enable(id);
            }
            catch (ServiceException serviceException)
            {
                if (serviceException.InnerException != null)
                {
                    if (serviceException.InnerException is ValidationException)
                        AddModelErrors((ValidationException)serviceException.InnerException);
                    else if (serviceException.InnerException is RepositoryException)
                    {
                        LogError("Repository Exception occured while enabling companay", serviceException);
                        ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                    }
                    else
                    {
                        LogError("Service Exception occured while enabling companay", serviceException);
                        ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                    }
                }
            }
            catch (Exception e)
            {
                LogError("Exception occured while enabling companay", e);
                ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                _service.Delete(id);
            }
            catch (ServiceException serviceException)
            {
                if (serviceException.InnerException != null)
                {
                    if (serviceException.InnerException is ValidationException)
                        AddModelErrors((ValidationException)serviceException.InnerException);
                    else if (serviceException.InnerException is RepositoryException)
                    {
                        LogError("Repository Exception occured while deleting companay", serviceException);
                        ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                    }
                    else
                    {
                        LogError("Service Exception occured while deleting companay", serviceException);
                        ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                    }
                }
            }
            catch (Exception e)
            {
                LogError("Exception occured while deleting companay", e);
                ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));
            }

            return RedirectToAction("Index");
        }

        public static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateCompanyGrouping(CompanyViewModel editedItem)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CreateAlarmConfiguration(string Name, bool isSelected)
        {
            if (Session["CreateCompany_DefaultSubscription"] != null)
            {
                IList<CompanyDefaultSubscription> lstCompanyDefaultSubscription = Session["CreateCompany_DefaultSubscription"] as IList<CompanyDefaultSubscription>;
                lstCompanyDefaultSubscription.ToList().ForEach(x =>
                {
                    if (x.Name.Equals(Name))
                    {
                        x.isSelected = isSelected;
                    }
                });

                Session["CreateCompany_DefaultSubscription"] = null;
                Session["CreateCompany_DefaultSubscription"] = lstCompanyDefaultSubscription;
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult UpdateAlarmSelection(string Name, bool isSelected)
        {
            if (Session["EditCompany_DefaultSubscription"] != null)
            {
                IList<CompanyDefaultSubscription> lstCompanyDefaultSubscription = Session["EditCompany_DefaultSubscription"] as IList<CompanyDefaultSubscription>;
                lstCompanyDefaultSubscription.ToList().ForEach(x =>
                {
                    if (x.Name.Equals(Name))
                    {
                        x.isSelected = isSelected;
                    }
                });

                Session["EditCompany_DefaultSubscription"] = null;
                Session["EditCompany_DefaultSubscription"] = lstCompanyDefaultSubscription;
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult UpdateAlarmDetails(string Alarmconfigurations)
        {
            IList<CompanyDefaultSubscription> objlstCompanyDefaultSubscription = new List<CompanyDefaultSubscription>();
            CompanyDefaultSubscription objCompanyDefaultSubscription = new CompanyDefaultSubscription();
            if (Alarmconfigurations != null)
            {
                string[] DefaultSubcription = Alarmconfigurations.Split(':');
                if (DefaultSubcription != null && DefaultSubcription.Count() > 0)
                {
                    DefaultSubcription.ToList().ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x))
                        {
                            string[] strSubscription = x.Split(',');
                            objCompanyDefaultSubscription.Id = Convert.ToInt32(strSubscription[0]);
                            objCompanyDefaultSubscription.Name = strSubscription[1];
                            objCompanyDefaultSubscription.isSelected = Convert.ToBoolean(strSubscription[2]);
                            objlstCompanyDefaultSubscription.Add(objCompanyDefaultSubscription);
                        }
                    });
                }
            }
            Session["EditCompany_DefaultSubscription"] = objlstCompanyDefaultSubscription;
            return Json(new { success = true });
        }

        #endregion

        public ActionResult ComapnySelectedGroupLevel2Items_Read([DataSourceRequest] DataSourceRequest request)
        {
            return new EmptyResult();
            //var Grouplevel2Items = _companyService.GetGrouping2LevelsByCompanyId(2);
            //return Json(Grouplevel2Items.ToDataSourceResult(request));
        }

        public ActionResult GetSelectedGroupLevel2Items(string CompanyId, string FirstLevelGroupId)
        {
            var Comapny = _companyService.Get(Convert.ToInt32(CompanyId));
            var Grouplevel2Items = Comapny.CompanyGrouping1Levels.Where(x => x.Id == Convert.ToInt32(FirstLevelGroupId)).FirstOrDefault();
            //item.AvailableGrouping2LevelItemList = Grouplevel2Items.CompanyGrouping1Levels.First().CompanyGrouping2Levels;
            //return Json(Grouplevel2Items.CompanyGrouping2Levels.ToList());
            SelectList select = null;
            if (Grouplevel2Items.CompanyGrouping2Levels != null && Grouplevel2Items.CompanyGrouping2Levels.Any())
            {
                select = new SelectList(Grouplevel2Items.CompanyGrouping2Levels, "Id", "Name");
            }
            //if (select == null)
            //select = new SelectList(new List<CompanyGrouping2Level>(), "Id", "Name");

            return Json(select);
        }

        public ActionResult GetAreaFromAssignedGroup(string CompanyId, string FirstLevelGroupId)
        {
            var Comapny = _companyService.Get(Convert.ToInt32(CompanyId));
            var Grouplevel2Items = Comapny.CompanyGrouping1Levels.Where(x => x.Id == Convert.ToInt32(FirstLevelGroupId)).FirstOrDefault();
            IList<int> SelectedGrouplevel2Items = new List<int>();

            if (Grouplevel2Items.CompanyGrouping2Levels != null && Grouplevel2Items.CompanyGrouping2Levels.Any())
            {
                SelectedGrouplevel2Items = Grouplevel2Items.CompanyGrouping2Levels.Select(x => x.Id).ToList();
            }

            var Grouping2Levels = _companyService.GetGrouping2LevelsByCompanyId(Convert.ToInt32(CompanyId)).Where(x => !SelectedGrouplevel2Items.Contains(x.Id));
            var items = Grouping2Levels.Select(item => new SelectListItem
            {
                Text = item.CompanyGrouping1Level.Name + " / " + item.Name,
                Value = item.Id.ToString()
            }).ToList();

            //AvailableGrouping2AllItems = new MultiSelectList(items, "Value", "Text");

            if (items == null)
                return new EmptyResult();

            return Json(items);
        }

        public ActionResult GetUnAssignedSites(string CompanyId, string GroupingLevel2Id)
        {
            var Comapny = _companyService.Get(Convert.ToInt32(CompanyId));
            var SelectedCompanySites = _companyService.GetSitesByCompanyId(Convert.ToInt32(CompanyId));
            if (!string.IsNullOrEmpty(GroupingLevel2Id))
            {
                var objAvailableSites = SelectedCompanySites.Where(x => !x.CompanyGrouping2Level.Id.Equals(Convert.ToInt32(GroupingLevel2Id))).ToList();
                var items = objAvailableSites.Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                }).ToList();

                if (items == null)
                    return new EmptyResult();
                else
                    return Json(items);
            }
            else
            {
                return new EmptyResult();
            }
        }


        //[HttpPost]
        //public ActionResult UDFMangementSave(List<CompanyInventoryViewModel> UDFData)
        //{
        //   // objlstCompanyInventoryViewModel = UDFData;
            
        //    foreach (var item in UDFData)
        //    {
        //        CompanyInventory objCompanyInventory = new CompanyInventory();
        //        objCompanyInventory.InventoryKey = item.InventoryKey;
        //        objCompanyInventory.InventoryValue = item.InventoryValue;
        //        objCompanyInventory.Id = item.Id;
        //        lstCompanyInventory.Add(objCompanyInventory);
        //    }
        //    TempData["CompanyInventory"] = lstCompanyInventory;
        //    return null;
        //}

        public ActionResult UDFMangementSave(string ExternalCompanyId, string Key)
        {
            CompanyInventory objCompanyInventory = new CompanyInventory();
            objCompanyInventory.ExternalCompanyId = Convert.ToInt32(ExternalCompanyId);
            objCompanyInventory.InventoryKey = Key;
            _companyInventoryService.Create(objCompanyInventory);
            List<CompanyInventoryViewModel> lstCmpnyInvntryViewModel = GetCompanyInventoryDetailsByExternalComany(Convert.ToInt32(ExternalCompanyId));
            return Json(lstCmpnyInvntryViewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UDFManagementEdit(string ExternalCompanyId, string Key, string InventoryId)
        {
            int intCompanyInventoryId = Convert.ToInt32(ExternalCompanyId);
            if (intCompanyInventoryId > 0)
            {
                CompanyInventory objCompanyInventory = _companyInventoryService.Get(Convert.ToInt32(InventoryId));
                objCompanyInventory.InventoryKey = Key;
                _companyInventoryService.Update(objCompanyInventory);
            }
            List<CompanyInventoryViewModel> lstCmpnyInvntryViewModel = GetCompanyInventoryDetailsByExternalComany(intCompanyInventoryId);
            return Json(lstCmpnyInvntryViewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UDFManagementDelete(string CompanyInventoryId, string CompanyExternalId)
        {
            int intCompanyInventoryId = Convert.ToInt32(CompanyInventoryId);
            if (intCompanyInventoryId > 0)
            {
               CompanyInventory objCompanyInventory = _companyInventoryService.Get(intCompanyInventoryId);
               objCompanyInventory.DeletedKey = objCompanyInventory.Id;
               _companyInventoryService.Update(objCompanyInventory);
            }
            List<CompanyInventoryViewModel> lstCmpnyInvntryViewModel = GetCompanyInventoryDetailsByExternalComany(Convert.ToInt32(CompanyExternalId));
            return Json(lstCmpnyInvntryViewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUDFManagementDetails(int CompanyId)
        {
            List<CompanyInventoryViewModel> lstCmpnyInvntryViewModel = GetCompanyInventoryDetailsByExternalComany(CompanyId);
            return Json(lstCmpnyInvntryViewModel, JsonRequestBehavior.AllowGet);
        }

        private List<CompanyInventoryViewModel> GetCompanyInventoryDetailsByExternalComany(int ExternalCompanyId)
        {
            var ResultSet = _companyInventoryService.GetInventoryByExternalComanyd(ExternalCompanyId);
            List<CompanyInventoryViewModel> lstCmpnyInvntryViewModel = new List<CompanyInventoryViewModel>();
            foreach (var item in ResultSet)
            {
                CompanyInventoryViewModel objCompanyInventoryViewModel = new CompanyInventoryViewModel();
                objCompanyInventoryViewModel.Id = item.Id;
                objCompanyInventoryViewModel.InventoryKey = item.InventoryKey;
                lstCmpnyInvntryViewModel.Add(objCompanyInventoryViewModel);
            }
            return lstCmpnyInvntryViewModel;
        }

        #region Ajax Call

        public ActionResult AsyncGrouping2AllLevels(int companyId)
        {
            var cvm = new CompanyViewModel
            {
                AvailableGrouping2AllItemList = _companyService.GetGrouping2LevelsByCompanyId(companyId).OrderBy(x => x.Name).ToList()
            };

            return Json(cvm.AvailableGrouping2AllItems);
        }

        public ActionResult AsyncGrouping1Levels(int companyId)
        {
            SelectList select = null;

            var items = _companyService.GetGrouping1LevelsByCompanyId(companyId);

            if (items != null && items.Any())
            {
                select = new SelectList(items, "Id", "Name");
            }

            if (select == null)
                select = new SelectList(new List<Site>(), "Id", "Name");

            return Json(select);
        }

        public ActionResult AsyncGrouping2Levels(string group1LevelId, int companyId)
        {
            SelectList select = null;

            if (group1LevelId.IsInteger())
            {
                var items = _companyService.GetGrouping2LevelsByGrouping1LevelId(int.Parse(group1LevelId), companyId);

                if (items != null && items.Any())
                {
                    select = new SelectList(items, "Id", "Name");
                }
            }

            if (select == null)
                select = new SelectList(new List<Site>(), "Id", "Name");

            return Json(select);
        }

        public ActionResult AsyncSites(CompanyGroupingLevelViewModel levelModel)
        {
            SelectList select = null;

            var sites = _siteService.GetSitesByCompanyGrouping2Level(levelModel.GroupingLevel2Id);

            if (sites != null && sites.Any())
            {
                select = new SelectList(sites, "Id", "Name");
            }

            if (select == null)
                select = new SelectList(new List<Site>(), "Id", "Name");

            return Json(select);
        }

        public ActionResult ValidateCompanyId(string id, string externalCompanyId)
        {
            try
            {
                if (id == "undefined")
                {
                    if (!string.IsNullOrEmpty(externalCompanyId))
                    {
                        return Json(_companyService.ValidateCompanyId(int.Parse(externalCompanyId)),
                                    JsonRequestBehavior.AllowGet);
                    }
                }
                else return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Company Id number is invalid", JsonRequestBehavior.AllowGet);
            }


            return new EmptyResult();
        }

        public ActionResult AsyncGetGroupingLevel1(int CompanyId)
        {
            SelectList select = null;
            var company = _companyService.Get(CompanyId);
            var firstlevelgroup = company.CompanyGrouping1Levels;

            if (firstlevelgroup != null && firstlevelgroup.Any())
            {
                select = new SelectList(firstlevelgroup, "Id", "Name");
            }

            if (select == null)
                select = new SelectList(new List<Site>(), "Id", "Name");

            return Json(select);
        }

        public ActionResult AsyncCreateGroupingLevel1(CompanyGroupingLevelViewModel levelModel)
        {
            var level1 = new CompanyGrouping1Level();

            try
            {
                if (!string.IsNullOrEmpty(levelModel.GroupingLevel1Name))
                {
                    var company = _companyService.Get(levelModel.CompanyId);

                    if (CheckIfLevel1NameExists(levelModel.GroupingLevel1Name, levelModel.GroupingLevel1Id, company.CompanyGrouping1Levels))
                        return JsonError(company.FirstLevelGrouping + " name exists in the company");

                    level1.Company = company;
                    level1.Name = levelModel.GroupingLevel1Name;
                    company.CompanyGrouping1Levels.Add(level1);

                    _companyService.Update(company);

                    return Json(new { levelId = level1.Id, levelName = level1.Name });
                }
            }
            catch(Exception e)
            {
                LogError("Exception occured while creating grouping level 1", e);
                return JsonError("Grouping Level LogError");
            }

            return JsonOK();
        }

        public ActionResult AsyncCreateGroupingLevel2(CompanyGroupingLevelViewModel levelModel)
        {
            var level2 = new CompanyGrouping2Level();
            try
            {
                if (!string.IsNullOrEmpty(levelModel.GroupingLevel2Name))
                {
                    var company = _companyService.Get(levelModel.CompanyId);
                    var companyGroupingLevel1 = company.CompanyGrouping1Levels.Where(x => x.Id == levelModel.GroupingLevel1Id).Single();

                    if (CheckIfLevel2NameExists(levelModel.GroupingLevel2Name, levelModel.GroupingLevel2Id,
                                                _companyService.GetGrouping2LevelsByGrouping1LevelId(levelModel.GroupingLevel1Id)))
                    {
                        return JsonError(company.SecondLevelGrouping + " name exists in the " + company.FirstLevelGrouping.ToLower());
                    }

                    level2.Name = levelModel.GroupingLevel2Name;
                    level2.CompanyGrouping1Level = companyGroupingLevel1;
                    companyGroupingLevel1.CompanyGrouping2Levels.Add(level2);

                    _companyService.Update(company);

                    return Json(new
                    {
                        levelId = level2.Id,
                        levelName = level2.Name,
                        levelNameComplete = level2.CompanyGrouping1Level.Name + " / " + level2.Name
                    });
                }
            }
            catch(Exception e)
            {
                LogError("Exception occured while creating grouping level 2", e);
                return JsonError("Grouping Level LogError");
            }

            return JsonOK();
        }

        public ActionResult AsyncReasignSite(CompanyGroupingLevelViewModel levelModel)
        {
            try
            {
                var company = _companyService.Get(levelModel.CompanyId);
                var level2 = _companyService.GetGrouping2LevelById(levelModel.GroupingLevel2Id);
                var site = _siteService.Get(levelModel.SiteId);

                if (level2 != null && site != null)
                {
                    site.CompanyGrouping2Level = level2;
                    _companyService.Update(company);
                }
            }
            catch(Exception e)
            {
                LogError("Exception occured while reassigning site", e);
                return JsonError("Site Reasign LogError");
            }

            return JsonOK();
        }

        public ActionResult AsyncReasignArea(CompanyGroupingLevelViewModel levelModel)
        {
            try
            {
                var company = _companyService.Get(levelModel.CompanyId);
                var level1 = company.CompanyGrouping1Levels.Where(x => x.Id == levelModel.GroupingLevel1Id).Single();
                var level2 = _companyService.GetGrouping2LevelById(levelModel.GroupingLevel2Id);

                if (level1 != null && level2 != null)
                {
                    level2.CompanyGrouping1Level = level1;
                    _companyService.Update(company);

                    return Json(new
                    {
                        levelId = level2.Id,
                        levelName = level2.Name,
                        levelNameComplete = level2.CompanyGrouping1Level.Name + " / " + level2.Name
                    });
                }
            }
            catch(Exception e)
            {
                LogError("Exception occured while reassigning area", e);
                return JsonError("Area Reasign LogError");
            }

            return JsonOK();
        }

        public ActionResult AsyncChangeLevel1Name(CompanyGroupingLevelViewModel levelModel)
        {
            CompanyGrouping1Level level1 = null;
            try
            {
                var company = _companyService.Get(levelModel.CompanyId);
                level1 = company.CompanyGrouping1Levels.Where(x => x.Id == levelModel.GroupingLevel1Id).Single();

                if (level1 != null)
                {
                    if (!string.IsNullOrEmpty(levelModel.GroupingLevel1Name))
                    {
                        if (CheckIfLevel1NameExists(levelModel.GroupingLevel1Name, levelModel.GroupingLevel1Id,
                                                    company.CompanyGrouping1Levels))
                            return JsonError(company.FirstLevelGrouping + " name exists in the company");

                        level1.Name = levelModel.GroupingLevel1Name;
                        _companyService.Update(company);
                    }
                }
            }
            catch(Exception e)
            {
                LogError("Exception occured while changing level name", e);
                return JsonError("Grouping Name Change LogError");
            }

            var level2List = new List<object>();
            if (level1 != null)
            {
                level2List = level1.CompanyGrouping2Levels.Select(level2 =>
                                                                       Json(new
                                                                       {
                                                                           levelId = level2.Id,
                                                                           levelName = level1.Name + " / " + level2.Name
                                                                       })).Cast<object>().ToList();
            }

            return level1 != null
                       ? Json(new
                       {
                           levelId = level1.Id,
                           levelName = level1.Name,
                           level2Items = level2List
                       })
                       : JsonOK();

        }

        public ActionResult AsyncChangeLevel2Name(CompanyGroupingLevelViewModel levelModel)
        {
            CompanyGrouping2Level level2 = null;
            try
            {
                var company = _companyService.Get(levelModel.CompanyId);
                level2 = _companyService.GetGrouping2LevelById(levelModel.GroupingLevel2Id);

                if (level2 != null)
                {
                    if (!string.IsNullOrEmpty(levelModel.GroupingLevel2Name))
                    {
                        if (CheckIfLevel2NameExists(levelModel.GroupingLevel2Name, levelModel.GroupingLevel2Id,
                                                    _companyService.GetGrouping2LevelsByGrouping1LevelId(
                                                        levelModel.GroupingLevel1Id)))
                        {
                            return JsonError(company.SecondLevelGrouping + " name exists in the " +
                                          company.FirstLevelGrouping.ToLower());
                        }

                        level2.Name = levelModel.GroupingLevel2Name;
                        _companyService.Update(company);
                    }
                }
            }
            catch(Exception e)
            {
                LogError("Exception occured while changing level 2 name", e);
                return JsonError("Grouping Name Change LogError");
            }

            return level2 != null
                       ? Json(new
                       {
                           levelId = level2.Id,
                           levelName = level2.Name,
                           levelNameComplete = level2.CompanyGrouping1Level.Name + " / " + level2.Name
                       })
                       : JsonOK();
        }

        public ActionResult AsyncRemoveGrouping1Level(CompanyGroupingLevelViewModel levelModel)
        {
            try
            {
                var company = _companyService.Get(levelModel.CompanyId);
                var level1 = company.CompanyGrouping1Levels.Where(x => x.Id == levelModel.GroupingLevel1Id).Single();

                if (level1.CompanyGrouping2Levels.Count() > 0)
                {
                    return Json(new
                    {
                        levelId = level1.Id,
                        Status = "LogError",
                        Message = "The " + company.FirstLevelGrouping.ToLower() + " can't be deleted because it has assigned " +
                                   company.SecondLevelGrouping.ToLower()

                    });
                }

                if (_userService.IsGroupingLevel1MonitoringByUsers(levelModel.GroupingLevel1Id))
                {
                    return Json(new
                    {
                        levelId = level1.Id,
                        Status = "LogError",
                        Message = " The " + company.FirstLevelGrouping.ToLower() + " is being monitored by users"

                    });
                }

                level1.RemoveRelation();
                _companyService.RemoveCompanyGroupLevel1(level1);
                _companyService.Update(company);
            }
            catch(Exception e)
            {
                LogError("Exception occured while removing grouping level 1", e);
                return Json(new
                {
                    levelId = levelModel.GroupingLevel1Id,
                    Status = "LogError",
                    Message = "Grouping Level LogError"

                });
            }

            return Json(new
            {
                levelId = levelModel.GroupingLevel1Id,
                Status = "OK"

            });
        }

        public ActionResult AsyncRemoveGrouping2Level(CompanyGroupingLevelViewModel levelModel)
        {
            try
            {
                var company = _companyService.Get(levelModel.CompanyId);
                var level2 = _companyService.GetGrouping2LevelById(levelModel.GroupingLevel2Id);

                if (level2.Sites.Count() > 0)
                {
                    return Json(new
                    {
                        levelId = level2.Id,
                        Status = "LogError",
                        Message = " The " + company.SecondLevelGrouping.ToLower() + " can't be deleted because it has assigned sites"
                    });
                }

                if (_userService.IsGroupingLevel2MonitoringByUsers(levelModel.GroupingLevel2Id))
                {
                    return Json(new
                    {
                        levelId = level2.Id,
                        Status = "LogError",
                        Message = " The " + company.SecondLevelGrouping.ToLower() + " is being monitored by users"
                    });
                }

                level2.RemoveRelation();
                _companyService.Update(company);

            }
            catch(Exception e)
            {
                LogError("Exception occured while removing grouping level 2", e);
                return Json(new
                {
                    levelId = levelModel.GroupingLevel2Id,
                    Status = "LogError",
                    Message = "Grouping Level LogError"

                });
            }

            return Json(new
            {
                levelId = levelModel.GroupingLevel2Id,
                Status = "OK"

            });
        }

        [HttpPost]
        public ActionResult DuplicateCheck(String Name)
        {
            bool blnExists = false;
            try
            {
                IList<Company> objLstCompany = null;
                if (TempData["Companies"] != null)
                    objLstCompany = (List<Company>)TempData["Companies"];
                else
                    objLstCompany = _companyService.GetAll();

                foreach (Company objCompany in objLstCompany)
                {
                    if (objCompany.Name == Name)
                    {
                        blnExists = true;
                    }
                }
            }
            catch(Exception e)
            {
                LogError("Exception occured while checking for duplicate", e);
                return Json(true);
            }
            return Json(blnExists);
        }

        #endregion
    }

}
