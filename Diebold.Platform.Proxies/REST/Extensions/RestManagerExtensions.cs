using Diebold.Platform.Proxies.REST.Enums;

namespace Diebold.Platform.Proxies.REST.Extensions
{
    public static class RestManagerExtensions
    {
        #region Custom to REST

        public static RestSharp.Method GetMethod(this RequestMethod requestMethod)
        {
            switch (requestMethod)
            {
                case RequestMethod.GET:
                    return RestSharp.Method.GET;
                case RequestMethod.POST:
                    return RestSharp.Method.POST;
                case RequestMethod.PUT:
                    return RestSharp.Method.PUT;
                case RequestMethod.DELETE:
                    return RestSharp.Method.DELETE;
                default:
                    return RestSharp.Method.GET;
            }
        }

        public static RestSharp.DataFormat GetContentFormat(this ContentFormat requestFormat)
        {
            switch (requestFormat)
            {
                case ContentFormat.Json:
                    return RestSharp.DataFormat.Json;
                case ContentFormat.Xml:
                    return RestSharp.DataFormat.Xml;
                default:
                    return RestSharp.DataFormat.Xml;
            }
        }

        #endregion


        #region REST to Custom

        public static ContentFormat GetContentFormat(this string contentType)
        {
            if (contentType.Contains("application/json"))
                return ContentFormat.Json;

            if (contentType.Contains("application/xml"))
                return ContentFormat.Xml;

            return ContentFormat.Unsupported;

        }

        public static Enums.ResponseStatus GetResponseStatus(this RestSharp.ResponseStatus responseStatus)
        {
            switch (responseStatus)
            {
                case RestSharp.ResponseStatus.Aborted:
                    return Enums.ResponseStatus.Aborted;
                case RestSharp.ResponseStatus.Completed:
                    return Enums.ResponseStatus.Completed;
                case RestSharp.ResponseStatus.Error:
                    return Enums.ResponseStatus.Error;
                case RestSharp.ResponseStatus.None:
                    return Enums.ResponseStatus.None;
                case RestSharp.ResponseStatus.TimedOut:
                    return Enums.ResponseStatus.TimedOut;
                default:
                    return Enums.ResponseStatus.None;
            }
        } 
        #endregion
    }
}
