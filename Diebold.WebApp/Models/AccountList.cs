using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class AccountList
    {
        public int id {get;set;}
        public string Account { get; set; }
        public string Site { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}