using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Impl
{
    public class PortletService : BaseCRUDService<Portlets>, IPortletService
    {
        public PortletService(IIntKeyedRepository<Portlets> repository, IUnitOfWork unitOfWork,
                                IValidationProvider validationProvider, ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {
            }
        public IList<Portlets> GetAllUserPortlet()
        {
            var lstPortlet = _repository.All().ToList();
            return lstPortlet;
        }

        public Portlets GetById(int intId)
        {
            Portlets objPortlet;
            try
            {
                objPortlet = _repository.FindBy(u => u.Id == intId);
            }
            catch (Exception e)
            {
                throw new Exception("Unknown User", e);
            }
            return objPortlet;
        }

        public Portlets GetByInternalName(string strInternalName)
        {
            Portlets objPortlet;
            try
            {
                objPortlet = _repository.FindBy(u => u.InternalName.Equals(strInternalName));
            }
            catch (Exception e)
            {
                throw new Exception("Unknown User", e);
            }
            return objPortlet;
        }

        //public IList<Portlets> GetByQuery(string strQry)
        //{

        //    var query = Session.CreateQuery("select count(distinct status.Device.id) from AlertStatus status " +
        //                                        " where status.Device.id not in " +
        //                                        "(select status2.Device.id from AlertStatus status2 where IsOk = 0 or IsAcknowledged = 0) " +
        //                                        " and exists (select monitor.Device.id from UserDeviceMonitor monitor " +
        //                                            "where User.id = :userId and monitor.Device.id = status.Device.id " +
        //                                        " and monitor.Device.IsDisabled = 0 and monitor.Device.DeletedKey = null " +
        //                                        " and monitor.Device.Gateway.IsDisabled = 0 and monitor.Device.Gateway.DeletedKey = null " +
        //                                        " and monitor.Device.Gateway.Site.IsDisabled = 0 and monitor.Device.Gateway.Site.DeletedKey = null " +
        //                                        " and monitor.Device.Company.IsDisabled = 0 and monitor.Device.Company.DeletedKey = null) ");
        //    //query.SetParameter("userId", userId);

        //    return (int)query.UniqueResult<Int64>();

        //    var lstPortlet = _repository.All().ToList();
        //    return lstPortlet;
        //}
    }
}
