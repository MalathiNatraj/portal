using Newtonsoft.Json;

namespace Diebold.Platform.Proxies.DTO
{
    public class ResponseDTO:BaseResponseDTO
    {
        public payload payload { get; set; }
    }

    public class payload
    {
        public string txid { get; set; }
        public CommandResponse command_response { get; set; }
        public CommandResponseMessage[] messages { get; set; }
    }

    public class DeviceMediaCaptureResponseDTO : ResponseDTO
    {
        public payload payload { get; set; }
    }
}
