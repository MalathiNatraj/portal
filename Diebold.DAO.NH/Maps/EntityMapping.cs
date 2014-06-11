using NHibernate.Mapping.ByCode.Conformist;

namespace Diebold.DAO.NH.Maps
{
    public class EntityMapping<T> : ClassMapping<T> where T : class
    {
        public EntityMapping()
        {
            DynamicInsert(true);
            DynamicUpdate(true);

            Lazy(false);            
        }
    }
}
