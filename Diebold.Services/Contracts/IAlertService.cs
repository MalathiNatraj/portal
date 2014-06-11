using System;
using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Domain.Enums;
using Diebold.Services.Extensions;

namespace Diebold.Services.Contracts
{
    public interface IAlertService : ICRUDService<AlertInfo>
    {
        int GetOkDevicesCount(int userId);
        int GetAlertedDevicesCount(int userId);

        DateTime? GetLastAlertTimeStamp(int userId);

        void AcknowledgeAlert(int alertId);

        Page<AlertStatus> GetAlertsByStatus(int pageNumber, int pageSize, string sortBy, bool ascending, DashboardFilter filter);

        IList<AlertStatus> GetPendingAlertsByDevice(int deviceId, int alarmConfigurationId);

        AlertStatus GetAlertStatusByPK(int id);

        AlertInfo GetLastAlertInfoByDeviceAndIdentifier(int deviceId, int alarmId, string elementIdentifier);

        AlertStatus GetLastAlertStatusByDeviceAndIdentifier(int deviceId, int alarmId, string elementIdentifier);

        Page<ResolvedAlert> GetResolvedAlertsFormDevice(int pageNumber, int pageSize, string sortBy, bool ascending,
                                                        int deviceId);

        int GetConfiguredAlarmsBySiteAndCurrentUserCount(int siteId);

        Page<ResultsReport> GetAlertsForReport(int pageIndex, int recordsCount, string sortBy, bool ascending,
                                   IList<string> alarmTypes, IList<int> users, string deviceStatus, string dateType,
                                    DateTime dateFrom, DateTime dateTo, IList<int> deviceIds, bool groupLevelSelected);

        IList<ResultsReport> GetAlertsForReport(IList<string> alarmTypes, IList<int> users, string deviceStatus, string dateType,
                            DateTime dateFrom, DateTime dateTo, IList<int> deviceIds, string sortBy, bool ascending, bool groupLevelSelected);

        void CreateAlert(IList<AlertInfo> alerts);

        bool ValidateSendNotification(AlertInfo alertInfo);
        
        void Create(AlertInfo alertInfo);

        IList<AlertStatus> GetAlertsByDevice(int DeviceId);

        IList<AlertStatus> GetAlertDetailsByDeviceType(string strdeviceType);

        IList<AlertStatus> GetAlertDetailsByDeviceId(int deviceId);

        IList<ResolvedAlert> getPreviouslyAcknoledgedAlets(int deviceId);

        IList<AlertStatus> GetAllAlertDetails();

        IList<AlertStatus> GetAllAlertDetailsByParentType(String ParentType);

        AlertStatus GetAlertStatusByDeviceIdandAlarmConfigId(int DeviceId, int AlarmConfigId);
        void ModifyAlert(int alertId, AlertInfo alertInfo);
        void CreateMultipleItems(List<AlertInfo> lstAlertInfo);
        //string getEMC(AlertStatus objAlert);
        string getEMC(string alarmName, int deviceId);

        void CreateClearAlert(IList<AlertInfo> alerts);
    }
}
