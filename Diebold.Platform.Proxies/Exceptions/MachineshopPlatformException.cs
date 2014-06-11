using System;
using System.Linq;
using System.Net;
using Diebold.Platform.Proxies.REST.Enums;
using Diebold.Platform.Proxies.REST.ErrorHandling;

namespace Diebold.Platform.Proxies.Exceptions
{
    public class MachineshopPlatformException : Exception
    {
        public ErrorResponse ErrorResponse { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
        public string ResponseContent { get; set; }
        public string RequestContent { get; set; }

        public string GetErrorMessage()
        {
            return ErrorResponse != null ? ErrorResponse.Errors.First().Message : Message;
        }

        public MachineshopPlatformException(string message)
            : base(message)
        {
        }

        public MachineshopPlatformException(string message, Exception e)
            : base(message, e)
        {
        }

        public MachineshopPlatformException(string message, HttpStatusCode statusCode, ResponseStatus responseStatus)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseStatus = responseStatus;
        }

        public MachineshopPlatformException(string message, HttpStatusCode statusCode, ResponseStatus responseStatus,  string responseContent, string requestContent)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseStatus = responseStatus;
            ResponseContent = responseContent;
            RequestContent = requestContent;
        }

        public MachineshopPlatformException(string message, HttpStatusCode statusCode, ResponseStatus responseStatus, ErrorResponse errorResponse)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseStatus = responseStatus;
            ErrorResponse = errorResponse;
        }

        public MachineshopPlatformException(string message, HttpStatusCode statusCode, ResponseStatus responseStatus, ErrorResponse errorResponse, string responseContent, string requestContent)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseStatus = responseStatus;
            ErrorResponse = errorResponse;
            ResponseContent = responseContent;
            RequestContent = requestContent;
        }
    }
}
