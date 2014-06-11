
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DieboldMobile.Models
{
    public class NotificationViewModel
    {
        public NotificationViewModel()
        {
            //Status =  new List<Status>();
        }

        public Alert Alert { get; set; }
    }

    public class Alert
    {
        public string DeviceId { get; set; }

        public string AlarmName { get; set; }

        public string AlertDate { get; set; }

        public bool AlertActive { get; set; }

        public string RelationalOperator { get; set; }

        public object Threshold { get; set; }

        public object Value { get; set; }
    }

    public class CapabilityType
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool Collection { get; set; }
    }

}