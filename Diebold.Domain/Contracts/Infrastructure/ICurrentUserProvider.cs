using Diebold.Domain.Entities;

namespace Diebold.Domain.Contracts.Infrastructure
{
    public interface ICurrentUserProvider
    {
        User CurrentUser { get; }
        bool UsernameExists { get; }
    }
}
