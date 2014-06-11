using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Contracts.Infrastructure;
using Ninject.Modules;
using System.Configuration;
using NHibernate;
using Diebold.DAO.NH.Infrastructure;
using Diebold.Domain.Contracts;
using Diebold.Domain;
using Diebold.DAO.NH.Helpers;

namespace Diebold.DAO.NH.Config
{
    public class NHModule : NinjectModule
    {
        public override void Load()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DieboldDB"].ConnectionString;

            CryptoUtility objCryptoUtility = new CryptoUtility();            
            var DecryptConString = objCryptoUtility.DecryptAES(connectionString);            

            var helper = new NHibernateHelper(DecryptConString);

            Bind<ISessionFactory>().ToConstant(helper.SessionFactory)
                .InSingletonScope();

            /*
            Bind<ISession>().ToProvider(new NHSessionProvider())
                .InRequestScope();
            */

            Bind<IUnitOfWork>().To<NHUnitOfWork>()
                .InRequestScope();

        }
    }
}
