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
    public class SiteAccNumberViewModel : BaseMappeableViewModel<SiteAccountNumber>
    {
        static SiteAccNumberViewModel()
        {
            Mapper.CreateMap<SiteAccountNumber, SiteAccNumberViewModel>()
                //.ForMember(dest => dest.SiteId , opt => opt.MapFrom(src => src.Site.Id))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.siteId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserId , opt => opt.MapFrom(src => src.User.Id));

            Mapper.CreateMap<SiteAccNumberViewModel, SiteAccountNumber>()
              .ForMember(dest => dest.User, opt => opt.MapFrom(src => new Gateway { Id = src.UserId }))
              //.ForMember(dest => dest.Site, opt => opt.MapFrom(src => new Company { Id = src.SiteId }));
              .ForMember(dest => dest.siteId, opt => opt.MapFrom(src => new Company { Id = src.SiteId }));
        }
        public SiteAccNumberViewModel(SiteAccountNumber SiteAccNum)
        {
            Mapper.Map(SiteAccNum, this); 
        }

        public SiteAccNumberViewModel()
        {
        }

        public string AccountNumber { get; set; }
        public bool IsAssociatedWithFA { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; }
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }        
    }
}