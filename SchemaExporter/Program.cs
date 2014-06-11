using System;
using System.Configuration;
using Diebold.DAO.NH.Infrastructure;

namespace SchemaExporter
{
    class Program
    {
        static void Main(string[] args)
        {

            //IKernel kernel = new StandardKernel();
            //kernel.Load("*.dll");

            var connectionString = ConfigurationManager.ConnectionStrings["DieboldDB"].ConnectionString;
            var helper = new NHibernateHelper(connectionString);

            helper.CreateSchema();

            Console.Read();
        }
    }
}
