using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class UserDefaults : IntKeyedEntity
    {
        public virtual string FilterName { get; set; }
        public virtual int FilterValue { get; set; }
        public virtual User User { get; set; }
        public virtual string InternalName { get; set; }
        public virtual string AlertType { get; set; }
    }
}
