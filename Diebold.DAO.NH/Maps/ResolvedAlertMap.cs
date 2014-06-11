using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class ResolvedAlertMap : IntKeyedEntityMapping<ResolvedAlert>
    {
        public ResolvedAlertMap()
        {
            ManyToOne(u => u.User, mtom =>
            {
                mtom.Fetch(FetchKind.Join);
                mtom.NotNullable(true); 
                mtom.Column("UserId");
            });

            ManyToOne(u => u.Device, mtom =>
            {
                mtom.Fetch(FetchKind.Join);
                mtom.NotNullable(true); 
                mtom.Column("DeviceId");
            });

            ManyToOne(u => u.AlarmConfiguration, mtom =>
            {
                mtom.Fetch(FetchKind.Join);
                mtom.NotNullable(true); 
                mtom.Column("AlarmConfigurationId");
            });

            Property(x => x.AcknoledgeDate, mapping =>
            {
                mapping.NotNullable(true);
            });

            Property(u => u.ElementIdentifier, c =>
            {
                c.NotNullable(false);
                c.Length(128);
            });
        }
    }
}
