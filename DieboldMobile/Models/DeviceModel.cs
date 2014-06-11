using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Diebold.Domain.Entities;
using AutoMapper;

namespace DieboldMobile.Models
{
    public class DeviceModel : BaseMappeableViewModel<Dvr>
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public int SiteId { get; set; }
        public String SiteName { get; set; }
        public String Address { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Zip { get; set; }
        public String Location { get; set; }
        public String Device { get; set; }        
    }
}