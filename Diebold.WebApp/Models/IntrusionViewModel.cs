using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Domain.Entities;

namespace Diebold.WebApp.Models
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
        public List<IntrusionReportModel> IntruReportModelList { get; set; }
        public List<IntrusionUserCodeModel>UserCodeModelList { get; set; }
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
        string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != null && value.Contains('|'))
                {
                    String[] splits = value.Split('|');
                    _name = splits.First();
                    if (splits.Last() == "6")
                    {
                        HasImage = true;
                        HasVideo = true;
                    }
                    else
                    {
                        HasVideo = false;
                        HasImage = false;
                    }
                }
                else
                {
                    _name = value;
                    HasImage = false;
                    HasVideo = false;
                }
            }
        }
        public bool ByPass { get; set; }
        public bool HasVideo { get; set; }
        public bool HasImage { get; set; }

        public string GetFullSection() { return this.AreaNumber + "-" + this.ZoneNumber;  }
    }
    public class IntrusionReportModel
    {
        public string type { get; set; }
        public string datetime { get; set; }
        public string user { get; set; }
        public string message { get; set; }
    }
    public class IntrusionUserCodeModel
    {
        public string UserNumber { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string Zip { get; set; }
        public string ProfileNumber { get; set; }
        public Dictionary<string, string> AccessLevels { get; set; }
        public string tempDate { get; set; }

        public IntrusionUserCodeModel()
        {
            this.AccessLevels = new Dictionary<string, string>();

        }
    }
    public class IntrusionUserCodeViewModel
    {
        public string UserNumber { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string Zip { get; set; }
        public DeviceType DeviceType { get; set; }
        public string DeviceName { get; set; }
        public int DeviceId { get; set; }
        public List<ProfileNumberModel> ProfileNumbers { get; set; }
        public KeyValuePair<string, int> AccessLevels { get; set; }
        public string tempDate { get; set; }

        public IntrusionUserCodeViewModel()
        {
            this.AccessLevels = new KeyValuePair<string, int>();
            this.ProfileNumbers = new List<ProfileNumberModel>();

        }
    }
  
    
}
