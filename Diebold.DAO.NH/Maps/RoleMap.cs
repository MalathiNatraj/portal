using Diebold.Domain.Entities;
using NHibernate.Type;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class RoleMap : TrackeableEntityMapping<Role>
    {
        public RoleMap()
        {
            Property(property => property.Name, mapping =>
            {
                mapping.NotNullable(true);
                mapping.Length(32);
                //mapping.UniqueKey("UQ_User_RoleComposite");
            });

            //Property(property => property.DeletedKey, mapping =>
            //{
            //    mapping.UniqueKey("UQ_User_RoleComposite");
            //});

            Set(prop => prop.Actions,
                collectionMapping =>
                {
                    collectionMapping.Table("RoleActions");
                    collectionMapping.Cascade(Cascade.All);
                    collectionMapping.Lazy(CollectionLazy.NoLazy);
                    collectionMapping.Key(keyMapping => keyMapping.Column("RoleId"));
                    collectionMapping.Fetch(CollectionFetchMode.Join);
                    collectionMapping.Inverse(false);
                },
                mapping =>
                    mapping.Element(map =>
                        map.Type<EnumStringType<Diebold.Domain.Entities.Action>>())
            );

            //Set(prop => prop.Portlets,
            //    collectionMapping =>
            //    {
            //        collectionMapping.Table("RolePortlets");
            //        collectionMapping.Cascade(Cascade.All);
            //        collectionMapping.Lazy(CollectionLazy.NoLazy);
            //        collectionMapping.Key(keyMapping => keyMapping.Column("RoleId"));
            //        collectionMapping.Fetch(CollectionFetchMode.Join);
            //        collectionMapping.Inverse(false);
            //    },
            //    mapping =>
            //        mapping.Element(map =>
            //            map.Type<Diebold.Domain.Entities.Portlets>())
            //);

            Bag(u => u.RolePortlets, collectionMapping =>
            {
                collectionMapping.Key(key => key.Column("RoleId"));
                collectionMapping.Cascade(Cascade.All);
                collectionMapping.Lazy(CollectionLazy.Lazy);
                collectionMapping.Inverse(false);
            }, map => map.OneToMany());

            Bag(u => u.RolePageActions, collectionmapping =>
            {
                collectionmapping.Key(key => key.Column("RoleId"));
                collectionmapping.Cascade(Cascade.All);
                collectionmapping.Lazy(CollectionLazy.Lazy);
                collectionmapping.Inverse(false);
            }, map => map.OneToMany());

        }
    }
}
