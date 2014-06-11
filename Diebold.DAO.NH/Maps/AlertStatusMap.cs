using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class AlertStatusMap : IntKeyedEntityMapping<AlertStatus>
    {
        public AlertStatusMap()
        {   
            ManyToOne(x => x.Device, mapping =>
            {
                mapping.Fetch(FetchKind.Join);
                mapping.Column("DeviceId");
                mapping.NotNullable(true);
                //mapping.Lazy(LazyRelation.Proxy);
            });

            ManyToOne(x => x.Alarm, mapping =>
            {
                mapping.Fetch(FetchKind.Join);
                mapping.Column("AlarmConfigurationId");
                mapping.NotNullable(true);
            });

            /*
            Property(x => x.Severity, mapping =>
            {
                mapping.Type<EnumStringType<AlarmSeverity>>();
                mapping.NotNullable(true);
                mapping.Length(32);
            });
            */

            Property(x => x.AlertCount, mapping =>
            {
                mapping.NotNullable(true);
            });

            Property(x => x.IsAcknowledged, mapping =>
            {
                mapping.NotNullable(true);
            });

            Property(x => x.IsOk, mapping =>
            {
                mapping.NotNullable(true);
            });

            Property(x => x.FirstAlertTimeStamp, mapping =>
            {
                mapping.NotNullable(false);
            });

            Property(x => x.LastAlertTimeStamp, mapping =>
            {
                mapping.NotNullable(false);
            });

            Property(u => u.ElementIdentifier, c =>
            {
                c.NotNullable(false);
                c.Length(128);
            });
        }
    }
}
