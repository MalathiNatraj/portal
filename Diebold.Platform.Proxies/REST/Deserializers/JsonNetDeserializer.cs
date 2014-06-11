using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Extensions;

namespace Diebold.Platform.Proxies.REST.Deserializers
{
    public class JsonNetDeserializer : IDeserializer
    {
        public T Deserialize<T>(RestResponse response) where T : new()
        {
            var target = new T();

            string content = null;

            if (target is IList)
			{
				if (RootElement.HasValue())
				{
					var root = FindRoot(response.Content);
					content = root.ToString();
				}
			} else
            {
                content = response.Content;
            }

            return JsonConvert.DeserializeObject<T>(content);
        }

        private JToken FindRoot(string content)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            JObject json = JObject.Parse(content);
            JToken root = json.Root;

            if (RootElement.HasValue())
                root = json.SelectToken(RootElement);

            return root;
        }

        public string RootElement { get; set; }

        public string Namespace { get; set; }

        public string DateFormat { get; set; }
    }
}   