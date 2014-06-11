using NHibernate;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Tool.hbm2ddl;

namespace Diebold.DAO.NH.Infrastructure
{
    public class NHibernateHelper
    {
        private readonly string _connectionString;

		private ISessionFactory _sessionFactory;
        

		public ISessionFactory SessionFactory
		{
			get { return _sessionFactory ?? (_sessionFactory = CreateSessionFactory()); }
		}

		public NHibernateHelper(string connectionString)
		{
			_connectionString = connectionString;
		}

		private ISessionFactory CreateSessionFactory()
        {   
            return Configuration.BuildSessionFactory();
		}

        private Configuration Configuration
        {
            get
            {
                var mapper = new ModelMapper();
                mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
                HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

                new SettingsFactory();

                var cfg = new Configuration();

                cfg.DataBaseIntegration(c =>
                {
                    c.LogFormattedSql = true;
                    c.LogSqlInConsole = true;

                    c.PrepareCommands = true;

                    c.Dialect<MsSql2008Dialect>();
                    c.ConnectionString = _connectionString;
                    c.BatchSize = 100;
                    c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;

                    //c.ExceptionConverter<MSSQLExceptionConverter>();
                });

                cfg.AddMapping(domainMapping);

                return cfg;
            }
        }

        public void CreateSchema()
        {
            new SchemaExport(Configuration).SetOutputFile("ddl.sql").Create(true, true);
        }
    }
}
