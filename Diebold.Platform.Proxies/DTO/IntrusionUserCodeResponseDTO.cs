using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class IntrusionUserCodeResponseDTO : BaseResponseDTO
    {
        public IntrusionUCPayLoad payload { get; set; } 
    }
    public class IntrusionUCPayLoad
    {
        public string name { get; set; }
        public string txid { get; set; }                
        public CommandResponse command_response { get; set; }
        public CommandResponseMessage[] messages { get; set; }
        public IntrusionUCStatusReport SparkIntrusionReport { get; set; }
        public IntrusionUCStatusReport SparkIntrusionResponse { get; set; }
    }
    public class IntrusionUCStatusReport
    {
        public string name { get; set; }
        public IntruUserCodeInformationList UserCodeInformationList { get; set; }
        public IntruUserCodeInformationList UserCodeInformationList2 { get; set; }
    }
    public class IntruUserCodeInformationList
    {
        public string name { get; set; }               
        public IntruUserCodeInformation[] UserCodeInformation { get; set; }
        public IntruUserCodeInformation UserCodeInformation2 { get; set; }
        
    }
    public class IntrusionUCProperties
    {        
        public ResponseProperty[] property { get; set; }
    }
    public class IntruUserCodeInformation
    {
        public string name { get; set; }
        public IntrusionUCProperties properties { get; set; }
        public IntruUserCodeAreasAuthorityLevelProperties AreasAuthorityLevel { get; set; }
    }
    public class IntruUserCodeAreasAuthorityLevelProperties
    {
        public string name { get; set; }
        public IntrusionUCProperties properties { get; set; }
        
        
    }
}
