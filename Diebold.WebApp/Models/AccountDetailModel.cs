using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class AccountDetailModel
    {        
        public String CompanyName { get; set; }
        public int SiteCount { get; set; }
        public int DeviceCount { get; set; }
        public int IntrusionDevices { get; set; }
        public int AccessDevices { get; set; }
        public int HealthDevices { get; set; }        
        public int VideoDevices { get; set; }
        public string CompanyLogo { get; set; }
    }
}
