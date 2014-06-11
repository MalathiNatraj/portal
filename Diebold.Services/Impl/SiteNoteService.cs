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
    public class SiteNoteService : BaseCRUDTrackeableService<SiteNote>, ISiteNoteSite
    {
        public SiteNoteService(IIntKeyedRepository<SiteNote> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {        
            }

        public IList<SiteNote> GetSiteNotebySiteId(int SiteId)
        {
            return _repository.All().Where(x => x.Site.Id == SiteId && x.DeletedKey == null).ToList();
        }

    }
}
