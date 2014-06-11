using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class VideoHealthCheckModel
    {
        public int Id { get; set; }
        public String PollingStatus { get; set; }
        public DateTime LastUpdated { get; set; }
        public String LastUpdatedS { get; set; }
        public int DaysRecorded { get; set; }
        public bool IsCurrentlyRecording { get; set; }
        public String Status { get; set; }
        public String AlarmPresent { get; set; }
        public int DeviceListId { get; set; }
    }
}