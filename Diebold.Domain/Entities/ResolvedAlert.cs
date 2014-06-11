using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class ResolvedAlert : IntKeyedEntity
    {
        public virtual Device Device { get; set; }

        public virtual AlarmConfiguration AlarmConfiguration { get; set; }

        public virtual User User { get; set; }

        public virtual DateTime AcknoledgeDate { get; set; }

        public virtual string ElementIdentifier { get; set; }
    }
}
