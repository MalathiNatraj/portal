using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class GatewayMap : JoinedSubclassMapping<Gateway>
    {
        public GatewayMap()
        {
            Key(x => x.Column("Id"));

            Property(x => x.Name, mapping =>
            {
                mapping.NotNullable(true);
                mapping.Length(32);
            });
            
            Property(x => x.TimeZone, mapping =>
            {
                mapping.NotNullable(true);
                mapping.Length(32);
            });

            Property(x => x.Protocol, mapping =>
            {
                mapping.NotNullable(true);
            });


            //ManyToOne(x => x.Site, mapping =>
            //{
            //    mapping.Fetch(FetchKind.Join);
            //    mapping.Column("SiteId");
            //    mapping.NotNullable(true);

            //});
            
            Property(x => x.Protocol, mapping =>
            {
                mapping.NotNullable(true);
                mapping.Length(32);
            });

            Property(x => x.MacAddress, mapping =>
            {
                mapping.NotNullable(true);
                mapping.Length(32);
            });

            Property(x => x.EMCId);

            Property(u => u.ExternalDeviceId, c =>
            {
                c.NotNullable(true);
            });

            Property(u => u.ReportBufferSize, c =>
            {
                c.NotNullable(true);
            });

            Property(x => x.LastUpdate, c =>
            {
                c.NotNullable(true);
            });

            Property(u => u.Address1, c =>
            {
                c.Length(32);
            });

            Property(u => u.Address2, c =>
            {
                c.Length(32);
            });

            Property(u => u.City, c =>
            {
                c.Length(32);
            });

            Property(u => u.State, c =>
            {
                c.Length(32);
            });

            Property(u => u.Zip, c =>
            {
                c.Length(32);
            });

            Property(u => u.Notes, c =>
            {
                c.Length(512);
            });

            Property(x => x.LastUsedEmcZoneNumber, mapping => mapping.NotNullable(true));

            ManyToOne(u => u.Company, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("CompanyId");
            });   
        }
    }
}
