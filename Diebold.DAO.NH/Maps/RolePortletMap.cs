using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Maps
{
    public class RolePortletMap : IntKeyedEntityMapping<RolePortlets>
    {
        public RolePortletMap()
        {
            //Property(u => u.Role, c =>
            //            {
            //                c.NotNullable(true);
            //                c.Length(32);
            //            });

            //Property(u => u.Portlets, c =>
            //            {
            //                c.NotNullable(true);
            //                c.Length(32);
            //            });

            ManyToOne(u => u.Role, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("RoleId");
            });

            ManyToOne(u => u.Portlets, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("PortletId");
            });   
        }
    }
}
