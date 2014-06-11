using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Diebold.Domain.Entities;
using Diebold.WebApp.Models;
using Diebold.Services.Contracts;
using Lib.Web.Mvc.JQuery.JqGrid;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web;
using System.IO;
using Diebold.Services.Exceptions;
using Diebold.Domain.Exceptions;
using System;
using Diebold.Domain.Contracts.Infrastructure;

namespace Diebold.WebApp.Controllers
{
    public class SiteController : BaseCRUDTrackeableController<Site, SiteViewModel>
    {

        private readonly ISiteService _siteService;
        private readonly IDvrService _deviceService;
        private readonly ICompanyService _companyService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserService _UserService;
        protected readonly IUserDefaultsService _userDefaultService;
        protected readonly ISiteAccNumberService _siteAccNumberService;
        protected readonly ISiteLogoDetailsService _siteLogoDetailsService;

        public SiteController(ISiteService service, ICompanyService companyService, ICurrentUserProvider currentUserProvider, IUserService userService, IDvrService deviceService, IUserDefaultsService userDefaultService, ISiteAccNumberService siteAccNumberService,
            ISiteLogoDetailsService siteLogoDetailsService) 
            : base(service)
        {
            this._siteService = service;
            this._companyService = companyService;
            this._currentUserProvider = currentUserProvider;
            this._UserService = userService;
            this._deviceService = deviceService;
            _userDefaultService = userDefaultService;
            _siteAccNumberService = siteAccNumberService;
            _siteLogoDetailsService = siteLogoDetailsService;
        }
        
        protected override SiteViewModel MapEntity(Site item)
        {
            return new SiteViewModel(item);
        }
         
        public override ActionResult Index()
        {
            return View();
        }

        //
        //GET: /Gateway/Create
       // public override ActionResult Create(IEnumerable<HttpPostedFileBase> files)
        public override ActionResult Create()
        {
            ViewBag.IsCreate = true;
            int maxID = this.getMaxSiteID() + 1;
            var model = new SiteViewModel();

            var companies = this._companyService.GetAllEnabled();
            model.SiteId = maxID;
            model.AvailableCompanyList = companies;
            model.AvailableCompanyGrouping1LevelList = new List<CompanyGrouping1Level>();
            model.AvailableCompanyGrouping2LevelList = new List<CompanyGrouping2Level>();


            _siteAccNumberService.DeleteSiteAccountNumber(maxID);
            //if (files != null && files.Count() > 0)
            //{
            //    string strFileName = files.ToList()[0].FileName;
            //    byte[] content;//= new byte[str.Length];
            //    using (Stream str = files.ToList()[0].InputStream)
            //    {
            //        content = new byte[str.Length];
            //        str.Read(content, 0, content.Length);
            //    }

            //   // item.FileName = strFileName;
            //    //item.CompanyLogo = content;

            //    model.FileName = strFileName;
            //    model.SiteLogo = content;
            //}

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CreateSite(CompanyViewModel newItem, IEnumerable<HttpPostedFileBase> files)
        public ActionResult CreateSite(SiteViewModel newItem, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Site item = newItem.MapFromViewModel();
                    var siteLogoDet = new SiteLogoDetails();

                    if (files != null && files.Count() > 0)
                    {
                        string strFileName = files.ToList()[0].FileName;
                        byte[] content;//= new byte[str.Length];
                        using (Stream str = files.ToList()[0].InputStream)
                        {
                            content = new byte[str.Length];
                            str.Read(content, 0, content.Length);
                        }

                        siteLogoDet.FileName = strFileName;
                        siteLogoDet.SiteLogo = content;
                    }
                    //SetDefault(item);
                    //MapGroupingLevels(item, newItem);
                    _siteService.getGeoCoordinates(item);
                    _siteService.CreateSiteDetails(item, siteLogoDet);
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
                            LogError("Repository Exception occured while creating site", serviceException);
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.InnerException.Message));
                        }
                        else
                        {
                            LogError("Service Exception occured while creating site", serviceException);
                            ModelState.AddModelError("ServiceError", string.Format(serviceException.Message));
                        }
                    }
                }
                catch (Exception e)
                {
                    LogError("Exception occured while creating site", e);
                    ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));
                }
            }

            this.InitializeViewModel(newItem);
            return View(newItem);            
        }
        protected override SiteViewModel InitializeViewModel(SiteViewModel item)
        {
            ViewBag.IsCreate = true;

            var companies = this._companyService.GetAllEnabled();
            item.AvailableCompanyList = companies;
            item.AvailableCompanyGrouping1LevelList = new List<CompanyGrouping1Level>();
            item.AvailableCompanyGrouping2LevelList = new List<CompanyGrouping2Level>();

            return item;
        }

        public override ActionResult Edit(int id)
        {
            ViewBag.IsCreate = false;

            var site = _siteService.Get(id);
            _siteService.getGeoCoordinates(site);
            SiteLogoDetails siteLogoDet = _siteLogoDetailsService.GetSiteLogoDetailsbySiteId(id);

            var itemToEdit = MapEntity(site);
            if (siteLogoDet.SiteLogo != null)
            {
                itemToEdit.FileName = siteLogoDet.FileName;
                itemToEdit.SiteLogo = siteLogoDet.SiteLogo;
            }
            //if (files != null && files.Count() > 0)
            //{
            //    //var filestream = files.ToList()[0].InputStream;
            //    string strFileName = files.ToList()[0].FileName;
            //    //string strFileFullName = "$##$&_" + DateTime.Now.Date.ToString() + "_" + "$_&$_" + strFileName;
            //    //string filewithfullPath = Path.Combine(Server.MapPath("~/App_Data"), strFileFullName);
            //    //files.ToList()[0].SaveAs(filewithfullPath);
            //    //byte[] fileContent = ReadFile(filewithfullPath);
            //    byte[] content;//= new byte[str.Length];
            //    using (Stream str = files.ToList()[0].InputStream)
            //    {
            //        content = new byte[str.Length];
            //        str.Read(content, 0, content.Length);
            //    }

            //    itemToEdit.FileName = strFileName;
            //    itemToEdit.SiteLogo = content;
            //}

            itemToEdit.AvailableCompanyList = this._companyService.GetAllEnabled();

            itemToEdit.AvailableCompanyGrouping1LevelList = site.CompanyGrouping2Level.CompanyGrouping1Level.Company.CompanyGrouping1Levels;
            itemToEdit.AvailableCompanyGrouping2LevelList = site.CompanyGrouping2Level.CompanyGrouping1Level.CompanyGrouping2Levels;
            return View(itemToEdit);
        }

        
        [AcceptVerbs(HttpVerbs.Post)]       
        public ActionResult EditSiteDetails(int id, SiteViewModel editedItem, IEnumerable<HttpPostedFileBase> files)
        {
            var site = _siteService.Get(id);
            if (ModelState.IsValid)
            {
                try
                {
                    var itemFromView = editedItem.MapFromViewModel();
                    //To retain image on update if user doesn't update image
                    //itemFromView.SiteLogo = site.SiteLogo;
                    SiteLogoDetails siteLogoDet = _siteLogoDetailsService.GetSiteLogoDetailsbySiteId(id);

                    if (files != null && files.Count() > 0)
                    {
                        string strFileName = files.ToList()[0].FileName;

                        byte[] content;
                        using (Stream str = files.ToList()[0].InputStream)
                        {
                            content = new byte[str.Length];
                            str.Read(content, 0, content.Length);
                        }

                        siteLogoDet.FileName = strFileName;
                        editedItem.FileName = strFileName;
                        
                        siteLogoDet.SiteLogo = content;
                        editedItem.SiteLogo = content;
                    }
                    //itemFromView.Latitude = null;
                    //itemFromView.Longitude = null;
                    _siteService.getGeoCoordinates(itemFromView);
                   
                    _siteService.UpdateSiteDetails(itemFromView, siteLogoDet);
                    return View("Index");
                    
                }
                catch (ServiceException serviceException)
                {
                    LogError("Service Exception occured while editing site", serviceException);
                    if (serviceException.InnerException != null)
                    {
                        if (serviceException.InnerException is ValidationException)
                        {
                            AddModelErrors((ValidationException)serviceException.InnerException);
                        }
                        else
                        {
                            ModelState.AddModelError("ServiceError", serviceException.InnerException.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogError("Exception occured while editing site", e);
                    ModelState.AddModelError("", string.Format("Unexpected LogError: [{0}]", e.Message));
                }
            }

            editedItem.AvailableCompanyList = this._companyService.GetAllEnabled();            
            editedItem.AvailableCompanyGrouping1LevelList = site.CompanyGrouping2Level.CompanyGrouping1Level.Company.CompanyGrouping1Levels;
            editedItem.AvailableCompanyGrouping2LevelList = site.CompanyGrouping2Level.CompanyGrouping1Level.CompanyGrouping2Levels;

            return View("Edit", editedItem);
        }


        public static byte[] ReadFile(string filePath)
        {
            byte[] buffer = null;
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
        

        // To load company dropdown list
        public ActionResult AsyncCompany()
        {
            var siteViewModel = new SiteViewModel();
            siteViewModel.AvailableCompanyList = _companyService.GetAllEnabled();
            var availableCompany = this._companyService.GetAllEnabled();
            return Json(availableCompany.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsyncGrouping1Levels(int companyId)
        {
            var siteViewModel = new SiteViewModel();        
            siteViewModel.AvailableCompanyGrouping1LevelList = _companyService.GetGrouping1LevelsByCompanyId(companyId);                        
            var items = _companyService.GetGrouping1LevelsByCompanyId(companyId);
            return Json(items.Select(c => new { Value = c.Id, Text = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsyncGrouping2Levels(int group1LevelId)
        {          
            var items = _companyService.GetGrouping2LevelsByGrouping1LevelId(group1LevelId);
            return Json(items.Select(c => new { Value = c.Id, Text = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidateSiteId(string id, string siteId)
        {
            try
            {
                if (id == "undefined" || id == "0")
                {
                    if (!string.IsNullOrEmpty(siteId))
                    {
                        return Json(_siteService.ValidateSiteId(int.Parse(siteId)),
                                    JsonRequestBehavior.AllowGet);
                    }
                }
                else return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                LogError("Exception occured while validating site", e);
                return Json("Site Id number is invalid", JsonRequestBehavior.AllowGet);
            }


            return new EmptyResult();
        }

        public ActionResult SiteInfo()
        {
            IList<Site> objlstSites = _siteService.GetSitesByUser(_currentUserProvider.CurrentUser.Id);
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            oSerializer.MaxJsonLength = Int32.MaxValue;
            string jsonResponse = oSerializer.Serialize(prepareSiteMapData(objlstSites));
            objlstSites = null;
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        private List<SiteMapModel> prepareSiteMapData(IList<Site> objlstSites)
        {
            List<SiteMapModel> objlstSiteMap = new List<SiteMapModel>();
            SiteMapModel objSiteMapModel = null;
            string strCommaWithSpace = ", ";
            IList<int> siteIdList = new List<int>();
            IDictionary<int, IList<Dvr>> siteDeviceMap = new Dictionary<int, IList<Dvr>>();
            foreach (Site obSite in objlstSites)
            {
                siteIdList.Add(obSite.Id);
                IList<Dvr> devicelist = new List<Dvr>();
                siteDeviceMap.Add(obSite.Id, devicelist);
            }
            IList<Dvr> devices = _deviceService.GetAllDevicesBySiteList(siteIdList);
            foreach (Dvr device in devices)
            {
                siteDeviceMap[device.Site.Id].Add(device);
            }
            foreach (Site objSite in objlstSites)
            {
                objSiteMapModel = new SiteMapModel();
                objSiteMapModel.Id = objSite.Id;
                objSiteMapModel.Location = objSite.Name;
                objSiteMapModel.Address = objSite.Address1 + strCommaWithSpace + objSite.City + strCommaWithSpace + objSite.State + strCommaWithSpace + objSite.Country + strCommaWithSpace + objSite.Zip;
                if (objSite.CompanyGrouping2Level != null && objSite.CompanyGrouping2Level.CompanyGrouping1Level != null)
                {
                    //objSiteMapModel.LocationContact = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactName;
                    //objSiteMapModel.ContactEmail = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactEmail;
                    //objSiteMapModel.ContactPhone = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactOffice;
                    objSiteMapModel.LocationContact = objSite.ContactName;
                    objSiteMapModel.ContactEmail = objSite.ContactEmail;
                    objSiteMapModel.ContactPhone = objSite.ContactNumber;
                    IList<Dvr> objlstDevice = siteDeviceMap[objSite.Id];
                    IDictionary<String, int> parentTypeDeviceCountMap = _UserService.GetDevicesCount(objlstDevice);
                    objSiteMapModel.DVRDevicesCount = parentTypeDeviceCountMap["DVR"];
                    objSiteMapModel.AccessDevicesCount = parentTypeDeviceCountMap["ACCESS"];
                    objSiteMapModel.IntrusionDevicesCount = parentTypeDeviceCountMap["INTRUSION"];
                    objSiteMapModel.TotalDevicesCount = (objSiteMapModel.DVRDevicesCount + objSiteMapModel.AccessDevicesCount + objSiteMapModel.IntrusionDevicesCount);
                }
                objlstSiteMap.Add(objSiteMapModel);
            }
            //Setting default slection for sitmap inorder to display information on map
            if(objlstSiteMap.Count>0)
            {
                objlstSiteMap[objlstSiteMap.Count - 1].DefaultSiteLocation = objlstSiteMap[objlstSiteMap.Count - 1].Id;
            }
            return objlstSiteMap;
        }

        public ActionResult GetSiteImage(int siteId)
        {
            SiteLogoDetails objSiteLogo = _siteLogoDetailsService.GetSiteLogoDetailsbySiteId(siteId);
            if (objSiteLogo.SiteLogo != null)
            {
                string siteStream = "data:image/png;base64," + Convert.ToBase64String(objSiteLogo.SiteLogo);
                return Json(siteStream, JsonRequestBehavior.AllowGet);
            }
            return Json("");
        }

        public ActionResult Site_Read([DataSourceRequest] DataSourceRequest request)
        {
            int pageIndex = request.Page;
            int rowCount = request.PageSize;
            var sites = _siteService.GetSitesPerPage(pageIndex, rowCount);           
            var TotalSiteCount = _siteService.GetSitesCount();
            IList<SiteViewModel> objlstsite = new List<SiteViewModel>();
            sites.ToList().ForEach(x => 
            {
                SiteViewModel objsite = new SiteViewModel();
                objsite.Id = x.Id;
                objsite.Name = x.Name;
                objsite.Address = x.Address1 + ", " + x.City + ", " + x.State + ", " + x.Country;
                objsite.CompanyName = x.CompanyGrouping2Level.CompanyGrouping1Level.Company.Name;
                objsite.CompanyId = x.CompanyGrouping2Level.CompanyGrouping1Level.Company.Id;
                objsite.IsDisabled = x.IsDisabled;
                objlstsite.Add(objsite);
            });
            sites = null;
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            // Initialize  the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = objlstsite.ToList(), // Process data (paging and sorting applied)
                Total = TotalSiteCount // Total number of records
            };
            return Json(result, JsonRequestBehavior.AllowGet);
            //return new ContentResult()
            //{
            //    Content = serializer.Serialize(objlstsite.ToList().ToDataSourceResult(request)),
            //    ContentType = "application/json",

            //};          
        }        
        public int getMaxSiteID()
        {
            var maxSiteID = _siteService.GetMaxSiteId();
            return maxSiteID;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteSite(int id)
        {
            try
            {
                var site = _siteService.Get(id);
                SiteLogoDetails siteLogoDet = _siteLogoDetailsService.GetSiteLogoDetailsbySiteId(id);
               // _siteService.DeleteSiteDetails(id, _currentUserProvider.CurrentUser.Id);
                _siteService.DeleteSiteDetails(site, _currentUserProvider.CurrentUser.Id, siteLogoDet);

                //_siteService.Delete(id);
                if (Session["SelectedSiteId"] != null)
                {
                    if (Session["SelectedSiteId"].ToString() == id.ToString())
                    {
                        Session["SelectedSiteId"] = null;
                    }
                }
                if (Session["SelectedFASiteId"] != null)
                {
                    if (Session["SelectedFASiteId"].ToString() == id.ToString())
                    {
                        Session["SelectedFASiteId"] = null;
                    }
                }                
                //IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "SITEINFORMATION");
                //if (lstUserDefaults.Count() > 0)
                //{
                //    _userDefaultService.Delete(lstUserDefaults.First().Id);
                //}                
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                LogError("Exception occured while deleting site", e);
                return Json("LogError occured deleting site", JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisableSite(int id)
        {
            try
            {
                _siteService.Disable(id);
                //SiteLogoDetails objSiteLogo = new SiteLogoDetails();
                //objSiteLogo = _siteLogoDetailsService.GetSiteLogoDetailsbySiteId(id);
                //_siteLogoDetailsService.Disable(objSiteLogo.Id);
                return new EmptyResult();
            }
            catch(Exception e)
            {
                LogError("Exception occured while disabling site", e);
                return Json("LogError occured by disabling site", JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EnableSite(int id)
        {
            try
            {
                _siteService.Enable(id);
                //SiteLogoDetails objSiteLogo = new SiteLogoDetails();
                //objSiteLogo = _siteLogoDetailsService.GetSiteLogoDetailsbySiteId(id);
                //_siteLogoDetailsService.Enable(objSiteLogo.Id);
                return new EmptyResult();
            }
            catch(Exception e)
            {
                LogError("Exception occured while enabling site", e);
                return Json("LogError occured by enabling site", JsonRequestBehavior.AllowGet);
            }
        }
        // Add Site Account Number Details
        public ActionResult AddsiteAccNumber(int siteId, string accNumber, bool isAssociated)
        {
            try
            {
                //var note = new SiteNote
                //{
                //    Date = DateTime.Now,
                //    Site = _siteService.Get(siteId),
                //    Text = Notes,
                //    User = _userService.GetUserByUserName(User.Identity.Name)
                //};
                //_siteNoteService.Create(note);
                //// Rebind Grid with newly added data
                //Dictionary<string, bool> dctRoleDetails = GetRoleDetails();
                //IList<SiteNote> lstSiteNote = _siteNoteService.GetSiteNotebySiteId(siteId).OrderByDescending(x => x.Date).ToList();
                //IList<SiteNoteViewModel> lstSiteNoteViewModel = new List<SiteNoteViewModel>();
                //lstSiteNote.ToList().ForEach(x => { lstSiteNoteViewModel.Add(new SiteNoteViewModel { DisplayDate = x.Date.ToString("MM/dd HH:mm tt"), Text = x.Text, UserName = x.User.Name, Id = x.Id, isNotesViewable = dctRoleDetails["NotesViewable"], isNotesEditable = dctRoleDetails["NotesEditable"], isNotesDeleteable = dctRoleDetails["NotesDeletable"] }); });
                //lstSiteNoteViewModel.ToList().ForEach(x =>
                //{
                //    if (x.Text.Length > 15)
                //    {
                //        x.Text = x.Text.Substring(0, 15) + "...";
                //    }
                //});
                //return Json(lstSiteNoteViewModel.ToList(), JsonRequestBehavior.AllowGet);

                var note = new SiteAccountNumber
                {
                    Date = DateTime.Now,
                    //Site = _siteService.Get(siteId),
                    siteId = siteId,
                    AccountNumber = accNumber,
                    IsAssociatedWithFA = isAssociated,
                    User = _UserService.GetUserByUserName(User.Identity.Name)
                };
                _siteAccNumberService.Create(note);

                // Rebind Grid with newly added data
                //Dictionary<string, bool> dctRoleDetails = GetRoleDetails();

                //IList<SiteAccountNumber> lstSiteAccNumber = _siteAccNumberService.GetSiteAccNumberbySiteId(siteId, _currentUserProvider.CurrentUser.Id).OrderByDescending(x => x.Date).ToList();
                IList<SiteAccountNumber> lstSiteAccNumber = _siteAccNumberService.GetSiteAccNumberbySiteId(siteId).OrderByDescending(x => x.Date).ToList();
                IList<SiteAccNumberViewModel> lstSiteAccNumberViewModel = new List<SiteAccNumberViewModel>();

                lstSiteAccNumber.ToList().ForEach(x => { lstSiteAccNumberViewModel.Add(new SiteAccNumberViewModel { DisplayDate = x.Date.ToString("MM/dd HH:mm tt"), AccountNumber = x.AccountNumber, UserName = x.User.Name, Id = x.Id, IsAssociatedWithFA = x.IsAssociatedWithFA }); });
                lstSiteAccNumberViewModel.ToList().ForEach(x =>
                {
                    if (x.AccountNumber.Length > 15)
                    {
                        x.AccountNumber = x.AccountNumber.Substring(0, 15) + "...";
                    }
                });
                return Json(lstSiteAccNumberViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (ServiceException ex)
            {
                LogError("Service Exception occured while adding site account number", ex);
                return Json(ex.Message);
            }
        }

        public ActionResult GetSiteAccNumberInformation(int siteId)
        {
            try
            {
                //IList<SiteAccountNumber> lstSiteAccNumber = _siteAccNumberService.GetSiteAccNumberbySiteId(siteId, _currentUserProvider.CurrentUser.Id).OrderByDescending(x => x.Date).ToList();
                IList<SiteAccountNumber> lstSiteAccNumber = _siteAccNumberService.GetSiteAccNumberbySiteId(siteId).OrderByDescending(x => x.Date).ToList();
                IList<SiteAccNumberViewModel> lstSiteAccNumberViewModel = new List<SiteAccNumberViewModel>();
                lstSiteAccNumber.ToList().ForEach(x => { lstSiteAccNumberViewModel.Add(new SiteAccNumberViewModel { DisplayDate = x.Date.ToString("MM/dd HH:mm tt"), AccountNumber = x.AccountNumber, UserName = x.User.Name, Id = x.Id, IsAssociatedWithFA = x.IsAssociatedWithFA }); });
                lstSiteAccNumberViewModel.ToList().ForEach(x =>
                {
                    if (x.AccountNumber.Length > 15)
                    {
                        x.AccountNumber = x.AccountNumber.Substring(0, 15) + "...";
                    }
                });
                return Json(lstSiteAccNumberViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError("Service Exception occured while getting site notes", ex);
                return Json(ex.Message);
            }
        }

        public ActionResult RemoveSiteAccNumber(int SiteAccNumberId, int siteId)
        {
            if (SiteAccNumberId != 0)
            {
                SiteAccountNumber objSiteAccNumber = new SiteAccountNumber();
                objSiteAccNumber = _siteAccNumberService.Get(SiteAccNumberId);
                objSiteAccNumber.DeletedKey = objSiteAccNumber.Id;
                objSiteAccNumber.IsDisabled = true;
                _siteAccNumberService.Update(objSiteAccNumber);

                // Rebind Grid with newly added data             
                //IList<SiteAccountNumber> lstSiteAccNumber = _siteAccNumberService.GetSiteAccNumberbySiteId(siteId, _currentUserProvider.CurrentUser.Id).OrderByDescending(x => x.Date).ToList();
                IList<SiteAccountNumber> lstSiteAccNumber = _siteAccNumberService.GetSiteAccNumberbySiteId(siteId).OrderByDescending(x => x.Date).ToList();
                IList<SiteAccNumberViewModel> lstSiteAccNumViewModel = new List<SiteAccNumberViewModel>();

                lstSiteAccNumber.ToList().ForEach(x => { lstSiteAccNumViewModel.Add(new SiteAccNumberViewModel { DisplayDate = x.Date.ToString("MM/dd HH:mm tt"), AccountNumber = x.AccountNumber, UserName = x.User.Name, Id = x.Id, IsAssociatedWithFA = x.IsAssociatedWithFA }); });

                lstSiteAccNumViewModel.ToList().ForEach(x =>
                {
                    if (x.AccountNumber.Length > 15)
                    {
                        x.AccountNumber = x.AccountNumber.Substring(0, 15) + "...";
                    }
                });
                return Json(lstSiteAccNumViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("LogError while removing Site Account Number", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewEditSiteAccNumberDetails(int SiteAccNumberId, int siteId)
        {
            //SiteNote objSiteNote = new SiteNote();
            //SiteNoteViewModel objSiteNoteViewModel = new SiteNoteViewModel();
            //if (SiteNoteId != 0)
            //{
            //    objSiteNote = _siteNoteService.Get(SiteNoteId);
            //    // Map to Site View Model
            //    objSiteNoteViewModel.Text = objSiteNote.Text;
            //    objSiteNoteViewModel.Id = objSiteNote.Id;
            //}
            //else
            //{
            //    objSiteNote = null;
            //}
            //return Json(objSiteNoteViewModel, JsonRequestBehavior.AllowGet);

            SiteAccountNumber objSiteAccNumber = new SiteAccountNumber();
            SiteAccNumberViewModel objSiteAccNumberViewModel = new SiteAccNumberViewModel();
            if (SiteAccNumberId != 0)
            {
                objSiteAccNumber = _siteAccNumberService.Get(SiteAccNumberId);
                // Map to Site View Model
                objSiteAccNumberViewModel.AccountNumber = objSiteAccNumber.AccountNumber;
                objSiteAccNumberViewModel.IsAssociatedWithFA = objSiteAccNumber.IsAssociatedWithFA;
                objSiteAccNumberViewModel.Id = objSiteAccNumber.Id;
            }
            else
            {
                objSiteAccNumber = null;
            }
            return Json(objSiteAccNumberViewModel, JsonRequestBehavior.AllowGet);
        }        
        
        // EditSite AccountNumber Details
        public ActionResult EditSiteAccNumberDetails(int siteId, string accNumber, int SiteAccountId, bool isAssociated)
        {
            try
            {
                SiteAccountNumber siteAccNumber = _siteAccNumberService.Get(SiteAccountId);
                siteAccNumber.Date = DateTime.Now;
                siteAccNumber.AccountNumber = accNumber;
                siteAccNumber.IsAssociatedWithFA = isAssociated;

                siteAccNumber.User = _UserService.GetUserByUserName(User.Identity.Name);
                _siteAccNumberService.Update(siteAccNumber);
                
                // Rebind Grid with newly added data
                //IList<SiteAccountNumber> lstSiteAccNumber = _siteAccNumberService.GetSiteAccNumberbySiteId(siteId, _currentUserProvider.CurrentUser.Id).OrderByDescending(x => x.Date).ToList();
                IList<SiteAccountNumber> lstSiteAccNumber = _siteAccNumberService.GetSiteAccNumberbySiteId(siteId).OrderByDescending(x => x.Date).ToList();
                IList<SiteAccNumberViewModel> lstSiteAccNumberViewModel = new List<SiteAccNumberViewModel>();

                lstSiteAccNumber.ToList().ForEach(x =>
                {
                    lstSiteAccNumberViewModel.Add(new SiteAccNumberViewModel { DisplayDate = x.Date.ToString("MM/dd HH:mm tt"), AccountNumber = x.AccountNumber, UserName = x.User.Name, Id = x.Id, IsAssociatedWithFA = x.IsAssociatedWithFA });
                });
                
                lstSiteAccNumberViewModel.ToList().ForEach(x =>
                {
                    if (x.AccountNumber.Length > 15)
                    {
                        x.AccountNumber = x.AccountNumber.Substring(0, 15) + "...";
                    }
                });                
                return Json(lstSiteAccNumberViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (ServiceException ex)
            {
                LogError("Service Exception occured while adding site account number", ex);
                return Json(ex.Message);
            }
        }
    }
}
