using System.Collections.Generic;
using System.Net;

namespace Diebold.Platform.Proxies.REST.Structures
{
    public class GenericResponse
    {
        public IDictionary<string, string> Headers { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
    }
}
