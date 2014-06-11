using System.Collections;
using System.Collections.Generic;
using Diebold.Platform.Proxies.REST.Enums;
using Diebold.Platform.Proxies.REST.Structures;

namespace Diebold.Platform.Proxies.REST
{
    public interface IRestManager
    {
        IDictionary<string, string> Headers { get; }
        string BaseUrl { get; }
        string Username { get; }
        string Password { get; }
        ContentFormat RequestFormat { get; }

        /// <summary>
        /// Request to the given resource.
        /// Default request method is POST. Default request format is Xml.
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <returns></returns>
        T Request<T>(string resource) where T : new();

        /// <summary>
        /// Request to the given resource.
        /// Default request method is POST. Default request format is Xml.
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="requestParameters">The object with properties to add as parameters</param>
        /// <returns></returns>
        T Request<T>(string resource, RequestParameters requestParameters) where T : new();

        /// <summary>
        /// Request to the given resource.
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="requestParameters">The object with properties to add as parameters</param>
        /// <param name="requestMethod">The method used in the request</param>
        /// <returns></returns>
        T Request<T>(string resource, RequestParameters requestParameters, RequestMethod requestMethod) where T : new();

        /// <summary>
        /// Request to the given resource.
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="requestParameters">The object with properties to add as parameters</param>
        /// <param name="requestMethod">The method used in the request</param>
        /// <param name="requestFormat">The format used in the request</param>
        /// <returns></returns>
        T Request<T>(string resource, RequestParameters requestParameters, RequestMethod requestMethod, ContentFormat requestFormat) where T : new();

        /// <summary>
        /// Request to the given resource.
        /// 
        /// Uses the last part of the Uri (resource) as the name for the root element.
        /// </summary>
        /// <typeparam name="T">The type of the item of the list expected</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="requestParameters">The object with properties to add as parameters</param>
        /// <param name="requestMethod">The method used in the request</param>
        /// <returns>A list of T objects</returns>
        IList<T> RequestCollection<T>(string resource, RequestParameters requestParameters, RequestMethod requestMethod) where T : new();


        /// <summary>
        /// Request to the given resource.
        /// </summary>
        /// <typeparam name="T">The type of the item of the list expected</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="requestParameters">The object with properties to add as parameters</param>
        /// <param name="requestMethod">The method used in the request</param>
        /// <param name="rootElement">The name of the element used as root element for the array</param>
        /// <returns>A list of T objects</returns>
        IList<T> RequestCollection<T>(string resource, RequestParameters requestParameters, RequestMethod requestMethod, string rootElement) where T : new();

        /// <summary>
        /// Executes a GET request and returns an object of type T
        /// </summary>
        /// <typeparam name="T">The type of the expected result</typeparam>
        /// <param name="resource">The resource url</param>
        /// <returns></returns>
        T GETRequest<T>(string resource) where T : new();

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
        T GETRequest<T>(string resource, params object[] paramList) where T : new();

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
        T GETRequest<T>(string resource, ContentFormat requestFormat, params object[] paramList) where T : new();

        /// <summary>
        /// Executes a GET request and returns a collection of T objects
        /// 
        /// Uses the last part of the Uri (resource) as the name for the root element.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list returned</typeparam>
        /// <param name="resource">The resource url</param>
        /// <returns></returns>
        IList<T> GETRequestCollection<T>(string resource) where T : new();

        /// <summary>
        /// Executes a GET request and returns a collection of T objects
        /// </summary>
        /// <typeparam name="T">The type of the items in the list returned</typeparam>
        /// <param name="resource">The resource url</param>
        /// <param name="rootElement">The name the collection has in the result</param>
        /// <returns></returns>
        IList<T> GETRequestCollection<T>(string resource, string rootElement) where T : new();

        /// <summary>
        /// Request to the given resource returning a GenericResponse (not typed response).
        /// </summary>
        /// <param name="resource">The resource url</param>
        /// <param name="requestParameters">The object with properties to add as parameters</param>
        /// <returns></returns>
        GenericResponse Request(string resource, RequestParameters requestParameters);

        /// <summary>
        /// Request to the given resource returning a GenericResponse (not typed response).
        /// 
        /// </summary>
        /// <param name="resource">The resource uri</param>
        /// <param name="requestParameters">The request parameters</param>
        /// <param name="requestMethod">The method used in the request</param>
        /// <returns></returns>
        GenericResponse Request(string resource, RequestParameters requestParameters, RequestMethod requestMethod);

        /// <summary>
        /// Request to the given resource returning a GenericResponse (not typed response).
        /// </summary>
        /// <param name="resource">The resource uri</param>
        /// <param name="requestParameters">The request parameters</param>
        /// <param name="requestMethod">The method used in the request</param>
        /// <param name="requestFormat">The format used in the request</param>
        /// <returns></returns>
        GenericResponse Request(string resource, RequestParameters requestParameters, RequestMethod requestMethod, ContentFormat requestFormat);

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
        GenericResponse GETRequest(string resource, ContentFormat requestFormat, params object[] paramList);

        /// <summary>
        /// Async Request to the given resource
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="requestParameters"></param>
        /// <param name="requestMethod"></param>
        void AsyncRequest(string resource, RequestParameters requestParameters, RequestMethod requestMethod);
    }
}