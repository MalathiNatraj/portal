using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class CompanyInventory : TrackeableEntity
    {
        public virtual string InventoryKey { get; set; }
        public virtual int ExternalCompanyId { get; set; }
    }
}
