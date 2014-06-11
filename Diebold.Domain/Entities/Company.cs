using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class Company : TrackeableEntity
    {
        public Company()
        {
            Subscriptions = new HashSet<Subscription>();
            CompanyGrouping1Levels = new List<CompanyGrouping1Level>();
            Devices = new List<Device>();
        }

        public virtual int ExternalCompanyId { get; set; }
        
        public virtual string Name { get; set; }

        public virtual string FirstLevelGrouping { get; set; }

        public virtual string SecondLevelGrouping { get; set; }

        public virtual string ThirdLevelGrouping { get; set; }

        public virtual string FourthLevelGrouping { get; set; }

        public virtual string PrimaryContactName { get; set; }

        public virtual string PrimaryContactExtension { get; set; }

        public virtual string PrimaryContactOffice { get; set; }

        public virtual string PrimaryContactMobile { get; set; }

        public virtual string PrimaryContactEmail { get; set; }

        public virtual bool PrimaryContactOfficePreferred { get; set; }

        public virtual bool PrimaryContactMobilePreferred { get; set; }

        public virtual ReportingFrequency ReportingFrequency { get; set; }
        
        public virtual ICollection<Subscription> Subscriptions { get; set; }

        public virtual IList<CompanyGrouping1Level> CompanyGrouping1Levels { get; set; }

        public virtual IList<Device> Devices { get; set; }

        public override string ToString()
        {
            return "Company " + Name;
        }

        public virtual string FileName { get; set; }
        public virtual byte[] CompanyLogo { get; set; }

        // public virtual IList<CompanyInventory> CompanyInventory { get; set; }
    }
}
