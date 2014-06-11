using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class AccessGroupResponseDTO : BaseResponseDTO
    {
        public AccessGrpPayload payload { get; set; }
    }   

    public class AccessGrpPayload
    {
        public string name { get; set; }
        public string txid { get; set; }
        public CommandResponse command_response { get; set; }
        
        public CardHoldersList cardholderslist { get; set; }
        public AccessGroupProperties properties { get; set; }
        public SparkAccessGroupResponseControl SparkAccessControlResponse { get; set; }
        public CommandResponseMessage[] messages { get; set; }
    }
    public class SparkAccessGroupResponseControl
    {
        public AccessGrpInformation AccessGroupInformation { get; set; }
        public ReadersList readerslist { get; set; }        
        public AccControlReportList AccessControlReportsList { get; set; }
    }

    public class AccessGrpInformation
    {
        public AccessGrpTimePeriodList accessgrouptimeperiodlist { get; set; }       
        public AccGrpProperties properties { get; set; }
    }
    public class AccGrpProperties
    {
        public AccessProperty[] property { get; set; }
    }

    public class AccessGrpTimePeriodList
    {
        public string name { get; set; }
        public AccessGrpTimePeriod[] accessgrouptimeperiod { get; set; }
    }

    public class AccessGrpTimePeriod
    {
        public AccessGrpProperties properties { get; set; }
    }

    public class AccessGrpProperties
    {
        public AccessGrpPropertyList propertylist { get; set; }
        public AccessProperty[] property { get; set; }
        public AGReadersList readerslist { get; set; }
    }

    public class AccessGrpPropertyList
    {
        public string name { get; set; }
        public AccessPropertyItem[] propertyitem { get; set; }
    }

    public class AGReadersList
    {
        public string name { get; set; }
        public AccessGroupNamePropertyCollection[] readerinformation { get; set; }
    }

    public class AccessGroupNamePropertyCollection
    {
        public string name { get; set; }
        public AccessReaderPropertiesItems properties { get; set; }
    }    
    public class AccessGroupNameReaderPropertyCollection
    {
        public AccessReaderPropertiesItems properties { get; set; }    
    }
    public class AccessGroupReaderPropertiesItems
    {
        public AccessProperty[] property { get; set; }
    } 
}
