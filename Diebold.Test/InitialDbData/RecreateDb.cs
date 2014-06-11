using System.Configuration;
using Diebold.DAO.NH.Infrastructure;
using Xunit;

namespace Diebold.Test.InitialDbData
{
    public class RecreateDb
    {
        [Fact]
        public void RecreateDatabaseWithData()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DieboldDB"].ConnectionString;
            var helper = new NHibernateHelper(connectionString);

            helper.CreateSchema();

            using (var session = helper.SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                new DataCreator(session).Create();
                transaction.Commit();
            }
        }
    }
}