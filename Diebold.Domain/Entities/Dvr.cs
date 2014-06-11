using System.Collections.Generic;
namespace Diebold.Domain.Entities
{
    public class Dvr : Device
    {
        public virtual string HostName { get; set; }
        public virtual string DeviceKey { get; set; }
        public virtual string TimeZone { get; set; }
        public virtual string ZoneNumber { get; set; }
        public virtual int NumberOfCameras { get; set; }

        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual bool IsInDST { get; set; }
        public virtual string PortA { get; set; }
        public virtual string PortB { get; set; }

        public virtual Gateway Gateway { get; set; }
        public virtual DeviceType DeviceType { get; set; }
        public virtual PollingFrequency PollingFrequency { get; set; }
        public virtual HealthCheckVersion HealthCheckVersion { get; set; }
        public virtual IList<Camera> Cameras { get; set; }
        public virtual string RemovedCameras { get; set; }

        public virtual Site Site { get; set; }

        public Dvr()
        {
            Cameras = new List<Camera>();
        }

        public override string ToString()
        {
            return "Dvr " + Name;
        }

        public override bool IsDvr
        {
            get { return true; }
        }
    }
}
