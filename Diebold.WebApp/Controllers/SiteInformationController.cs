using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.WebApp.Models;
using Diebold.Services.Contracts;
using Diebold.Services.Exceptions;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Helpers;
using System.Configuration;
using System.IO;
using Diebold.Exporter;

namespace Diebold.WebApp.Controllers
{
    public class SiteInformationController : BaseController
    {
        //
        // GET: /SiteInformation/
        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;
        protected readonly IDvrService _deviceService;
        private readonly ISiteService _siteService;
        protected readonly IUserDefaultsService _userDefaultService;
        protected readonly ISiteNoteSite _siteNoteService;
        protected readonly ISiteDocumentService _siteDocumentService;
        protected readonly IImpersonateService _impersonateService;
        protected readonly ICompanyInventoryService _companyInventoryService;
        protected readonly ISiteInventoryService _siteInventoryService;
        private readonly ISiteLogoDetailsService _siteLogoDetailsService;

        public SiteInformationController(
            IDvrService deviceService,
            ISiteService siteService,
            IUserService userService,
            ICurrentUserProvider currentUserProvider,
            IUserDefaultsService userDefaultService,
            ISiteNoteSite siteNoteService,
            ISiteDocumentService siteDocumentService,
            IImpersonateService impersonateService,
            ICompanyInventoryService companyInventoryService,
            ISiteInventoryService siteInventoryService,
            ISiteLogoDetailsService siteLogoDetailsService
            )
        {
            _deviceService = deviceService;
            _siteService = siteService;
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _userDefaultService = userDefaultService;
            _siteNoteService = siteNoteService;
            _siteDocumentService = siteDocumentService;
            _impersonateService = impersonateService;
            _companyInventoryService = companyInventoryService;
            _siteInventoryService = siteInventoryService;
            _siteLogoDetailsService = siteLogoDetailsService;
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllSiteDetailsforSearch()
        {
            IList<SiteViewModel> objlstSiteView = new List<SiteViewModel>();
            var siteList = _siteService.GetSitesByUser(_currentUserProvider.CurrentUser.Id).Where(x => x.DeletedKey == null);
            siteList.ToList().ForEach(x => objlstSiteView.Add(new SiteViewModel(x)));
            return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, Address = c.Address1, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSiteDetails()
        {
            var siteList = _siteService.GetSitesByUser(_currentUserProvider.CurrentUser.Id).Where(x => x.DeletedKey == null);
            IList<SiteViewModel> objlstSiteView = new List<SiteViewModel>();

            if (siteList != null)
            {
                /* Changed the method Call because we need to display all the sites associated with the user even when the device is not associated. */
                siteList.ToList().ForEach(x => objlstSiteView.Add(new SiteViewModel(x)));
            }
            // If session is empty then check for default value is present or not then return.
            // GetDefault Selection Item
            IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, "SITEINFORMATION");

            bool deletedSite = true;
            if (Session["SelectedSiteId"] != null)
            {
                deletedSite = _siteService.ValidateSiteIdForSiteInfo(Convert.ToInt32(Session["SelectedSiteId"]));
            }
            else if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
            {
                deletedSite = _siteService.ValidateSiteIdForSiteInfo(Convert.ToInt32(lstUserDefaults.First().FilterValue));
            }

            // Get Value from Session and return 
            if (Session["SelectedSiteId"] != null)
            {
                int SiteId = (int)Session["SelectedSiteId"];
                if (deletedSite == false)
                {
                    return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, Address = c.Address1, City = c.City, State = c.State, Zip = c.Zip, DefaultSelectedValue = SiteId }), JsonRequestBehavior.AllowGet);
                }
            }



            if (lstUserDefaults != null && lstUserDefaults.Count() > 0)
            {
                if (objlstSiteView.Where(x => x.Id == lstUserDefaults.First().FilterValue).Count() > 0)
                {
                    if (deletedSite == false)
                    {
                        return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, Address = c.Address1, City = c.City, State = c.State, Zip = c.Zip, DefaultSelectedValue = lstUserDefaults.First().FilterValue }), JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, Address = c.Address1, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(objlstSiteView.Select(c => new { Id = c.Id, Name = c.Name, Address = c.Address1, City = c.City, State = c.State, Zip = c.Zip }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationDetailByLocationId(int LocationId)
        {
            Session["SelectedSiteId"] = LocationId;
            Site objSite = new Site();
            objSite = _siteService.Get(LocationId);
            SiteLogoDetails objSiteLogo = _siteLogoDetailsService.GetSiteLogoDetailsbySiteId(LocationId);


            string strCommaWithSpace = ", ";
            //Try to use SiteView model Instead of using SiteInfo()
            SiteInfo objsiteInfo = new SiteInfo();
            //objsiteInfo.LocationContact = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactName;

            //objsiteInfo.ContactEmail = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactEmail;
            //objsiteInfo.ContactPersonPhone = objSite.CompanyGrouping2Level.CompanyGrouping1Level.Company.PrimaryContactOffice;
            objsiteInfo.Id = objSite.Id;
            objsiteInfo.LocationContact = objSite.ContactName;
            objsiteInfo.ContactEmail = objSite.ContactEmail;
            objsiteInfo.ContactPersonPhone = objSite.ContactNumber;
            objsiteInfo.Location = objSite.Name;
            objsiteInfo.Address = objSite.Address1 + strCommaWithSpace + objSite.City + strCommaWithSpace + objSite.State + strCommaWithSpace + objSite.Country + strCommaWithSpace + objSite.Zip;
            objsiteInfo.notes = objSite.Notes;
            if (objSiteLogo!= null && objSiteLogo.SiteLogo != null)
            {
                objsiteInfo.SiteLogo = objSiteLogo.SiteLogo;
            }
            IList<Dvr> objlstDevice = _deviceService.GetDevicesBySiteId(objSite.Id);
            IDictionary<String, int> parentTypeDeviceCountMap = _userService.GetDevicesCount(objlstDevice);
            objsiteInfo.VideoHealthDevice = parentTypeDeviceCountMap["DVR"];
            objsiteInfo.AccessDevice = parentTypeDeviceCountMap["ACCESS"];
            objsiteInfo.IntrusionDevice = parentTypeDeviceCountMap["INTRUSION"];
            objsiteInfo.DeviceCount = objsiteInfo.VideoHealthDevice + objsiteInfo.AccessDevice + objsiteInfo.IntrusionDevice;
            Dictionary<string, bool> dctRoleDetails = GetRoleDetails();
            IList<SiteNote> lstSiteNote = _siteNoteService.GetSiteNotebySiteId(LocationId).OrderByDescending(x => x.Date).ToList();
            IList<SiteNoteViewModel> lstSiteNoteViewModel = new List<SiteNoteViewModel>();
            lstSiteNote.ToList().ForEach(x => { lstSiteNoteViewModel.Add(new SiteNoteViewModel { DisplayDate = x.Date.ToString("MM/dd HH:mm tt"), Text = x.Text, UserName = x.User.Username, Id = x.Id, isNotesViewable = dctRoleDetails["NotesViewable"], isNotesEditable = dctRoleDetails["NotesEditable"], isNotesDeleteable = dctRoleDetails["NotesDeletable"] }); });
            lstSiteNoteViewModel.ToList().ForEach(x =>
            {
                if (x.Text.Length > 15)
                {
                    x.Text = x.Text.Substring(0, 15) + "...";
                }
            });
            objsiteInfo.SiteNote = lstSiteNoteViewModel.ToList();
            var SiteDocument = _siteDocumentService.GetSiteDocumentbyUserandSiteId(_currentUserProvider.CurrentUser.Id, objSite.Id).Where(y => y.IsPrimary == true).Select(x => x.FileName);
            if (SiteDocument != null && SiteDocument.Count() > 0)
            {
                objsiteInfo.DefaultDocument = SiteDocument.First().Substring(SiteDocument.First().LastIndexOf('\\') + 1, SiteDocument.First().Length - (SiteDocument.First().LastIndexOf('\\') + 22)) + ".pdf";
            }
            else
            {
                objsiteInfo.DefaultDocument = string.Empty;
            }
            string strsiteImage = string.Empty;
            if (objsiteInfo.SiteLogo != null)
            {
                strsiteImage = "data:image/png;base64," + Convert.ToBase64String(objsiteInfo.SiteLogo);
            }
            objsiteInfo.siteImage = strsiteImage;
            if (objsiteInfo.SiteNote == null || objsiteInfo.SiteNote.Count() < 1)
            {
                SiteNoteViewModel objSiteNote = new SiteNoteViewModel();
                objSiteNote.isNotesEditable = dctRoleDetails["NotesEditable"];
                objSiteNote.isNotesViewable = dctRoleDetails["NotesViewable"];
                objSiteNote.isNotesDeleteable = dctRoleDetails["NotesDeletable"];
                List<SiteNoteViewModel> lstStNoteViewModel = new List<SiteNoteViewModel>();
                lstStNoteViewModel.Add(objSiteNote);
                objsiteInfo.SiteNote = lstStNoteViewModel;
            }
            string CurrentUserRole = _currentUserProvider.CurrentUser.Role.Name;

            var result = new
            {
                objsiteInfo.Id,
                objsiteInfo.LocationContact,
                objsiteInfo.ContactEmail,
                objsiteInfo.ContactPersonPhone,
                objsiteInfo.Location,
                objsiteInfo.Address,
                objsiteInfo.notes,
                objsiteInfo.siteImage,
                objsiteInfo.VideoHealthDevice,
                objsiteInfo.AccessDevice,
                objsiteInfo.IntrusionDevice,
                objsiteInfo.DeviceCount,
                objsiteInfo.SiteNote,
                CurrentUserRole,
                objsiteInfo.DefaultDocument
            };
            objsiteInfo = null;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public int GetSitesByUser()
        {
            int testUserID = _currentUserProvider.CurrentUser.Id;
            var siteList = _userService.GetSitesByUser(_currentUserProvider.CurrentUser.Id, null);
            return siteList.Count();
        }

        public ActionResult AddsiteNotes(int siteId, string Notes)
        {
            try
            {
                var note = new SiteNote
                {
                    Date = DateTime.Now,
                    Site = _siteService.Get(siteId),
                    Text = Notes,
                    User = _userService.GetUserByUserName(User.Identity.Name)
                };
                _siteNoteService.Create(note);
                // Rebind Grid with newly added data
                Dictionary<string, bool> dctRoleDetails = GetRoleDetails();
                IList<SiteNote> lstSiteNote = _siteNoteService.GetSiteNotebySiteId(siteId).OrderByDescending(x => x.Date).ToList();
                IList<SiteNoteViewModel> lstSiteNoteViewModel = new List<SiteNoteViewModel>();
                lstSiteNote.ToList().ForEach(x => { lstSiteNoteViewModel.Add(new SiteNoteViewModel { DisplayDate = x.Date.ToString("MM/dd HH:mm tt"), Text = x.Text, UserName = x.User.Name, Id = x.Id, isNotesViewable = dctRoleDetails["NotesViewable"], isNotesEditable = dctRoleDetails["NotesEditable"], isNotesDeleteable = dctRoleDetails["NotesDeletable"] }); });
                lstSiteNoteViewModel.ToList().ForEach(x =>
                {
                    if (x.Text.Length > 15)
                    {
                        x.Text = x.Text.Substring(0, 15) + "...";
                    }
                });
                return Json(lstSiteNoteViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (ServiceException ex)
            {
                LogError("Service Exception occured while adding site notes", ex);
                return Json(ex.Message);
            }
        }

        public ActionResult GetSiteNoteInformation(int LocationId)
        {
            try
            {
                IList<SiteNote> lstSiteNote = _siteNoteService.GetSiteNotebySiteId(LocationId);
                IList<SiteNoteViewModel> lstSiteNoteViewModel = new List<SiteNoteViewModel>();
                lstSiteNote.ToList().ForEach(x => { lstSiteNoteViewModel.Add(new SiteNoteViewModel(x)); });
                return Json(lstSiteNoteViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError("Service Exception occured while getting site notes", ex);
                return Json(ex.Message);
            }
        }
        public ActionResult SaveDefaultValue(int SiteId, string InternalName, string ControlName)
        {
            IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName);
            if (lstUserDefaults.Count() > 0)
            {
                lstUserDefaults.First().FilterValue = SiteId;
                _userDefaultService.Update(lstUserDefaults.First());
            }
            else
            {
                UserDefaults objUserDefaults = new UserDefaults();
                objUserDefaults.FilterName = ControlName;
                objUserDefaults.FilterValue = SiteId;
                objUserDefaults.InternalName = InternalName;
                objUserDefaults.User = _userService.Get(_currentUserProvider.CurrentUser.Id);
                _userDefaultService.Create(objUserDefaults);
            }
            return Json("RecordModified", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClearSiteInfoDefaultValue(string InternalName, string ControlName)
        {
            try
            {
                IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(_currentUserProvider.CurrentUser.Id, InternalName);
                if (lstUserDefaults.Count() > 0)
                {
                    _userDefaultService.Delete(lstUserDefaults.First().Id);
                    return Json("Defaults Cleared", JsonRequestBehavior.AllowGet);
                }
                return Json("No Defaults", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        public ActionResult RemoveSiteNote(int SiteNoteId, int LocationId)
        {
            if (SiteNoteId != 0)
            {
                SiteNote objSiteNote = new SiteNote();
                objSiteNote = _siteNoteService.Get(SiteNoteId);
                objSiteNote.DeletedKey = objSiteNote.Id;
                objSiteNote.IsDisabled = true;
                _siteNoteService.Update(objSiteNote);
                // Rebind Grid with newly added data
                Dictionary<string, bool> dctRoleDetails = GetRoleDetails();
                IList<SiteNote> lstSiteNote = _siteNoteService.GetSiteNotebySiteId(LocationId).OrderByDescending(x => x.Date).ToList();
                IList<SiteNoteViewModel> lstSiteNoteViewModel = new List<SiteNoteViewModel>();
                lstSiteNote.ToList().ForEach(x => { lstSiteNoteViewModel.Add(new SiteNoteViewModel { DisplayDate = x.Date.ToString("MM/dd HH:mm tt"), Text = x.Text, UserName = x.User.Name, Id = x.Id, isNotesViewable = dctRoleDetails["NotesViewable"], isNotesEditable = dctRoleDetails["NotesEditable"], isNotesDeleteable = dctRoleDetails["NotesDeletable"] }); });
                lstSiteNoteViewModel.ToList().ForEach(x =>
                {
                    if (x.Text.Length > 15)
                    {
                        x.Text = x.Text.Substring(0, 15) + "...";
                    }
                });
                return Json(lstSiteNoteViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("LogError while removing SiteNote", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewEditSiteNoteDetails(int SiteNoteId, int LocationId)
        {
            SiteNote objSiteNote = new SiteNote();
            SiteNoteViewModel objSiteNoteViewModel = new SiteNoteViewModel();
            if (SiteNoteId != 0)
            {
                objSiteNote = _siteNoteService.Get(SiteNoteId);
                // Map to Site View Model
                objSiteNoteViewModel.Text = objSiteNote.Text;
                objSiteNoteViewModel.Id = objSiteNote.Id;
            }
            else
            {
                objSiteNote = null;
            }
            return Json(objSiteNoteViewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditSiteNoteDetails(int siteId, string Notes, int SiteNoteId)
        {
            try
            {
                SiteNote siteNote = _siteNoteService.Get(SiteNoteId);
                siteNote.Date = DateTime.Now;
                siteNote.Text = Notes;
                siteNote.User = _userService.GetUserByUserName(User.Identity.Name);
                _siteNoteService.Update(siteNote);
                // Rebind Grid with newly added data
                Dictionary<string, bool> dctRoleDetails = GetRoleDetails();
                IList<SiteNote> lstSiteNote = _siteNoteService.GetSiteNotebySiteId(siteId).OrderByDescending(x => x.Date).ToList();
                IList<SiteNoteViewModel> lstSiteNoteViewModel = new List<SiteNoteViewModel>();
                lstSiteNote.ToList().ForEach(x => { lstSiteNoteViewModel.Add(new SiteNoteViewModel { DisplayDate = x.Date.ToString("MM/dd HH:mm tt"), Text = x.Text, UserName = x.User.Name, Id = x.Id, isNotesViewable = dctRoleDetails["NotesViewable"], isNotesEditable = dctRoleDetails["NotesEditable"], isNotesDeleteable = dctRoleDetails["NotesDeletable"] }); });
                lstSiteNoteViewModel.ToList().ForEach(x =>
                {
                    if (x.Text.Length > 15)
                    {
                        x.Text = x.Text.Substring(0, 15) + "...";
                    }
                });
                return Json(lstSiteNoteViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (ServiceException ex)
            {
                LogError("Service Exception occured while adding site notes", ex);
                return Json(ex.Message);
            }
        }

        public ActionResult SaveSiteDocument(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                if (_impersonateService.impersonateValidUser())
                {
                    Site site = _siteService.Get((int)Session["SelectedSiteId"]);
                    foreach (var file in files)
                    {
                        // Some browsers send file names with full path.
                        // We are only interested in the file name.
                        // var fileName = Path.GetFileName(file.FileName);
                        // var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                        SiteDocument objSiteDocument = new SiteDocument();
                        string targetFolder = ConfigurationManager.AppSettings["DocumentsFileServer"];
                        string targetFolderIncidentsId = Path.Combine(targetFolder + "\\" + "Diebold Documents" + "\\" + _currentUserProvider.CurrentUser.Name);
                        if (!System.IO.Directory.Exists(targetFolderIncidentsId))
                        {
                            Directory.CreateDirectory(targetFolderIncidentsId);
                        }
                        if (System.IO.Directory.Exists(targetFolderIncidentsId))
                        {
                            DateTime currentDateTime = DateTime.Now;
                            string targetFileName = Path.Combine(targetFolderIncidentsId, file.FileName.Substring(0, file.FileName.LastIndexOf('.')) + currentDateTime.ToString("ddMMyyyyHHmmssfff") + ".pdf");
                            string[] FileLocation = targetFileName.Split(new string[] { targetFolder }, StringSplitOptions.None);
                            if (!System.IO.File.Exists(targetFileName))
                            {
                                file.SaveAs(targetFileName);
                                // Save Items to DB
                                objSiteDocument.Date = currentDateTime;
                                objSiteDocument.FileName = FileLocation[1].Substring(1, FileLocation[1].Length - 1);
                                objSiteDocument.Site = site;
                                objSiteDocument.User = _currentUserProvider.CurrentUser;
                                objSiteDocument.IsPrimary = false;
                                _siteDocumentService.Create(objSiteDocument);
                            }
                        }
                    }
                }
                _impersonateService.undoImpersonation();
            }
            return Content("");
        }

        public ActionResult GetSiteDocumentsbySiteId(string SiteId)
        {
            List<SiteDocumentViewModel> lstSiteDocumentViewModel = GetSiteNoteDocumentDetails(SiteId);
            return Json(lstSiteDocumentViewModel.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewSiteDocumentFileContent(string Id)
        {
            var SiteNoteDocument = _siteDocumentService.Get(Convert.ToInt32(Id));
            string DocumentFileServerVirtualPath = ConfigurationManager.AppSettings["DocumentFileServerVirtualPath"];            
            SiteNoteDocument.FileName = SiteNoteDocument.FileName.Replace(@"\", @"/");            
            //string testLoadFrom = DocumentFileServerVirtualPath.Replace("{0}", SiteNoteDocument.FileName.Substring(0, SiteNoteDocument.FileName.LastIndexOf('.')) + ".pdf");

            string LoadFrom = DocumentFileServerVirtualPath.Replace("{0}", (SiteNoteDocument.FileName.Substring(0, SiteNoteDocument.FileName.LastIndexOf('/') + 1)
               + Uri.EscapeDataString(SiteNoteDocument.FileName.Substring(SiteNoteDocument.FileName.LastIndexOf('/') + 1, SiteNoteDocument.FileName.LastIndexOf('.') - SiteNoteDocument.FileName.LastIndexOf('/') - 1)) + ".pdf"));

            Response.Clear();
            Response.ClearHeaders();
            // Response.ContentType = "application/pdf";
            var cd = new System.Net.Mime.ContentDisposition
            {
                // FileName = "Site Document.pdf",
                FileName = SiteNoteDocument.FileName.Substring(SiteNoteDocument.FileName.LastIndexOf('/') + 1, SiteNoteDocument.FileName.Length - SiteNoteDocument.FileName.LastIndexOf('/') - 21) + ".pdf",
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            Response.Flush();

            byte[] response = new System.Net.WebClient().DownloadData(LoadFrom);
            Response.OutputStream.Write(response, 0, response.Length);
            Response.Flush();
            Response.Close();
            return Json(LoadFrom, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSiteDocument(string SiteId, string DocumentId)
        {
            var SiteNoteDocument = _siteDocumentService.Get(Convert.ToInt32(DocumentId));
            SiteNoteDocument.DeletedKey = Convert.ToInt32(DocumentId);
            _siteDocumentService.Update(SiteNoteDocument);
            // After Delete of a document reload the other items and display in Grid
            List<SiteDocumentViewModel> lstSiteDocumentViewModel = GetSiteNoteDocumentDetails(SiteId);
            return Json(lstSiteDocumentViewModel.ToList(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetSiteInventoryDetails(string SiteId)
        {
            IList<SiteInventoryViewModel> lstSiteInventoryViewModel = new List<SiteInventoryViewModel>();
            int intSiteId = Convert.ToInt32(SiteId);
            var SiteInventoryDetails = _siteInventoryService.GetSiteInventoryBySiteandCompany(intSiteId);
            Dictionary<string, bool> dctRoleDetails = GetRoleDetails();
            if (SiteInventoryDetails != null && SiteInventoryDetails.Count() > 0)
                foreach (var item in SiteInventoryDetails.ToList())
                {
                    SiteInventoryViewModel objSiteInventoryViewModel = new SiteInventoryViewModel();
                    if (item.CompanyInventoryId != null && item.CompanyInventoryId > 0)
                        objSiteInventoryViewModel.InventoryKeyId = item.CompanyInventoryId;
                    else
                        objSiteInventoryViewModel.InventoryKeyId = 0;
                    objSiteInventoryViewModel.InventoryValue = item.InventoryValue;
                    objSiteInventoryViewModel.InventoryKey = item.InventoryKey;
                    objSiteInventoryViewModel.ExternalCompanyId = item.ExternalCompanyId;
                    objSiteInventoryViewModel.isInventoryViewable = dctRoleDetails["InventoryViewable"];
                    objSiteInventoryViewModel.isInventoryEditable = dctRoleDetails["InventoryEditable"];
                    lstSiteInventoryViewModel.Add(objSiteInventoryViewModel);
                }
            else
            {
                SiteInventoryViewModel objSiteInventoryViewModel = new SiteInventoryViewModel();
                objSiteInventoryViewModel.InventoryValue = "No Records Found"; // This is a Dummay value added to check whether any records are present or not and to display a empty grid
                objSiteInventoryViewModel.isInventoryDeleteable = dctRoleDetails["InventoryViewable"];
                objSiteInventoryViewModel.isInventoryDeleteable = dctRoleDetails["InventoryEditable"];
                lstSiteInventoryViewModel.Add(objSiteInventoryViewModel);
            }
            return Json(lstSiteInventoryViewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditSiteInventoryDetails(string CompanyInventoryId, string InventoryKey, string InventoryValue, string SiteId)
        {
            int intInventoryId = Convert.ToInt32(CompanyInventoryId);
            int intSiteId = Convert.ToInt32(SiteId);
            var SiteInventoryDetails = _siteInventoryService.GetSiteInventorybySiteIdCompanyInventoryId(intSiteId, intInventoryId);
            if (intInventoryId != null && intInventoryId > 0)
            {
                // Update the record
                var SI = SiteInventoryDetails.First();
                SI.InventoryValue = InventoryValue;
                _siteInventoryService.Update(SI);
            }
            else
            {
                // Insert the record
                var CI = _companyInventoryService.GetInventoryBySiteId(intSiteId);
                if (CI != null)
                {
                    var CInventoryDetails = CI.Where(x => x.InventoryKey.Equals(InventoryKey));
                    SiteInventoryViewModel objSiteInventoryViewModel = new SiteInventoryViewModel();
                    objSiteInventoryViewModel.InventoryKeyId = CInventoryDetails.First().Id;
                    objSiteInventoryViewModel.SiteId = intSiteId;
                    objSiteInventoryViewModel.InventoryValue = InventoryValue;
                    SiteInventory objSiteInventory = MapEntity(objSiteInventoryViewModel);
                    _siteInventoryService.Create(objSiteInventory);
                }

            }
            // Rebind the Grid
            IList<SiteInventoryViewModel> lstSiteInventoryViewModel = new List<SiteInventoryViewModel>();
            var SiteInventoryResultSet = _siteInventoryService.GetSiteInventoryBySiteandCompany(intSiteId);
            Dictionary<string, bool> dctRoleDetails = GetRoleDetails();
            if (SiteInventoryResultSet != null && SiteInventoryResultSet.Count() > 0)
                foreach (var item in SiteInventoryResultSet.ToList())
                {
                    SiteInventoryViewModel objSiteInventoryViewModel = new SiteInventoryViewModel();
                    if (item.CompanyInventoryId != null && item.CompanyInventoryId > 0)
                        objSiteInventoryViewModel.InventoryKeyId = item.CompanyInventoryId;
                    else
                        objSiteInventoryViewModel.InventoryKeyId = 0;
                    objSiteInventoryViewModel.InventoryValue = item.InventoryValue;
                    objSiteInventoryViewModel.InventoryKey = item.InventoryKey;
                    objSiteInventoryViewModel.ExternalCompanyId = item.ExternalCompanyId;
                    objSiteInventoryViewModel.isInventoryViewable = dctRoleDetails["InventoryViewable"];
                    objSiteInventoryViewModel.isInventoryEditable = dctRoleDetails["InventoryEditable"];
                    objSiteInventoryViewModel.isInventoryDeleteable = dctRoleDetails["InventoryDeletable"];
                    lstSiteInventoryViewModel.Add(objSiteInventoryViewModel);
                }
            else
            {
                SiteInventoryViewModel objSiteInventoryViewModel = new SiteInventoryViewModel();
                objSiteInventoryViewModel.isInventoryDeleteable = dctRoleDetails["InventoryViewable"];
                objSiteInventoryViewModel.isInventoryDeleteable = dctRoleDetails["InventoryEditable"];
                objSiteInventoryViewModel.isInventoryDeleteable = dctRoleDetails["InventoryDeletable"];
                lstSiteInventoryViewModel.Add(objSiteInventoryViewModel);
            }
            return Json(lstSiteInventoryViewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateSiteDocumnet(string DocumentId, string SiteId)
        {
            // var SiteNoteDocument = _siteDocumentService.Get(Convert.ToInt32(DocumentId));
            // SiteNoteDocument.IsPrimary = !SiteNoteDocument.IsPrimary;
            int intSiteId = Convert.ToInt32(SiteId);
            int intDocumentId = Convert.ToInt32(DocumentId);
            var SiteNoteDocument = _siteDocumentService.GetSiteDocumentbyUserandSiteId(_currentUserProvider.CurrentUser.Id, intSiteId);
            var CurrentlyChecked = SiteNoteDocument.Where(x => x.IsPrimary == true).Select(y => y.Id).FirstOrDefault();
            SiteNoteDocument.ToList().ForEach(x =>
            {
                if (x.Id == intDocumentId)
                {
                    if (CurrentlyChecked != null)
                    {
                        if (CurrentlyChecked == intDocumentId)
                        {
                            x.IsPrimary = false;
                        }
                        else
                        {
                            x.IsPrimary = true;
                        }
                    }
                    else
                    {
                        x.IsPrimary = true;
                    }
                }
                else
                {
                    x.IsPrimary = false;
                }
            });
            _siteDocumentService.Update(SiteNoteDocument);
            // After update of a document reload the other items and display in Grid
            List<SiteDocumentViewModel> lstSiteDocumentViewModel = GetSiteNoteDocumentDetails(SiteId);
            return Json(lstSiteDocumentViewModel.ToList(), JsonRequestBehavior.AllowGet);
        }
        private Dictionary<string, bool> GetRoleDetails()
        {
            bool isNotesViewable;
            bool isNotesEditable;
            bool isNotesDeleteable;
            bool isDocumentsViewable;
            bool isDocumentsEditable;
            bool isDocumentsDeletable;
            bool isInventoryViewable;
            bool isInventoryEditable;
            var RoleDetails = _currentUserProvider.CurrentUser.Role;
            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.ViewNotes))
                isNotesViewable = true;
            else
                isNotesViewable = false;

            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.EditNotes))
                isNotesEditable = true;
            else
                isNotesEditable = false;

            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.DeleteNotes))
                isNotesDeleteable = true;
            else
                isNotesDeleteable = false;

            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.ViewDocuments))
                isDocumentsViewable = true;
            else
                isDocumentsViewable = false;

            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.EditDocuments))
                isDocumentsEditable = true;
            else
                isDocumentsEditable = false;

            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.DeleteDocuments))
                isDocumentsDeletable = true;
            else
                isDocumentsDeletable = false;

            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.ViewInventory))
                isInventoryViewable = true;
            else
                isInventoryViewable = false;

            if (RoleDetails.Actions.Contains(Diebold.Domain.Entities.Action.EditInventory))
                isInventoryEditable = true;
            else
                isInventoryEditable = false;

            Dictionary<string, bool> dcRoleDetails = new Dictionary<string, bool>();
            dcRoleDetails.Add("NotesViewable", isNotesViewable);
            dcRoleDetails.Add("NotesEditable", isNotesEditable);
            dcRoleDetails.Add("NotesDeletable", isNotesDeleteable);
            dcRoleDetails.Add("DocumentsViewable", isDocumentsViewable);
            dcRoleDetails.Add("DocumentsEditable", isDocumentsEditable);
            dcRoleDetails.Add("DocumentsDeletable", isDocumentsDeletable);
            dcRoleDetails.Add("InventoryViewable", isInventoryViewable);
            dcRoleDetails.Add("InventoryEditable", isInventoryEditable);
            return dcRoleDetails;
        }

        private List<SiteDocumentViewModel> GetSiteNoteDocumentDetails(string SiteId)
        {
            Dictionary<string, bool> dctRoleDetails = GetRoleDetails();
            IList<SiteDocumentViewModel> lstSiteDocumentViewModel = new List<SiteDocumentViewModel>();
            IList<SiteDocument> lstSiteDocuments = _siteDocumentService.GetSiteDocumentbySiteId(Convert.ToInt32(SiteId)).OrderByDescending(x => x.Date).ToList();
            if (lstSiteDocuments != null && lstSiteDocuments.Count() > 0)
            {
                lstSiteDocuments.ToList().ForEach(x => lstSiteDocumentViewModel.Add(new SiteDocumentViewModel
                {
                    DisplayDate = x.Date.ToString("MM/dd HH:mm tt"),
                    // FileName = x.FileName.Substring(0, 15) + "...",
                    FileName = x.FileName.Substring(x.FileName.LastIndexOf('\\') + 1, x.FileName.Length - (x.FileName.LastIndexOf('\\') + 22)) + ".pdf",
                    FileURL = x.FileName.Substring(0, x.FileName.LastIndexOf('.') - 17).Replace(@"\", @"\\"),
                    UserName = x.User.Username,
                    Id = x.Id,
                    IsPrimary = x.IsPrimary,
                    isDocumentsViewable = dctRoleDetails["DocumentsViewable"],
                    isDocumentsEditable = dctRoleDetails["DocumentsEditable"],
                    isDocumentsDeleteable = dctRoleDetails["DocumentsDeletable"]
                }));
            }
            else
            {
                SiteDocumentViewModel objSiteDocumentViewModel = new SiteDocumentViewModel();
                objSiteDocumentViewModel.Id = 0;
                objSiteDocumentViewModel.isDocumentsViewable = dctRoleDetails["DocumentsViewable"];
                objSiteDocumentViewModel.isDocumentsEditable = dctRoleDetails["DocumentsEditable"];
                objSiteDocumentViewModel.isDocumentsDeleteable = dctRoleDetails["DocumentsDeletable"];
                lstSiteDocumentViewModel.Add(objSiteDocumentViewModel);
            }
            return lstSiteDocumentViewModel.ToList();
        }

        private SiteInventory MapEntity(SiteInventoryViewModel item)
        {
            SiteInventory objSiteInventory = new SiteInventory();
            Site objSite = new Site();
            CompanyInventory objCompanyInventory = new CompanyInventory();
            objSiteInventory.InventoryValue = item.InventoryValue;
            objSite.Id = item.SiteId.Value;
            objSiteInventory.Site = objSite;
            objCompanyInventory.Id = item.InventoryKeyId.Value;
            objSiteInventory.CompanyInventory = objCompanyInventory;
            return objSiteInventory;
        }
    }
}
