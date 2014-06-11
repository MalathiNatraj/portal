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
    public class CompanyInventoryViewModel : BaseMappeableViewModel<CompanyInventory>
    {
         static CompanyInventoryViewModel()
        {
            Mapper.CreateMap<CompanyInventory, CompanyInventoryViewModel>()
                .ForMember(dest => dest.InventoryKey, opt => opt.MapFrom(src => src.InventoryKey));
        }

        public CompanyInventoryViewModel(SiteDocument SiteDocument)
        {
            Mapper.Map(SiteDocument, this); 
        }

        public CompanyInventoryViewModel()
        {
        }

        public string InventoryKey { get; set; }
        public int ExternalCompanyId { get; set; }
        public bool isInventoryViewable { get; set; }
        public bool isInventoryEditable { get; set; }
        public bool isInventoryDeleteable { get; set; }
    }
}