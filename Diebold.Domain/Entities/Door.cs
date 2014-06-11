using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class Door 
    {
        public string DoorName { get; set; }
        public string Online { get; set; }
        public string DoorStatus { get; set; }
        public string MomentaryUnlock { get; set; }
        public string ReaderId { get; set; }
    }
}
