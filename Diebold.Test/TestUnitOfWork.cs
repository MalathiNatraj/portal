using Diebold.DAO.NH.Infrastructure;
using NHibernate;

namespace Diebold.Test
{

    public class GlobalUnitOfWork : NHUnitOfWork
    {
        public GlobalUnitOfWork(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public override void Commit()
        {
            //throw new System.NotImplementedException();
        }

        public override void Rollback()
        {
            //throw new System.NotImplementedException();
        }

        public void FinalCommit()
        {
            base.Commit();
        }

        public void FinalRollback()
        {
            base.Rollback();
        }
    }
}
