using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface IEventDescriptionFiltersService : ICRUDTrackeableService<EventDescriptionFilters>
    {
        IList<EventDescriptionFilters> GetEventDescriptionByEventIds(List<string> eventIds);
    }
}
