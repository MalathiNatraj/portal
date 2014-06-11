using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class RoleAction : IEquatable<RoleAction>
    {
        public virtual Action Action { get; set; }

        public bool Equals(RoleAction other)
        {
            return this.Action == other.Action;
        }
    }
}
