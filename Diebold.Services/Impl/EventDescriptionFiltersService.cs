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
    public class EventDescriptionFiltersService : BaseCRUDTrackeableService<EventDescriptionFilters>, IEventDescriptionFiltersService
    {
        public EventDescriptionFiltersService(IIntKeyedRepository<EventDescriptionFilters> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {        
            }

        public IList<EventDescriptionFilters> GetEventDescriptionByEventIds(List<string> eventIds)
        {
            var DataResult = _repository.All().Where(x=>eventIds.Select(y=>y).Contains(x.EventId));
            return DataResult.ToList();
        }
    }
}
