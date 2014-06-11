using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.DTO
{
    public class AlarmDTO
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "capabilityType")]
        public string CapabilityType { get; set; }

        [JsonProperty(PropertyName = "relationalOperator")]
        public string RelationalOperator { get; set; }

        [JsonProperty(PropertyName = "threshold")]
        public object Threshold { get; set; }

        [JsonProperty(PropertyName = "callbackURL")]
        public string CallbackURL { get; set; }
    }
}
