using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.WebApp.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Services.Exceptions;
using System.Net;
using System.Xml;
using System.Data;
using System.IO;

namespace Diebold.WebApp.Controllers
{
    public class RssController : BaseController
    {
        private readonly IRSSFeedService _RssFeedService;
        private readonly ICurrentUserProvider _currentUserProvider;
        public RssController(
           ICurrentUserProvider currentUserProvider,
           IRSSFeedService RssFeedService
           )
        {
            _currentUserProvider = currentUserProvider;
            _RssFeedService = RssFeedService;
        }
        
        public ActionResult RSSEditing_Read([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                return Json(GetRssFeeds().ToDataSourceResult(request));
            }
            catch(Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        private List<RSSFeedViewModel> GetRssFeeds()
        {
            var RssFeed = _RssFeedService.GetAllActiveRSSFeedsByUser(_currentUserProvider.CurrentUser.Id).ToList().OrderByDescending(x => x.Id).ToList();
            return ProcessRSSItem(RssFeed);
        }


        public ActionResult RSSFeedRead_Popup()
        {
            try
            {
                List<RSSFeedViewModel> objlstRssFeedModel = new List<RSSFeedViewModel>();
                var RssFeed = _RssFeedService.GetAllActiveRSSFeedsByUser(_currentUserProvider.CurrentUser.Id).ToList().OrderByDescending(x => x.Id).ToList();
                RssFeed.ForEach(x =>
                {
                    objlstRssFeedModel.Add(new RSSFeedViewModel(x));
                });

                return Json(objlstRssFeedModel);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public int RssFeedCreate(string name, string url)
        {
            try
            {
                RSSFeedViewModel objRssFeedModel = new RSSFeedViewModel();
                RSSFeed objRssFeed = new RSSFeed();
                objRssFeed.Name = name;
                objRssFeed.Url = url;
                objRssFeed.User = _currentUserProvider.CurrentUser;
                var IsNameAlreadyAdded = _RssFeedService.GetAllRSSFeedsByUser(_currentUserProvider.CurrentUser.Id);
                if (!IsNameAlreadyAdded.Any(x => x.Name.Equals(name)))
                {
                    _RssFeedService.Create(objRssFeed);

                    List<RSSFeedViewModel> objlstRSSFeedModel = new List<RSSFeedViewModel>();

                    var RssFeed = _RssFeedService.GetAllActiveRSSFeedsByUser(_currentUserProvider.CurrentUser.Id).ToList().OrderByDescending(x => x.Id).ToList();
                    RssFeed.ForEach(x =>
                    {
                        objlstRSSFeedModel.Add(new RSSFeedViewModel(x));
                    });
                    var RssFeedCount = objlstRSSFeedModel.Count;
                    return RssFeedCount;
                }
                else
                {
                    int returnValue = 0;
                    if (IsNameAlreadyAdded.Any(x => x.Name.Equals(name)))
                    {
                        ModelState.AddModelError("DuplicateEntry", "Link Name already Added");
                        returnValue = -1;
                    }
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return 0;
            }
        }

        public int GetRSSFeedCount()
        {
            List<RSSFeedViewModel> objlstRSSFeedModel = new List<RSSFeedViewModel>();

            var RssFeed = _RssFeedService.GetAllActiveRSSFeedsByUser(_currentUserProvider.CurrentUser.Id).ToList().OrderByDescending(x => x.Id).ToList();
            RssFeed.ForEach(x =>
            {
                objlstRSSFeedModel.Add(new RSSFeedViewModel(x));
            });
            var RssFeedCount = objlstRSSFeedModel.Count;
            return RssFeedCount;
        }

        public ActionResult GetRSSFeed()
        {
            try
            {
                List<RSSFeedViewModel> objlstRSSFeedModel = new List<RSSFeedViewModel>();
                var RssFeed = _RssFeedService.GetAllActiveRSSFeedsByUser(_currentUserProvider.CurrentUser.Id).ToList().OrderByDescending(x => x.Id).ToList();
                RssFeed.ForEach(x =>
                {
                    objlstRSSFeedModel.Add(new RSSFeedViewModel(x));
                });
                return Json(objlstRSSFeedModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult GetRSSFeedDetails()
        {
            try
            {
                List<RSSFeedViewModel> objlstRSSFeedModel = new List<RSSFeedViewModel>();
                var RssFeed = _RssFeedService.GetAllActiveRSSFeedsByUser(_currentUserProvider.CurrentUser.Id).ToList().OrderByDescending(x => x.Id).ToList();
                RssFeed.ForEach(x =>
                {
                    objlstRSSFeedModel.Add(new RSSFeedViewModel(x));
                });

                objlstRSSFeedModel = ProcessRSSItem(RssFeed);
                return Json(objlstRSSFeedModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return JsonError(ex.Message);
            }
        }

        public ActionResult DeleteRssFeed(int RssFeedId)
        {
            try
            {
                _RssFeedService.Delete(RssFeedId);
                List<RSSFeedViewModel> objlstRssFeedModel = new List<RSSFeedViewModel>();
                var RSSFeed = _RssFeedService.GetAllActiveRSSFeedsByUser(_currentUserProvider.CurrentUser.Id).ToList().OrderByDescending(x => x.Id).ToList();
                RSSFeed.ForEach(x =>
                {
                    objlstRssFeedModel.Add(new RSSFeedViewModel(x));
                });

                objlstRssFeedModel = ProcessRSSItem(RSSFeed);
                return Json(objlstRssFeedModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        public List<RSSFeedViewModel> ProcessRSSItem(List<RSSFeed> objlstRssFeedModel)
        {
            
            List<RSSFeedViewModel> objlstFinalRssFeedModel = new List<RSSFeedViewModel>();

            // To get the maximum number of RSS Feeds links
            // MaxRssLinksCount is added in model which is used for javascript validation (user restricted to add maximum number of links)
            int MaxRssLinksCount = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["MaxRSSFeedLinks"]);
            int lstRssCountCheck = 0;
            int lstCount = objlstRssFeedModel.Count;
            // To get the feeds upto maxCountcheck 
            int maxCountcheck = MaxRssLinksCount;
            if (lstCount > MaxRssLinksCount)
            {
                objlstRssFeedModel = objlstRssFeedModel.Take(MaxRssLinksCount).ToList();
            }

            foreach (var rssFeedItems in objlstRssFeedModel)
            {
                lstRssCountCheck = lstRssCountCheck + 1;

                var rssURL = rssFeedItems.Url;
                System.Net.WebRequest myRequest = System.Net.WebRequest.Create(rssURL);
                System.Net.WebResponse myResponse = myRequest.GetResponse();
                System.IO.Stream rssStream = myResponse.GetResponseStream();
                System.Xml.XmlDocument rssDoc = new System.Xml.XmlDocument();
                rssDoc.Load(rssStream);

                System.Xml.XmlNodeList rssItems = rssDoc.SelectNodes("rss/channel/item");

                string title = string.Empty;
                string link = string.Empty;
                string pubDate = string.Empty;
                RSSFeedViewModel objchildRSSFeed = null;

                // To limit the maximum number of RSS Feeds to 2
                int MaxRssItemCount =Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["MaxRSSFeedCount"]);

                // To get the RSS Feeds original count
                int rssItemCount = rssItems.Count;
                if (lstRssCountCheck <= 5)
                {
                    //Feed count will be 2 for 6th to 10th items
                    if (rssItemCount > 3)
                    {
                        rssItemCount = 3;
                    }

                }
                else if (lstRssCountCheck > 5 && lstRssCountCheck <= 10 )
                {
                    //Feed count will be 2 for 6th to 10th items
                    if (rssItemCount > MaxRssItemCount)
                    {
                        rssItemCount = MaxRssItemCount;
                    }
                }

                for (int i = 0; i < rssItemCount; i++)
                {
                    objchildRSSFeed = new RSSFeedViewModel();
                    objchildRSSFeed.Id = i+1;
                    objchildRSSFeed.ParentId = rssFeedItems.Id;
                    objchildRSSFeed.URL = rssFeedItems.Url;
                    objchildRSSFeed.UserId = rssFeedItems.User.Id;
                    objchildRSSFeed.MaxRSSLinksCount = MaxRssLinksCount;

                    title = string.Empty;
                    link = string.Empty;
                    pubDate = string.Empty;

                    System.Xml.XmlNode rssFeed;

                    rssFeed = rssItems.Item(i).SelectSingleNode("title");
                    if (rssFeed != null)
                    {
                        title = rssFeed.InnerText;
                    }

                    rssFeed = rssItems.Item(i).SelectSingleNode("link");
                    if (rssFeed != null)
                    {
                        link = rssFeed.InnerText;
                    }

                    rssFeed = rssItems.Item(i).SelectSingleNode("pubDate");
                    if (rssFeed != null)
                    {
                        pubDate = rssFeed.InnerText;                       
                    }

                    objchildRSSFeed.Description = "<p><b><a style='color:blue;font-family:Helvetica;font-size:11px;' href='" + link + "' target='new'>" + title + "</a></b></p><p style='color:Gray;font-family:Helvetica;font-size:11px;'><strong>" + pubDate + "</strong></p>";
                    objlstFinalRssFeedModel.Add(objchildRSSFeed);                    
                }
            }

            return objlstFinalRssFeedModel;
        }       
    }
}
