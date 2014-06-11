using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Contracts;
using Diebold.Services.Impl;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface ISiteAccNumberService : ICRUDTrackeableService<SiteAccountNumber>
    {
        //IList<SiteAccountNumber> GetSiteAccNumberbySiteId(int SiteId, int UserId);
        IList<SiteAccountNumber> GetSiteAccNumberbySiteId(int SiteId);
        void DeleteSiteAccountNumber(int SiteId);
    }
}
