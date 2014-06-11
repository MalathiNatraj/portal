using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Diebold.RemoteService.Proxies.EMC.DTO
{
    [XmlRootAttribute(ElementName = "newalarm")]
    public class AlarmDTO
    {
        public enum AlarmTypes
        {
            Alarm = 271, Poll = 900
        }

        public enum AvailableStatus {Restored = 1, Alarm = 2, Trouble = 3}

        public AlarmDTO(){}

        public AlarmDTO(AlarmTypes type, string emcAccountNumber)
        {
            SiteId = emcAccountNumber;
            AlarmId = "";
            AlarmType = (int) type;
            Data = "";
            Status = (int)AvailableStatus.Alarm;
            Zone = "001";
        }

        [XmlElement(ElementName = "siteid")]
        public string SiteId { get; set; }

        [XmlElement(ElementName = "alarmid")]
        public string AlarmId { get; set; }

        [XmlElement(ElementName = "alarmtype")]
        public int AlarmType { get; set; }

        [XmlElement(ElementName = "zone")]
        public string Zone { get; set; }

        [XmlElement(ElementName = "status")]
        public int Status { get; set; }

        [XmlElement(ElementName = "data")]
        public string Data { get; set; }

    }
}
