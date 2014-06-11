using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class SiteDocument : TrackeableEntity
    {
        public virtual User User { get; set; }
        public virtual Site Site { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string FileName { get; set; }
        public virtual bool IsPrimary { get; set; }
    }
}
