using System;
using System.Collections.Generic;
using System.Linq;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Domain.Enums;
using Diebold.Services.Contracts;
using Diebold.Services.Exceptions;
using Diebold.Services.Extensions;
using Diebold.Services.Infrastructure;
using System.Linq.Dynamic;

namespace Diebold.Services.Impl
{
    public class LogService : BaseService, ILogService
    {        
        protected readonly IValidationProvider _validationProvider;
        protected readonly IIntKeyedRepository<HistoryLog> _repository;
        protected readonly ICurrentUserProvider _currentUserProvider;

        public LogService(IIntKeyedRepository<HistoryLog> repository,
                           IUnitOfWork unitOfWork,
                           IValidationProvider validationProvider,
                            ICurrentUserProvider currentUserProvider) : base(unitOfWork)
        {
            _repository = repository;
            _validationProvider = validationProvider;
            _currentUserProvider = currentUserProvider;
        }

        public void Log(LogAction action)
        {
            Log(action, _currentUserProvider.CurrentUser, null, null, null, null, null);
        }

        public void Log(LogAction action, User user)
        {
            Log(action, user, null, null, null, null, null);
        }
        
        public void Log(LogAction action, string description)
        {
            Log(action, _currentUserProvider.CurrentUser, description, null, null, null, null);
        }

        public void Log(LogAction action, User user, string description)
        {
            Log(action, user, description, null, null, null, null);
        }

        public void Log(LogAction action, string description, CompanyGrouping1Level companyGrouping1Level)
        {
            Log(action, _currentUserProvider.CurrentUser, description, companyGrouping1Level, null, null, null);
        }

        public void Log(LogAction action, string description, CompanyGrouping1Level companyGrouping1Level, CompanyGrouping2Level companyGrouping2Level)
        {
            Log(action, _currentUserProvider.CurrentUser, description, companyGrouping1Level, companyGrouping2Level, null, null);
        }

        public void Log(LogAction action, string description, CompanyGrouping1Level companyGrouping1Level, CompanyGrouping2Level companyGrouping2Level, Site site)
        {
            Log(action, _currentUserProvider.CurrentUser, description, companyGrouping1Level, companyGrouping2Level, site, null);
        }

        public void Log(LogAction action, string description, CompanyGrouping1Level companyGrouping1Level, CompanyGrouping2Level companyGrouping2Level, Site site, Device device)
        {
            Log(action, _currentUserProvider.CurrentUser, description, companyGrouping1Level, companyGrouping2Level, site, device);
        }

        public void Log(LogAction action, string description, CompanyGrouping1Level companyGrouping1Level, CompanyGrouping2Level companyGrouping2Level, Site site, Device device, User user)
        {
            Log(action, user, description, companyGrouping1Level, companyGrouping2Level, site, device);
        }

        public void Log(LogAction action, User user, string description, CompanyGrouping1Level companyGrouping1Level, CompanyGrouping2Level companyGrouping2Level, Site site, Device device)
        {
            try
            {
                var item = new HistoryLog
                {
                    Date = DateTime.Now,
                    Action = action,
                    User = user,
                    Description = description,

                    CompanyGrouping1Level = companyGrouping1Level,
                    CompanyGrouping2Level = companyGrouping2Level,
                    Site = site,
                    Device = device,
                };

                _validationProvider.Validate(item);

                _repository.Add(item);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();

                var errorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;

                _logger.Debug(" Error when logging: " + errorMessage);

                throw new ServiceException(errorMessage, e);
            }
        }

        public IList<HistoryLog> GetAllLogHistory(string sortingName, bool @ascending, List<LogAction> actionTypes, List<int> userIds, string dateType, DateTime dateFrom, DateTime dateTo, List<int> group1Ids, List<int> group2Ids, List<int> siteIds, List<int> deviceIds, bool groupLevelSelected, UserStatus userStatus)
        {
            _logger.Debug("Custom & Standard Audit Report: LogService - GetAllLogHistory started ");            
            var logHistory = GetQueryForLogHistory(sortingName, @ascending, actionTypes, userIds, dateFrom, dateTo, group1Ids, group2Ids, siteIds, deviceIds, groupLevelSelected, userStatus);
            _logger.Debug("Custom & Standard Audit Report: LogService - GetAllLogHistory Completed " + logHistory);
            _logger.Debug("Custom & Standard Audit Report: LogService - GetAllLogHistory Completed");
            return logHistory.ToList();
        }

        public Page<HistoryLog> GetPagedLogHistory(int pageNumber, int pageSize, string sortingName, bool ascending,
            List<LogAction> actionTypes, List<int> userIds, string dateType, DateTime dateFrom, DateTime dateTo,
            List<int> group1Ids, List<int> group2Ids, List<int> siteIds, List<int> deviceIds, bool groupLevelSelected, UserStatus userStatus)
        {
            var logHistory = GetQueryForLogHistory(sortingName, @ascending, actionTypes, userIds, dateFrom, dateTo, group1Ids, group2Ids, siteIds, deviceIds, groupLevelSelected, userStatus);

            return logHistory.ToPage(pageNumber, pageSize);
        }

        private IQueryable<HistoryLog> GetQueryForLogHistory(string sortingName, bool @ascending, ICollection<LogAction> actionTypes, ICollection<int> userIds,
                                                 DateTime dateFrom, DateTime dateTo, ICollection<int> group1Ids, ICollection<int> group2Ids,
                                                 ICollection<int> siteIds, ICollection<int> deviceIds, bool groupLevelSelected, UserStatus userStatus)
        {
            var orderBy = string.Format("{0} {1}", sortingName, (ascending ? string.Empty : "DESC"));

            var logHistory = _repository.All();

            if (actionTypes.Count > 0)
            {
                logHistory = logHistory.Where(x => actionTypes.Contains(x.Action));
            }

            if (userIds.Count > 0)
            {
                logHistory = logHistory.Where(x => userIds.Contains(x.User.Id));
            }

            if (userStatus != UserStatus.AllUsers)
            {
                switch (userStatus)
                {
                    case UserStatus.ActiveUsers: logHistory = logHistory.Where(x => x.User.IsDisabled == false && x.User.DeletedKey == null);
                        break;
                    case UserStatus.DeletedUsers:logHistory = logHistory.Where(x => x.User.DeletedKey != null);
                        break;
                }
            }

            logHistory = logHistory.Where(x => x.Date >= dateFrom);
            logHistory = logHistory.Where(x => x.Date <= dateTo.AddDays(1)); //to include last day.

            if (groupLevelSelected)
            {
                logHistory = logHistory.Where(x => group1Ids.Contains(x.CompanyGrouping1Level.Id) ||
                                                   group2Ids.Contains(x.CompanyGrouping2Level.Id)  ||
                                                   siteIds.Contains(x.Site.Id) ||
                                                   deviceIds.Contains(x.Device.Id));

            }

            logHistory = logHistory.OrderBy(orderBy);
            return logHistory;
        }
    }
}
