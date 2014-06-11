using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Diebold.RemoteService.Proxies.EMC
{
    /// <summary>
    /// Overrides WriteEndElement to add a closing tag to empty elements
    /// </summary>
    public class XmlTextWriterFull : XmlTextWriter
    {
        public XmlTextWriterFull(TextWriter sink)
            : base(sink)
        {
        }

        public override void WriteEndElement()
        {
            base.WriteFullEndElement();
        }

    }
}
