using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace Diebold.DAO.NH.Maps
{
    public class CompanySusbscriptionMap
    {
        public static Action<IComponentElementMapper<Diebold.Domain.Entities.CompanySubscription>> Mapping()
                {
                    return c =>
                    {
                        c.Property(prop => prop.Subscription,
                            mapping =>
                            {
                                mapping.Type<EnumStringType<Diebold.Domain.Entities.Subscription>>();
                                mapping.Length(255);
                            });
                    };
                }
    }
}