using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class SiteAccountNumber : TrackeableEntity
    {
        public User User { get; set; }
       // public Site Site { get; set; }
        public DateTime Date { get; set; }
        public string AccountNumber { get; set; }
        public bool IsAssociatedWithFA { get; set; }
        public int siteId { get; set; }
        public string Name { get; set; }
    }
}
