﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class ResponsePropertyCollection
    {
        public string name { get; set; }
        public ResponseProperty[] properties { get; set; }
    }
}
