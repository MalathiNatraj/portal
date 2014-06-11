using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class AlertInfoMap : IntKeyedEntityMapping<AlertInfo>
    {
        public AlertInfoMap()
        {
            ManyToOne(x => x.Device, mapping =>
            {
                mapping.Fetch(FetchKind.Join);
                mapping.Column("DeviceId");
                mapping.NotNullable(true);
            });

            ManyToOne(x => x.Alarm, mapping =>
            {
                mapping.Fetch(FetchKind.Join);
                mapping.Column("AlarmConfigurationId");
                mapping.NotNullable(true);
            });

            Property(x => x.DateOccur, c => c.NotNullable(true));

            Property(x => x.IsDeviceOk, c => c.NotNullable(true));

            Property(x => x.GroupId, c => c.NotNullable(true));

            Property(x => x.Value, c => c.NotNullable(true));

            Property(u => u.ElementIdentifier, c =>
            {
                c.NotNullable(false);
                c.Length(128);
            });
        }
    }
}
