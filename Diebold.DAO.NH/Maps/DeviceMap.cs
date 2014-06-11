using NHibernate;
using NHibernate.Mapping.ByCode;
using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Maps
{
    public class DeviceMap : TrackeableEntityMapping<Device>
    {
        public DeviceMap()
        {
            Property(u => u.Name, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.ExternalDeviceId, c =>
            {
                c.NotNullable(false);
                c.Length(32);
                c.Unique(true);
            });
            
            Property(u => u.ReportBufferSize, c => c.NotNullable(false));

            ManyToOne(u => u.Company, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("CompanyId");
            });

            Bag(u => u.AlarmConfigurations, collectionMapping =>
            {
                collectionMapping.Key(key => key.Column("DeviceId"));
                collectionMapping.Cascade(Cascade.All);
                collectionMapping.Lazy(CollectionLazy.Lazy);
                collectionMapping.Inverse(false);
            }, map => map.OneToMany());

            Bag(u => u.AlertStatus, collectionMapping =>
            {
                collectionMapping.Key(key => key.Column("DeviceId"));
                collectionMapping.Cascade(Cascade.All);
                collectionMapping.Lazy(CollectionLazy.Lazy);
                collectionMapping.Inverse(false);
            }, map => map.OneToMany());

            Bag(u => u.DeviceStatus, collectionMapping =>
            {
                collectionMapping.Key(key => key.Column("DeviceId"));
                collectionMapping.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                collectionMapping.Lazy(CollectionLazy.Lazy);
                collectionMapping.Inverse(false);
            }, map => map.OneToMany());

        }
    }
}
