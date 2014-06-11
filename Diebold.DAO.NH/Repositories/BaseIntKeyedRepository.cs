using System.Collections.Generic;
using Diebold.Domain.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts.Infrastructure;

namespace Diebold.DAO.NH.Repositories
{
    public class BaseIntKeyedRepository<T> : BaseReadOnlyRepository<T>, IIntKeyedRepository<T> where T : IntKeyedEntity
    {
        public BaseIntKeyedRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        
        public T Load(int id)
        {
            return this.Session.Load<T>(id);
        }

        public void Refresh(T item)
        {
            this.Session.Refresh(item);
        }

        public void Evict(T item)
        {
            this.Session.Evict(item);
        }

        public void Merge(T item)
        {
            this.Session.Merge(item);
        }

        public virtual bool Add(T entity)
        {
            Validate(entity);
            this.Session.Save(entity);
            return true;
        }

        public bool Add(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Validate(item);
                this.Session.Save(item);
            }

            return true;
        }

        public virtual bool Update(T entity)
        {
            Validate(entity);
            this.Session.Update(entity);
            return true;
        }

        public virtual bool Delete(T entity)
        {
            this.Session.Delete(entity);
            return true;
        }

        public bool Delete(IEnumerable<T> entities)
        {
            foreach (T item in entities)
            {
                this.Session.Delete(item);
            }

            return true;
        }

        public T FindBy(int id)
        {
            //puede devolver null si no lo encuentra
            return this.Session.Get<T>(id);
        }

        protected virtual void Validate(T entity)
        {
            
        }
    }
}