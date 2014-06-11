using Diebold.Domain.Entities;
using System.Collections.Generic;

namespace Diebold.Domain.Contracts
{
    public interface ISiteRepository : IIntKeyedRepository<Site>
    {
        IList<Site> GetSitesPerPage(int pageIndex, int rowCount);
        int GetSitesCount();
    }
}
