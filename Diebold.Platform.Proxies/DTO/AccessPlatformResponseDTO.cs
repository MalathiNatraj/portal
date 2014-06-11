using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class AccessPlatformResponseDTO : BasePlatformResponseDTO
    {
        public PlatformAccessStatusPayLoad payload { get; set; }
    }
    public class PlatformAccessStatusPayLoad
    {
        public string name { get; set; }
        public string txid { get; set; }
        public CommandResponse command_response { get; set; }
        public CommandResponseMessage[] messages { get; set; }
        public PlatformAccessStatusReport SparkAccessControlReport { get; set; }
    }
    public class PlatformAccessStatusReport
    {
        public PlatformAccessStatusProperties properties { get; set; }
        public PlatformAccessStatusDoorStatusDataList DoorStatusDataList { get; set; }
    }
    public class PlatformAccessStatusProperties
    {
        public PlatformAccessProperty property { get; set; }
    }
    public class PlatformAccessStatusDoorStatusDataList
    {
        public string name { get; set; }
        public PlatformAccessStatusDoorStatusData[] DoorStatusData { get; set; }
    }
    public class PlatformAccessProperty
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
        public string ACErrorCode { get; set; }
    }
    public class PlatformAccessStatusDoorStatusData
    {
        public string name { get; set; }
        public PlatformAccessDoorStatusProperties properties { get; set; }
    }
    public class PlatformAccessDoorStatusProperties
    {
        public PlatformAccessDoorProperty property { get; set; }     
    }
    public class PlatformAccessDoorProperty
    {
        public string readerID { get; set; }
        public string doorName { get; set; }
        public string readerOnline { get; set; }
        public string doorStatus { get; set; }
    }
}
