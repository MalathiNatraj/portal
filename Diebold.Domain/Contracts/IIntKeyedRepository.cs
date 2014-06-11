using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.Domain.Contracts
{
    public interface IIntKeyedRepository<TEntity> : IRepository<TEntity> where TEntity : IntKeyedEntity
    {
        TEntity FindBy(int id);

        TEntity Load(int id);

        void Refresh(TEntity item);

        void Evict(TEntity item);
        void Merge(TEntity item);
    }
}
