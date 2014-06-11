using System;
using System.Globalization;
using System.Web.Mvc;
using AutoMapper;
using Diebold.Domain.Entities;
using Diebold.Domain.Enums;
using Diebold.Services.Extensions;
using Diebold.WebApp.Infrastructure.Helpers;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.ComponentModel;

namespace Diebold.WebApp.Models
{
    public class AlertListDashboardViewModel : BaseMappeableViewModel<AlertStatus>
    {

        static AlertListDashboardViewModel()
        {
            Mapper.CreateMap<AlertStatus, AlertListDashboardViewModel>()
                .ForMember(dest => dest.Ack, opt => opt.MapFrom(src => src.AckColor.ToString()))
                .ForMember(dest => dest.FirstOccur, opt => opt.MapFrom(src => src.FirstAlertTimeStamp))
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device.Name))
                .ForMember(dest => dest.AlertName, opt => opt.MapFrom( src => string.Format("{0}: {1} {2} ({3})", src.Device.Name,
                                                                     src.Alarm.AlarmType.Value.GetDescription(),
                                                                     AlarmHelper.GetAlertDescriptionForAlert((AlarmType)src.Alarm.AlarmType, src.ElementIdentifier, (Dvr)src.Device),
                                                                     src.AlertCount)))

                .ForMember(dest => dest.IsDeviceOk, opt => opt.MapFrom(src => src.IsOk));
        }

        public AlertListDashboardViewModel()
        {
        }

        public AlertListDashboardViewModel(AlertStatus alert)
        {
            Mapper.Map(alert, this); 
        }

        [JqGridColumnFormatter("$.ackColumnFormatter")]
        [JqGridColumnSortable(true, Index = "IsAcknowledged")]
        [JqGridColumnLabel(Label = "Ack")]
        [JqGridColumnLayout(Width = 40, Alignment = JqGridAlignments.Center)]
        public string Ack { get; set; }

        [JqGridColumnLabel(Label = "Alert")]
        [JqGridColumnLayout(Width = 360)]
        [JqGridColumnSortable(true, Index = "Device.Name, Alarm.AlarmType")]
        public string AlertName { get; set; }
        [HiddenInput]
        public string DeviceName { get; set; }

        [HiddenInput]
        public int DeviceId { get; set; }

        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.Date, OutputFormat = "m/d g:i A")] // 28/03/2012 03:22:30 p.m. -> 3/28 3:22PM
        [JqGridColumnLabel(Label = "Date")]
        [JqGridColumnSortable(true, Index = "FirstAlertTimeStamp")]
        [JqGridColumnLayout(Width = 120, Alignment = JqGridAlignments.Center)]
        public DateTime? FirstOccur { get; set; }

        [JqGridColumnFormatter("$.currentStatusColumnFormatter")]
        [JqGridColumnLabel(Label = "Status")]
        [JqGridColumnSortable(true, Index = "IsOk")]
        [JqGridColumnLayout(Width = 60, Alignment = JqGridAlignments.Center)]
        public bool IsDeviceOk { get; set; }
        
        [JqGridColumnFormatter("$.actionColumnFormatter")]
        [JqGridColumnSortable(false)]
        [DisplayName("Actions")]
        [JqGridColumnLayout(Width = 80, Alignment = JqGridAlignments.Left)]
        public string ActionColumn { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Acknowledgment { get; set; }
        public string Occur { get; set; }
        public string Status { get; set; }
        
    }
}