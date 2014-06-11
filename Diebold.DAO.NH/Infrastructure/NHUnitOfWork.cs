using System;
using NHibernate;
using System.Data;
using Diebold.Domain.Contracts.Infrastructure;

namespace Diebold.DAO.NH.Infrastructure
{
    public class NHUnitOfWork : IUnitOfWork
    {
        private readonly ISessionFactory _sessionFactory;
		private ITransaction _transaction;

        private ISession _session; 

		public ISession Session 
        {
            get
            {
                //lazy session & transaction generator
                if (this._session == null)
                {

                    _session = _sessionFactory.OpenSession();
                    _session.FlushMode = FlushMode.Auto;

                    _transaction = _session.BeginTransaction(IsolationLevel.ReadCommitted);
                }

                return this._session;
            }           
        }

        public NHUnitOfWork(ISessionFactory sessionFactory)
		{
            
			_sessionFactory = sessionFactory;
            /*
			Session = _sessionFactory.OpenSession();
			Session.FlushMode = FlushMode.Auto;
            
			_transaction = Session.BeginTransaction(IsolationLevel.ReadCommitted);
            */
        }

		public void Dispose()
		{
            if ((_session != null) && (_session.IsOpen))
			{
                _session.Close();
			}

            //disposing session and transaction??
		}

		public virtual void Commit()
		{
			if (!_transaction.IsActive)
			{
				throw new InvalidOperationException("No active transation");
			}
			_transaction.Commit();
		}

		public virtual void Rollback()
		{
			if(_transaction.IsActive)
			{
				
                _transaction.Rollback();
			}
		}
    }
}
