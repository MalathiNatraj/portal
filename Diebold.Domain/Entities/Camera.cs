using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class Camera : TrackeableEntity
    {
        public virtual string Name { get; set; }
        public virtual string Channel { get; set; }
        public virtual bool Active { get; set; }
    }
}
