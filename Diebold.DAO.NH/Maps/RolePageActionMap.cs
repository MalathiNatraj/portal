using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Maps
{
    public class RolePageActionMap : IntKeyedEntityMapping<RolePageActions>
    {
        public RolePageActionMap()
        {
            ManyToOne(u => u.Role, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("RoleId");
            });

            ManyToOne(u => u.ActionDetails, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("ActionDetailId");
            });   
           
        }
    }
}
