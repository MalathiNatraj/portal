using System;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Repositories
{
    public class AlertStatusRepository : BaseIntKeyedRepository<AlertStatus>, IAlertStatusRepository
    {
        public AlertStatusRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public int GetCountOKDevicesByUser(int userId)
        {
            var query = Session.CreateQuery("select count(distinct status.Device.id) from AlertStatus status " +
                                                " where status.Device.id not in " +
                                                "(select status2.Device.id from AlertStatus status2 where IsOk = 0 or IsAcknowledged = 0) " +
                                                " and exists (select monitor.Device.id from UserDeviceMonitor monitor " +
                                                    "where User.id = :userId and monitor.Device.id = status.Device.id " +
                                                " and monitor.Device.IsDisabled = 0 and monitor.Device.DeletedKey = null " +
                                                " and monitor.Device.Gateway.IsDisabled = 0 and monitor.Device.Gateway.DeletedKey = null " +
                                                " and monitor.Device.Site.IsDisabled = 0 and monitor.Device.Site.DeletedKey = null "+
                                                " and monitor.Device.Company.IsDisabled = 0 and monitor.Device.Company.DeletedKey = null) ");
            query.SetParameter("userId", userId);

            return (int)query.UniqueResult<Int64>();
        }

        public int GetCountAlertedDevicesByUser(int userId)
        {
            var query = Session.CreateQuery("select count(distinct status.Device.id) from AlertStatus status " +
                                                    "where (IsOk = 0 or IsAcknowledged = 0) " +
                                                    " and exists (select monitor.Device.id from UserDeviceMonitor monitor where User.id = :userId and monitor.Device.id = status.Device.id " +
                                                    " and monitor.Device.IsDisabled = 0 and monitor.Device.DeletedKey = null " +
                                                    " and monitor.Device.Gateway.IsDisabled = 0 and monitor.Device.Gateway.DeletedKey = null " +
                                                    " and monitor.Device.Site.IsDisabled = 0 and monitor.Device.Site.DeletedKey = null "+
                                                    " and monitor.Device.Company.IsDisabled = 0 and monitor.Device.Company.DeletedKey = null) ");
            query.SetParameter("userId", userId);

            return (int)query.UniqueResult<Int64>();
        }
    }
}
