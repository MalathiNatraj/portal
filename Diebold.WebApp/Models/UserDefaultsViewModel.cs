using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class UserDefaultsViewModel
    {
        public string FilterName { get; set; }
        public int FilterValue { get; set; }
        public string InternalName { get; set; }
        public int UserId { get; set; }
        public string AlertType { get; set; }
    }
}