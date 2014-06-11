﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class EventDescriptionFiltersMap : TrackeableEntityMapping<EventDescriptionFilters>
    {
        public EventDescriptionFiltersMap()
        {
            Property(u => u.EventId, c =>
            {
                c.NotNullable(false);
            });
            Property(u => u.Description, c =>
            {
                c.NotNullable(false);
            });
            Property(u => u.Type, c =>
            {
                c.NotNullable(false);
            });
        }
    }
}
