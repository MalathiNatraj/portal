using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.Domain.Contracts
{
    public interface ISiteInventoryRepository : IIntKeyedRepository<SiteInventory>
    {
        IList<SiteInventoryDetails> GetSiteInventoryBySiteandCompany(int SiteId, int CompanyId);
    }
}
