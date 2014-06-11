using System;
using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;

namespace Diebold.Services.Contracts
{
    public interface IDvrService : ICRUDTrackeableService<Dvr>, IDeviceService
    {
        //bool DeviceIsEnabled(string name);

        IList<string> GetAllHealthCheckVersions();
        IList<Dvr> GetDevicesBySiteId(int siteId);
        Page<Dvr> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition);
        Page<Dvr> GetDiagnosticPage(int pageNumber, int pageSize, string sortBy, bool ascending, string companyId, string siteId, string gatewayId);
        IList<Dvr> GetDevicesByCompanyGrouping2Level(int companyGrouping2LevelId);
        IList<Dvr> GetAllDevicesBySiteList(IList<int> siteIdList);
        IList<Dvr> GetDevicesByCompanyGrouping1Level(int companyGrouping1LevelId);
        void SetCameras(int deviceId, IList<Camera> cameras);
        void SetAlarmConfigurations(int deviceId, IList<AlarmConfiguration> alarmConfigurations);
        bool ValidateDeviceKey(int gatewayId, string deviceKey);
        IList<Dvr> GetAllDevicesForDisplay();
        int GetAllDevicesforMaxId();
        Dvr GetDeviceByInstanceName(string instanceName);
        IList<Dvr> GetDevicesByGatewayId(int gatewayId);
        string Encrypt(string plainText, string passPhrase);
        string Decrypt(string cipherText, string passPhrase);
        void DeleteDuplicateAlarmConfiguration(string deviceType, int companyId);
    }
}