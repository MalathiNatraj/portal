using Newtonsoft.Json;
using System.Collections.Generic;

namespace Diebold.Platform.Proxies.DTO
{
    public class StatusDTO:BaseResponseDTO
    {
        public StausPayload payload { get; set; }

        public string isGateWay { get; set; }
    }

    public class StausPayload
    {
        public SparkGatewayReport SparkGatewayReport { get; set; }

        public SparkDvrReport SparkDvrReport { get; set; }

        public string txid { get; set; }
        public CommandResponse command_response { get; set; }
        public CommandResponseMessage[] messages { get; set; }
    }

    public class SparkDvrReport
    {
        public string name { get; set; }

        public Properties properties { get; set; }

        public AgentDvrStatusList AgentDvrStatusList { get; set; }

    }

    public class Properties
    {
        public ResponseProperty[] property { get; set; }

        public PropertyList[] propertyList { get; set; }
    }

    public class PropertyList
    {
        public string name { get; set; }

        public PropertyValue[] propertyItem { get; set; }


    }

    public class PropertyValue
    {
        public string value { get; set; }
    }


    public class SparkGatewayReport
    {

        public string name { get; set; }


        public Properties properties { get; set; }

        public AgentDvrStatusList AgentStatusList { get; set; }

    }

    public class AgentDvrStatusList
    {
        public string name { get; set; }

        public GWAgentDVRStatus[] AgentStatus { get; set; }
    }

    public class AgentDvrStatus
    {
        public string name { get; set; }

        public ResponseProperty[] properties { get; set; }
    }

    public class GWAgentDVRStatus
    {
        public string name { get; set; }

        public Properties properties { get; set; }
    }
}
