using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class SiteAccountNumberMap : TrackeableEntityMapping<SiteAccountNumber>
    {
        public SiteAccountNumberMap()
        {
            Property(u => u.Date, c =>
            {
                c.NotNullable(true);
            });
            Property(u => u.AccountNumber, c =>
            {
                c.NotNullable(false);
            });
            Property(u => u.IsAssociatedWithFA, c =>
            {
                c.NotNullable(false);
            });
            Property(u => u.siteId, c =>
            {
                c.NotNullable(false);
            });  
            ManyToOne(u => u.User, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("UserId");
            });
            //ManyToOne(u => u.Site, mto =>
            //{
            //    mto.Fetch(FetchKind.Join);
            //    mto.NotNullable(true);
            //    mto.Column("SiteId");
            //});     
        }
    }
}
