using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class SiteMap : TrackeableEntityMapping<Site>
    {
        public SiteMap()
        {

            Property(u => u.SiteId, c =>
            {
                c.NotNullable(true);
            });

            Property(u => u.Name, c =>
                {
                    c.NotNullable(true);
                    c.Length(70);
                });
            
           
            Property(u => u.ParentAssociation, c =>
                {
                    c.NotNullable(true);
                    c.Length(70);
                });

            Property(u => u.DieboldName, c =>
                {
                    c.NotNullable(true);
                    c.Length(70);
                });

            Property(u => u.Address1, c =>
                {
                    c.NotNullable(true);
                    c.Length(70);
                });

            Property(u => u.Address2, c =>
                {
                    c.Length(70);
                });

            Property(u => u.City, c =>
                {
                    c.NotNullable(true);
                    c.Length(32);
                });

            Property(u => u.State, c =>
                {
                    c.NotNullable(true);
                    c.Length(52);
                });

            Property(u => u.Zip, c =>
                {
                    c.NotNullable(true);
                    c.Length(32);
                });

            Property(u => u.County, c =>
                {
                    c.NotNullable(true);
                    c.Length(60);
                });

            Property(u => u.Country, c =>
                {
                    c.NotNullable(true);
                    c.Length(60);
                });

            Property(u => u.Latitude, c =>
            {
                c.Length(16);
            });

            Property(u => u.Longitude, c =>
            {
                c.Length(16);
            });

            Property(u => u.CCMFStatus, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.Notes, c =>
            {
                c.Length(512);
            });

            Property(u => u.SharepointURL, c =>
            {
                c.Length(250);
            });

            /*
            Bag(u => u.Gateways,
               collectionMapping =>
               {
                   collectionMapping.Key(key => key.Column("SiteId"));
                   collectionMapping.Cascade(Cascade.Persist);
                   collectionMapping.Lazy(CollectionLazy.Lazy);
               }, map => map.OneToMany());
            */
            
            ManyToOne(u => u.CompanyGrouping2Level, mto =>
            {
                mto.Fetch(FetchKind.Select);
                mto.Lazy(LazyRelation.Proxy);
                mto.NotNullable(true);
                mto.Column("CompanyGrouping2LevelId");
            });
            Property(u => u.ContactName, c =>
            {
                c.Length(32);
            });

            Property(u => u.ContactEmail, c =>
            {
                c.Length(100);
            });

            Property(u => u.ContactNumber, c =>
            {
                c.Length(32);
            });
        }
    }
}
