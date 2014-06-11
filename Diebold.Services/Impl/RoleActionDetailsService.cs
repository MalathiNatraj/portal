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
    public class RoleActionDetailsService : BaseCRUDService<RolePageActions>, IRoleActionDetailsService
    {
        public RoleActionDetailsService(IIntKeyedRepository<RolePageActions> repository, IUnitOfWork unitOfWork,
                                         IValidationProvider validationProvider, ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            //_repository = repository;
        }

        public IList<RolePageActions> GetAllRoleActionByroleId(int RoleId)
        {
            return _repository.All().Where(x => x.Role.Id == RoleId).ToList();
        }
    }
}
