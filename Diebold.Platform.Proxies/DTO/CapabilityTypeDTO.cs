using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.DTO
{
    public class CapabilityTypeDTO
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "dataType")]
        public string DataType { get; set; }

        [JsonProperty(PropertyName = "collection")]
        public bool IsCollection { get; set; }
    }
}
