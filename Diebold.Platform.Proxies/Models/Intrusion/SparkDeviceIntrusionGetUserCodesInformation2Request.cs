using System.Collections.Generic;
using Diebold.Platform.Proxies.Utilities;

namespace Diebold.Platform.Proxies.Models.Intrusion
{
    public class SparkDeviceIntrusionGetUserCodesInformation2Request : SparkDeviceIntrusionRequest
    {
        
        public SparkDeviceIntrusionGetUserCodesInformation2Request(SparkDeviceHeader sparkHeader) : base(sparkHeader)
        {
        }

        internal override void BuildRequest(dynamic body)
        {
            body.SparkIntrusionCommand(new { name = "GetUserCodesInformation2" }, Xml.Fragment(SparkIntrusionCommand =>
            {
                SparkIntrusionCommand.properties(Xml.Fragment(properties =>
                {
                    BuildProperties(properties);
                }));
            }));
        }
    }
}