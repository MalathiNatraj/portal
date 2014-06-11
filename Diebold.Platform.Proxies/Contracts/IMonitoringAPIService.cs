using System.Collections.Generic;
using Diebold.Platform.Proxies.DTO;
using System;

namespace Diebold.Platform.Proxies.Contracts
{
    public interface IMonitoringAPIService
    {
        string PlaceonTestAPI(string monitoring);
        string PlaceonTestAPIforDDChange(string monitoring);

        string RunReport(string ReportName, DateTime dtFromDate, DateTime dtToDate, string strReportparams);

        string FireWidgetReport(string strReportParams);
    }
}
