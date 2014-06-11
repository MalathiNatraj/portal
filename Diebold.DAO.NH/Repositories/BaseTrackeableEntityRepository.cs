using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Contracts;
using Diebold.Domain;
using NHibernate.Linq;

namespace Diebold.DAO.NH.Repositories
{
    public class BaseTrackeableEntityRepository<T> : BaseIntKeyedRepository<T>, ITrackeableEntityRepository<T> where T : TrackeableEntity
    {
        public BaseTrackeableEntityRepository(IUnitOfWork unitOfWork)
            :base(unitOfWork)
        {
        }
        
        public void Enable(int id)
        {
            T itemToEnable = this.Load(id);
            itemToEnable.IsDisabled = false;

            this.Session.Update(itemToEnable);
        }

        public void Disable(int id)
        {
            T itemToEnable = this.Load(id);
            itemToEnable.IsDisabled = true;

            this.Session.Update(itemToEnable);
        }

        public override bool Add(T entity)
        {
            entity.IsDisabled = false;
            entity.IsDeleted = false;

            return base.Add(entity);
        }

        public override bool Delete(T entity)
        {
            entity.IsDeleted = true;
            this.Session.Update(entity);

            return true;
        }

        public override IQueryable<T> All()
        {
            return this.Session.Query<T>().Where(x => x.IsDeleted == false);
        }
    }
}
