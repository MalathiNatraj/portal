using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class Portlets : IntKeyedEntity
    {        
        public virtual string Name { get; set; }
        public virtual string InternalName { get; set; }
        public virtual int ColumnNo { get; set; }
    }
}
