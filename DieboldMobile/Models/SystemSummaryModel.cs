using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DieboldMobile.Models
{
    public class SystemSummaryModel
    {
        public int DeviceTypeId { get; set; }
        public string Status { get; set; }
        public int Value { get; set; }
        public string DeviceName { get; set; }
    }
}
