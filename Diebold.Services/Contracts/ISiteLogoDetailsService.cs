using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Contracts;
using Diebold.Services.Impl;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface ISiteLogoDetailsService : ICRUDTrackeableService<SiteLogoDetails>
    {
        SiteLogoDetails GetSiteLogoDetailsbySiteId(int SiteId);

        void Evict(SiteLogoDetails item);
    }
}
