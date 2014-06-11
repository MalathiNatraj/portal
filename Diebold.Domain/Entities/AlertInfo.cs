using System;

namespace Diebold.Domain.Entities
{
    public class AlertInfo : IntKeyedEntity
    {
        public virtual Device Device { get; set; }

        public virtual DateTime DateOccur { get; set; } //The "alertDate" value uses the ISO 8601 Date Format: YYYY-MM-DDThh:mm:ssTZD

        public virtual bool IsDeviceOk { get; set; }

        public virtual AlarmConfiguration Alarm { get; set; }

        public virtual int GroupId { get; set; }

        public virtual string Value { get; set; }
        public virtual string ElementIdentifier { get; set; }

        /// <summary>
        /// Defines if it creates a new alert or just the status.
        /// </summary>
        public bool SatisfiesAlertCondition { get; set; }
        
        public override string ToString()
        {
            return "Alert for " + Alarm.ToString();
        }

        public AlertInfo()
        {
            SatisfiesAlertCondition = true;
        }

        public string Status { get; set; }
    }
}
