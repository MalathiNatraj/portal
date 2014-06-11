using System;
using System.Collections.Generic;
using System.Linq;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Domain.Exceptions;
using System.Text;
using NHibernate.Transform;

namespace Diebold.DAO.NH.Repositories
{
    public class ActionDetailsRepository : BaseIntKeyedRepository<ActionDetails>, IActionDetailsRepository
    {
        public ActionDetailsRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {}

        public IList<ActionDetails> GetAllActions()
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append("select Id,ActionKey,ActionValue from ActionDetails");
            var query = this.Session.CreateSQLQuery(sbQuery.ToString());
            query.SetResultTransformer(Transformers.AliasToBean(typeof(ActionDetails)));
            var result = query.List<ActionDetails>();
            return result;
        }
    }
}
