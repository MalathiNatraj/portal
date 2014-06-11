using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class PreferencesModel
    {
        public String ImageUrl { get; set; }
        public String PortletDescription { get; set; }
        public String PortletHeading { get; set; }
        public String PortletStatus { get; set; }
        public int Id { get; set; }
        public String PortletDisplayHeader { get; set; }
    }
}