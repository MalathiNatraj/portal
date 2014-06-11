using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class SiteInfo
    {
        public int Id { get; set; }
        public String Site { get; set; }
        public String GatewayName { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public int MonitoredDevices { get; set; }
        public int AlaramsConfigured { get; set; }
        public String Conf { get; set; }
        public String CompanyLogo { get; set; }
        // Added as per new Requirement
        public String Location { get; set; }
        public string LocationContact { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPersonPhone { get; set; }
        public int DeviceCount { get; set; }
        public int VideoHealthDevice { get; set; }
        public int AccessDevice { get; set; }
        public int IntrusionDevice { get; set; }
        public int Location_Id { get; set; }
        public string notes { get; set; }
        public byte[] SiteLogo { get; set; }
        public string siteImage { get; set; }
        public List<SiteNoteViewModel> SiteNote { get; set; }
        public string DefaultDocument { get; set; }
    }

    public class LocationList
    {
        public String Device { get; set; }
        public String Location { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public int Zip { get; set; }
        public int Location_Id { get; set; }
    }

    public class LocationDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BaseLocationViewModel
    {
        public IList<LocationList> locationList { get; set; }
        public IList<SiteInfo> siteInfo { get; set; }
        public int SiteCount { get; set; }
        public IList<SiteNoteViewModel> SiteNote { get; set; }
    }
}