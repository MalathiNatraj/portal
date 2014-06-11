using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace Diebold.DAO.NH.Maps
{
    public class RoleActionMap
    {        
        public static Action<IComponentElementMapper<Diebold.Domain.Entities.RoleAction>> Mapping()
        {
            return c =>
            {
                c.Property(prop => prop.Action,
                    mapping =>
                    {
                        mapping.Type<EnumStringType<Diebold.Domain.Entities.Action>>();
                        mapping.Length(25);
                    });
            };
        }
        
    }

}
