using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class SiteMapModel
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string LocationContact { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public int DVRDevicesCount { get; set; }
        public int AccessDevicesCount { get; set; }
        public int IntrusionDevicesCount { get; set; }
        public int TotalDevicesCount { get; set; }
        public int DefaultSiteLocation { get; set; }
        public string SiteImage { get; set; }
        
        public string SessionLatitude { get; set; }
        public string SessionLongitude { get; set; }
        public string SessionAddress { get; set; }
        public string SessionLocation { get; set; }
        public string SessionLocationContact { get; set; }
        public string SessionContactEmail { get; set; }
        public string SessionContactPhone { get; set; }
        public string SessionDVRCount { get; set; }
        public string SessionAccessCount { get; set; }
        public string SessionIntrusionCount { get; set; }
        public string SessionId { get; set; }
    }
}