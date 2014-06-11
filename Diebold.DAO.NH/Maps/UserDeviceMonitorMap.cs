using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class UserDeviceMonitorMap : IntKeyedEntityMapping<UserDeviceMonitor>
    {
        public UserDeviceMonitorMap()
        {
            ManyToOne(u => u.Device, mtom =>
                {
                    mtom.Fetch(FetchKind.Join);
                    mtom.NotNullable(true);
                    mtom.Column("DeviceId");
                });

            ManyToOne(u => u.User, mtom =>
                {
                    mtom.Fetch(FetchKind.Join);
                    mtom.NotNullable(true);
                    mtom.Column("UserId");
                    mtom.Index("UserDeviceMonitorMap_ByUserIndex");
                });

            ManyToOne(u => u.UserMonitorGroup, mtom =>
                {
                    mtom.Fetch(FetchKind.Join);
                    mtom.NotNullable(true); //should allow null? 
                    mtom.Column("UserMonitorGroupId");
                    mtom.Index("UserDeviceMonitorMap_ByGroupIndex");
                });

        }
    }
}
