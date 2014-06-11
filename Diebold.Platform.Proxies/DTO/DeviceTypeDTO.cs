using System.Collections.Generic;
using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.DTO
{
    public class DeviceTypeDTO
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "unassignedSlotRequired")]
        public string UnassignedSlotRequired { get; set; }

        [JsonProperty(PropertyName = "deviceChildrenRestriction")]
        public string DeviceChildrenRestriction { get; set; }

        [JsonProperty(PropertyName = "configurationTypes")]
        public List<ConfigurationTypeDTO> ConfigurationTypes { get; set; }
        
        [JsonProperty(PropertyName = "capabilityTypes")]
        public List<CapabilityTypeDTO> Capabilities { get; set; }

        [JsonProperty(PropertyName = "actionTypes")]
        public List<ActionTypeDTO> Actions { get; set; }
    }
}
