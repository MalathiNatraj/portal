using System;
using System.Collections.Generic;
using Diebold.Platform.Proxies.Enums;
using Newtonsoft.Json;
using Diebold.Domain.Entities;

namespace Diebold.Platform.Proxies.DTO
{
    public class DeviceDTO
    {        
        private static string FormatDeviceKey(DeviceTypeEnum type, string macAddress, string deviceKey = null)
        {
            return type == DeviceTypeEnum.SparkGateway ? macAddress : string.Format("{0}-{1}", macAddress, deviceKey);
        }

        private static string GetCustomDeviceType(DeviceType deviceType)
        {
            switch(deviceType.ToString())
            {
                case "Costar111": return "Costar111";
                case "ipConfigure530": return "ipconfigure530";
                default : return deviceType.ToString();
            }
        }

        public DeviceDTO(){}

        //REAL DEVICE
        public DeviceDTO(DeviceTypeEnum type, DeviceType deviceType,  string macAddress, string deviceKey,
            int pollingFrecuency, string parentDeviceId, ConfigurationDTO configurationDto,
            IList<AlarmDTO> alarmList,bool Dst)
        {
            DeviceType = GetCustomDeviceType(deviceType);
            ExternalDeviceKey = FormatDeviceKey(type, macAddress, deviceKey);
            ParentDeviceId = parentDeviceId;
            PollingFrequency = pollingFrecuency;
            Configuration = configurationDto;
            Alarms = alarmList;
            IsInDst = Dst;
        }

        //GATEWAY
        public DeviceDTO(DeviceTypeEnum type, string macAddress, ConfigurationDTO configurationDto)
        {
            DeviceType = type.ToString();
            ExternalDeviceKey = FormatDeviceKey(type, macAddress);
            PollingFrequency = 60; //default value?
            Configuration = configurationDto;
        }

        [JsonProperty(PropertyName = "deviceKey")]
        public string ExternalDeviceKey { get; set; }

        [JsonProperty(PropertyName = "deviceType")]
        public string DeviceType { get; set; }

        [JsonProperty(PropertyName = "parentDeviceId")]
        public string ParentDeviceId { get; set; }

        [JsonProperty(PropertyName = "pollingFrequency")]
        public int PollingFrequency { get; set; }

        [JsonProperty(PropertyName = "configuration")]
        public ConfigurationDTO Configuration { get; set; }

        [JsonProperty(PropertyName = "alarms")]
        public IList<AlarmDTO> Alarms { get; set; }

        [JsonIgnore]
        public string ExternalDeviceId { get; set; }

        [JsonIgnore]
        public bool IsInDst { get; set; }
        
        [JsonIgnore]
        public bool OnLine { get; set; }


        
    }
}
