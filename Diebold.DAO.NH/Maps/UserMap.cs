using NHibernate.Mapping.ByCode;
using Diebold.Domain.Entities;
using NHibernate.Type;

namespace Diebold.DAO.NH.Maps
{
    public class UserMap : TrackeableEntityMapping<User>
    {
        public UserMap()
        {
            Property(u => u.Email, c =>
            {
                c.NotNullable(true);
                c.Length(100);
                //c.UniqueKey("UQ_User_Email");
            });
            Property(u => u.FirstName, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });
            Property(u => u.LastName, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });
            Property(u => u.Username, c =>
            {
                c.NotNullable(true);
                c.Length(32);
                //c.UniqueKey("UQ_User_Username");
            });
            Property(u => u.Phone, c =>
            {
                c.NotNullable(true);
                c.Length(20);
            });
            Property(u => u.OfficePhone, c =>
            {
                c.Length(20);
            });
            Property(u => u.Extension, c =>
            {
                c.Length(10);
            });
            Property(u => u.Mobile, c =>
            {
                c.Length(20);
            });
            Property(u => u.Title, c =>
            {
                c.Length(50);
            });
            Property(u => u.Text1, c =>
            {
                c.Length(255);
            });
            Property(u => u.Text2, c =>
            {
                c.Length(255);
            });
            Property(u => u.UserPin, c =>
            {
                c.Length(6);
            });

            Property(u => u.PreferredContact, c =>
            {
                c.Type<EnumStringType<PreferredContact>>();
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.TimeZone, c =>
            {
                c.NotNullable(true);
                c.Length(50);
            });
            
            ManyToOne(u => u.Role, mtom =>
                {
                    mtom.Fetch(FetchKind.Join);
                    mtom.NotNullable(true);
                    mtom.Column("RoleId");
                }
            );

            ManyToOne(u => u.Company, mtom =>
            {
                mtom.Fetch(FetchKind.Join);
                mtom.NotNullable(true);
                mtom.Column("CompanyId");
            }
);
            Bag(u => u.Links, collectionMapping =>
            {
                collectionMapping.Key(key => key.Column("UserId"));
                collectionMapping.Cascade(Cascade.All);
                collectionMapping.Lazy(CollectionLazy.Lazy);
                collectionMapping.Inverse(false);
            }, map => map.OneToMany());

            Bag(u => u.RSSFeeds, collectionMapping =>
            {
                collectionMapping.Key(key => key.Column("UserId"));
                collectionMapping.Cascade(Cascade.All);
                collectionMapping.Lazy(CollectionLazy.Lazy);
                collectionMapping.Inverse(false);
            }, map => map.OneToMany());

            Bag(u => u.userPortletsPreferences, collectionMapping =>
            {
                collectionMapping.Key(key => key.Column("UserId"));
                collectionMapping.Cascade(Cascade.All);
                collectionMapping.Lazy(CollectionLazy.Lazy);
                collectionMapping.Inverse(false);
            }, map => map.OneToMany());
        }
    }
}
