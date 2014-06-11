using System.Collections.Generic;
using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.REST.ErrorHandling
{
    public class Error
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
    
    public class ErrorResponse
    {
        [JsonProperty(PropertyName = "errors")]
        public IList<Error> Errors { get; set; }
    }
}
