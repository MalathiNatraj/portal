using System.Collections.Generic;

namespace Diebold.Domain.Entities
{
    public class Role : TrackeableEntity
    {        
        public virtual string Name { get; set; }

        public virtual ICollection<Action> Actions { get; set; }
        public virtual IList<RolePortlets> RolePortlets { get; set; }
        public virtual IList<RolePageActions> RolePageActions { get; set; }

        public Role()
        {
            Actions = new HashSet<Action>();
            RolePortlets = new List<RolePortlets>();
            RolePageActions = new List<RolePageActions>();
        }

        public override string ToString()
        {
            return "Role " + Name;
        }
    }
}
