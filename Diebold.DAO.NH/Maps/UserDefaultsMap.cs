using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace Diebold.DAO.NH.Maps
{
    public class UserDefaultsMap : IntKeyedEntityMapping<UserDefaults>
    {
        public UserDefaultsMap()
        {
            ManyToOne(u => u.User, c =>
            {
                c.Fetch(FetchKind.Join);
                c.Column("UserId");
                c.NotNullable(true);
            });

            Property(u => u.FilterName, c =>
            {
                c.NotNullable(false);
            });

            Property(u => u.FilterValue, c =>
            {
                c.NotNullable(false);
            });

            Property(u => u.InternalName, c =>
            {
                c.NotNullable(false);
            });

            Property(u => u.AlertType, c =>
            {
                c.NotNullable(false);
            });
        }
    }
}
