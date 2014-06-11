using Diebold.DAO.NH.Infrastructure;
using NHibernate;

namespace DataGenerator
{
    public class GlobalUnitOfWork : NHUnitOfWork
    {
        public GlobalUnitOfWork(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public override void Commit()
        {
            
        }

        public override void Rollback()
        {

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
