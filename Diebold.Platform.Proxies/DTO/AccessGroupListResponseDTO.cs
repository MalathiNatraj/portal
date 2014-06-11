using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class AccessGroupListResponseDTO : BaseResponseDTO
    {
        public AccessGroupPayload payload { get; set; }
    }

    public class SparkAccessControlResponse
    {
        public AGProperties properties { get; set; }
    }
   
    public class AccessGroupPayload
    {
        public string name { get; set; }
        public SparkAccessControlResponse SparkAccessControlResponse { get; set; }
        public CommandResponse command_response { get; set; }
        public CommandResponseMessage[] messages { get; set; }
    }

    public class AGProperties
    {
        public AGPropertyList propertyList { get; set; }
    }

    public class AGPropertyList
    {
        public string name { get; set; }
        public AGPropertyItem[] propertyItem { get; set; }
    }

    public class AGPropertyItem
    {
        public string value { get; set; }
    }

   
}
