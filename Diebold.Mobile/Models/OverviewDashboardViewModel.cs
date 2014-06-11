using System.ComponentModel;
using AutoMapper;
using Diebold.Domain.Entities;

namespace DieboldMobile.Models
{
    public class OverviewDashboardViewModel : BaseMappeableViewModel<AlertInfo>
    {
        static OverviewDashboardViewModel()
        {
            Mapper.CreateMap<AlertInfo, OverviewDashboardViewModel>();
        }

        public OverviewDashboardViewModel()
        {
        }

        public OverviewDashboardViewModel(AlertInfo alert)
        {
            Mapper.Map(alert, this);
        }

        [DisplayName("Sites")]
        public int SiteCount { get; set; }

        [DisplayName("Gateways")]
        public int GatewayCount { get; set; }

        [DisplayName("Devices")]
        public int DeviceCount { get; set; }

        public int DevicesOnRedCount { get; set; }
        public int DevicesOnGreenCount { get; set; }

        [DisplayName("Last Alert")]
        public string LastAlert { get; set; }
    }
}