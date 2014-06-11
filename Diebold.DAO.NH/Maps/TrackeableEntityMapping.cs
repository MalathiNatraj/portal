using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Maps
{
    public class TrackeableEntityMapping<T> : IntKeyedEntityMapping<T> where T : TrackeableEntity
    {
        public TrackeableEntityMapping()
        {
            Property(u => u.DeletedKey, c => c.NotNullable(false));

            Property(u => u.IsDisabled, c => c.NotNullable(true));
        }

        //protected override void RegisterPropertyMapping<TProperty>(System.Linq.Expressions.Expression
        //        <Func<T, TProperty>> property, Action<NHibernate.Mapping.ByCode.IPropertyMapper> mapping)
        //{
        //    if(property.Parameters.First().Type.Name == "Role")
        //        base.RegisterPropertyMapping<TProperty>(property, mapping);    
        //}

    }
}
