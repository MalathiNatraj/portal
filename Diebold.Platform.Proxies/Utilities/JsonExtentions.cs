using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Platform.Proxies.Utilities
{
    public static class  JsonExtentions
    {
        #region header/device settings
        private static dynamic setHeaders(dynamic data) {
            data.callback_url = string.Format("{0}/PlatformResponse/{1}", ConfigurationManager.AppSettings["ResponseCallBackUrl"].ToString(), data.name);
            var p = new Dictionary<String, object>();
            p["Accept"] = "application / json";
            p["Content-Type"] = "application / json";
            data.callback_headers = p;
            return data;
        }
        private static dynamic setDevice(dynamic data, string deviceName, string id)
        {
            data.Devices = new
            {
                HTTP = new[]{ 
                    new {
                        DeviceType = deviceName,
                        Identifier = new {
                            //_id = id,
                            created_at = DateTime.UtcNow.ToString(),
                            ip_address = ConfigurationManager.AppSettings["GatewayIpAddress"],
                            port = ConfigurationManager.AppSettings["GatewayPort"],
                            updated_at = DateTime.UtcNow.ToString()
                        }
                    }
                }
            };
            data.command_type = "HTTP";
            data.device_instance_id = id;

            return data;
        }
        private static dynamic setPayload(dynamic data, string deviceName, string id) {

            data = setHeaders(data);
            data = setDevice(data, deviceName, id);
            var payload = new
            {
                name = id,
                commands = new[] { data }
            };
            return payload;
        }
        #endregion

        public static string toAddUserCode2Request(this IntrusionDTO data)
        {
            dynamic command = new ExpandoObject();
            command.name = "UserCodeAdd2";
            command.command_type = "HTTP";
            command.device_instance_id = data.DeviceInstanceId;
            command.key_value_pairs = new object[] { };

            dynamic device = new ExpandoObject();
            device.request = "";
            device.deviceType = "SparkIntrusion";
            device.timeSent = DateTime.UtcNow.ToString();
            device.deviceKey = data.ExternalDeviceKey;


            var userCodeInfo = new Dictionary<string, string>();
            userCodeInfo.Add("userNumber", data.UserNumber);
            userCodeInfo.Add("userCode", data.UserCode);
            userCodeInfo.Add("pinCode", data.PinCode);
            userCodeInfo.Add("profileNumber", data.ProfileNumber);
            userCodeInfo.Add("tempDate", string.Empty);
            userCodeInfo.Add("userName", data.UserName);


            device.userCodeInformation2 = new
            {
                name = "userCodeInformation2",
                properties = new
                {
                    property = userCodeInfo.Select(x => new { name = x.Key, value = x.Value })
                },
                areasAuthorityLevel = new {
                    name = "areasAuthorityLevel",
                    properties = new
                    {
                        property = data.AccessLevels.Select(x => new { name = x.Key, value = x.Value })
                    }
                }

            };

            command.key_value_pairs = device;

            
            return Newtonsoft.Json.JsonConvert.SerializeObject(setHeaders(command));
        }
        public static string toModifyUserCode2Request(this IntrusionDTO data)
        {
            dynamic command = new ExpandoObject();
            command.name = "UserCodeModify2";
            command.command_type = "HTTP";
            command.device_instance_id = data.DeviceInstanceId;
            command.key_value_pairs = new object[] { };

            dynamic device = new ExpandoObject();
            device.request = "";
            device.deviceType = "SparkIntrusion";
            device.timeSent = DateTime.UtcNow.ToString();
            device.deviceKey = data.ExternalDeviceKey;


            var userCodeInfo = new Dictionary<string, string>();
            userCodeInfo.Add("userNumber", data.UserNumber);
            userCodeInfo.Add("userCode", data.UserCode);
            userCodeInfo.Add("pinCode", data.PinCode);
            userCodeInfo.Add("profileNumber", data.ProfileNumber);
            userCodeInfo.Add("tempDate", string.Empty);
            userCodeInfo.Add("userName", data.UserName);


            device.userCodeInformation2 = new
            {
                name = "userCodeInformation2",
                properties = new
                {
                    property = userCodeInfo.Select(x => new { name = x.Key, value = x.Value })
                },
                areasAuthorityLevel = new
                {
                    name = "areasAuthorityLevel",
                    properties = new
                    {
                        property = data.AccessLevels.Select(x => new { name = x.Key, value = x.Value })
                    }
                }

            };

            command.key_value_pairs = device;


            return Newtonsoft.Json.JsonConvert.SerializeObject(setHeaders(command));
        }
        public static string toDeleteUserCode2Request(this IntrusionDTO data)
        {
            dynamic command = new ExpandoObject();
            command.name = "UserCodeDelete2";
            command.command_type = "HTTP";
            command.device_instance_id = data.DeviceInstanceId;
            command.key_value_pairs = new object[] { };

            dynamic device = new ExpandoObject();
            device.request = "";
            device.deviceType = "SparkIntrusion";
            device.timeSent = DateTime.UtcNow.ToString();
            device.deviceKey = data.ExternalDeviceKey;


            var userCodeInfo = new Dictionary<string, string>();
            userCodeInfo.Add("userNumber", data.UserNumber);


            command.key_value_pairs = device;


            return Newtonsoft.Json.JsonConvert.SerializeObject(setHeaders(command));
        }
    }
}
