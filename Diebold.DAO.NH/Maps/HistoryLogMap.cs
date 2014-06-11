using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace Diebold.DAO.NH.Maps
{
    public class HistoryLogMap : IntKeyedEntityMapping<HistoryLog>
    {
        public HistoryLogMap()
        {
            Property(u => u.Action, c =>
            {
                c.Type<EnumStringType<LogAction>>();
                c.NotNullable(true);
            });

            Property(u => u.Date, c =>
            {
                c.NotNullable(true);
            });

            ManyToOne(u => u.User, c =>
            {
                c.Fetch(FetchKind.Join);
                c.Column("UserId");
                c.NotNullable(true);
            });

            Property(u => u.Description, c =>
            {
                c.NotNullable(false);
            });


            
            ManyToOne(u => u.CompanyGrouping1Level, c =>
            {
                c.Fetch(FetchKind.Join);
                c.Column("CompanyGrouping1LevelId");
                c.NotNullable(false);
            });
            
            ManyToOne(u => u.CompanyGrouping2Level, c =>
            {
                c.Fetch(FetchKind.Join);
                c.Column("CompanyGrouping2LevelId");
                c.NotNullable(false);
            });

            ManyToOne(u => u.Site, c =>
            {
                c.Fetch(FetchKind.Join);
                c.Column("SiteId");
                c.NotNullable(false);
            });

            ManyToOne(u => u.Device, c =>
            {
                c.Fetch(FetchKind.Join);
                c.Column("DeviceId");
                c.NotNullable(false);
            });

            //Lazy(true);
        }
    }
}
