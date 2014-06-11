using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;


namespace Diebold.DAO.NH.Repositories
{
    public class UserDeviceMonitorRepository : BaseIntKeyedRepository<UserDeviceMonitor>, IUserDeviceMonitorRepository
    {
        public UserDeviceMonitorRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public int GetCountOfMonitoredDevicesByUser(int userId)
        {
            //use this HQL instead of a LINQ query, for performance.
            //this HQL query is not joining the USER table to perform the UserId Filter
            var query = this.Session.CreateQuery("select count(*) from UserDeviceMonitor where User.id = :userId " +
                " and Device.IsDisabled = 0 and Device.DeletedKey = null ");
            query.SetParameter("userId", userId);
            
            return (int)query.UniqueResult<Int64>();
        }

        public int GetCountOfMonitoredDevicesByUserAndSite(int userId, int siteId)
        {
            /*
            var query = Session.Query<UserDeviceMonitor>().Where(p => p.User.Id == userId && p.Device.Gateway.Site.Id == siteId);
            return query.Count();
            */

            //use this HQL instead of a LINQ query, for performance.
            //this HQL query is not joining the USER table to perform the UserId Filter
            var query = this.Session.CreateQuery("select count(*) from UserDeviceMonitor where User.id = :userId and Device.Site.id = :siteId " +
                " and Device.IsDisabled = 0 and Device.DeletedKey = null ");
            query.SetParameter("userId", userId);
            query.SetParameter("siteId", siteId);

            return (int)query.UniqueResult<Int64>();
        }

        public IList<User> GetUsersMonitoringDevice(int deviceId)
        {
            return this.Session.QueryOver<UserDeviceMonitor>().Where(userDevices => userDevices.Device.Id == deviceId).Select(
                userDevices => userDevices.User).List<User>();
        }

        public int GetCountOfMonitoredGatewaysByUser(int userId)
        {
            //use this HQL instead of a LINQ query, for performance.
            //this HQL query is not joining the USER table to perform the UserId Filter
            var query = this.Session.CreateQuery("select count(distinct Device.Gateway) from UserDeviceMonitor where User.id = :userId " +
                " and Device.IsDisabled = 0 and Device.DeletedKey = null ");
            query.SetParameter("userId", userId);

            return (int)query.UniqueResult<Int64>();
        }
        public int GetCountOfMonitoredDevicesByUserAndDeviceType(int userId, string deviceType)
        {
            //use this HQL instead of a LINQ query, for performance.
            //this HQL query is not joining the USER table to perform the UserId Filter
            var query = this.Session.CreateQuery("select count(*) from UserDeviceMonitor where User.id = :userId " +
                //" and Device.IsDisabled = 0 and Device.DeletedKey = null and Device.DeviceType = deviceType");
                " and Device.IsDisabled = 0 and Device.DeletedKey = null");
            query.SetParameter("userId", userId);
            query.SetParameter("deviceType", deviceType);

            return (int)query.UniqueResult<Int64>();
        }

        public IList<Device> GetDevicesByUser(int userId)
        {
            var devices = this.Session.QueryOver<UserDeviceMonitor>().Where(User => User.Id == userId).Select(
                userDevices => userDevices.Device).List<Device>();

            return devices;
        }
    }
}
