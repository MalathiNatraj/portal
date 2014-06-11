using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.DTO
{
    public class ConfigurationTypeDTO
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
