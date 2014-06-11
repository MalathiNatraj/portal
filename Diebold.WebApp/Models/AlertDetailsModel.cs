using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class AlertDetailsModel
    {
        public String SiteName { get; set; }
        public String GatewayName { get; set; }
        public String Address { get; set; }
        public String State { get; set; }
        public String MonitoredDevicesCount { get; set; }
        public Int32 SiteId { get; set; }
        public String MonitoredDevicesAlarmsCount { get; set; }
        public Int32 DeviceId { get; set; }
        public String DeviceName { get; set; }
        public String Alert { get; set; }
        public String DeviceIpHostname { get; set; }
        public String Recorded { get; set; }
        public String Unattended { get; set; }
        public String Threshold { get; set; }
        
    }
}