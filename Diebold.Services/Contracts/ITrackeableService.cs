using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface ITrackeableService<T> where T : TrackeableEntity
    {
        void Enable(int pk);

        void Disable(int pk);
    }
}