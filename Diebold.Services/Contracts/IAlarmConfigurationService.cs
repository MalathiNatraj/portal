using System.Collections.Generic;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface IAlarmConfigurationService : ICRUDService<AlarmConfiguration>
    {
        IList<AlarmConfiguration> GetAllByAlarmType(AlarmType type);
        IList<AlarmConfiguration> GetDefaultAlarmConfiguration(DeviceType type);
        IList<AlarmConfiguration> GetAlarmConfigurationByDevice(DeviceType type, Device device);
        IList<AlarmConfiguration> GetAlarmConfigurationForCreate(DeviceType type, Company company);
        IList<AlarmConfiguration> GetAlarmConfigurationForEdit(DeviceType type, Device device);
        IList<AlarmConfiguration> GetAlarmConfigurationByDeviceId(Device device);

        IList<string> GetAllAlarmSeverities();
        IList<string> GetAllAlarmOperators();
        IList<string> GetAllAlarmTypes();
        void UpdateAlarmConfiguration(IList<AlarmConfiguration> alarmConfigurations);
        void CreateAlarmConfiguration(IList<AlarmConfiguration> alarmConfigurations);

        AlarmConfiguration GetByDeviceAndCapability(int deviceId, AlarmType type);
        IList<string> GetAllAlarmParentType();

        IList<AlarmConfiguration> GetALLAlarmConfigByDeviceID(int deviceId);
    }
}
