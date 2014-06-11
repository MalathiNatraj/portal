using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class CompanyGrouping2Level : IntKeyedEntity
    {
        public CompanyGrouping2Level()
        {
            Sites = new List<Site>();
        }

        public virtual string Name { get; set; }
        public virtual CompanyGrouping1Level CompanyGrouping1Level { get; set; }
        public virtual IList<Site> Sites { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var companyGrouping2Level = (CompanyGrouping2Level)obj;
            return (Id == companyGrouping2Level.Id);
        }

        public void RemoveRelation()
        {
            CompanyGrouping1Level.CompanyGrouping2Levels.Remove(this);
            CompanyGrouping1Level = null;
        }
    }
}
