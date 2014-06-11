using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Diebold.WebApp.Models;
using Kendo.Mvc.Extensions;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Services.Exceptions;

namespace Diebold.WebApp.Controllers
{
    public class LinksController : BaseController
    {
        //
        // GET: /Links/
        private readonly IUserService _userService;
        private readonly ILinkService _linkService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public LinksController(

            IUserService userService,
            ICurrentUserProvider currentUserProvider,
            ILinkService linkService

            )
        {
            _userService = userService;
            _currentUserProvider = currentUserProvider;
            _linkService = linkService;
        }

        public ActionResult Index()
        {
            return View();
        }
        // GET: /Intrusion/

        public ActionResult Editing_Read([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                List<LinkViewModel> objLinksModel = GetLinks();
                return Json(objLinksModel.ToDataSourceResult(request));
            }
            catch(Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult PopulateLinks()
        {
            return Json(GetLinks());
        }

        private List<LinkViewModel> GetLinks()
        {
            List<LinkViewModel> objLinksModel = new List<LinkViewModel>();
            // Take only 10 items and display as per Greg mail on 21 Feb
            var Links = _linkService.GetAllActiveLinksByUser(_currentUserProvider.CurrentUser.Id).ToList().Take(10).ToList();
            Links.ForEach(x =>
            {
                objLinksModel.Add(new LinkViewModel(x));
            });
            return objLinksModel;
        }

        public int AddNewLink(string linkName, string url)
        {
            LinkViewModel objLinksModel = new LinkViewModel();
            Link objLink = new Link();
            objLink.Name = linkName;
            objLink.Url = url;
           
            objLink.User = _currentUserProvider.CurrentUser;

            var IsNameAlreadyAdded = _linkService.GetAllActiveLinksByUser(_currentUserProvider.CurrentUser.Id);

            if ((!IsNameAlreadyAdded.Any(x => x.Name.Equals(linkName))) && (!IsNameAlreadyAdded.Any(x => x.Url.Equals(url))))
            {
                _linkService.Create(objLink);

                List<LinkViewModel> objlstLinksModel = new List<LinkViewModel>();

                var Links = _linkService.GetAllActiveLinksByUser(_currentUserProvider.CurrentUser.Id).ToList();
                Links.ForEach(x =>
                {
                    objlstLinksModel.Add(new LinkViewModel(x));
                });

                var linksCount = objlstLinksModel.Count;
                return linksCount;                
            }
            else
            {
                int returnValue = 0;
                if (IsNameAlreadyAdded.Any(x => x.Name.Equals(linkName)))
                {
                    ModelState.AddModelError("DuplicateEntry", "Link Name already Added");
                    returnValue = -1;
                }
                else if (IsNameAlreadyAdded.Any(x => x.Url.Equals(url)))
                {
                    ModelState.AddModelError("DuplicateEntry", "Link URL already Added");                    
                    returnValue = -2;
                }
                return returnValue;
            }            
        }

        public ActionResult DeleteLinks(int linkID)
        {
           

            _linkService.Delete(linkID);

            List<LinkViewModel> objlstLinksModel = new List<LinkViewModel>();

            var Links = _linkService.GetAllActiveLinksByUser(_currentUserProvider.CurrentUser.Id).ToList().ToList(); 
            Links.ForEach(x =>
            {
                objlstLinksModel.Add(new LinkViewModel(x));
            });

            return Json(objlstLinksModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActiveLinksByUser()
        {
            List<LinkViewModel> objlstLinksModel = new List<LinkViewModel>();

            var Links = _linkService.GetAllActiveLinksByUser(_currentUserProvider.CurrentUser.Id).ToList().ToList();
            Links.ForEach(x =>
            {
                objlstLinksModel.Add(new LinkViewModel(x));
            });

            return Json(objlstLinksModel, JsonRequestBehavior.AllowGet);
        }
    }
}

