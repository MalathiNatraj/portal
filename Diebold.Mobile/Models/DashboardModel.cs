using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DieboldMobile.Models
{
    public class DashboardModel
    {
        public List<DeviceListDashboardViewModel> AlertListDashboardViewModel { get; set; }
        public List<DeviceListDashboardViewModel> AlertListDashboardViewModelAccess { get; set; }
        public List<DeviceListDashboardViewModel> AlertListDashboardViewModelIntrusion { get; set; }
    }
}