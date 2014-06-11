using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class SiteNoteMap : TrackeableEntityMapping<SiteNote>
    {
        public SiteNoteMap()
        {
            Property(u => u.Date, c =>
            {
                c.NotNullable(true);
            });
            Property(u => u.Text, c =>
            {
                c.NotNullable(false);
            });
            ManyToOne(u => u.User, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("UserId");
            });
            ManyToOne(u => u.Site, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("SiteId");
            });     
        }
    }
}
