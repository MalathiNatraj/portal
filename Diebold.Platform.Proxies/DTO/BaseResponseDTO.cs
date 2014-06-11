using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class BaseResponseDTO
    {
        public string device_instance_id { get; set; }
        public string device_datetime { get; set; }
        public string device_type { get; set; }
        public string raw_data { get; set; }
    }
}
