using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Contracts
{
    public interface ITrackeableEntityRepository<TEntity> : IIntKeyedRepository<TEntity> where TEntity : TrackeableEntity
    {
        void Enable(int id);

        void Disable(int id);
    }
}
