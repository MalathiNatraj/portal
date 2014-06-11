using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DieboldMobile.Models
{
    public class IntrusionViewModel : Controller
    {
        public string PollingStatus { get; set; }
        public string Status { get; set; }
        public string Area { get; set; }
        public string AreaStatus { get; set; }
        public string Online { get; set; }
        public string Arm { get; set; }
        public int DeviceId { get; set; }
        public string Site { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string userName { get; set; }
        public string userCode { get; set; }
        public string userNumber { get; set; }
        public string profileNumber { get; set; }

        public IList<ProfileNumberModel> ProfileNumberList { get; set; }
        public List<AreaModel> AreModelList { get; set; }
        public List<ZoneModel> ZoneModelList { get; set; }
    }

    public class ProfileNumberModel
    {
        public String ProfileNumberID { get; set; }
        public String ProfileNumber { get; set; }
    }

    public class AreaModel
    {
        public int AreaNumber { get; set; }
        public bool Armed { get; set; }
        public bool ScheduleStatus { get; set; }
        public bool LateStatus { get; set; }
        public string AreaName { get; set; }
        public string Online { get; set; }
        public string Status { get; set; }
    }

    public class ZoneModel
    {
        public int AreaNumber { get; set; }
        public int ZoneNumber { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public bool ByPass { get; set; }
    }
}