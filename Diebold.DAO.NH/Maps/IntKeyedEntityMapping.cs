using NHibernate.Mapping.ByCode;
using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Maps
{
    public class IntKeyedEntityMapping<T> : EntityMapping<T> where T : IntKeyedEntity
    {
        public IntKeyedEntityMapping()
        {
            Id(x => x.Id, mapper => mapper.Generator(Generators.HighLow, gmap => gmap.Params(new { max_low = 100 })));
        }
    }
}
