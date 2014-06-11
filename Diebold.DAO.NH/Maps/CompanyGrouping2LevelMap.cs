using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class CompanyGrouping2LevelMap : IntKeyedEntityMapping<CompanyGrouping2Level>
    {
        public CompanyGrouping2LevelMap()
        {
            Property(u => u.Name, c =>
                    {
                        c.NotNullable(true);
                        c.Length(128);
                    });

            ManyToOne(u => u.CompanyGrouping1Level, mto =>
                    {
                        mto.Fetch(FetchKind.Join);
                        mto.NotNullable(true);
                        mto.Column("CompanyGrouping1LevelId");
                        Lazy(false);
                    });

            Bag(u => u.Sites,
                collectionMapping =>
                {
                    collectionMapping.Key(key => key.Column("CompanyGrouping2LevelId"));
                    collectionMapping.Cascade(Cascade.All);
                    collectionMapping.Lazy(CollectionLazy.NoLazy);
                    collectionMapping.Fetch(CollectionFetchMode.Join);
                    collectionMapping.Inverse(false);
                }, map => map.OneToMany());
        }
    }
}