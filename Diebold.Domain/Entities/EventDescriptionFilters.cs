using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class EventDescriptionFilters : TrackeableEntity
    {
        public string EventId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
