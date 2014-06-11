using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class SiteInventory : TrackeableEntity
    {
        public virtual CompanyInventory CompanyInventory { get; set; }
        // public virtual int? CompanyInventoryId { get; set; }
        public virtual Site Site { get; set; }
        // public virtual int? SiteId { get; set; }
        public virtual string InventoryValue { get; set; }
        // public virtual string InventoryKey { get; set; }
    }

    public class SiteInventoryDetails
    {
        public virtual int Id { get; set; }
        public virtual string InventoryValue { get; set; }
        public virtual string InventoryKey { get; set; }
        public virtual int? CompanyInventoryId { get; set; }
        public virtual int? SiteId { get; set; }
        public virtual int? ExternalCompanyId { get; set; }
    }
}
