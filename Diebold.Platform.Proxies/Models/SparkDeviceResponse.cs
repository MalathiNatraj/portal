namespace Diebold.Platform.Proxies.Models
{
    public abstract class SparkDeviceResponse
    {
        public SparkDeviceHeader Header { get; set; }

        internal string ResponseData { get; set; }

        public SparkDeviceResponse(string responseData)
        {
            this.ResponseData = responseData;
        }

        
    }
}