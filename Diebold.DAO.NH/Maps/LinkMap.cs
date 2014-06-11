using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class LinkMap : TrackeableEntityMapping<Link>
    {
        public LinkMap()
        {
            Property(u => u.Name, c =>
            {
                c.NotNullable(true);
            });
            Property(u => u.Url, c =>
            {
                c.NotNullable(false);
            });
            Property(u => u.Description, c =>
            {
                c.NotNullable(false);
            });
            ManyToOne(u => u.User, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("UserId");
            });            
        }

    }
}
