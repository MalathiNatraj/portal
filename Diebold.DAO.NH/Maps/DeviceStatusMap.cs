using System.Data;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Diebold.DAO.NH.Maps
{
    public class DeviceStatusMap : IntKeyedEntityMapping<DeviceStatus>
    {
        public DeviceStatusMap()
        {
            Property(x => x.Name, mapping =>
            {
                mapping.NotNullable(true);
            });

            Property(x => x.DataType, mapping =>
            {
                mapping.NotNullable(false);
                mapping.Type<EnumStringType<DataType>>();
            });

            Property(x => x.Value, mapping =>
            {
                mapping.NotNullable(false);
                mapping.Length(600);
            });

            Property(x => x.IsCollection, mapping =>
            {
                mapping.NotNullable(true);
            });
        }
    }
}
