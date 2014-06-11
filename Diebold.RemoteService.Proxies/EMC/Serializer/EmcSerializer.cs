using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Diebold.RemoteService.Proxies.EMC.Serializer
{
    public class EmcSerializer
    {
         public static string Serialize<T>(object dto) where T : new()
         {
             var serializer = new XmlSerializer(dto.GetType());
             var nameSpace = new XmlSerializerNamespaces();
             nameSpace.Add("", "");

             var stringBuilder = new StringBuilder();            
             using (StringWriter stringWriter = new StringWriter(stringBuilder))
             {
                 using(XmlTextWriterFull xmlTextWriter = new XmlTextWriterFull(stringWriter))
                 {                     
                     serializer.Serialize(xmlTextWriter, (T)dto, nameSpace);

                     xmlTextWriter.Flush();
                     xmlTextWriter.Close();
                 }

                 stringWriter.Flush();
                 stringWriter.Close();
             }
             return stringBuilder.ToString();
         }
    }
}
