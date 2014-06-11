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
    public class RolePortletService : BaseCRUDService<RolePortlets>, IRolePortletService
    {
        //private readonly IIntKeyedRepository<RolePortlets> _repository;
        public RolePortletService(IIntKeyedRepository<RolePortlets> repository, IUnitOfWork unitOfWork,
                                         IValidationProvider validationProvider, ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            //_repository = repository;
        }

        public IList<RolePortlets> GetAllRolePortletByroleId(int RoleId)
        {
            return _repository.All().Where(x => x.Role.Id == RoleId).ToList();
        }

    }
}
