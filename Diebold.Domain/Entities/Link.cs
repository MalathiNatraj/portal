using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class Link : TrackeableEntity
    {
        public Link()
        {
            //Gateways = new List<Gateway>();
        }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public User User { get; set; }
    }
}
