using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Contracts;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Impl
{
    public class UserDefaultsService : BaseCRUDService<UserDefaults>, IUserDefaultsService
    {
        public UserDefaultsService(IIntKeyedRepository<UserDefaults> repository, IUnitOfWork unitOfWork,
                                         IValidationProvider validationProvider, ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            //_repository = repository;
        }

        public IList<UserDefaults> GetUserDefaultsUserandPortlet(int UserId, string PortletName)
        {
            return _repository.All().Where(x => x.User.Id == UserId && x.InternalName == PortletName).ToList();
        }

        public IList<UserDefaults> GetUserDefaultsUserandPortlet(int UserId, string PortletName, string filterName)
        {
            return _repository.All().Where(x => x.User.Id == UserId && x.InternalName == PortletName && x.FilterName.Equals(filterName)).ToList();
        }
    }
}
