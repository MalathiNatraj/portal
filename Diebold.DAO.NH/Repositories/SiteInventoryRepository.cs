using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using NHibernate.Transform;

namespace Diebold.DAO.NH.Repositories
{
    public class SiteInventoryRepository : BaseIntKeyedRepository<SiteInventory>, ISiteInventoryRepository
    {
        public SiteInventoryRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IList<SiteInventoryDetails> GetSiteInventoryBySiteandCompany(int SiteId, int CompanyId)
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append("select SI.Id, SI.SiteId, SI.InventoryValue, SI.CompanyInventoryId, CI.InventoryKey from SiteInventory SI right outer join  CompanyInventory CI");
            sbQuery.Append(" on CI.Id=SI.CompanyInventoryId and SI.SiteId = " + SiteId);
            sbQuery.Append(" where CI.ExternalCompanyId = " + CompanyId + " and CI.DeletedKey is null");
            var query = this.Session.CreateSQLQuery(sbQuery.ToString());
            query.SetResultTransformer(Transformers.AliasToBean(typeof(SiteInventoryDetails)));
            var result = query.List<SiteInventoryDetails>();
            return result;
        }
    }
}
