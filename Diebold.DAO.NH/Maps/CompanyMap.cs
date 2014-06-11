using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace Diebold.DAO.NH.Maps
{
    public class CompanyMap : TrackeableEntityMapping<Company>
    {
        public CompanyMap()
        {
            Property(u => u.ExternalCompanyId, c =>
                                           {
                                               c.NotNullable(true);
                                               c.Length(32);
                                               c.Unique(true);
                                           });

            Property(u => u.Name, c =>
                                      {
                                          c.NotNullable(true);
                                          c.Length(32);
                                      });

            Property(u => u.FirstLevelGrouping, c =>
                                                    {
                                                        c.NotNullable(true);
                                                        c.Length(32);
                                                    });

            Property(u => u.SecondLevelGrouping, c =>
                                                     {
                                                         c.NotNullable(true);
                                                         c.Length(32);
                                                     });

            Property(u => u.ThirdLevelGrouping, c =>
                                                    {
                                                        c.NotNullable(true);
                                                        c.Length(32);
                                                    });

            Property(u => u.FourthLevelGrouping, c =>
                                                     {
                                                         c.NotNullable(true);
                                                         c.Length(32);
                                                     });

            Property(u => u.PrimaryContactName, c =>
                                                    {
                                                        c.NotNullable(true);
                                                        c.Length(32);
                                                    });

            Property(u => u.PrimaryContactExtension, c =>
                                                         {
                                                             c.NotNullable(false);
                                                             c.Length(32);
                                                         });

            Property(u => u.PrimaryContactOffice, c =>
                                                      {
                                                          c.NotNullable(true);
                                                          c.Length(32);
                                                      });

            Property(u => u.PrimaryContactMobile, c =>
                                                      {
                                                          c.NotNullable(false);
                                                          c.Length(32);
                                                      });

            Property(u => u.PrimaryContactOfficePreferred, c => c.NotNullable(true));

            Property(u => u.PrimaryContactMobilePreferred, c => c.NotNullable(true));

            Property(u => u.ReportingFrequency, c =>
                                                    {
                                                        c.Type<EnumStringType<ReportingFrequency>>();
                                                        c.NotNullable(true);
                                                        c.Length(32);
                                                    });

            Property(u => u.PrimaryContactEmail, c =>
                                                     {
                                                         c.NotNullable(true);
                                                         c.Length(100);
                                                     });

            Set(u => u.Subscriptions,
                collectionMapping =>
                    {
                        collectionMapping.Table("CompanySubscription");
                        collectionMapping.Cascade(Cascade.All);
                        collectionMapping.Lazy(CollectionLazy.NoLazy);
                        collectionMapping.Key(keyMapping => keyMapping.Column("CompanyId"));
                        collectionMapping.Fetch(CollectionFetchMode.Join);
                        collectionMapping.Inverse(false);
                    },
                mapping =>
                mapping.Element(map =>
                                map.Type<EnumStringType<Diebold.Domain.Entities.Subscription>>())
                );
            
            Bag(u => u.Devices,
               collectionMapping =>
               {
                   collectionMapping.Key(key => key.Column("CompanyId"));
                   collectionMapping.Cascade(Cascade.None);
                   collectionMapping.Lazy(CollectionLazy.Lazy);
                   collectionMapping.Inverse(true);
               }, map => map.OneToMany());
            
            Bag(u => u.CompanyGrouping1Levels,
                collectionMapping =>
                {
                    collectionMapping.Key(key => key.Column("CompanyId"));
                    collectionMapping.Cascade(Cascade.All); //.Include(Cascade.DeleteOrphans));
                    collectionMapping.Lazy(CollectionLazy.NoLazy);
                    collectionMapping.Inverse(true);
                }, map => map.OneToMany());

            Property(u => u.FileName, c =>
            {
                c.NotNullable(false);
                c.Length(32);
            });

            Property(u => u.CompanyLogo, c =>
            {
                c.NotNullable(false);
                c.Length(100000);
            });
        }
    }
}
