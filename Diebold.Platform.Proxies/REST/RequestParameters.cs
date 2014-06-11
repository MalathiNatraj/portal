namespace Diebold.Platform.Proxies.REST
{
    public class RequestParameters
    {
        public object PostData { get; set; }
        public object[] UriData { get; set; }
        public PostFile PostFile { get; set; }

        public RequestParameters()
        {
        }

        /// <summary>
        /// The parameters to be added to the request
        /// </summary>
        /// <param name="postData">The data to be posted. This data can be any object that could be serialized as Json or Xml</param>
        public RequestParameters(object postData)
        {
            PostData = postData;
        }

        public RequestParameters(object[] uriData)
        {
            UriData = uriData;
        }

        public RequestParameters(PostFile postFile)
        {
            PostFile = postFile;
        }

        /// <summary>
        /// The parameters to be added to the request
        /// </summary>
        /// <param name="postData">The data to be posted. This data can be any object that could be serialized as Json or Xml</param>
        /// <param name="uriData"></param>
        public RequestParameters(object postData, params object[] uriData)
        {
            PostData = postData;
            UriData = uriData;
        }
    }

    /// <summary>
    /// Container for files to be uploaded with requests
    /// </summary>
    public class PostFile
    {

        ///<summary>
        /// Container for files to be uploaded with requests
        ///</summary>
        ///<param name="name">The parameter name to use in the request.</param>
        ///<param name="data">The data to use as the file's contents.</param>
        ///<param name="filename">The filename to use in the request.</param>
        ///<param name="contentType">The content type to use in the request.</param>
        public PostFile(string name, byte[] data, string filename, string contentType)
        {
            Name = name;
            Data = data;
            FileName = filename;
            ContentType = contentType;
        }

        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The data to use as the file's contents
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// Name of the file to use when uploading
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// MIME content type of file
        /// </summary>
        public string ContentType { get; set; }

    }
}