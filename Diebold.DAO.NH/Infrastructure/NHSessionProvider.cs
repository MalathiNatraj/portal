using Diebold.Domain.Contracts.Infrastructure;
using NHibernate;
using Ninject.Activation;
using Ninject;

namespace Diebold.DAO.NH.Infrastructure
{
    public class NHSessionProvider : Provider<ISession>
    {
        protected override ISession CreateInstance(IContext context)
        {
            NHUnitOfWork unitOfWork = (NHUnitOfWork)context.Kernel.Get<IUnitOfWork>();
            return unitOfWork.Session;
        }
    }
}
