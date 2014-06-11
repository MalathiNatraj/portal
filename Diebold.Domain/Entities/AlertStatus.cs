using System;
using Diebold.Domain.Enums;

namespace Diebold.Domain.Entities
{
    public class AlertStatus : IntKeyedEntity
    {
        public AlertStatus()
        {
            FirstAlertTimeStamp = null;
            LastAlertTimeStamp = null;

            AlertCount = 0;
            IsAcknowledged = false;
            IsOk = false;
        }

        public virtual Device Device { get; set; }

        public virtual AlarmConfiguration Alarm { get; set; }

        public virtual int AlertCount { get; set; }

        public virtual bool IsAcknowledged { get; set; }

        public virtual bool IsOk { get; set; }

        public virtual DateTime? FirstAlertTimeStamp { get; set; }

        public virtual DateTime? LastAlertTimeStamp { get; set; }

        public Acknowledge AckColor
        {
            get {
                if (!IsAcknowledged) return Acknowledge.Red;
                
                return IsOk ? Acknowledge.Green : Acknowledge.Yellow;
            }
        }

        public virtual string ElementIdentifier { get; set; }
    }
}
