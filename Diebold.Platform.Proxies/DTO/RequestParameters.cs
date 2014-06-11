using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class DeviceRequestParameters
    {
        public int ID { get; set; }
        public string CommandName { get; set; }
        public string CommandType { get; set; }
        public string Request { get; set; }
        public string TxId { get; set; }
        public DateTime TimeSent { get; set; }
        public string DeviceKey { get; set; }
        public string DeviceType { get; set; }
        public string DeviceInstnaceType { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string PortA { get; set; }
        public string PortB { get; set; }
        public string TimeZone { get; set; }
        public string Dst { get; set; }
        public string ReportingFrequency { get; set; }
        public string ReportingBufferSize { get; set; }
        public string OnLine { get; set; }
        public string IpAddress { get; set; }
        public string HostUrl { get; set; }
        public string Port { get; set; }
        public string CallbackUrl { get; set; }
        public IList<AlarmDTO> Alarms { get; set; }
        public List<Property> Properties { get; set; }
        public Property Property { get; set; }
        public List<AccecGroupTimePeriod> AccessGroupTimePeriods { get; set; }
        public List<AccessGroupReader> AccessGroupReaders { get; set; }
        public string AccessGroupName { get; set; }
        public string AccessGroupDesc { get; set; }
        public List<AccessDTO> AccessGroupInformation { get; set; }
       
    }
   
    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public UserCodeInformationPropery UserCodeInformation { get; set; }
        public List<Property> CardHolderInformation { get; set; }
    }
    public class UserCodeInformationPropery {
        public List<Property> Properties { get; set; }
        public List<Property> AreasAuthorityLevels { get; set; }
    }
    public class AccecGroupTimePeriod
    {
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
        public string Days { get; set; }
    }

    public class AccessGroupReader
    {
        public string ReaderId { get; set; }
        public string ReaderName { get; set; }
    }
}
