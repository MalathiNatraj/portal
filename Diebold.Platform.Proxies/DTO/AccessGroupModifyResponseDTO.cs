using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class AccessGroupModifyResponseDTO :BaseResponseDTO
    {
        public AccGrpPayload payload { get; set; }
    }
    public class AccGrpPayload
    {
        public string name { get; set; }
        public string txid { get; set; }
        public CommandResponse command_response { get; set; }
        public AccessGroupInfo AccessGroupInformation { get; set; }                     
        public CommandResponseMessage[] messages { get; set; }
    }    
    public class AccessGroupInfo
    {
        public AccGroupTimePeriodList AccessGroupTimePeriodList { get; set; }        
        public AccGroupProperties properties { get; set; }
    }
    public class AccGroupTimePeriodList
    {
        public string name { get; set; }
        public AccGroupTimePeriod[] AccessGroupTimePeriod { get; set; }
    }
    public class AccGroupTimePeriod
    {
        public AccGroupProperties properties { get; set; }
    }     
    public class AccGroupProperties
    {
        public AccessProperty[] property { get; set; }
        public AccGroupReadersList readerslist { get; set; }
    }
    public class AccGroupReadersList
    {
        public string name { get; set; }
        public AccGroupNamePropertyCollection[] readerinformation { get; set; }

    }
    public class AccGroupNamePropertyCollection
    {
        public string name { get; set; }
        public AccGroupReaderPropertiesItems properties { get; set; }
    }    
    public class AccGroupReaderPropertiesItems
    {
        public AccessProperty[] property { get; set; }
    } 
}
