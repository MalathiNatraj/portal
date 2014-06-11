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
    public class RSSFeedService : BaseCRUDTrackeableService<RSSFeed>, IRSSFeedService
    {
        public RSSFeedService(IIntKeyedRepository<RSSFeed> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {        
            }

        public IList<RSSFeed> GetAllRSSFeedsByUser(int UserId)
        {
            return _repository.All().Where(x => x.User.Id == UserId && x.DeletedKey == null).ToList();
        }

        public IList<RSSFeed> GetAllActiveRSSFeedsByUser(int UserId)
        {
            return _repository.All().Where(x => x.User.Id == UserId && x.DeletedKey == null && x.IsDisabled == false).ToList();
        }
    }
}
