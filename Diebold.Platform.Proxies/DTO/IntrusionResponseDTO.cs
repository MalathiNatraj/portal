using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class IntrusionResponseDTO : BaseResponseDTO
    {
        public IntrusionPayLoad payload { get; set; }
    }

    public class IntrusionPayLoad
    {
        public string name { get; set; }
        public string txid { get; set; }
        //public IntrusionreportsList intrusionreportslist { get; set; }
        public UserCodeInformationList userscodeinformationlist { get; set; }
        public IntrusionReadersList readerslist { get; set; }
        public DoorStatusDataList doorstatusdatalist { get; set; }
        public AccessControlReportsList accesscontrolreportslist { get; set; }
        public CommandResponse command_response { get; set; }
        public CommandResponseMessage[] messages { get; set; }
        public IntrusionStatusReport SparkIntrusionReport { get; set; }
        public IntrusionStatusReport SparkIntrusionResponse { get; set; }
    }
    public class IntrusionStatusReport
    {
        public string name { get; set; }
        public IntrusionProperties properties { get; set; }
        public AreasStatusList AreasStatusList { get; set; }
        public UserCodeInformation UserCodeInformation { get; set; }
        public IntrusionreportsList IntrusionReportsList { get; set; }
        public DeviceMediaImageProperties IntrusionImageData { get; set; }
        public DeviceMediaVideoProperties IntrusionVideoData { get; set; }

    }

    public class DeviceMediaImageProperties {

        public string name { get; set; }
        public DeviceMediaProperty properties { get; set; }
    }
    public class DeviceMediaVideoProperties
    {

        public string name { get; set; }
        public DeviceMediaProperty properties { get; set; }
    }
    public class DeviceMediaProperty {
        public ResponseProperty property { get; set; }    
    }
    public class AccessControlReportsList
    {
        public string name { get; set; }
        public AccessControlReport[] accesscontrolreport { get; set; }
    }

    public class AccessControlReport : IntrusionNamePropertyCollection
    {
    }

    public class DoorStatusDataList
    {
        public string name { get; set; }
        public DoorStatusData[] doorstatusdata { get; set; }
    }

    public class DoorStatusData:IntrusionNamePropertyCollection
    {
    }

    public class IntrusionReadersList
    {
        public string name { get; set; }
        public ReaderInformation[] readerinformation { get; set; }
    }

    public class ReaderInformation : IntrusionNamePropertyCollection
    {
    }

    public class UserCodeInformationList
    {
        public string name { get; set; }
        public UserCodeInformation[] usercodeinformation { get; set; }
    }

    public class UserCodeInformation
    {
        public string name { get; set; }
        public IntrusionProperties properties { get; set; }
    }

    public class AreasStatusList
    {
        public string name { get; set; }
        public IntrusionAreasStatuslist[] AreaStatus { get; set; }
       
    }

    public class IntrusionAreasStatuslist
    {
        public IntrusionProperties properties { get; set; }
        public ZonesStatusList ZonesStatusList { get; set; }
        
    }

    public class ZonesStatusList
    {
        public string name { get; set; }
        public Zonestatus[] ZoneStatus { get; set; }
    }

    public class Zonestatus
    {
        public string name { get; set; }
        public IntrusionProperties properties { get; set; }
    }

    public class IntrusionreportsList
    {
        public string name { get; set; }
        public IntrusionReport[] IntrusionReport { get; set; }
    }

    public class IntrusionReport 
    {
        public IntrusionProperties properties { get; set; }
    }

    public class IntrusionProperties
    {
        public IntrusionPropertyList propertyList { get; set; }
        public ResponseProperty[] property { get; set; }
    }


    public class IntrusionPropertyList
    {
        public string name { get; set; }
        public IntrusionPropertyItem[] propertyitem { get; set; }
    }

    public class IntrusionPropertyItem
    {
        public string value { get; set; }
    }
   
    public class IntrusionNamePropertyCollection
    {
        public string name { get; set; }
        public ResponseProperty[] properties { get; set; }
        public ResponseProperty[] property { get; set; }
    }
}
