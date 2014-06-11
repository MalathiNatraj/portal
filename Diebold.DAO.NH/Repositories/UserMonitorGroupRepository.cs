using System;
using System.Collections.Generic;
using System.Linq;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using System.Text;
using NHibernate.Transform;

namespace Diebold.DAO.NH.Repositories
{
    public class UserMonitorGroupRepository : BaseIntKeyedRepository<UserMonitorGroup>, IUserMonitorGroupRepository
    {
        public UserMonitorGroupRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        
        public IList<int> GetMonitorGroupMatches(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id)
        {
           var query = this.Session.CreateQuery("select id from UserMonitorGroup where Device.id is null and "+
                                                 "(Site.id = :siteId or " +
                                                 "FirstGroupLevel.id = :firstGroupLevelId " +
                                                 "or SecondGroupLevel.id = :secondGroupLevelId) " +
                                                 "order by siteId desc,CompanyGrouping2LevelId desc,CompanyGrouping1LevelId desc");
            
            query.SetParameter("siteId", siteId);
            query.SetParameter("firstGroupLevelId", groupingLevel1Id);
            query.SetParameter("secondGroupLevelId", groupingLevel2Id);

            return query.List<int>();
        }

        public IList<UserMonitorGroup> GetUserMonitorGroup(int gatewayId, int siteId, int groupingLevel1Id, int groupingLevel2Id)
        {
            return FilterBy(x => x.Device == null && (x.Site.Id == siteId || x.FirstGroupLevel.Id == groupingLevel1Id || x.SecondGroupLevel.Id == groupingLevel2Id)).ToList<UserMonitorGroup>();
        }

        public bool IsUserMonitoringGateway(int userId, int gatewayId)
        {
            var query = this.Session.CreateQuery("select count(gateway.id) from UserMonitorGroup where User.id = :userId and Gateway.id = :gatewayId");
            query.SetParameter("userId", userId);
            query.SetParameter("gatewayId", gatewayId);

            return query.UniqueResult<Int64>() > 0;
        }

        public bool IsUserMonitoringSite(int userId, int siteId)
        {
            var query = this.Session.CreateQuery("select count(Site.id) from UserMonitorGroup where User.id = :userId and Site.id = :siteId");
            query.SetParameter("userId", userId);
            query.SetParameter("siteId", siteId);

            return query.UniqueResult<Int64>() > 0;
        }

        public bool IsUserMonitoringGroupingLevel1(int userId, int groupingLevel1Id)
        {
            var query = this.Session.CreateQuery("select count(FirstGroupLevel.id) from UserMonitorGroup where User.id = :userId and FirstGroupLevel.id = :firstGroupLevelId");
            query.SetParameter("userId", userId);
            query.SetParameter("firstGroupLevelId", groupingLevel1Id);

            return query.UniqueResult<Int64>() > 0;
        }

        public bool IsUserMonitoringGroupingLevel2(int userId, int groupingLevel2Id)
        {
            var query = this.Session.CreateQuery("select count(SecondGroupLevel.id) from UserMonitorGroup where User.id = :userId and SecondGroupLevel.id = :secondGroupLevelId");
            query.SetParameter("userId", userId);
            query.SetParameter("secondGroupLevelId", groupingLevel2Id);

            return query.UniqueResult<Int64>() > 0;
        }

        public bool IsGroupingLevel1MonitoringByUsers(int groupingLevel1Id)
        {
            var query = this.Session.CreateQuery("select count(User.id) from UserMonitorGroup where FirstGroupLevel.id = :firstGroupLevelId");
            query.SetParameter("firstGroupLevelId", groupingLevel1Id);

            return query.UniqueResult<Int64>() > 0;
        }

        public bool IsGroupingLevel2MonitoringByUsers(int groupingLevel2Id)
        {
            var query = this.Session.CreateQuery("select count(User.id) from UserMonitorGroup where SecondGroupLevel.id = :secondGroupLevelId");
            query.SetParameter("secondGroupLevelId", groupingLevel2Id);

            return query.UniqueResult<Int64>() > 0;
        }


        public IList<CompanyGrouping1Level> GetMonitoringGrouping1LevelsByUser(int userId)
        {
            var query = All().Where(x => x.User.Id == userId && x.FirstGroupLevel != null && x.SecondGroupLevel == null)
                             .Select(umg => umg.FirstGroupLevel);

            return query.ToList();
        }

        public IList<UserMonitorGroup> GetUserMonitorGroupByUser(int userId)
        {
            var query = All().Where(x => x.User.Id == userId);
            return query.ToList();
        }

        public IList<CompanyGrouping2Level> GetMonitoringGrouping2LevelsByUser(int userId, int? firstGroupLevelId)
        {
            var query = All().Where(x => x.User.Id == userId && x.SecondGroupLevel != null && x.Site == null);

            if (firstGroupLevelId != null)
                query = query.Where(x => x.FirstGroupLevel.Id == firstGroupLevelId);

            return query.Select(umg => umg.SecondGroupLevel).ToList();
        }


        public IList<Site> GetMonitoringSitesByUser(int userId, int? secondGroupLevelId)
        {
            var query = All().Where(x => x.User.Id == userId && x.Site != null && x.Device == null && x.Site.DeletedKey == null);

            if (secondGroupLevelId != null)
                query = query.Where(x => x.SecondGroupLevel.Id == secondGroupLevelId);

            return query.Select(umg => umg.Site).ToList();
        }

        public IList<Dvr> GetMonitoringDevicesByUser(int userId, int? siteId)
        {
            var query = All().Where(x => x.User.Id == userId && x.Device != null && x.Device.DeletedKey == null);

            if (siteId != null)
                query = query.Where(x => x.Site.Id == siteId);

            return query.Select(umg => (Dvr)umg.Device).ToList();
        }
        public IList<Device> GetSitesByUser(int userId, int? secondGroupLevelId)
        {
            //var query = All().Where(x => x.User.Id == userId && x.Site != null && x.Site.DeletedKey == null).Select(y=> y.Device);
            //var query = All().Where(x => x.User.Id == userId && x.Site != null && x.Site.DeletedKey == null);

            //if (secondGroupLevelId != null)
            //    query = query.Where(x => x.SecondGroupLevel.Id == secondGroupLevelId);

            //return query.Select(umg => umg.Device).ToList();
            var query = All().Where(x => x.User.Id == userId && x.Site.DeletedKey == null).Select(y => y.Device);
            return query.ToList();
        }

        public IList<UserMonitorGroup> GetByDeviceId(int deviceId)
        {
            var query = All().Where(x => x.Device.Id == deviceId);
            if (query != null)
                return query.ToList();
            else
                return new List<UserMonitorGroup>();
        }

        public IList<DeviceCounts> GetDeviceCountsByUser(int userId)
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append("select count(dvr.id) DeviceCount, 'Intrusion' DeviceType  from  dvr dvr inner join Device dv on dvr.Id = dv.Id  inner join UserMonitorGroup ug on dv.Id = ug.DeviceId where dvr.DeviceType in('dmpXR500','dmpXR100') and dv.DeletedKey is null and  ug.UserId =" + userId);
            sbQuery.Append(" union ");
            sbQuery.Append("select COUNT(dvr.id) DeviceCount,'Health' DeviceType from dvr dvr inner join Device dv on dvr.Id = dv.Id inner join UserMonitorGroup ug on dv.Id = ug.DeviceId where dvr.DeviceType in('Costar111','VerintEdgeVr200','ipConfigure530') and dv.DeletedKey is null and ug.UserId =" + userId);
            sbQuery.Append(" union ");
            sbQuery.Append("select COUNT(dvr.id) DeviceCount,'Access' DeviceType   from dvr dvr inner join Device dv on dvr.Id = dv.Id inner join UserMonitorGroup ug on dv.Id = ug.DeviceId where dvr.DeviceType in('eData300','eData524') and dv.DeletedKey is null and ug.UserId =" + userId);
           
           var query = this.Session.CreateSQLQuery(sbQuery.ToString());
           query.SetResultTransformer(Transformers.AliasToBean(typeof(DeviceCounts)));
           var result = query.List<DeviceCounts>();
           return result;
        }
    }
}
