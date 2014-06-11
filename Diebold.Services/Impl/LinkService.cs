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
    public class LinkService : BaseCRUDTrackeableService<Link>, ILinkService
    {        
        public LinkService(IIntKeyedRepository<Link> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {        
            }

        public IList<Link> GetAllLinksByUser(int UserId)
        {
            return _repository.All().Where(x => x.User.Id == UserId && x.DeletedKey == null).OrderBy(x=>x.Name).ToList();
        }

        public IList<Link> GetAllActiveLinksByUser(int UserId)
        {
            return _repository.All().Where(x => x.User.Id == UserId && x.DeletedKey == null).OrderBy(x => x.Name).ToList();
        }
    }
}
