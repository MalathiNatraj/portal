using System;
using System.ComponentModel;
using AutoMapper;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Diebold.Domain.Entities;

namespace Diebold.WebApp.Models
{
    public class GatewayDiagnosticViewModel : BaseMappeableViewModel<Device>
    {
        private Gateway gateway;


        static GatewayDiagnosticViewModel()
        {
            Mapper.CreateMap<Gateway, GatewayDiagnosticViewModel>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.GatewayName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.MacAddress, opt => opt.MapFrom(src => src.MacAddress))
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate));
        }

        public GatewayDiagnosticViewModel()
        {
        }

        public GatewayDiagnosticViewModel(Gateway gateway)
        {
            Mapper.Map(gateway, this);
        }

        [JqGridColumnLabel(Label = "Gateway Name")]
        public string GatewayName { get; set; }

        [JqGridColumnLabel(Label = "Company Name")]
        public string CompanyName { get; set; }

        [JqGridColumnLabel(Label = "Site ID")]
        public string SiteId { get; set; }

        [JqGridColumnLabel(Label = "MAC Address")]
        public string MacAddress { get; set; }

        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.Date, OutputFormat = "m/d g:i A")]
        [JqGridColumnLabel(Label = "Last Update")]
        public DateTime? LastUpdate { get; set; }
        
        [JqGridColumnFormatter("$.gatewayActionColumnFormatter")]
        [JqGridColumnSortable(false)]
        [DisplayName("Actions")]
        public string ActionColumn { get; set; }
    }
}