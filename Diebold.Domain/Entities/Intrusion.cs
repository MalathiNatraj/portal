using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{

    public class Intrusion
    {
        public string PollingStatus { get; set; }
        public string Status { get; set; }
        public string Area { get; set; }
        public string AreaNumber { get; set; }
        public string AreaStatus { get; set; }
        public string Online { get; set; }
        public string Arm { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DeviceType DeviceType { get; set; }
        public string zoneNumber { get; set; }
        public string startDateTime { get; set; }
        public string endDateTime { get; set; }

        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string ProfileNumber { get; set; }
        public string Zip { get; set; }
        public string UserNumber { get; set; }
        public string intrusionerrorcode { get; set; }
        public string tempDate { get; set; }
        public List<Area> AreaList { get; set; }
        public IList<ProfileNumberListModel> ProfileNumberList { get; set; }
        public List<DeviceProperty> Properties { get; set; }
        public List<ReportList> ReportList { get; set; }
        public List<UserCodeList> UserCodeList { get; set; }
        public Dictionary<string, string> AreasAuthorityLevel { get; set; }

        public string DeviceInstanceId { get; set; }
    }
    public class ProfileNumberListModel
    {
        public String profileNum { get; set; }
    }

    public class DeviceProperty
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class ReportList
    {
        public string type { get; set; }
        public string datetime { get; set; }
        public string user { get; set; }
        public string message { get; set; }
    }
    public class UserCodeList
    {
        public string UserNumber { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string Zip { get; set; }
        public string ProfileNumber { get; set; }
        public string tempDate { get; set; }
        public Dictionary<string, string> Areas { get; set; }
    }
}
