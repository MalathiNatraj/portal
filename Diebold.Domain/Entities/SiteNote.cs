using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class SiteNote : TrackeableEntity
    {
        public User User { get; set; }
        public Site Site { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}
