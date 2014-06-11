using System;
using System.Collections.Generic;

namespace Diebold.Platform.Proxies.Models
{
    public class SparkDeviceHeader
    {
        public string Request { get; set; }

        public string TxId { get; set; }

        public DateTime TimeSent { get; set; }

        public string DeviceKey { get; set; }

        public string DeviceType { get; set; }

        public string Status { get; set; }

        public List<SparkDeviceHeaderMessage> Messages { get; set; }
    }
}