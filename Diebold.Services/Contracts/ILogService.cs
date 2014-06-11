using System;
using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Domain.Enums;
using Diebold.Services.Extensions;

namespace Diebold.Services.Contracts
{
    public interface ILogService
    {
        void Log(LogAction action);
        void Log(LogAction action, User user);
        void Log(LogAction action, string description);
        void Log(LogAction action, User user, string description);
        void Log(LogAction action, string description, CompanyGrouping1Level companyGrouping1Level);
        void Log(LogAction action, string description, CompanyGrouping1Level companyGrouping1Level, CompanyGrouping2Level companyGrouping2Level);
        void Log(LogAction action, string description, CompanyGrouping1Level companyGrouping1Level, CompanyGrouping2Level companyGrouping2Level, Site site);
        void Log(LogAction action, string description, CompanyGrouping1Level companyGrouping1Level, CompanyGrouping2Level companyGrouping2Level, Site site, Device device);
        void Log(LogAction action, string description, CompanyGrouping1Level companyGrouping1Level, CompanyGrouping2Level companyGrouping2Level, Site site, Device device, User user);
        
        Page<HistoryLog> GetPagedLogHistory(int pageNumber, int pageSize, string sortingName, bool ascending, List<LogAction> actionTypes, List<int> userIds, string dateType, DateTime dateFrom, DateTime dateTo,
            List<int> group1Ids, List<int> group2Ids, List<int> siteIds, List<int> deviceIds, bool groupLevelSelected, UserStatus userStatus);

        IList<HistoryLog> GetAllLogHistory(string sortingName, bool ascending, List<LogAction> actionTypes, List<int> userIds, string dateType, DateTime dateFrom, DateTime dateTo,
            List<int> group1Ids, List<int> group2Ids, List<int> siteIds, List<int> deviceIds, bool groupLevelSelected, UserStatus userStatus);
    }
}