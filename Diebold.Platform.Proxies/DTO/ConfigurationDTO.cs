using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.DTO
{
    public class ConfigurationDTO
    {
        public ConfigurationDTO()
        {
            ReportBufferSize = "1";
        }

        [JsonProperty(PropertyName = "ip")]
        public string IP { get; set; }

        [JsonProperty(PropertyName = "portA")]
        public string PortA { get; set; }

        [JsonProperty(PropertyName = "portB")]
        public string PortB { get; set; }

        [JsonProperty(PropertyName = "timezone")]
        public string TimeZone { get; set; }

        [JsonProperty(PropertyName = "dst")]
        public string DaySavingTime { get; set; }

        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "reportBufferSize")]
        public string ReportBufferSize { get; set; }
    }
}
