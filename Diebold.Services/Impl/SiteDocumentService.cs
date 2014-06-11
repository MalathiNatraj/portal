using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Impl
{
    public class SiteDocumentService : BaseCRUDTrackeableService<SiteDocument>, ISiteDocumentService
    {
        public SiteDocumentService(IIntKeyedRepository<SiteDocument> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {        
            }

        public IList<SiteDocument> GetSiteDocumentbySiteId(int SiteId)
        {
            return _repository.All().Where(x => x.Site.Id == SiteId && x.DeletedKey == null).ToList();
        }

        public IList<SiteDocument> GetSiteDocumentbyUserandSiteId(int CurrentUserId, int SiteId)
        {
            return _repository.All().Where(x => x.Site.Id == SiteId && x.User.Id == CurrentUserId && x.DeletedKey == null).ToList();
        }
    }
}
