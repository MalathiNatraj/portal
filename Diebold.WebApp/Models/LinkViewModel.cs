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
    public class LinkViewModel : BaseMappeableViewModel<Link>
    {
        private static readonly String maxLinkConfigValue = System.Web.Configuration.WebConfigurationManager.AppSettings["MaxLinksCount"];
        
        static LinkViewModel()
        {
            Mapper.CreateMap<Link, LinkViewModel>()
                .ForMember(dest => dest.LinkName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LinkURL, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.UserId , opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.UserName , opt => opt.MapFrom(src => src.User.Username));
        }

        public LinkViewModel(Link link)
        {
            Mapper.Map(link, this); 
        }

        public LinkViewModel()
        {
        }

        [JqGridColumnSortable(true, Index = "Name")]
        [JqGridColumnLabel(Label = "Name")]
        [DisplayName("Name: (*)")]
        [StringLength(32)]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9.!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]+(( )+[a-zA-z0-9.!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]+)*$", ErrorMessage = "Please enter a valid link name")]
        public string LinkName { get; set; }

        [JqGridColumnSortable(true, Index = "URL")]
        [JqGridColumnLabel(Label = "URL")]
        [DisplayName("URL: (*)")]
        [StringLength(32)]
        [Required]        
        public string LinkURL { get; set; }

        [JqGridColumnSortable(true, Index = "Description")]
        [JqGridColumnLabel(Label = "Description")]
        [DisplayName("Description: ")]
        //[StringLength(32)]        
        public string Description { get; set; }


        [JqGridColumnSortable(true, Index = "User.Id")]
        [JqGridColumnLabel(Label = "User ID")]
        [DisplayName("User ID:")]
        public int UserId { get; set; }

        [JqGridColumnSortable(true, Index = "User.FirstName")]
        [JqGridColumnLabel(Label = "User Name")]
        //[StringLength(32)]
        [DisplayName("User Name:")]
        public string UserName { get; set; }

        [JqGridColumnFormatter("$.actionColumnFormatter")]
        [JqGridColumnSortable(false)]
        [DisplayName("Actions")]
        [JqGridColumnLayout(Width = 50)]
        public string ActionColumn { get; set; }

        public int MaxLinksCount
        {
            get { return Convert.ToInt32(maxLinkConfigValue); }
        }
    }
}