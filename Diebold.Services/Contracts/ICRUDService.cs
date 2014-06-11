using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;

namespace Diebold.Services.Contracts
{
    public interface ICRUDService<T> where T : IntKeyedEntity
    {
        void Create(T item);

        void Delete(int pk);

        void Update(T item);

        void Update(IList<T> item);

        T Get(int pk);

        IList<T> GetAll();

        //TODO: Should be replaced by GetPage.
        IList<T> GetAll(int pageNumber, int pageSize, string sortBy, bool ascending, out int recordCount);

        Page<T> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending);

        IList<T> LookUp();
    }
}
