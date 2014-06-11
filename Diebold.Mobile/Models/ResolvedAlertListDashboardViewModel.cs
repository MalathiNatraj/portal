using System;
using AutoMapper;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;

namespace DieboldMobile.Models
{
    public class ResolvedAlertListDashboardViewModel : BaseMappeableViewModel<ResolvedAlert>
    {
        static ResolvedAlertListDashboardViewModel()
        {
            Mapper.CreateMap<ResolvedAlert, ResolvedAlertListDashboardViewModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.AcknoledgeDate))
                .ForMember(dest => dest.Alert, opt => opt.MapFrom(src => src.AlarmConfiguration.AlarmType.Value.GetDescription()))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => string.Format("{0} {1}", src.User.FirstName, src.User.LastName)));
        }

        public ResolvedAlertListDashboardViewModel()
        {
        }

        public ResolvedAlertListDashboardViewModel(ResolvedAlert alert)
        {
            Mapper.Map(alert, this);
        }

        public DateTime Date { get; set; }

        public string Alert { get; set; }

        public string User { get; set; }
    }
}