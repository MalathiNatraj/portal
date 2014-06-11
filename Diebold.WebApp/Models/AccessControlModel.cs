using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class AccessControlModel
    {
        public String AlertOne { get; set; }
        public String AlertTwo { get; set; }
        public String AlertThree { get; set; }
        public String AlertFour { get; set; }
        public String AlertFive { get; set; }

        public String ImageUrl { get; set; }
        public int Id { get; set; }
        public String Name { get; set; }
        public String Status { get; set; }
        public String Online { get; set; }
        public String MomentaryUnlock { get; set; }
        public int DeviceId { get; set; }
        public String PollingStatus { get; set; }
        public String AccStatus { get; set; }
    }
}