using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class CompanyGrouping1LevelMap : IntKeyedEntityMapping<CompanyGrouping1Level>
    {
        public CompanyGrouping1LevelMap()
        {
            Property(u => u.Name, c =>
            {
                c.NotNullable(true);
                c.Length(128);
            });

            ManyToOne(u => u.Company, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("CompanyId");
            });
            
            Bag(u => u.CompanyGrouping2Levels,
               collectionMapping =>
               {
                   collectionMapping.Key(key => key.Column("CompanyGrouping1LevelId"));
                   collectionMapping.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                   collectionMapping.Lazy(CollectionLazy.NoLazy);
                   collectionMapping.Fetch(CollectionFetchMode.Join);
                   collectionMapping.Inverse(true);
               }, map => map.OneToMany());
        }
    }
}
