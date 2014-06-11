using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class UserPortletsPreferences : TrackeableEntity
    {
        public virtual int SeqNo { get; set; }
        public virtual int ColumnNo { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual Portlets Portlets { get; set; }
        public int PortletId{ get; set; }
        
    }
}
