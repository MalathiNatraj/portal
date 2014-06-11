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
    public class UserPortletsPreferencesService : BaseCRUDTrackeableService<UserPortletsPreferences>, IUserPortletsPreferences
    {
        public UserPortletsPreferencesService(IIntKeyedRepository<UserPortletsPreferences> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {        
            }

        public IList<UserPortletsPreferences> GetAllPortletsByUser(int UserId)
        {
            return _repository.All().Where(x => x.User.Id == UserId).ToList();
        }

        public IList<UserPortletsPreferences> GetAllActivePortletsByUser(int UserId)
        {
            return _repository.All().Where(x => x.User.Id == UserId && x.IsDisabled == false).ToList();
        }

        public IList<UserPortletsPreferences> GetInActivePortletsByUserforLiveView(int UserId)
        {
            return _repository.All().Where(x => x.User.Id == UserId && x.IsDisabled == true && x.Portlets.InternalName.Contains("LIVEVIEW")).ToList();
        }
    }
}
