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
    public class SiteInventoryService : BaseCRUDTrackeableService<SiteInventory>, ISiteInventoryService
    {
        private readonly ISiteInventoryRepository _siteInventoryRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ICompanyService _companyService;
        public SiteInventoryService(IIntKeyedRepository<SiteInventory> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider, ISiteInventoryRepository siteInventoryRepository, 
                           ISiteRepository siteRepository, ICompanyService companyService,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {
                _siteInventoryRepository = siteInventoryRepository;
                _siteRepository = siteRepository;
                _companyService = companyService;
            }

        public IList<SiteInventoryDetails> GetSiteInventoryBySiteandCompany(int SiteId)
        {
            // Get Company Id
            var SiteDetails = _siteRepository.All().Where(x => x.Id == SiteId);
            int CompanyId = _companyService.GetAll().Where(x => x.ExternalCompanyId == SiteDetails.First().CompanyGrouping2Level.CompanyGrouping1Level.Company.ExternalCompanyId && x.DeletedKey == null).Select(y => y.ExternalCompanyId).First();
            // Get Site Inventory Based on Site Id and Company Id
            IList<SiteInventoryDetails> lstSiteInventory = new List<SiteInventoryDetails>();
            lstSiteInventory = _siteInventoryRepository.GetSiteInventoryBySiteandCompany(SiteId, CompanyId);
            return lstSiteInventory;
        }

        public IList<SiteInventory> GetSiteInventorybySiteIdCompanyInventoryId(int SiteId, int CompanyInventoryId)
        {
            var ResultSet = _repository.All().Where(x => x.CompanyInventory.Id == CompanyInventoryId && x.Site.Id == SiteId && x.DeletedKey == null);
            return ResultSet.ToList();
        }
    }
}
