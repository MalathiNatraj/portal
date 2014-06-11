using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class UserMonitorGroupMap : IntKeyedEntityMapping<UserMonitorGroup>
    {
        public UserMonitorGroupMap()
        {
            ManyToOne(u => u.User, mtom =>
            {
                mtom.Fetch(FetchKind.Join);
                mtom.NotNullable(true); //should allow null? 
                mtom.Column("UserId");
            });

            ManyToOne(u => u.FirstGroupLevel, mtom =>
            {
                mtom.Fetch(FetchKind.Join);
                mtom.NotNullable(false);
                mtom.Column("CompanyGrouping1LevelId");
            });

            ManyToOne(u => u.SecondGroupLevel, mtom =>
            {
                mtom.Fetch(FetchKind.Join);
                mtom.NotNullable(false);
                mtom.Column("CompanyGrouping2LevelId");
            });

            ManyToOne(u => u.Site, mtom =>
            {
                mtom.Fetch(FetchKind.Join);
                mtom.NotNullable(false);
                mtom.Column("SiteId");
            });

            ManyToOne(u => u.Device, mtom =>
            {
                mtom.Fetch(FetchKind.Join);
                mtom.NotNullable(false);
                mtom.Column("DeviceId");
            });
        }
    }
}
