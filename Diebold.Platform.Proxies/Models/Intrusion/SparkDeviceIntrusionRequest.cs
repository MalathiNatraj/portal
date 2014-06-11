using System;

namespace Diebold.Platform.Proxies.Models.Intrusion
{
    public abstract class SparkDeviceIntrusionRequest : SparkDeviceRequest
    {
        
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string PinCode { get; set; }
        public string ProfileNumber { get; set; }
        public string TempDate { get; set; }
        public string UserNumber { get; set; }

        public SparkDeviceIntrusionRequest(SparkDeviceHeader sparkHeader)
            : base(sparkHeader)
        {
           
        }

        internal abstract override void BuildRequest(dynamic body);

        internal virtual void BuildProperties(dynamic properties) { 
            if (!string.IsNullOrWhiteSpace(UserName))
                    properties.userName(UserName);

                if (!string.IsNullOrWhiteSpace(UserCode))
                    properties.userCode(UserCode);

                if (!string.IsNullOrWhiteSpace(PinCode))
                    properties.pinCode(PinCode);

                if (!string.IsNullOrWhiteSpace(ProfileNumber))
                    properties.profileNumber(ProfileNumber);

                if (!string.IsNullOrWhiteSpace(TempDate))
                    properties.tempDate(TempDate);

                if (!string.IsNullOrWhiteSpace(UserNumber))
                    properties.userNumber(UserNumber);
        }
    }
}