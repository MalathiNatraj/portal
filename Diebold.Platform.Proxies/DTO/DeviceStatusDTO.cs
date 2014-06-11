using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.DTO
{
    public class DeviceStatusDTO
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public object Value { get; set; }

        [JsonProperty(PropertyName="collection")]
        public bool IsCollection { get; set; }
    }
}