using System.Linq;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Domain.Exceptions;

namespace Diebold.DAO.NH.Repositories
{
    public class RoleRepository : BaseIntKeyedRepository<Role>, IRoleRepository
    {
        private readonly IUserRepository _userRepository;

        public RoleRepository(IUnitOfWork unitOfWork, IUserRepository userRepository)
            : base(unitOfWork)
        {
            _userRepository = userRepository;
        }

        public override bool Delete(Role entity)
        {
            var query = _userRepository.All().Where(x => x.Role.Id == entity.Id && x.DeletedKey == null);

            if (query.Any())
            {
                throw new RepositoryException("The role is being used by another user.");
            }

            entity.LogicalDelete();

            return true;
        }

        protected override void Validate(Role entity)
        {
            var query = base.All().Where(x => x.Name == entity.Name && x.Id != entity.Id && x.DeletedKey == null);

            if (query.Any())
            {
                throw new RepositoryException("A role with that name already exists.");
            }

            return;
        }
    }
}
