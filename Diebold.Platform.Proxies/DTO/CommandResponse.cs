using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class CommandResponse
    {
        public string status { get; set; }
    }

    public class CommandResponseMessage
    {
        public string code { get; set; }
        public string description { get; set; }
    }
}
