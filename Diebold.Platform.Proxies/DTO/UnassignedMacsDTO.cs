using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.DTO
{
    public class UnassignedMacsDTO
    {
        //[JsonProperty(PropertyName = "unassignedSlots")]
        public List<string> unassignedSlots { get; set; }

        public List<string> Items
        {
            get
            {
                return unassignedSlots;
            }
        }
    }
}
