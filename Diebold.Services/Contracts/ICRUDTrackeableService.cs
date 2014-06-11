using System.Collections.Generic;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface ICRUDTrackeableService<T> : ICRUDService<T>, ITrackeableService<T> where T : TrackeableEntity
    {
        IList<T> GetAllEnabled();
    }
}
