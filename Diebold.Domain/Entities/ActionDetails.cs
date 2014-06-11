using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class ActionDetails : IntKeyedEntity
    {
        public virtual int ActionKey { get; set; }
        public virtual string ActionValue { get; set; }
    }
}
