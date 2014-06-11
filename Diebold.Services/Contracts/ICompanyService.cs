using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;

namespace Diebold.Services.Contracts
{
    public interface ICompanyService : ICRUDTrackeableService<Company>
    {
        bool DeviceIsEnabled(string name);
        bool ValidateCompanyId(int companyId);
        IList<string> GetAllReportingFrequencies();
        IList<string> GetAllSubscriptions();
        Page<Company> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition);
        IList<CompanyGrouping2Level> GetGrouping2LevelsByGrouping1LevelName(string grouping1LevelName, int companyId);
        IList<CompanyGrouping2Level> GetGrouping2LevelsByGrouping1LevelId(int grouping1LevelId, int companyId);
        IList<CompanyGrouping2Level> GetGrouping2LevelsByCompanyId(int companyId);
        IList<CompanyGrouping1Level> GetGrouping1LevelsByCompanyId(int companyId);
        IList<CompanyGrouping2Level> GetGrouping2LevelsByGrouping1LevelId(int grouping1LevelId);
        IList<Site> GetSitesByCompanyId(int companyId);
        CompanyGrouping2Level GetGrouping2LevelById(int grouping2LevelId);
        bool ValidateGrouping1Id(int grouping1Id);
        bool ValidateGrouping2Id(int grouping2Id);
        Company Load(int id);
        void RemoveCompanyGroupLevel1(CompanyGrouping1Level grouping1Level);
    }
}