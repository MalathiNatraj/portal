using AutoMapper;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;

namespace Diebold.WebApp.Models
{
    public class AlertDetailViewModel : BaseMappeableViewModel<AlertStatus>
    {
        static AlertDetailViewModel()
        {
            Mapper.CreateMap<AlertStatus, AlertDetailViewModel>()
                 .ForMember(dest => dest.AlertName, opt => opt.MapFrom(src => src.Alarm.AlarmType.Value.GetDescription()))
                 .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Device.IsDvr ? ((Dvr) src.Device).Site.Address1 : string.Empty))
                 .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Device.IsDvr ? ((Dvr)src.Device).Site.City + ", " + ((Dvr)src.Device).Site.State : string.Empty))
                 
                 .ForMember(dest => dest.DeviceIpHostname, opt => opt.MapFrom(src => src.Device.IsDvr ? ((Dvr) src.Device).HostName : null ))
                 .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Device.IsDvr ? ((Dvr)src.Device).Site.Id : 0 ))
                 
                 .ForMember(dest => dest.Recorded, opt => opt.MapFrom(src => src.FirstAlertTimeStamp.HasValue ?  src.FirstAlertTimeStamp.Value.GetTimeAgo() : string.Empty))
                 .ForMember(dest => dest.Unattended, opt => opt.MapFrom(src => src.FirstAlertTimeStamp.HasValue ? src.FirstAlertTimeStamp.Value.GetTimeAgo() : string.Empty))
                 
                 .ForMember(dest => dest.GatewayName, opt => opt.MapFrom(src => src.Device.IsDvr ? ((Dvr)src.Device).Name : null))
                 .ForMember(dest => dest.SiteName, opt => opt.MapFrom(src => src.Device.IsDvr ? ((Dvr)src.Device).Site.Name : null))

                 .ForMember(dest => dest.Operator, opt => opt.MapFrom(src => src.Alarm.Operator))
                 .ForMember(dest => dest.Threshold, opt => opt.MapFrom(src => src.Alarm.Threshold))
                 .ForMember(dest => dest.DataType, opt => opt.MapFrom(src => src.Alarm.DataType));

            Mapper.CreateMap<AlertDetailViewModel, AlertStatus>();
        }

        public AlertDetailViewModel()
        {
        }

        public AlertDetailViewModel(AlertStatus alert)
        {
            Mapper.Map(alert, this); 
        }

        public string SiteName { get; set; }

        public string GatewayName { get; set; }
        
        public string Address { get; set; }

        public string State { get; set; }

        public string MonitoredDevicesCount { get; set; }

        public int SiteId { get; set; }

        public string MonitoredDevicesAlarmsCount { get; set; }

        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string AlertName { get; set; }

        public string DeviceIpHostname { get; set; }

        public string Recorded { get; set; }

        public string Unattended { get; set; }

        public virtual AlarmOperator Operator { get; set; }

        public virtual string Threshold { get; set; }

        public virtual DataType DataType { get; set; }

        public virtual string AlertDescription {
            get
            {
                if (AlertName == string.Empty) return string.Empty;

                if (DataType == DataType.Boolean)
                {
                    return AlertName;
                }
                
                var text = AlertName;
                text += " " + Operator.GetDescription().SeparatedCamelCase();
                text += " " + Threshold;

                return text;
            }
        }

    }
}