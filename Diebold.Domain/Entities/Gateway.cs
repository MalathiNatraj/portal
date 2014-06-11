using System;

namespace Diebold.Domain.Entities
{
    public class Gateway : Device
    {
        public virtual int Protocol { get; set; }
        public virtual string TimeZone { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string Notes { get; set; }
        public virtual string MacAddress { get; set; }
        public virtual int EMCId { get; set; }
        public virtual DateTime LastUpdate { get; protected set; }
        public virtual int LastUsedEmcZoneNumber { get; set; }
        public virtual Gateway gateway { get; set; }
        
        public override Company Company { get; set; }

        public Gateway()
        {
            Touch();
        }

        public virtual void Touch()
        {
            LastUpdate = DateTime.Now;
        }

        public override string ToString()
        {
            return "Gateway " + Name;
        }

        public override bool IsGateway
        {
            get { return true;  }
        }
    }
}
