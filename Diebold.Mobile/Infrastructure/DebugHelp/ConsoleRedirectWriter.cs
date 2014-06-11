using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace DieboldMobile.Infrastructure.DebugHelp
{
    public class ConsoleRedirectWriter : TextWriter
    {
        public override void WriteLine(string value)
        {
            System.Diagnostics.Debug.WriteLine(value);
            base.WriteLine(value);
        }
        public override void Write(string value)
        {
            System.Diagnostics.Debug.Write(value);
            base.Write(value);
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

    }
}