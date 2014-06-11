using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class WeatherAlertViewModel
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string DateEpoch { get; set; }
        public string Expires { get; set; }
        public string ExpiresEpoch { get; set; }
        public string Message { get; set; }
        public string Phenomena { get; set; }
    }
}