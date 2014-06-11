using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Contracts;
using Diebold.Services.Impl;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface ISiteInventoryService : ICRUDTrackeableService<SiteInventory>
    {
        IList<SiteInventoryDetails> GetSiteInventoryBySiteandCompany(int SiteId);
        IList<SiteInventory> GetSiteInventorybySiteIdCompanyInventoryId(int SiteId, int CompanyInventoryId);
    }
}
