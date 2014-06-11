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
    public class CompanyInventoryService : BaseCRUDTrackeableService<CompanyInventory>, ICompanyInventoryService
    {
        private readonly IIntKeyedRepository<Company> _companyRepository;
        private readonly ISiteService _SiteRepository;
        public CompanyInventoryService(IIntKeyedRepository<CompanyInventory> repository,                          
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                           IIntKeyedRepository<CompanyGrouping1Level> companyGrouping1LevelRepository, IReadOnlyRepository<CompanyGrouping2Level> companyGrouping2LevelRepository,
                           IIntKeyedRepository<Company> companyRepository, ISiteService SiteRepository,
                           ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
            {
                _companyRepository = companyRepository;
                _SiteRepository = SiteRepository;
            }

        public IList<CompanyInventory> GetInventoryBySiteId(int SiteId)
        {
            var SiteDetails = _SiteRepository.GetAll().Where(x => x.Id == SiteId);
            return _repository.All().Where(x => x.ExternalCompanyId == SiteDetails.First().CompanyGrouping2Level.CompanyGrouping1Level.Company.ExternalCompanyId && x.DeletedKey == null).ToList();
        }

        public IList<CompanyInventory> GetInventoryByExternalComanyd(int ExtCompanyId)
        {
            var SiteDetails = _repository.All().Where(x => x.ExternalCompanyId == ExtCompanyId && x.DeletedKey == null).ToList();
            return SiteDetails;
        }
    }
}
