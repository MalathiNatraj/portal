using System;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;

namespace Diebold.Services.Impl
{
    public class MembershipService : BaseService, IMembershipService
    {
        private readonly IUserRepository _repository;

        public MembershipService(IUserRepository repository, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
        }

        public User GetUserByUserName(string userName)
        {
            User user;

            try
            {
                user = _repository.FindBy(u => u.Username == userName.Split('@')[0] && u.DeletedKey == null && u.IsDisabled == false);
            }
            catch (Exception e)
            {
                throw new Exception("Unknown User", e);
            }

            return user;
        }

        public bool UsernameExists(string userName)
        {
            bool usernameExists;

            try
            {
                _repository.FindBy(u => u.Username == userName.Split('@')[0] && u.DeletedKey == null && u.IsDisabled == false);
                usernameExists = true;
            }
            catch (Exception)
            {
                usernameExists = false;
            }

            return usernameExists;
        }
    }
}
