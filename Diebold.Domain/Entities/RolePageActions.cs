using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class RolePageActions : IntKeyedEntity
    {
        public virtual Role Role { get; set; }
        public virtual ActionDetails ActionDetails { get; set; }
      
    }
}
