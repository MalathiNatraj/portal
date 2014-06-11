using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class AccessCardHolderResponseDTO : BaseResponseDTO
    {
        public AccessCHPayload payload { get; set; }
    }
    public class AccessCHPayload
    {
        public string name { get; set; }
        public string txid { get; set; }
        public CommandResponse command_response { get; set; }
        public AccessGroupInformation accessgroupinformation { get; set; }
        public AccessGroupProperties properties { get; set; }
        public SparkAccessCHResponseControl SparkAccessControlResponse { get; set; }
        public CommandResponseMessage[] messages { get; set; }
    }
    public class SparkAccessCHResponseControl
    {
        public string name { get; set; }
        public CHoldersList cardholderslist { get; set; }
    }
    public class CHoldersList
    {
        public string name { get; set; }
        public AccessCardHolderInformation[] CardHolderInformation { get; set; }
    }
    public class AccessCardHolderInformation
    {
        public AccessCHPropertiesItems properties { get; set; }
    }

    public class AccessCHPropertiesItems
    {
        public AccessCHProperty[] property { get; set; }
    }
    public class AccessCHProperty
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}
