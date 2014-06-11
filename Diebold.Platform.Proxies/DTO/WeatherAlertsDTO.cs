using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class WeatherAlertsDTO : BaseResponseDTO
    {
        public WeatherAlert[] alerts { get; set; }
    }

    public class WeatherAlert
    {
        public string  type { get; set; }
        public string description { get; set; }
        public string date { get; set; }
        public string date_epoch { get; set; }
        public string expires { get; set; }
        public string expires_epoch { get; set; }
        public string message { get; set; }
        public string phenomena { get; set; }
        public string significance { get; set; }
        public Zones[] ZONES { get; set; }
        public StormBased stormbased { get; set; }
    }

    public class Zones
    {
        public string state { get; set; }
        public string ZONE { get; set; }
    }

    public class StormBased
    {
 
    }
}
