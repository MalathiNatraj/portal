using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.DTO
{
    public class ActionTypeDTO
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
