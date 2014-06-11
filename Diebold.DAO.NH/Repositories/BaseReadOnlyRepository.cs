using System;
using System.Linq;
using Diebold.DAO.NH.Infrastructure;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Diebold.DAO.NH.Repositories
{
    public class BaseReadOnlyRepository<T> : IReadOnlyRepository<T> where T : IntKeyedEntity
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseReadOnlyRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected ISession Session
        {
            get
            {
                return ((NHUnitOfWork)_unitOfWork).Session;
            }
        }

        public IQueryable<T> All()
        {
            return this.Session.Query<T>();
        }
        public IQueryable<T> All(int Skip, int Take)
        {
            return this.Session.Query<T>().Skip(Skip).Take(Take);
        }

        public T FindBy(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
           return FilterBy(expression).Single();
        }

        public IQueryable<T> FilterBy(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return this.Session.Query<T>().Where(expression);
        }
    }
}
