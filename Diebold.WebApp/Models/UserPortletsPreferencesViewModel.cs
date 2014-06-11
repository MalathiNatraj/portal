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
    public class UserPortletsPreferencesViewModel : BaseMappeableViewModel<UserPortletsPreferences>
    {
        static UserPortletsPreferencesViewModel()
        {
            Mapper.CreateMap<UserPortletsPreferences, UserPortletsPreferencesViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.PortletId, opt => opt.MapFrom(src => src.Portlets.Id))
                .ForMember(dest => dest.SeqNo, opt => opt.MapFrom(src => src.SeqNo))
                .ForMember(dest => dest.ColumnNo, opt => opt.MapFrom(src => src.ColumnNo));
        }

        public UserPortletsPreferencesViewModel(UserPortletsPreferences userPortletsPreferences)
        {
            Mapper.Map(userPortletsPreferences, this); 
        }

        public UserPortletsPreferencesViewModel()
        {
        }

        [DisplayName("User ID:")]
        [Required]
        public int UserId { get; set; }

        [DisplayName("Portlet ID:")]
        [Required]
        public int PortletId { get; set; }

        [DisplayName("SeqNo: (*)")]
        [Required]
        public int SeqNo { get; set; }

        [DisplayName("ColumnNo: (*)")]
        [Required]
        public int ColumnNo { get; set; }

        [DisplayName("Show: (*)")]
        [Required]
        public bool Show { get; set; }

        public User user { get; set; }

        // public Portlets portlets { get; set; }

        public string PortletName { get; set; }

        public string PortletInternalName { get; set; }

        public DashboardModel DashboardDetails { get; set; }
    }
}