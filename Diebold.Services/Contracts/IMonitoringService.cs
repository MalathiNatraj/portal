using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Services.Impl;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Services.Contracts
{
    public interface IMonitoringService
    {
        string PlaceonTest(Site site, string SelectedHours, string AccountNumber);
        string PlaceonTestDDChange(Site site, string SelectedHours, string AccountNumber);
        

        List<ReportsDTO> RunReport(DateTime dtFromDate, DateTime dtToDate, string strReportName, string AccountNumber);

    }
}
