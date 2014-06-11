using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class AccessResponseDTO : BaseResponseDTO
    {
        public AccessPayload payload { get; set; }
    }

    public class CardHoldersList
    {
        public string name { get; set; }
        public CardHolderInformation[] cardholderinformation { get; set; }
    }
   
    public class AccessPayload
    {
        public string name { get; set; }
        public string txid { get; set; }
        public CommandResponse command_response { get; set; }        
        public AccessGroupInformation accessgroupinformation { get; set; }
        public CardHoldersList cardholderslist { get; set; }
        public AccessGroupProperties properties { get; set; }        
        public SparkAccessResponseControl SparkAccessControlResponse { get; set; }
        public CommandResponseMessage[] messages { get; set; }
    }
    public class SparkAccessResponseControl
    {
        public CardHolderInformation CardHolderInformation { get; set; }
        public ReadersList readerslist { get; set; }
        public AccControlReportList AccessControlReportsList { get; set; }
    }

    public class AccessGroupInformation : AccessNamePropertyCollection
    {
        public AccessGroupTimePeriodList accessgrouptimeperiodlist { get; set; }
    }

    public class AccessGroupTimePeriodList
    {
        public string name { get; set; }
        public AccessGroupTimePeriod[] accessgrouptimeperiod { get; set; }
    }

    public class AccessGroupTimePeriod
    {
        public AccessGroupProperties properties { get; set; }
    }

    public class AccessGroupProperties
    {
        public AccessGroupPropertyList propertylist { get; set; }
        public AccessProperty[] property { get; set; }
        public ReadersList readerslist { get; set; }
    }

    public class AccessGroupPropertyList
    {
        public string name { get; set; }
        public AccessPropertyItem[] propertyitem { get; set; }
    }

    public class ReadersList
    {
        public string name { get; set; }
        public AccessNameReaderPropertyCollection[] readerinformation { get; set; }
    }

    public class CardHolderInformation 
    {
        public AccessStatusPropertiesItems properties { get; set; }
    }
    public class AccessStatusPropertiesItems
    {
        public AccessProperty[] property { get; set; }
    }

    public class AccessNamePropertyCollection
    {
        public string name { get; set; }
        public AccessProperty[] properties { get; set; }
    }
    public class AccessNameReaderPropertyCollection
    {
        public AccessReaderPropertiesItems properties { get; set; }
    }
    public class AccessReaderPropertiesItems
    {
        public AccessProperty[] property { get; set; }
    }
    public class AccessProperties : AccessNamePropertyCollection
    {
    }

    public class AccessProperty
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class AccessPropertyItem
    {
        public string value { get; set; }
    }
    public class AccControlReportList
    {
        public string name { get; set; }
        public AccControlReport[] AccessControlReport { get; set; }
    }
    public class AccControlReport
    {
        public AccProperties properties { get; set; }
    }
    public class AccProperties
    {        
        public ResponseProperty[] property { get; set; }
    }
}
