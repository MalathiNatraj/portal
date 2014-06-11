using System.Collections.Generic;
namespace Diebold.Domain.Entities
{
    public abstract class Device : TrackeableEntity
    {
        public virtual string Name { get; set; }
        public virtual int ReportBufferSize { get; set; }
        public virtual string ExternalDeviceId { get; set; }
        public virtual Company Company { get; set; }

        public virtual IList<AlarmConfiguration> AlarmConfigurations { get; set; }
        public virtual IList<AlertStatus> AlertStatus { get; set; }
        public virtual IList<DeviceStatus> DeviceStatus { get; set; }

        protected Device()
        {
            AlarmConfigurations = new List<AlarmConfiguration>();
        }

        public virtual bool IsDvr { get { return false; } }
        public virtual bool IsGateway { get { return false; } }
    }
}
