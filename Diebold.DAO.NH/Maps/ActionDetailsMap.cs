using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class ActionDetailsMap : IntKeyedEntityMapping<ActionDetails>
    {
        public ActionDetailsMap()
        {            
            Property(u => u.ActionKey, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.ActionValue, c =>
            {
                c.NotNullable(true);
                c.Length(40);
            });

        }
    }
}
