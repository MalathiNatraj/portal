using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class AccessStatusResponseDTO : BaseResponseDTO
    {
        public AccessStatusPayload payload { get; set; }
    }

    public class AccessStatusPayload
    {
        public string name { get; set; }
        public string txid { get; set; }
        public SparkAccessControlReport SparkAccessControlReport { get; set; }
        public SparkAccessControlReport SparkAccessControlResponse { get; set; }
        public CommandResponse command_response { get; set; }
        public CommandResponseMessage[] messages { get; set; }
    }

    public class SparkAccessControlReport
    {
        public AccessStatusProperties properties { get; set; }
        public AccessStatusDoorStatusDataList DoorStatusDataList { get; set; }        
    }

    public class AccessStatusProperties
    {
        public ResponseProperty[] property { get; set; }
    }
   
    public class AccessStatusDoorStatusDataList
    {
        public string name { get; set; }
        public AccessStatusDoorStatusData[] DoorStatusData { get; set; }
    }

    public class AccessStatusDoorStatusData
    {
        public string name { get; set; }
        public AccessStatusProperties properties { get; set; }
    }   

}
