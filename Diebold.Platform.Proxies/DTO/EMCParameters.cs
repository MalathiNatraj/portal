using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class EMCParameters
    {
        public string emc_ip { get; set; }
        public string emc_port { get; set; }
        public string siteId { get; set; }
        public string alarmId { get; set; }        
        public string alarmType { get; set; }
        public string zone { get; set; }
        public string status { get; set; }
        public string data { get; set; }
    }
}
