using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class Area
    {
        public int AreaNumber { get; set; }
        public bool Armed { get; set; }
        public bool ScheduleStatus { get; set; }
        public bool LateStatus { get; set; }
        public string AreaName { get; set; }
        public List<Zone> Zones { get; set; }
    }
}
