using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class Site : TrackeableEntity
    {
        public Site()
        {
        }

        public virtual int SiteId { get; set; }

        public virtual string Name { get; set; }

        public virtual string SecondLevelGrouping { get; set; }

        public virtual string SharepointURL { get; set; }

        public virtual string Notes { get; set; }

        public virtual string ParentAssociation { get; set; }

        public virtual string DieboldName { get; set; }

        public virtual string Address1 { get; set; }

        public virtual string Address2 { get; set; }

        public virtual string City { get; set; }

        public virtual string State { get; set; }

        public virtual string Zip { get; set; }

        public virtual string County { get; set; }

        public virtual string Country { get; set; }

        public virtual string CCMFStatus { get; set; }

        public virtual string AccountNumber { get; set; }

        public virtual string Latitude { get; set; }

        public virtual string Longitude { get; set; }
        
        public virtual CompanyGrouping2Level CompanyGrouping2Level { get; set; }

        public override string ToString()
        {
            return "Site " + Name;
        }
        
        public virtual string ContactName { get; set; }

        public virtual string ContactEmail { get; set; }

        public virtual string ContactNumber { get; set; }

        public virtual string FireMonitoringAccNumber { get; set; }
    }
}
