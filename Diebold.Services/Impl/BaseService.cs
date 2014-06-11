using Diebold.Domain.Contracts.Infrastructure;
using log4net;

namespace Diebold.Services.Impl
{
    public abstract class BaseService
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected static ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
