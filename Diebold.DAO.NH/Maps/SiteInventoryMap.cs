using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class SiteInventoryMap : TrackeableEntityMapping<SiteInventory>
    {
        public SiteInventoryMap()
        {
            Property(u => u.InventoryValue, c =>
            {
                c.NotNullable(true);
            });
            ManyToOne(u => u.CompanyInventory, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("CompanyInventoryId");
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
