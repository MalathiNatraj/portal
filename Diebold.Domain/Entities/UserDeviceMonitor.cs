namespace Diebold.Domain.Entities
{
    public class UserDeviceMonitor : IntKeyedEntity
    {
        public virtual User User { get; set; }
        public virtual Device Device { get; set; }
        public virtual UserMonitorGroup UserMonitorGroup { get; set; }
    }
}
