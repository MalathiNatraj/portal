using NHibernate.Mapping.ByCode;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace Diebold.DAO.NH.Maps
{
    public class DvrMap : JoinedSubclassMapping<Dvr>
    {
        public DvrMap()
        {
            Key(x => x.Column("Id"));

            Property(u => u.Name, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.HostName, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });

            Property(device => device.UserName, propertyMapper => propertyMapper.NotNullable(false));
            
            Property(device => device.Password, propertyMapper => propertyMapper.NotNullable(false));

            Property(device => device.PortA, propertyMapper => propertyMapper.NotNullable(false));

            Property(device => device.PortB, propertyMapper => propertyMapper.NotNullable(false));

            Property(device => device.IsInDST, propertyMapper => propertyMapper.NotNullable(true));

            Property(u => u.ExternalDeviceId, c =>
            {
                c.NotNullable(false);
                c.Length(32);
                c.Unique(true);
            });

            Property(u => u.DeviceKey, c =>
            {
                c.NotNullable(false);
                c.Length(32);
            });

            Property(u => u.DeviceType, c =>
            {
                c.Type<EnumStringType<DeviceType>>();
                c.NotNullable(true);
                c.Length(32);
            });
            
            Property(u => u.PollingFrequency, c =>
            {
                c.Type<EnumStringType<PollingFrequency>>();
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.HealthCheckVersion, c =>
            {
                c.Type<EnumStringType<HealthCheckVersion>>();
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.ZoneNumber, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.TimeZone, c =>
            {
                c.NotNullable(true);
                c.Length(50);
            });

            Property(u => u.NumberOfCameras, c => c.NotNullable(true));

            Property(u => u.ReportBufferSize, c => c.NotNullable(false));

           ManyToOne(u => u.Gateway, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("GatewayId");
            }
           );

           Bag(u => u.Cameras, collectionMapping =>
              {
                  collectionMapping.Key(key => key.Column("DvrId"));
                  collectionMapping.Cascade(Cascade.All);
                  collectionMapping.Lazy(CollectionLazy.Lazy);
                  collectionMapping.Inverse(false);
              }, map => map.OneToMany());

           ManyToOne(u => u.Site, mto =>
           {
               mto.Fetch(FetchKind.Join);
               mto.NotNullable(true);
               mto.Column("SiteId");
           });  
        }
    }
}
