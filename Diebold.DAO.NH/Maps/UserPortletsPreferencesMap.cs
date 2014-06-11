using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using Diebold.Domain.Entities;


namespace Diebold.DAO.NH.Maps
{
    public class UserPortletsPreferencesMap : TrackeableEntityMapping<UserPortletsPreferences>
    {
        public UserPortletsPreferencesMap()
        {
            Property(u => u.SeqNo, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.ColumnNo, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });


            ManyToOne(u => u.User, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("UserId");
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
