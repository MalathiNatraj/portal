using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Infrastructure;
using System.Transactions;

namespace Diebold.Services.Impl
{
    public class SiteLogoDetailsService : BaseCRUDTrackeableService<SiteLogoDetails>, ISiteLogoDetailsService
    {
        //GetSiteLogoDetailsbySiteId
        public SiteLogoDetailsService(IIntKeyedRepository<SiteLogoDetails> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {        
            }
        public SiteLogoDetails GetSiteLogoDetailsbySiteId(int SiteId)
        {
            return _repository.All().Where(x => x.siteId == SiteId && x.DeletedKey == null).FirstOrDefault();
        }
        public void Evict(SiteLogoDetails item)
        {
            _repository.Evict(item);
        }
    }
}
