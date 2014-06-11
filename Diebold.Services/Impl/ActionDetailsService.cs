using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Impl
{
    public class ActionDetailsService : BaseCRUDService<ActionDetails>, IActionDetailsService
    {
        private readonly IActionDetailsRepository _ActionDetailsRepository;
        public ActionDetailsService(IIntKeyedRepository<ActionDetails> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService,
                            IActionDetailsRepository ActionDetailsRepository)
            : base(repository, unitOfWork, validationProvider, logService)
            {
                _ActionDetailsRepository = ActionDetailsRepository;
            }

        public IList<ActionDetails> GetAllMASHourDetails()
        {
            try
            {
                var ActionResults = _ActionDetailsRepository.GetAllActions();
                return ActionResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
