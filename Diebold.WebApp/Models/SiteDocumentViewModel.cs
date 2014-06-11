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
    public class SiteDocumentViewModel : BaseMappeableViewModel<SiteDocument>
    {
        static SiteDocumentViewModel()
        {
            Mapper.CreateMap<SiteDocument, SiteDocumentViewModel>()
                .ForMember(dest => dest.SiteId , opt => opt.MapFrom(src => src.Site.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserId , opt => opt.MapFrom(src => src.User.Id));

            Mapper.CreateMap<SiteNoteViewModel, SiteNote>()
              .ForMember(dest => dest.User, opt => opt.MapFrom(src => new Gateway { Id = src.UserId }))
              .ForMember(dest => dest.Site, opt => opt.MapFrom(src => new Company { Id = src.SiteId }));
        }

        public SiteDocumentViewModel(SiteDocument SiteDocument)
        {
            Mapper.Map(SiteDocument, this); 
        }

        public SiteDocumentViewModel()
        {
        }

        public string FileName { get; set; }
        public string FileURL { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; }
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public string UserName { get; set; }
        public bool IsPrimary { get; set; }
        public bool isDocumentsViewable { get; set; }
        public bool isDocumentsEditable { get; set; }
        public bool isDocumentsDeleteable { get; set; }
    }
}