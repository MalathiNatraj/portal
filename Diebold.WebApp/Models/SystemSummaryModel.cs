using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class SystemSummaryModel
    {        
        public int DeviceTypeId { get; set; }
        public string Status { get; set; }
        public int Value { get; set; }
        public string DeviceName { get; set; }
        public int DisplayOrder { get; set; }
        public string SuccessStatus { get; set; }
        public string FailureStatus { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }
}