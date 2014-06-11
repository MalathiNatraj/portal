using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.DTO
{
    public class AddedDeviceResponseDTO
    {
        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }
    }
}
