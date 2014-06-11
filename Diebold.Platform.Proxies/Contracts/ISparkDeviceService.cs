using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Platform.Proxies.Models;

namespace Diebold.Platform.Proxies.Contracts
{
    interface ISparkDeviceService
    {
        SparkDeviceResponse Request(SparkDeviceRequest request);
    }
}
 