using System;
using System.Globalization;
using System.Web.Mvc;
using AutoMapper;
using Diebold.Domain.Entities;
using Diebold.Domain.Enums;
using Diebold.Services.Extensions;
using DieboldMobile.Infrastructure.Helpers;
using System.ComponentModel;

namespace DieboldMobile.Models
{
    public class AlertListDashboardViewModel : BaseMappeableViewModel<AlertStatus>
    {

        static AlertListDashboardViewModel()
        {
            Mapper.CreateMap<AlertStatus, AlertListDashboardViewModel>()
                .ForMember(dest => dest.Ack, opt => opt.MapFrom(src => src.AckColor.ToString()))
                .ForMember(dest => dest.FirstOccur, opt => opt.MapFrom(src => src.FirstAlertTimeStamp))
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device.Name))
                .ForMember(dest => dest.AlertName, opt => opt.MapFrom(src => string.Format("{0}: {1} {2} ({3})", src.Device.Name,
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

        public string Ack { get; set; }

        public string AlertName { get; set; }
        [HiddenInput]
        public string DeviceName { get; set; }

        [HiddenInput]
        public int DeviceId { get; set; }

        public DateTime? FirstOccur { get; set; }

        public bool IsDeviceOk { get; set; }

        [DisplayName("Actions")]
        public string ActionColumn { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Acknowledgment { get; set; }
        public string Occur { get; set; }
        public string Status { get; set; }

    }
}