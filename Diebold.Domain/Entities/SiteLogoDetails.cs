using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class SiteLogoDetails : TrackeableEntity
    {
        public int siteId { get; set; }
        public string FileName { get; set; }
        public byte[] SiteLogo { get; set; }
    }
}
