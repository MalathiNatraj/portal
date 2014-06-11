using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class CardholderModel
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String CardNumber { get; set; }
        public String Pin { get; set; }
        public String AccessGroup { get; set; }
        public DateTime Activationdate { get; set; }
        public DateTime Expirationdate { get; set; }
        public Boolean IsActive { get; set; }
        
    }
}