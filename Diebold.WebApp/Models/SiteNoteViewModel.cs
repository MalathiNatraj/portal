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
    public class SiteNoteViewModel : BaseMappeableViewModel<SiteNote>
    {
        static SiteNoteViewModel()
        {
            Mapper.CreateMap<SiteNote, SiteNoteViewModel>()
                .ForMember(dest => dest.SiteId , opt => opt.MapFrom(src => src.Site.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserId , opt => opt.MapFrom(src => src.User.Id));

            Mapper.CreateMap<SiteNoteViewModel, SiteNote>()
              .ForMember(dest => dest.User, opt => opt.MapFrom(src => new Gateway { Id = src.UserId }))
              .ForMember(dest => dest.Site, opt => opt.MapFrom(src => new Company { Id = src.SiteId }));
        }
        public SiteNoteViewModel(SiteNote SiteNote)
        {
            Mapper.Map(SiteNote, this); 
        }

        public SiteNoteViewModel()
        {
        }

        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; }
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public string UserName { get; set; }
        public bool isNotesViewable { get; set; }
        public bool isNotesEditable { get; set; }
        public bool isNotesDeleteable { get; set; }
    }
}