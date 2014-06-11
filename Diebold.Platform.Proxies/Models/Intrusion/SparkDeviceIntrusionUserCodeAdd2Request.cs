using System;
using System.Collections.Generic;
using Diebold.Platform.Proxies.Models;
using Diebold.Platform.Proxies.Models.Intrusion;
using Diebold.Platform.Proxies.Utilities;

namespace Diebold.Platform.Proxies.Models.Intrusion
{
    public class SparkDeviceIntrusionUserCodeAdd2Request : SparkDeviceIntrusionRequest
    {
        public Dictionary<string, string> AreasAuthorityLevels { get; set; }

        public SparkDeviceIntrusionUserCodeAdd2Request(SparkDeviceHeader sparkHeader)
            : base(sparkHeader)
        {
            AreasAuthorityLevels = new Dictionary<string, string>();
        }

        internal override void BuildRequest(dynamic body)
        {
            body.SparkIntrusionCommand(new { name = "UserCodeAdd2" }, Xml.Fragment(SparkIntrusionCommand =>
            {
                SparkIntrusionCommand.UserCodeInformation2(new { name = "userCodeInformation2" }, Xml.Fragment(UserCodeInformation2 => {
                    UserCodeInformation2.properties(Xml.Fragment(properties => {
                        BuildProperties(properties);
                    }));

                    UserCodeInformation2.AreasAuthorityLevel(new { name = "areasAuthorityLevel" }, Xml.Fragment(AreasAuthorityLevel => { 
                        AreasAuthorityLevel.properties(Xml.Fragment(properties => {
                            if (AreasAuthorityLevels != null)
                            {
                                foreach (var level in AreasAuthorityLevels)
                                {
                                    properties.property(new { name = level.Key, value = level.Value });
                                }
                            }
                        }));
                    }));
                }));

            }));
        }
    }
}