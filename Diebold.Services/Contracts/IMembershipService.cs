using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface IMembershipService
    {
        User GetUserByUserName(string userName);
        bool UsernameExists(string currentUserName);
    }
}
