using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class SiteDocumentMap : TrackeableEntityMapping<SiteDocument>
    {
        public SiteDocumentMap()
        {
            Property(u => u.Date, c =>
            {
                c.NotNullable(true);
            });
            Property(u => u.FileName, c =>
            {
                c.NotNullable(true);
            });
            Property(u => u.IsPrimary, c =>
            {
                c.NotNullable(true);
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
