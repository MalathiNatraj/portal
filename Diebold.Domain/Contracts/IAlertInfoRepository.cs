using System;
using System.Collections.Generic;
using Diebold.Domain.Entities;

namespace Diebold.Domain.Contracts
{
    public interface IAlertInfoRepository : IIntKeyedRepository<AlertInfo>
    {

        IList<ResultsReport> GetAlertsForReport(IList<string> alarmTypes, IList<int> userIds,
                                                string deviceStatus,
                                                string dateType, DateTime dateFrom, DateTime dateTo,
                                                IList<int> deviceIds, int? pageIndex, int? pageSize,
                                                string sortBy, bool ascending, bool groupLevelSelected, out int rowCount);
    }
}
