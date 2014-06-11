using System;
using System.Linq;
using System.Collections.Generic;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Domain.Exceptions;

namespace Diebold.DAO.NH.Repositories
{
    public class SiteRepository : BaseIntKeyedRepository<Site>, ISiteRepository
    {  

        public SiteRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {}

         protected override void Validate(Site entity)
        {
            var query = base.All().Where(x => x.SiteId == entity.SiteId && x.Id != entity.Id && x.DeletedKey == null);
            if (query.Any())
            {
                throw new RepositoryException("The site Id already exists.");
            }
            
            return;
        }
         public IList<Site> GetSitesPerPage(int pageIndex, int rowCount)
         {
             var lstSite = base.All((pageIndex - 1) * rowCount, rowCount).Where(x => x.DeletedKey == null).ToList();
             return lstSite;
         }
         public int GetSitesCount()
         {
             var query = this.Session.CreateQuery("select count(Id) from Site" +
                 " where DeletedKey = null");

             return (int)query.UniqueResult<Int64>();
         }
         
    }
}
