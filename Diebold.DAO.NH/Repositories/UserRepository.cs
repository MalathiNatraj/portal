using System.Linq;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Domain.Exceptions;

namespace Diebold.DAO.NH.Repositories
{
    public class UserRepository : BaseIntKeyedRepository<User>, IUserRepository 
    {
        public UserRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override void Validate(User entity)
        {
            var emailQuery = base.All().Where(x => x.Email == entity.Email && x.Id != entity.Id && x.DeletedKey == null);
            if (emailQuery.Any())
            {
                throw new RepositoryException("A user with that email already exists.");
            }

            var usernameQuery = base.All().Where(x => x.Username == entity.Username && x.Id != entity.Id && x.DeletedKey == null);
            if (usernameQuery.Any())
            {
                throw new RepositoryException("A user with that user name already exists.");
            }

            //var userpinQuery = base.All().Where(x => x.UserPin == entity.UserPin && x.Id != entity.Id && x.DeletedKey == null);
            //if (userpinQuery.Any())
            //{
            //    throw new RepositoryException("A user with that user pin already exists.");
            //}
            
            return;
        }
    }
}
