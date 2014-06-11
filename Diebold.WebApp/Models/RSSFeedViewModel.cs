using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Diebold.Domain.Entities;
using AutoMapper;
using System.ComponentModel;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Web.Mvc;
using Diebold.WebApp.Infrastructure.ClientValidators;

namespace Diebold.WebApp.Models
{
    public class RSSFeedViewModel : BaseMappeableViewModel<RSSFeed>
    {
        static RSSFeedViewModel()
        {
            Mapper.CreateMap<RSSFeed, RSSFeedViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
        }
        public RSSFeedViewModel(RSSFeed RssFeed)
        {
            Mapper.Map(RssFeed, this);
        }

        public RSSFeedViewModel()
        {
        }

        public string Name { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ActionColumn { get; set; }
        public int ParentId { get; set; }
        public int MaxRSSLinksCount { get; set; }
        public int RSSFeedCount { get; set; }
    }
}