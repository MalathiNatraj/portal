namespace Diebold.Domain.Entities
{
    /// <summary>
    /// Transient entity with information to notify
    /// </summary>
    public class Notification
    {
        public bool SendToEmail { get; set; }
        public bool SendToEmc { get; set; }
        public bool SendToLog { get; set; }
        // EMC
        public string EmcAccontNumber { get; set; }
        public string EmcDevicezone { get; set; }
        // Email
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string AlarmName { get; set; }
        public string DateOccur { get; set; }
        public string TimeZone { get; set; }
        public string SiteName { get; set; }
        public string SiteAddress1 { get; set; }
        public string SiteAddress2 { get; set; }
        public bool AlertCleared { get; set; }
    }
}
