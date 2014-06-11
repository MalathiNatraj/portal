using System;

namespace Diebold.Domain.Entities
{
    [Serializable]
    public class AlarmConfiguration : IntKeyedEntity, ICloneable
    {
        public virtual AlarmType? AlarmType { get; set; }
        public virtual DeviceType? DeviceType { get; set; }
        public virtual AlarmOperator Operator { get; set; }
        public virtual AlarmSeverity Severity { get; set; }
        public virtual Device Device { get; set; }
        public virtual string Threshold { get; set; }
        public virtual bool Sms { get; set; }
        public virtual bool Email { get; set; }
        public virtual bool Emc { get; set; }
        public virtual bool Log { get; set; }
        public virtual bool Ack { get; set; }
        public virtual bool Display { get; set; }
        public virtual AlarmParentType AlarmParentType { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual DataType DataType { get; set; }
        
        public override string ToString()
        {
            return "Alarm " + AlarmType.ToString();
        }

        public object Clone()
        {
            AlarmConfiguration alarmconfiguration = (AlarmConfiguration)this.MemberwiseClone();
            return alarmconfiguration;
        }
    }
}
