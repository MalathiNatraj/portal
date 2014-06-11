using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Diebold.Platform.Proxies.Exceptions;
using Diebold.Platform.Proxies.REST.Deserializers;
using Diebold.Platform.Proxies.REST.Enums;
using Diebold.Platform.Proxies.REST.ErrorHandling;
using Diebold.Platform.Proxies.REST.Extensions;
using Diebold.Platform.Proxies.REST.Structures;
using ElkRiv.API.SDK.REST.Serializers;
using Newtonsoft.Json;
using RestSharp;
using ResponseStatus = RestSharp.ResponseStatus;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using log4net.Core;
using log4net;


namespace Diebold.Platform.Proxies.REST
{
    [Serializable]
    public class RestManager : IRestManager
    {
        protected static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);    
        public IDictionary<string, string> Headers { get; private set; }

        public string BaseUrl { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public ContentFormat RequestFormat { get; private set; }

        #region Init

        public RestManager()
        {
        }

        public RestManager(string baseUrl, ContentFormat requestFormat)
        {
            BaseUrl = baseUrl;
            RequestFormat = requestFormat;
            Headers = new Dictionary<string, string>();
        }

        public RestManager(string baseUrl, string username, string password, IDictionary<string, string> headers)
        {
            BaseUrl = baseUrl;
            Username = username;
            Password = password;
            Headers = headers;    // Headers used on every request.
            RequestFormat = ContentFormat.Xml;
        }

        public RestManager(string baseUrl, string username, string password, IDictionary<string, string> headers, ContentFormat requestFormat)
        {
            BaseUrl = baseUrl;
            Username = username;
            Password = password;
            Headers = headers;    // Headers used on every request.
            RequestFormat = requestFormat;
        }

        #endregion

        #region Private Execute methods

        /// <summary>
        /// Defines which handler should be used for each content type returned.
        /// </summary>
        /// <param name="client">RestSharp client instance.</param>
        private static void SetHandlers(RestClient client)
        {
            // Clean handlers
            client.ClearHandlers();

            // Register default handlers
            client.AddHandler("application/json", new JsonNetDeserializer());
            client.AddHandler("text/json", new JsonNetDeserializer());
            client.AddHandler("text/x-json", new JsonNetDeserializer());
            client.AddHandler("text/javascript", new JsonNetDeserializer());
            client.AddHandler("*", new JsonNetDeserializer());
        }

        /// <summary>
        /// Used to return raw content from a HTTP response, without parsing it to any particular type.
        /// Useful mainly when it is necessary to return non-plain-text content types.
        /// </summary>
        /// <param name="request">RestSharp Rest Request.</param>
        /// <returns>An structure including Headers, Content, Content Type, HTTP Status Code and Description.</returns>
        private GenericResponse Execute(RestRequest request)
        {
            var client = new RestClient
                             {
                                 BaseUrl = BaseUrl,
                                 //Authenticator = new HttpBasicAuthenticator(Username, Password)
                             };

            SetHandlers(client);

            if (Headers != null)
            {
                foreach (var header in Headers)
                {
                    request.AddParameter(header.Key, header.Value, ParameterType.HttpHeader);
                }
            }

            var response = client.Execute(request);

            ValidateResponse(response, request);

            return new GenericResponse
                       {
                           Headers = response.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                           Content = response.Content,
                           ContentType = response.ContentType,
                           StatusCode = response.StatusCode,
                           StatusDescription = response.StatusDescription
                       };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">RestSharp Rest Request.</param>
        private void ExecuteAsync(RestRequest request)
        {
            var client = new RestClient
            {
                BaseUrl = BaseUrl,
                //Authenticator = new HttpBasicAuthenticator(Username, Password)
            };

            SetHandlers(client);

            if (Headers != null)
            {
                foreach (var header in Headers)
                {
                    request.AddParameter(header.Key, header.Value, ParameterType.HttpHeader);
                }
            }

            client.ExecuteAsync(request, (response)=> ValidateResponse(response, request));
        }

        /// <summary>
        /// Used to retrieve content returned from a HTTP request, parsed to an instance of type T.
        /// </summary>
        /// <typeparam name="T">The expected T type</typeparam>
        /// <param name="request">RestSharp Rest Request.</param>
        /// <returns>An instance of T, if response could match its structure.</returns>
        /// <exception cref="MachineshopPlatformException">If response couldn't match T object,
        /// this Exception is thrown, including Details of Error in its ServiceResponse field.</exception>
        private T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient
                             {
                                 BaseUrl = BaseUrl,
                                 //Authenticator = new HttpBasicAuthenticator(Username, Password)
                             };

            SetHandlers(client);

            if (Headers != null)
            {
                foreach (var header in Headers)
                {
                    request.AddParameter(header.Key, header.Value, ParameterType.HttpHeader);
                }
            }
            var response = client.Execute<T>(request);

            ValidateResponse(response, request);

            return response.Data;
        }

        private static string GetRequestContent(RestRequest request)
        {
            var requestContent = new StringBuilder();

            if (!string.IsNullOrEmpty(request.Resource))
                requestContent.Append("Resource: " + request.Resource + "\r\n");

            foreach (var parameter in request.Parameters)
            {
                requestContent.Append("Parameter: " + parameter.Value + "\r\n");
            }

            return requestContent.ToString();
        }

        #endregion

        #region Response Validation

        private static void ValidateResponseStandardValues(HttpStatusCode statusCode, ResponseStatus responseStatus, ContentFormat contentFormat, string content, RestRequest request)
        {
            if (statusCode == HttpStatusCode.ServiceUnavailable)
            {
                throw new MachineshopPlatformException("API service is not available.", statusCode, responseStatus.GetResponseStatus(), content, GetRequestContent(request));
            }

            if (statusCode == HttpStatusCode.NotFound)
            {
                throw new MachineshopPlatformException("API call not found.", statusCode, responseStatus.GetResponseStatus(), content, GetRequestContent(request));
            }

            if (statusCode == HttpStatusCode.Forbidden)
            {
                throw new MachineshopPlatformException("Forbidden call to API.", statusCode, responseStatus.GetResponseStatus(), content, GetRequestContent(request));
            }

            if (statusCode != HttpStatusCode.OK)
            {
                var errorResponse = ParseErrorResponse(content, contentFormat);
                throw new MachineshopPlatformException("Error calling Platform API.", statusCode, responseStatus.GetResponseStatus(), errorResponse, content, GetRequestContent(request));
            }

            if (responseStatus != ResponseStatus.Completed)
            {
                throw new MachineshopPlatformException("Error calling Platform API (Request not completed).", statusCode, responseStatus.GetResponseStatus(), content, GetRequestContent(request));
            }
        }

        private static void ValidateResponse(RestResponse response, RestRequest request)
        {
            ValidateResponseStandardValues(response.StatusCode, response.ResponseStatus, response.ContentType.GetContentFormat(),
                                   response.Content, request);
        }

        private static void ValidateResponse<T>(RestResponse<T> response, RestRequest request)
        {
            ValidateResponseStandardValues(response.StatusCode, response.ResponseStatus, response.ContentType.GetContentFormat(),
                                   response.Content, request);
            
            // If type is not what was expected, Data will be null.
            // Throw a RequestException with ServiceResponse set to the real answer from service.
            if (response.Data == null)
            {
                var errorResponse = ParseErrorResponse(response.Content, response.ContentType.GetContentFormat());
                throw new MachineshopPlatformException("Error calling Platform API.", response.StatusCode, response.ResponseStatus.GetResponseStatus(), errorResponse);
            }

            if (response.ContentType.GetContentFormat() == ContentFormat.Unsupported)
            {
                throw new MachineshopPlatformException("Content returned from Platform API not supported.", response.StatusCode, response.ResponseStatus.GetResponseStatus());
            }
            
            
        }

        private static ErrorResponse ParseErrorResponse(string content, ContentFormat contentFormat)
        {
            ErrorResponse errorResponse;

            try
            {
                switch (contentFormat)
                {
                    case ContentFormat.Json:
                        errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
                        break;

                    default:
                        //NOT TESTED
                        throw new NotImplementedException();

                        //var ns = new System.Xml.Serialization.XmlSerializerNamespaces();
                        //ns.Add(string.Empty, string.Empty);
                        //var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ErrorResponse));
                        //serviceResponse =
                        //    serializer.Deserialize(new StringReader(response.Content)) as ErrorResponse;
                }
            }
            catch (Exception)
            {
                throw new MachineshopPlatformException("Error parsing error from API call for " + contentFormat + " content format.");
            }

            return errorResponse;
        }

        #endregion

        #region Parameters

        private static void AddUriParameters(IRestRequest request, params object[] paramList)
        {
            if (paramList == null) return;

            for (var index = 0; index < paramList.Length; index++)
                if (paramList[index] != null)
                    request.AddParameter(index.ToString(), paramList[index], ParameterType.UrlSegment);
        }

        #endregion

        #region POST, PUT, DELETE

        /// <summary>
        /// Request to the given resource.
        /// Default request method is POST. Default request format is Xml.
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <returns></returns>
        public T Request<T>(string resource) where T : new()
        {
            return Request<T>(resource, null, RequestMethod.POST, RequestFormat, null);
        }

        /// <summary>
        /// Request to the given resource.
        /// Default request method is POST. Default request format is Xml.
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="requestParameters">The object with properties to add as parameters</param>
        /// <returns></returns>
        public T Request<T>(string resource, RequestParameters requestParameters) where T : new()
        {
            return Request<T>(resource, requestParameters, RequestMethod.POST, RequestFormat, null);
        }

        /// <summary>
        /// Request to the given resource.
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="requestParameters">The object with properties to add as parameters</param>
        /// <param name="requestMethod">The method used in the request</param>
        /// <returns></returns>
        public T Request<T>(string resource, RequestParameters requestParameters, RequestMethod requestMethod) where T : new()
        {
            return Request<T>(resource, requestParameters, requestMethod, RequestFormat, null);
        }

        /// <summary>
        /// Request to the given resource.
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="requestParameters">The object with properties to add as parameters</param>
        /// <param name="requestMethod">The method used in the request</param>
        /// <param name="requestFormat">The format used in the request</param>
        /// <returns></returns>
        public T Request<T>(string resource, RequestParameters requestParameters, RequestMethod requestMethod, ContentFormat requestFormat) where T : new()
        {
            return Request<T>(resource, requestParameters, requestMethod, requestFormat, null);
        }

        public T Request<T>(string resource, RequestParameters requestParameters, RequestMethod requestMethod, ContentFormat requestFormat, string rootElement) where T : new()
        {
            var request = PrepareRequest(resource, requestParameters, requestMethod, requestFormat, rootElement);

            return Execute<T>(request);
        }

        public IList<T> RequestCollection<T>(string resource, RequestParameters requestParameters, RequestMethod requestMethod) where T : new()
        {
            return RequestCollection<T>(resource, requestParameters, requestMethod, GetStandardRootElementFromUri(resource));
        }

        public IList<T> RequestCollection<T>(string resource, RequestParameters requestParameters, RequestMethod requestMethod, string rootElement) where T : new()
        {
            return Request<List<T>>(resource, requestParameters, requestMethod, RequestFormat, rootElement);
        }

        public IList<T> GETRequestCollection<T>(string resource) where T : new()
        {
            return GETRequestCollection<T>(resource, GetStandardRootElementFromUri(resource));
        }

        public IList<T> GETRequestCollection<T>(string resource, string rootElement) where T : new()
        {
            return Request<List<T>>(resource, null, RequestMethod.GET, RequestFormat, rootElement);
        }

        private string GetStandardRootElementFromUri(string uri)
        {
            return uri.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
        }
        
        /// <summary>
        /// Request to the given resource returning a GenericResponse (not typed response).
        /// </summary>
        /// <param name="resource">The resource url</param>
        /// <param name="requestParameters">The object with properties to add as parameters</param>
        /// <returns></returns>
        public GenericResponse Request(string resource, RequestParameters requestParameters)
        {
            var request = PrepareRequest(resource, requestParameters, RequestMethod.POST, RequestFormat, null);

            return Execute(request);
        }

        /// <summary>
        /// Request to the given resource returning a GenericResponse (not typed response).
        /// 
        /// </summary>
        /// <param name="resource">The resource uri</param>
        /// <param name="requestParameters">The request parameters</param>
        /// <param name="requestMethod">The method used in the request</param>
        /// <returns></returns>
        public GenericResponse Request(string resource, RequestParameters requestParameters, RequestMethod requestMethod)
        {
            var request = PrepareRequest(resource, requestParameters, requestMethod, RequestFormat, null);

            return Execute(request);
        }

        public void AsyncRequest(string resource, RequestParameters requestParameters, RequestMethod requestMethod)
        {
            var request = PrepareRequest(resource, requestParameters, requestMethod, RequestFormat, null);

            ExecuteAsync(request);
        }

        /// <summary>
        /// Request to the given resource returning a GenericResponse (not typed response).
        /// 
        /// </summary>
        /// <param name="resource">The resource uri</param>
        /// <param name="requestParameters">The request parameters</param>
        /// <param name="requestMethod">The method used in the request</param>
        /// <param name="requestFormat">The format used in the request</param>
        /// <returns></returns>
        public GenericResponse Request(string resource, RequestParameters requestParameters, RequestMethod requestMethod, ContentFormat requestFormat)
        {
            var request = PrepareRequest(resource, requestParameters, requestMethod, requestFormat, null);

            return Execute(request);
        }

        private static RestRequest PrepareRequest(string resource, RequestParameters requestParameters, RequestMethod requestMethod, ContentFormat requestFormat, string rootElement)
        {
            var request = new RestRequest // Don't ReSharp this!
                              {
                                  Resource = resource,
                                  Method = requestMethod.GetMethod(),
                                  RequestFormat = requestFormat.GetContentFormat(),
                                  RootElement = rootElement,
                                  Timeout = 60*1000
                              };

            request.JsonSerializer = new JsonNetSerializer();
            
            if (requestParameters != null)
            {
                // Add parameters to URI.
                if (requestParameters.UriData != null)
                    AddUriParameters(request, requestParameters.UriData);

                // Add data to body.
                if (requestParameters.PostData != null)
                    request.AddBody(requestParameters.PostData);

                // Add a file.
                if (requestParameters.PostFile != null)
                    request.AddFile(requestParameters.PostFile.Name, requestParameters.PostFile.Data, requestParameters.PostFile.FileName, null);
            }
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            string strAuthToken = "Basic " + Base64Encode.Encode64(System.Configuration.ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"] + ":X");
            request.AddHeader("Authorization", strAuthToken);
            return request;
        }

        #endregion

        #region GET

        /// <summary>
        /// Executes a GET request
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <returns></returns>
        public T GETRequest<T>(string resource) where T : new()
        {
            return GETRequest<T>(resource, null);
        }

        /// <summary>
        /// Executes a GET request
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="paramList">The params list to build up the resource url. This method will get the first
        /// parameters and use them as a UrlSegment. The remaining parameters will be added to the query string.
        /// Example:
        /// ("Catalogs/{0}/Categories/{1}/Products", 1, 32) will get  "Catalogs/1/Categories/32/Products"
        /// 
        /// ("Catalogs/{0}/Categories/{1}/Products?q={2}&i={3}&p={4}", 1, 32, "fire", true, 18) will get  "Catalogs/1/Categories/32/Products?q=fire&i=true&p=18"
        /// 
        /// </param>
        /// <returns></returns>
        public T GETRequest<T>(string resource, params object[] paramList) where T : new()
        {
            return Request<T>(resource, new RequestParameters(paramList), RequestMethod.GET);
        }

        /// <summary>
        /// Executes a GET request
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="requestFormat">The format used in the request</param>
        /// <param name="paramList">The params list to build up the resource url. This method will get the first
        /// parameters and use them as a UrlSegment. The remaining parameters will be added to the query string.
        /// Example:
        /// ("Catalogs/{0}/Categories/{1}/Products", 1, 32) will get  "Catalogs/1/Categories/32/Products"
        /// 
        /// ("Catalogs/{0}/Categories/{1}/Products?q={2}&i={3}&p={4}", 1, 32, "fire", true, 18) will get  "Catalogs/1/Categories/32/Products?q=fire&i=true&p=18"
        /// 
        /// </param>
        /// <returns></returns>
        public T GETRequest<T>(string resource, ContentFormat requestFormat, params object[] paramList) where T : new()
        {
            return Request<T>(resource, new RequestParameters(paramList), RequestMethod.GET, requestFormat);
        }

        /// <summary>
        /// Executes a GET request which returns a generic HTTP response (not parsed to any type of object).
        /// </summary>
        /// <param name="resource">The resource url</param>
        /// <param name="requestFormat">The format used in the request</param>
        /// <param name="paramList">The params list to build up the resource url. This method will get the first
        /// parameters and use them as a UrlSegment. The remaining parameters will be added to the query string.
        /// Example:
        /// ("Catalogs/{0}/Categories/{1}/Products", 1, 32) will get  "Catalogs/1/Categories/32/Products"
        /// 
        /// ("Catalogs/{0}/Categories/{1}/Products?q={2}&i={3}&p={4}", 1, 32, "fire", true, 18) will get  "Catalogs/1/Categories/32/Products?q=fire&i=true&p=18"
        /// 
        /// </param>
        /// <returns></returns>
        public GenericResponse GETRequest(string resource, ContentFormat requestFormat, params object[] paramList)
        {
            return Request(resource, new RequestParameters(paramList), RequestMethod.GET, requestFormat);
        }

        #endregion


        public string ExecuteAPICall(string url, string bodyContent, string commandName)
        {
            logger.Debug("Entered Method ExecuteAPICall");
            logger.Debug("Request JSON for "+commandName+" : " + bodyContent + " URL : " + url);
            string strBaseUrl = ConfigurationManager.AppSettings["MachineshopPlatformURL"].ToString() + url;
            logger.Debug("Base URL : " + strBaseUrl);
            string strAck = string.Empty;// response = null;
            try
            {
                logger.Debug("Inside try block");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strBaseUrl);
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var authorization = "Basic " + Base64Encode.Encode64(ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"].ToString() + ":X");
                request.Headers.Add("Authorization", authorization);

                request.Method = WebRequestMethods.Http.Post;
                request.Proxy = null;
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(bodyContent);
                
                request.ContentLength = data.Length;
                logger.Debug(" Received request.contentLength" + request.ContentLength.ToString());
                Stream newStream = request.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();
                Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);
                var response = task.ContinueWith(t => ReadStreamFromResponse(t.Result));
                strAck = response.Result;
                logger.Debug("Acknowldgement for " + commandName + " : " + strAck);
            }
            catch(Exception ex)
            {
                logger.Error("Exception occured while service API Call : " + strBaseUrl, ex);
                throw new Exception("Exception occured while processing request with platform", ex);
            }
            logger.Debug("Completed ExecuteAPICall without any Errors");
            return strAck;
        }

        public string ExecuteAPICallforDelete(string url, string commandName)
        {
            logger.Debug("Entered Method ExecuteAPICallforDelete");
            logger.Debug("Request JSON for " + commandName + " : " + " URL : " + url);
            string strBaseUrl = ConfigurationManager.AppSettings["MachineshopPlatformURL"].ToString() + url;
            logger.Debug("Base URL : " + strBaseUrl);
            string strAck = string.Empty;// response = null;
            try
            {
                logger.Debug("Inside try block");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strBaseUrl);
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var authorization = "Basic " + Base64Encode.Encode64(ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"].ToString() + ":X");
                request.Headers.Add("Authorization", authorization);

                logger.Debug("Method Delete start");
                request.Method = "DELETE";
                logger.Debug("Method Delete complete");
                request.Proxy = null;
               
                Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);
                var response = task.ContinueWith(t => ReadStreamFromResponse(t.Result));
                strAck = response.Result;
                logger.Debug("Acknowldgement for " + commandName + " : " + strAck);
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured while service API Call : " + strBaseUrl, ex);
                throw new Exception("Exception occured while processing request with platform", ex);
            }
            logger.Debug("Completed ExecuteAPICallforDelete without any Errors");
            return strAck;
        }

        public string ExecuteAPICallforIntrusion(string url, string commandName)
        {
            logger.Debug("Entered Method ExecuteAPICallforIntrusion");
           // logger.Debug("Request JSON for " + commandName + " : " + bodyContent + " URL : " + url);
            string strBaseUrl = ConfigurationManager.AppSettings["MachineshopPlatformURLforIntrusion"].ToString() + url;
        
            logger.Debug("Base URL : " + strBaseUrl);
            string strAck = string.Empty;// response = null;
            try
            {
                logger.Debug("Inside try block");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strBaseUrl);
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var authorization = "Basic " + Base64Encode.Encode64(ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"].ToString() + ":X");
                request.Headers.Add("Authorization", authorization);

                request.Method = WebRequestMethods.Http.Get;
                request.Proxy = null;              

                Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);
                var response = task.ContinueWith(t => ReadStreamFromResponse(t.Result));
                strAck = response.Result;
                logger.Debug("Acknowldgement for " + commandName + " : " + strAck);
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured while service API Call : " + strBaseUrl, ex);
                throw new Exception("Exception occured while processing request with platform", ex);
            }
            logger.Debug("Completed ExecuteAPICallforIntrusion without any Errors");
            return strAck;
        }

        public string ExecuteAPICallforSite(string url)
        {
            logger.Debug("Entered Method ExecuteAPICallforSite");
            // logger.Debug("Request JSON for " + commandName + " : " + bodyContent + " URL : " + url);
            string strBaseUrl = ConfigurationManager.AppSettings["MachineshopPlatformURLforSite"].ToString() + url;

            logger.Debug("Base URL : " + strBaseUrl);
            string strAck = string.Empty;// response = null;
            try
            {
                logger.Debug("Inside try block");
                Uri uri = new Uri(strBaseUrl, true);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var authorization = "Basic " + Base64Encode.Encode64(ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"].ToString() + ":X");
                request.Headers.Add("Authorization", authorization);

                request.Method = WebRequestMethods.Http.Get;
                request.Proxy = null;

                Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);
                var response = task.ContinueWith(t => ReadStreamFromResponse(t.Result));
                strAck = response.Result;
                logger.Debug("Acknowledgement for Site geo coordinates" + strAck);
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured while service API Call : " + strBaseUrl, ex);
                throw new Exception("Exception occured while processing request with platform", ex);
            }
            logger.Debug("Completed ExecuteAPICallforSite without any Errors");
            return strAck;
        }


        public string ExecuteAPICallforWeatherAlert(string State, string City)
        {
            logger.Debug("Entered Method ExecuteAPICallforWeatherAlert");
            // logger.Debug("Request JSON for " + commandName + " : " + bodyContent + " URL : " + url);
            string strBaseUrl = ConfigurationManager.AppSettings["MachineshopPlatformURLforWeatherAlert"].ToString() + "State=" + State + "&City=" + City;

            logger.Debug("Base URL : " + strBaseUrl);
            string strAck = string.Empty;// response = null;
            try
            {
                logger.Debug("Inside try block");
                Uri uri = new Uri(strBaseUrl, true);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var authorization = "Basic " + Base64Encode.Encode64(ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"].ToString() + ":X");
                request.Headers.Add("Authorization", authorization);

                request.Method = WebRequestMethods.Http.Get;
                request.Proxy = null;

                Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);
                var response = task.ContinueWith(t => ReadStreamFromResponse(t.Result));
                strAck = response.Result;
                logger.Debug("Acknowledgement for Site WeatherAlert" + strAck);
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured while service API Call : " + strBaseUrl, ex);
                throw new Exception("Exception occured while processing request with platform", ex);
            }
            logger.Debug("Completed ExecuteAPICallforSite without any Errors");
            return strAck;
        }

        public string ExecutePlatformAPICall(string url, string commandName)
        {
            logger.Debug("Entered Method ExecuteAPICall for " + commandName);           
            string strBaseUrl = ConfigurationManager.AppSettings["MachineshopPlatformURLforIntrusion"].ToString() + url;

            logger.Debug("Base URL for " + commandName + " : " + strBaseUrl);
            string strAck = string.Empty;
            try
            {
                logger.Debug("Inside try block");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strBaseUrl);
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var authorization = "Basic " + Base64Encode.Encode64(ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"].ToString() + ":X");
                request.Headers.Add("Authorization", authorization);

                request.Method = WebRequestMethods.Http.Get;
                request.Proxy = null;

                Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);
                var response = task.ContinueWith(t => ReadStreamFromResponse(t.Result));
                strAck = response.Result;
                logger.Debug("Acknowldgement for " + commandName + " : " + strAck);
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured while service API Call : " + strBaseUrl, ex);
                throw new Exception("Exception occured while processing " + commandName +" request with platform", ex);
            }
            logger.Debug("Completed ExecuteAPICall for " + commandName + " without any Errors");
            return strAck;
        }
        public string ExecutePlatformAPIforEMC(string bodyContent, string commandName)
        {
            logger.Debug("Entered Method ExecutePlatformAPIforEMC");
            logger.Debug("Request JSON for " + commandName + " : " + bodyContent);
            string strBaseUrl = ConfigurationManager.AppSettings["EMCEndPoint"].ToString();
            logger.Debug("Base URL for " + commandName + " : " + strBaseUrl);
            string strAck = string.Empty;
            try
            {
                logger.Debug("Inside try block");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strBaseUrl);
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var authorization = "Basic " + Base64Encode.Encode64(ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"].ToString() + ":X");
                request.Headers.Add("Authorization", authorization);

                request.Method = WebRequestMethods.Http.Post;
                request.Proxy = null;
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(bodyContent);

                request.ContentLength = data.Length;
                logger.Debug(" Received request.contentLength" + request.ContentLength.ToString());
                Stream newStream = request.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();

                Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);
                var response = task.ContinueWith(t => ReadStreamFromResponse(t.Result));
                strAck = response.Result;
                logger.Debug("Acknowldgement for " + commandName + " : " + strAck);
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured while service API Call : " + strBaseUrl, ex);
                throw new Exception("Exception occured while processing " + commandName + " request with platform", ex);
            }
            logger.Debug("Completed ExecuteAPICall for " + commandName + " without any Errors");
            return strAck;
        }

        public string ExecuteAPICallforSystemSummary(string url, string bodyContent, string commandName)
        {
            logger.Debug("Request JSON for system summary with " + commandName + " : " + bodyContent + " URL : " + url);
            string strAck = string.Empty;// response = null;
            try
            {
                string strBaseUrl = ConfigurationManager.AppSettings["MachineshopPlatformURLforSystemSummary"].ToString() + url;
                logger.Debug("Base URL : " + strBaseUrl);
                strAck = GenerateAPICall(strBaseUrl, bodyContent);
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured while service API Call : " + url, ex);
                throw ex;
            }
            logger.Debug("Response JSON for system summary with " + commandName + " : " + strAck);
            return strAck;
        }
        public string ExecuteAPICallforMAS(string url, string bodyContent, string commandName)
        {
            logger.Debug("Request JSON for " + commandName + " : " + bodyContent + " URL : " + url);
            string strAck = string.Empty;// response = null;
            try
            {
                string strBaseUrl = ConfigurationManager.AppSettings["MachineshopPlatformURLforMAS"].ToString() + url;
                logger.Debug("Base URL : " + strBaseUrl);
                strAck = GenerateAPICallForProductionTest(strBaseUrl, bodyContent); // Need to revert back
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured while service API Call : " + url, ex);
                throw ex;
            }
            logger.Debug("Response JSON for " + commandName + " : " + strAck);
            return strAck;
        }

        public string ExecuteAPICallforFireWidget(string url, string bodyContent)
        {
            logger.Debug("Request JSON for " + bodyContent + " URL : " + url);
            string strAck = string.Empty;// response = null;
            try
            {
                string strBaseUrl = ConfigurationManager.AppSettings["MachineshopPlatformURLforMAS"].ToString() + url;
                logger.Debug("Base URL : " + strBaseUrl);
                strAck = GenerateAPICall(strBaseUrl, bodyContent);
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured while service API Call : " + url, ex);
                throw ex;
            }
            logger.Debug("Response JSON for  : " + strAck);
            return strAck;
        }

        public string GenerateAPICall(string strBaseUrl, string bodyContent)
        {
            logger.Debug("Base URL : " + strBaseUrl + " Body Content : " + bodyContent);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strBaseUrl);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            var authorization = "Basic " + Base64Encode.Encode64(ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"].ToString() + ":X");
            request.Headers.Add("Authorization", authorization);

            request.Method = WebRequestMethods.Http.Post;
            request.Proxy = null;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(bodyContent);

            request.ContentLength = data.Length;

            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);
            var response = task.ContinueWith(t => ReadStreamFromResponse(t.Result));
            return response.Result;
        }

        // Need to be reverted back
        public string GenerateAPICallForProductionTest(string strBaseUrl, string bodyContent)
        {
            logger.Debug("Base URL : " + strBaseUrl + " Body Content : " + bodyContent);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strBaseUrl);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            var authorization = "Basic " + Base64Encode.Encode64(ConfigurationManager.AppSettings["PlatformAPIAuthrizationToken"].ToString() + ":X");
            request.Headers.Add("Authorization", authorization);

            request.Method = WebRequestMethods.Http.Post;
            request.Proxy = null;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(bodyContent);

            request.ContentLength = data.Length;

            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);
            var response = task.ContinueWith(t => ReadStreamFromResponse(t.Result));
            return response.Result;
        }

        public static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }

    }
}
