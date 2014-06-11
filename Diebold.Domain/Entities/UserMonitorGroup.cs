using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class UserMonitorGroup : IntKeyedEntity
    {
        public virtual User User { get; set; }
        public virtual CompanyGrouping1Level FirstGroupLevel { get; set; }
        public virtual CompanyGrouping2Level SecondGroupLevel { get; set; }
        public virtual Site Site { get; set; }
        public virtual Device Device { get; set; }
        public virtual Int64 UserId { get; set; }

        public UserMonitorGroup() { }
        public UserMonitorGroup(int id, int userId) { this.Id = id; this.UserId = userId; }
    }
}
