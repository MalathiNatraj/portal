using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class CompanyDefaultSubscription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isSelected { get; set; }
    }
}