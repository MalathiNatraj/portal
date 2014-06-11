using System;

namespace Diebold.Domain.Entities
{

    public class DeviceMedia : IntKeyedEntity
    {
        

        public virtual string MediaType { get; set; }

        public virtual string MediaId { get; set; }

        public virtual string Title { get; set; }

        public virtual string Status { get; set; }

        public virtual string Description { get; set; }

        public virtual string Notes { get; set; }

        public virtual string FileName { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public DeviceMedia() {
            Title = string.Empty;
            Description = string.Empty;
            Notes = string.Empty;
            FileName = string.Empty;
            CreatedDate = DateTime.UtcNow;
        }
    }
}