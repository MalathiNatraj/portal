using System.Linq;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Contracts;
using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Services.Infrastructure;
using Diebold.Platform.Proxies.Contracts;
using System;
using System.Reflection;
using Diebold.Services.Exceptions;


namespace Diebold.Services.Impl
{
    public class AlarmConfigurationService : BaseCRUDService<AlarmConfiguration>, IAlarmConfigurationService
    {
        public AlarmConfigurationService(IIntKeyedRepository<AlarmConfiguration> repository, IUnitOfWork unitOfWork,
                                         IValidationProvider validationProvider, ILogService logService, ICurrentUserProvider currentUserProvider)
            : base(repository, unitOfWork, validationProvider, logService)
        {}

        public IList<AlarmConfiguration> GetAllByAlarmType(AlarmType type)
        {
            return _repository.FilterBy(x => x.AlarmType == type).ToList();
        }

        public IList<AlarmConfiguration> GetDefaultAlarmConfiguration(DeviceType type)
        {
            return _repository.FilterBy(x => x.DeviceType == type).Where(x => x.Device == null && x.AlarmParentType.ToString().ToLower() != "deleted").ToList();            
        }

        public IList<AlarmConfiguration> GetAlarmConfigurationByDevice(DeviceType type, Device device)
        {
            return _repository.FilterBy(x => x.DeviceType == type).Where(x => x.Device == device && x.AlarmParentType.ToString().ToLower() != "deleted").ToList();
        }
        public IList<AlarmConfiguration> GetAlarmConfigurationByDeviceId(Device device)
        {
            return _repository.FilterBy(x => x.Device == device && x.AlarmParentType.ToString().ToLower() != "deleted").ToList();
        }
        public IList<AlarmConfiguration> GetAlarmConfigurationForCreate(DeviceType type, Company company)
        {
            IList<AlarmConfiguration> alarmsDefault = _repository.FilterBy(x => x.DeviceType == type).Where(x => x.Device == null && x.AlarmParentType.ToString().ToLower() != "deleted").ToList();

            return (from alarm in alarmsDefault
                    where company.Subscriptions.Contains((Subscription) Enum.Parse(typeof (AlarmType), alarm.AlarmType.ToString()))
                    select new AlarmConfiguration()
                               {
                                   AlarmType = alarm.AlarmType, 
                                   DeviceType = alarm.DeviceType,
                                   Operator = alarm.Operator,
                                   Severity = alarm.Severity,
                                   Threshold = alarm.Threshold,
                                   Email = alarm.Email,
                                   Emc = alarm.Emc,
                                   Log = alarm.Log,
                                   Sms = alarm.Sms,
                                   DataType = alarm.DataType
                               }).ToList();
        }

        public IList<AlarmConfiguration> GetAlarmConfigurationForEdit(DeviceType type, Device device)
        {
            return GetAlarmConfigurationByDevice(type, device);
        }

        public IList<string> GetAllAlarmSeverities()
        {
            return Enum.GetNames(typeof (AlarmSeverity)).ToList();
        }

        public IList<string> GetAllAlarmOperators()
        {
            return Enum.GetNames(typeof (AlarmOperator)).ToList();
        }

        public IList<string> GetAllAlarmTypes()
        {
            return Enum.GetNames(typeof(AlarmType)).ToList();
        }

        public override void Update(AlarmConfiguration item)
        {
            AlarmConfiguration persistentItem = this._repository.Load(item.Id);

            PropertyInfo[] destinationProperties = persistentItem.GetType().GetProperties();
            foreach (PropertyInfo destinationPI in destinationProperties)
            {
                PropertyInfo sourcePI = item.GetType().GetProperty(destinationPI.Name);

                if (sourcePI.GetValue(item, null) != null)
                {
                    destinationPI.SetValue(persistentItem,
                                           sourcePI.GetValue(item, null),
                                           null);
                }
            }

            this._validationProvider.Validate(item);
            this._repository.Update(persistentItem);
        }

        public override void Create(AlarmConfiguration item)
        {
            PropertyInfo[] destinationProperties = item.GetType().GetProperties();
            foreach (PropertyInfo destinationPI in destinationProperties)
            {
                PropertyInfo sourcePI = item.GetType().GetProperty(destinationPI.Name);

                if (sourcePI.GetValue(item, null) != null)
                {
                    destinationPI.SetValue(item,
                                           sourcePI.GetValue(item, null),
                                           null);
                }
            }

            this._validationProvider.Validate(item);
            this._repository.Add(item);
        }

        public void UpdateAlarmConfiguration(IList<AlarmConfiguration> alarmConfigurations)
        {
            try
            {
                foreach (AlarmConfiguration alarm in alarmConfigurations)
                {
                    if (alarm.Id != 0)
                        Update(alarm);
                }

                _unitOfWork.Commit();
            }
            catch (Exception E)
            {
                this._unitOfWork.Rollback();

                throw new ServiceException("Entity Create Exception", E);
            }
        }

        public void CreateAlarmConfiguration(IList<AlarmConfiguration> alarmConfigurations)
        {
            try
            {
                foreach (AlarmConfiguration alarm in alarmConfigurations)
                {
                    alarm.Id = 0;
                    Create(alarm);
                }

                _unitOfWork.Commit();
            }
            catch (Exception E)
            {
                this._unitOfWork.Rollback();

                throw new ServiceException("Entity Create Exception", E);
            }
        }

        public AlarmConfiguration GetByDeviceAndCapability(int deviceId, AlarmType type)
        {
            try
            {
                return _repository.FindBy(x => x.Device.Id == deviceId && x.AlarmType.HasValue && x.AlarmType.Value == type);               
            }
            catch(Exception)
            {
                throw new Exception("Combination of Device and Capability was not found.");
            }
        }

        public IList<AlarmConfiguration> GetALLAlarmConfigByDeviceID(int deviceId)
        {
            try
            {
                return _repository.All().Where(x => x.Device.Id == deviceId && x.AlarmType.HasValue && x.AlarmParentType.ToString().ToLower() != "deleted").ToList();
            }
            catch (Exception)
            {
                throw new Exception("Device not found for Device Id : " + deviceId);
            }
        }
        public IList<string> GetAllAlarmParentType()
        {
            var strParentTypes = Enum.GetNames(typeof(AlarmParentType)).ToList();
            strParentTypes.Remove("Deleted");
            return strParentTypes;
        }
    }
}
