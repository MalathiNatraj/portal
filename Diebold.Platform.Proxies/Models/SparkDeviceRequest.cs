using System;
using System.Linq;
using System.Xml;
using Diebold.Platform.Proxies.Utilities;

namespace Diebold.Platform.Proxies.Models
{
    public abstract class SparkDeviceRequest
    {
        dynamic xml;
        SparkDeviceHeader sparkHeader;
        public SparkDeviceRequest(SparkDeviceHeader sparkHeader)
        {
            this.sparkHeader = sparkHeader;
            xml = new Diebold.Platform.Proxies.Utilities.Xml();
            xml.Declaration();

        }

        internal abstract void BuildRequest(dynamic body);

        public virtual string Create()
        {
            xml.SparkDeviceCommand(new { version = "1.0" }, Xml.Fragment(SparkDeviceCommand =>
            {

                SparkDeviceCommand.header(Xml.Fragment(header =>
                {
                    header.request(sparkHeader.Request);
                    header.txId(sparkHeader.TxId);
                    header.timeSent(sparkHeader.TimeSent);
                    header.deviceKey(sparkHeader.DeviceKey);
                    header.deviceType(sparkHeader.DeviceType);
                }
                    )
                );

                SparkDeviceCommand.body(Xml.Fragment(body =>
                {
                    BuildRequest(body);
                }));
            }
            ));

            return xml;
        }
    }
}
