using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class CompanyGrouping1Level : IntKeyedEntity
    {
        public CompanyGrouping1Level()
        {
            CompanyGrouping2Levels = new List<CompanyGrouping2Level>();
        }

        public virtual string Name { get; set; }
        public virtual Company Company { get; set; }
        public virtual IList<CompanyGrouping2Level> CompanyGrouping2Levels { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var companyGrouping1Level = (CompanyGrouping1Level)obj;
            return (Id == companyGrouping1Level.Id);
        }

        public void RemoveRelation()
        {
            Company.CompanyGrouping1Levels.Remove(this);
            Company = null;
        }
    }
}
