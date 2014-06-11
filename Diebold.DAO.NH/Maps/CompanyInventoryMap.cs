using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class CompanyInventoryMap : TrackeableEntityMapping<CompanyInventory>
    {
        public CompanyInventoryMap()
        {
            Property(u => u.InventoryKey, c =>
            {
                c.NotNullable(true);
            });
            Property(u => u.ExternalCompanyId, c =>
            {
                c.NotNullable(true);
            });
        }
    }
}
