using System.Web.Mvc;
using AutoMapper;
using Diebold.Domain.Entities;
using Diebold.Domain.Enums;
using Diebold.Services.Extensions;
using Diebold.WebApp.Infrastructure.Helpers;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.ComponentModel;

namespace Diebold.WebApp.Models
{
    public class DeviceListDashboardViewModel : BaseMappeableViewModel<AlertStatus>
    {

        static DeviceListDashboardViewModel()
        {
            Mapper.CreateMap<AlertStatus, DeviceListDashboardViewModel>()
                .ForMember(dest => dest.Ack, opt => opt.MapFrom(src => src.AckColor.ToString()))
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device.Name))
                .ForMember(dest => dest.AlertName, opt => opt.MapFrom(src => string.Format("{0}: {1} {2} ({3})", src.Device.Name,
                                                                     src.Alarm.AlarmType.Value.GetDescription(),
                                                                     AlarmHelper.GetAlertDescriptionForAlert((AlarmType)src.Alarm.AlarmType, src.ElementIdentifier, (Dvr)src.Device),
                                                                     src.AlertCount)))
                .ForMember(dest => dest.IsDeviceOk, opt => opt.MapFrom(src => src.IsOk));   
        }

        public DeviceListDashboardViewModel()
        {
        }

        public DeviceListDashboardViewModel(AlertStatus alert)
        {
            Mapper.Map(alert, this); 
        }

        [JqGridColumnFormatter("$.ackColumnFormatter")]
        [JqGridColumnSortable(false)]
        [JqGridColumnLabel(Label = "Ack")]
        [JqGridColumnLayout(Width = 40, Alignment = JqGridAlignments.Center)]
        public string Ack { get; set; }

        [JqGridColumnLabel(Label = "Alert")]
        [JqGridColumnLayout(Width = 200)]
        [JqGridColumnSortable(true, Index = "Device.Name, Alarm.AlarmType")]
        public string AlertName { get; set; }

        [HiddenInput]
        public int DeviceId { get; set; }
        [HiddenInput]
        public string DeviceName { get; set; }

        [JqGridColumnFormatter("$.currentStatusColumnFormatter")]
        [JqGridColumnSortable(true, Index = "IsOk")]
        [JqGridColumnLabel(Label = "Status")]
        [JqGridColumnLayout(Width = 60, Alignment = JqGridAlignments.Center)]
        public bool IsDeviceOk { get; set; }
        
        [JqGridColumnFormatter("$.actionColumnFormatter")]
        [JqGridColumnSortable(false)]
        [DisplayName("Actions")]
        [JqGridColumnLayout(Width = 80, Alignment = JqGridAlignments.Left)]
        public string ActionColumn { get; set; }

        [JqGridColumnLabel(Label = "Date Time")]
        [JqGridColumnLayout(Width = 200)]
       // [JqGridColumnSortable(true, Index = "Device.Name, Alarm.AlarmType")]
        public System.DateTime? LastAlert { get; set; }

        public string LastAlertS { get; set; }

        [JqGridColumnLabel(Label = "Status")]
        [JqGridColumnLayout(Width = 200)]
       // [JqGridColumnSortable(true, Index = "Device.Name, Alarm.AlarmType")]
        public string Status { get; set; }

        public string Location { get; set; }

        public string LocationAddress { get; set; }

        public string ContactPersonName { get; set; }

        public string ContactPersonEmail { get; set; }

        public string ContactPersonPhone { get; set; }

        public int AlarmConfigId { get; set; }

        public bool IsAcknowledged { get; set; }
        
    }
}