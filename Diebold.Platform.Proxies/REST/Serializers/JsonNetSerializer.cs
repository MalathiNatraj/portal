using System.IO;
using RestSharp.Serializers;
using Newtonsoft.Json;

namespace ElkRiv.API.SDK.REST.Serializers
{
    public class JsonNetSerializer : ISerializer
    {
        private string _contentType = "application/json";

        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object value)
        {
            var json = new Newtonsoft.Json.JsonSerializer
                           {
                               NullValueHandling = NullValueHandling.Ignore,
                               ObjectCreationHandling = ObjectCreationHandling.Replace,
                               MissingMemberHandling = MissingMemberHandling.Ignore,
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           };

            //Type type = value.GetType();
            //if (type == typeof(DataRow))
            //    json.Converters.Add(new DataRowConverter());
            //else if (type == typeof(DataTable))
            //    json.Converters.Add(new DataTableConverter());
            //else if (type == typeof(DataSet))
            //    json.Converters.Add(new DataSetConverter());

            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw)
                             {
                                 QuoteChar = '"'
                             };

            //if (this.FormatJsonOutput)
            //    writer.Formatting = Formatting.Indented;
            //else
            //    writer.Formatting = Formatting.None;

            json.Serialize(writer, value);

            var output = sw.ToString();
            writer.Close();
            sw.Close();

            return output;
        }
    }
}   