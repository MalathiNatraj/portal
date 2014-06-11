using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class StatusPlatformDTO : BasePlatformResponseDTO
    {
        public PlatformStausPayload payload { get; set; }
        public string isGateWay { get; set; }
    }
    public class PlatformStausPayload
    {
        public string name { get; set; }
        public PlatformSparkGatewayReport SparkGatewayReport { get; set; }
        public PlatformSparkDvrReport SparkDvrReport { get; set; }
    }
   
    public class PlatformSparkDvrReport
    {
        public string name { get; set; }
        public DeviceProperties properties { get; set; }

        //public AgentDvrStatusList AgentDvrStatusList { get; set; }

    }
    public class DeviceProperties
    {
        public PlatformDeviceProperty property { get; set; }
        public DevicePropertyList[] propertyList { get; set; }
    }
    public class DevicePropertyList
    {
        public string name { get; set; }
        public string[] propertyItem { get; set; }
    }    
    public class PlatformDeviceProperty
    {
        public string deviceIdentifier {get; set;}
        public string daysRecorded {get; set;}
        public string isNotRecording {get; set;}
        public string networkDown {get; set;}
        public string estimatedFreeRecording {get; set;}
        public string startedOn {get; set;}
        public string upTime {get; set;}
        public string deviceFirmware {get; set;}
        public string timeStampAgent {get; set;}
        public string timeStampRecorder {get; set;}
        public string string1 {get; set;}
        public string string2 {get; set;}
        public string int1 {get; set;}
        public string int2 {get; set;}
        public string bool1 {get; set;}
        public string bool2 {get; set;}
        public string dvrErrorCode {get; set;}
    }
    public class PlatformSparkGatewayReport
    {
        public string name { get; set; }
        public GatewayProperties properties { get; set; }
        public PlatformAgentDvrStatusList AgentStatusList { get; set; }
    }
    public class GatewayProperties
    {
        public GatewayProperty property { get; set; }
    }
    public class GatewayProperty
    {
        public string gatewayIdentifier { get; set; }
        public string freeSpaceOnDrives { get; set; }
        public string memoryUsage { get; set; }
        public string smartData { get; set; }
        public string cpuUsage { get; set; }
        public string timeStamp { get; set; }         
    }
    public class PlatformAgentDvrStatusList
    {
        public string name { get; set; }

        public PlatformGWAgentDVRStatus[] AgentStatus { get; set; }
    }
    public class PlatformGWAgentDVRStatus
    {
        public string name { get; set; }
        public PlatformProperties properties { get; set; }
    }
    public class PlatformProperties
    {
        public PlatformGatewayProperty property { get; set; }        
    }
    public class PlatformGatewayProperty
    {
        public string deviceIdentifier { get; set; }
        public string model { get; set; }
        public string connected { get; set; }
    }    
}
