using System.Web.Mvc;
using AutoMapper;
using Diebold.Domain.Entities;
using Diebold.Domain.Enums;
using Diebold.Services.Extensions;
using DieboldMobile.Infrastructure.Helpers;
using System.ComponentModel;

namespace DieboldMobile.Models
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

        public string Ack { get; set; }

        public string AlertName { get; set; }

        [HiddenInput]
        public int DeviceId { get; set; }
        [HiddenInput]
        public string DeviceName { get; set; }

        public bool IsDeviceOk { get; set; }

        [DisplayName("Actions")]
        public string ActionColumn { get; set; }

        public System.DateTime? LastAlert { get; set; }

        public string LastAlertS { get; set; }

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