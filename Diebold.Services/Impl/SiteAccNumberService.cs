using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Infrastructure;
using System.Transactions;

namespace Diebold.Services.Impl
{
    public class SiteAccNumberService : BaseCRUDTrackeableService<SiteAccountNumber>, ISiteAccNumberService
    {
        public SiteAccNumberService(IIntKeyedRepository<SiteAccountNumber> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {        
            }

        //public IList<SiteAccountNumber> GetSiteAccNumberbySiteId(int SiteId, int UserId)
        //{
        //    return _repository.All().Where(x => x.siteId == SiteId && x.User.Id == UserId && x.DeletedKey == null).ToList();            
        //}

        public IList<SiteAccountNumber> GetSiteAccNumberbySiteId(int SiteId)
        {
            return _repository.All().Where(x => x.siteId == SiteId && x.DeletedKey == null).ToList();
        }        

        public void DeleteSiteAccountNumber(int SiteId)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {

                    var lstSiteAccNumber = (from siteAccNumber in _repository.All() where siteAccNumber.siteId == SiteId select siteAccNumber);

                    if (lstSiteAccNumber.Count() > 0)
                    {
                        IEnumerable<SiteAccountNumber> siteAccountNumbers = (IEnumerable<SiteAccountNumber>)lstSiteAccNumber.ToList();
                        if (siteAccountNumbers != null && siteAccountNumbers.Count() > 0)
                            _repository.Delete(siteAccountNumbers);
                    }
                    _unitOfWork.Commit();
                    scope.Complete();
                }
            }            
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw e;
            }
        }
    }
}
