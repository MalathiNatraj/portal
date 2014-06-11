using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class IntrusionDTO
    {
        public string ExternalDeviceKey { get; set; }
        public string DeviceInstanceId { get; set; }
        public string DeviceType { get; set; }
        public string AreaNumber { get; set; }
        public string StartDateTime { get; set; }
        public string StartEndTime { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string PinCode { get; set; }
        public string ProfileNumber { get; set; }
        public string TempDate { get; set; }
        public string ZoneNumber { get; set; }
        public string UserNumber { get; set; }
        public Dictionary<string, string> AccessLevels { get; set; } 
    }
}
