using System;
using Diebold.Platform.Proxies.Models;
using Diebold.Platform.Proxies.Models.Intrusion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Diebold.Test.Xml
{
    [TestClass]
    public class UnitTest1
    {
        SparkDeviceHeader sparkDeviceHeader = new SparkDeviceHeader() { 
            DeviceKey = "13453452",
            DeviceType = "TestType",
            TxId = "12341234123",
            Request = "testest",
            TimeSent = DateTime.Now
        };
        [TestMethod]
        public void Create_UserCodeAdd2_Intrusion_Request()
        {
            var request = new SparkDeviceIntrusionUserCodeAdd2Request(sparkDeviceHeader);
            request.PinCode = "234234";
            request.ProfileNumber = "12";
            request.TempDate = DateTime.Now.ToString();
            request.UserCode = "resre";
            request.UserName = "eddie";
            request.UserNumber = "234234243";
            request.AreasAuthorityLevels.Add("area01", "1");
            var d = request.Create();
            Console.Write(d);
            Assert.IsFalse(string.IsNullOrWhiteSpace(d));
        }

        [TestMethod]
        public void Intrusion_GetUserCodesInformation2_Request()
        {
            var request = new SparkDeviceIntrusionGetUserCodesInformation2Request(sparkDeviceHeader);
            request.UserName = "test";
            request.UserCode = "23423423";

            var d = request.Create();
            Console.Write(d);
            Assert.IsFalse(string.IsNullOrWhiteSpace(d));
        }
    }
}
