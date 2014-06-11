using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class IntrusionPlatformResponseDTO : BasePlatformResponseDTO
    {
        public IntrusionPlatformPayLoad payload { get; set; }
    }

    public class IntrusionPlatformPayLoad
    {
        public string name { get; set; }
        public string txid { get; set; }        
        public CommandResponse command_response { get; set; }
        public CommandResponseMessage[] messages { get; set; }
        public IntrusionStatusReport_Platform SparkIntrusionReport { get; set; }
        public IntrusionStatusReport SparkIntrusionResponse { get; set; }
    }
    public class IntrusionStatusReport_Platform
    {
        public string name { get; set; }
        public IntrusionProperties_Platform properties { get; set; }
        public AreaStatusList AreasStatusList { get; set; }       
    }
    public class IntrusionProperties_Platform
    {        
        public IntrusionPlatformProperty property { get; set; }
    }
    public class IntrusionPlatformProperty
    {
        public string deviceIdentifier { get; set; }
        public string networkDown { get; set; }
        public string softwareVersion { get; set; }
        public string string1 { get; set; }
        public string string2 { get; set; }
        public string int1 { get; set; }
        public string int2 { get; set; }
        public string bool1 { get; set; }
        public string bool2 { get; set; }
        public string intrusionErrorCode { get; set; }  
    }    
    
    public class AreaStatusList
    {
        public string name { get; set; }     
        public IntrusionPlatformAreasStatuslist[] AreaStatus { get; set; }

    }

    public class IntrusionPlatformAreasStatuslist
    {
        public AreaProperties properties { get; set; }
        public ZonesPlatformStatusList ZonesStatusList { get; set; }

    }
    public class AreaProperties
    {
        public AreaStatusResponseProperty property { get; set; }
    }
    public class AreaStatusResponseProperty
    {
        public string areaNumber { get; set; }
        public string armed { get; set; }
        public string scheduleStatus { get; set; }
        public string lateStatus { get; set; }
        public string areaName { get; set; }        
    }

    public class ZonesPlatformStatusList
    {
        public string name { get; set; }
        public PlatformZonestatus[] ZoneStatus { get; set; }
    }

    public class PlatformZonestatus
    {
        public string name { get; set; }
        public ZoneIntrusionProperties properties { get; set; }
    }

    public class ZoneIntrusionProperties
    {
        public ZoneResponseProperty property { get; set; }
    }
    public class ZoneResponseProperty
    {
        public string zoneNumber { get; set; }
        public string zoneStatus { get; set; }
        public string zoneName { get; set; }        
    }
}
