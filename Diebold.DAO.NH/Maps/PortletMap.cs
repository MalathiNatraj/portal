using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class PortletMap : IntKeyedEntityMapping<Portlets>
    {
        public PortletMap()
        {            
            Property(u => u.Name, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.InternalName, c =>
            {
                c.NotNullable(true);
                c.Length(250);
            });

            Property(u => u.ColumnNo, c =>
            {
                c.NotNullable(true);
                c.Length(250);
            });
        }
    }
}
