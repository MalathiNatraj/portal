using System;
using System.Collections.Generic;
using System.Linq;
using Diebold.Services.Contracts;
using Diebold.Domain.Entities;
using Diebold.Domain.Contracts;
using System.Linq.Dynamic;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Exceptions;
using Diebold.Services.Extensions;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Impl
{
    public class CompanyService : BaseCRUDTrackeableService<Company>, ICompanyService
    {
        private readonly IIntKeyedRepository<CompanyGrouping1Level> _companyGrouping1LevelRepository;
        private readonly IReadOnlyRepository<CompanyGrouping2Level> _companyGrouping2LevelRepository;

        public CompanyService(IIntKeyedRepository<Company> repository, IUnitOfWork unitOfWork,
                              IValidationProvider validationProvider,
                              IIntKeyedRepository<CompanyGrouping1Level> companyGrouping1LevelRepository, IReadOnlyRepository<CompanyGrouping2Level> companyGrouping2LevelRepository,
                              ILogService logService)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            _companyGrouping1LevelRepository = companyGrouping1LevelRepository;
            _companyGrouping2LevelRepository = companyGrouping2LevelRepository;
        }

        public bool DeviceIsEnabled(string name)
        {
            Company company;

            try
            {
                company = _repository.FindBy(u => u.Name == name);
            }
            catch (Exception e)
            {
                throw new Exception("Unknown Company", e);
            }

            return (!company.IsDisabled);
        }

        public IList<string> GetAllReportingFrequencies()
        {
            IList<string> retList = new List<string>();
            foreach (string pollingFrequency in Enum.GetNames(typeof(Diebold.Domain.Entities.ReportingFrequency)))
            {
                retList.Add(pollingFrequency);
            }

            return retList;
        }

        public IList<string> GetAllSubscriptions()
        {
            IList<string> retList = new List<string>();
            foreach (string subscription in Enum.GetNames(typeof(Diebold.Domain.Entities.Subscription)))
            {
                retList.Add(subscription);
            }

            return retList;
        }

        public Page<Company> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null);

            if (!string.IsNullOrEmpty(whereCondition))
            {
                query = query.Where(x => x.Name.Contains(whereCondition));
            }

            var orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));

            return query.OrderBy(orderBy).ToPage(pageNumber, pageSize);
        }

        public IList<CompanyGrouping1Level> GetGrouping1LevelsByCompanyId(int companyId)
        {
            var company = _repository.FindBy(x => x.Id == companyId && x.DeletedKey == null);

            if (company == null || company.CompanyGrouping1Levels == null)
                return null;
            return company.CompanyGrouping1Levels.OrderBy(x => x.Name).ToList();
        }

        public IList<CompanyGrouping2Level> GetGrouping2LevelsByGrouping1LevelName(string grouping1LevelName,
                                                                                   int companyId)
        {
            IList<CompanyGrouping2Level> list = new List<CompanyGrouping2Level>();

            var company = _repository.All().Where(x => x.Id == companyId);

            if (company.Any())
            {
                foreach (
                    var item in
                        company.First().CompanyGrouping1Levels.Where(item => item.Name == grouping1LevelName))
                {
                    list = item.CompanyGrouping2Levels;
                    break;
                }
            }

            return list;
        }

        public IList<CompanyGrouping2Level> GetGrouping2LevelsByGrouping1LevelId(int grouping1LevelId, int companyId)
        {
            var item = _repository.FindBy(x => x.Id == companyId && x.DeletedKey == null)
                .CompanyGrouping1Levels.Single(x => x.Id == grouping1LevelId);

            return item.CompanyGrouping2Levels.OrderBy(x => x.Name).ToList();
        }

        public IList<CompanyGrouping2Level> GetGrouping2LevelsByCompanyId(int companyId)
        {
            var company = _repository.FindBy(x => x.Id == companyId && x.DeletedKey == null);
            if (company != null && company.CompanyGrouping1Levels != null)
                return company.CompanyGrouping1Levels.SelectMany(item => item.CompanyGrouping2Levels).ToList();
            else
                return new List<CompanyGrouping2Level>();
        }

        public bool ValidateCompanyId(int externalCompanyId)
        {
            try
            {
                var validate = _repository.FindBy(x => x.ExternalCompanyId == externalCompanyId);
                return (validate == null);
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }

        public bool ValidateGrouping1Id(int grouping1Id)
        {
            try
            {
                var validate = _companyGrouping1LevelRepository.FindBy(x => x.Id == grouping1Id);
                return (validate != null);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ValidateGrouping2Id(int grouping2Id)
        {
            try
            {
                var validate = _companyGrouping2LevelRepository.FindBy(x => x.Id == grouping2Id);
                return (validate != null);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Company Load(int id)
        {
            return _repository.Load(id);
        }

        public IList<CompanyGrouping2Level> GetGrouping2LevelsByGrouping1LevelId(int grouping1LevelId)
        {
            var firstLevel = _companyGrouping1LevelRepository.FindBy(x => x.Id == grouping1LevelId);

            return firstLevel.CompanyGrouping2Levels.OrderBy(x => x.Name).ToList();
        }

        public CompanyGrouping2Level GetGrouping2LevelById(int grouping2LevelId)
        {
            var secondLevel = _companyGrouping2LevelRepository.FindBy(x => x.Id == grouping2LevelId);

            return secondLevel;
        }

        public IList<Site> GetSitesByCompanyId(int companyId)
        {
            var company = _repository.FindBy(x => x.Id == companyId);

            return (from grouping1 in company.CompanyGrouping1Levels
                    from grouping2 in grouping1.CompanyGrouping2Levels
                    from site in grouping2.Sites
                    where site.DeletedKey == null select site).OrderBy(x => x.Name).ToList();
        }

        public void RemoveCompanyGroupLevel1(CompanyGrouping1Level grouping1Level)
        {
            try
            {
                _companyGrouping1LevelRepository.Delete(grouping1Level);
            }
            catch (Exception e)
            {
                throw new ServiceException("Cannot delete group level", e);
            }
        }

        #region Logging

        #endregion
    }
}
