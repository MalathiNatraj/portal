using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Diebold.Domain.Entities;

namespace Diebold.WebApp.Models
{
    public class DeviceDiagnosticViewModel : BaseMappeableViewModel<Dvr>
    {
        static DeviceDiagnosticViewModel()
        {
            Mapper.CreateMap<Dvr, DeviceDiagnosticViewModel>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.CurrentAlerts, opt => opt.MapFrom(src => ""))
                .ForMember(dest => dest.GatewayName, opt => opt.MapFrom(src => src.Gateway.Name))
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LastReceived, opt => opt.MapFrom(src => GetLastReceivedAlertForDevice(src))) //TODO: put method in helper
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Site.SiteId.ToString()));
        }

        public DeviceDiagnosticViewModel()
        {
        }

        public DeviceDiagnosticViewModel(Dvr device)
        {
            Mapper.Map(device, this); 
        }

        [JqGridColumnLabel(Label = "Device Name")]
        public string DeviceName { get; set; }

        [JqGridColumnLabel(Label = "Company Name")]
        public string CompanyName { get; set; }

        [JqGridColumnLabel(Label = "Site ID")]
        public string SiteId { get; set; }

        [JqGridColumnLabel(Label = "Gateway")]
        public string GatewayName { get; set; }

        [HiddenInput]
        public string DeviceType { get; set; }

        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.Date, OutputFormat = "m/d g:i A")]
        [JqGridColumnLabel(Label = "Last Received")]
        public DateTime? LastReceived { get; set; }

        [JqGridColumnLabel(Label = "Current Alerts")]
        public string CurrentAlerts { get; set; }

        [JqGridColumnFormatter("$.deviceActionColumnFormatter")]
        [JqGridColumnSortable(false)]
        [JqGridColumnLayout(Width = 200)]
        [DisplayName("Actions")]
        public string ActionColumn { get; set; }
        
        private static DateTime? GetLastReceivedAlertForDevice(Device device)
        {
            return device.AlertStatus.Count > 0 ?
               device.AlertStatus.OrderByDescending(a => a.LastAlertTimeStamp).First().LastAlertTimeStamp :
               null;
        }
    }
}