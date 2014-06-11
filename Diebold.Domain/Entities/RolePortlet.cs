using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class RolePortlets : IntKeyedEntity
    {
        //public virtual int PortletId { get; set; }
        
        //public virtual int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual Portlets Portlets { get; set; }
    }
}
