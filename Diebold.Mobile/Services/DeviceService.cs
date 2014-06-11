using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DieboldMobile.Models;

namespace DieboldMobile.Services
{
    public class DeviceService
    {
        public IList<DeviceModel> GetDeviceDetails()
        {
            IList<DeviceModel> lstDevice = new List<DeviceModel>
            {
                //new DeviceModel{Id=1, Name="ESX-GW2-XP", Device="ESX-GW2-XP", Location="BOA", Address="12 Main", Address1="1st Street", City="City", State="AR", Zip=727},
                //new DeviceModel{Id =2,  Name="DEV-GW1-2K3", Device="DEV-GW1-2K3", Location="CA", Address="10 Cross",Address1="2 main", City="Arizona", State="Pinal", Zip=874},
                //new DeviceModel{Id = 3, Name="ipconfigure 60", Device="ipconfigure 60", Location="BOA", Address="11 Main",Address1="3rd Street", City="Arizona", State="Pinal", Zip=875},
                //new DeviceModel{Id = 4, Name="Dev-XC-test", Device="Dev-XC-test",Location="CA", Address="12 Cross", Address1="9th Street", City="Arizona", State="Pinal", Zip=874}
            };
            return lstDevice;
        }

        public IList<DeviceModel> GetDeviceDetailsforHealthCheck()
        {
            IList<DeviceModel> lstDevice = new List<DeviceModel>
            {
                //new DeviceModel{Id=1, Device = "DVD", Location="BOA", Address="12 Main", City="City", State="AR", Zip=727},
                //new DeviceModel{Id=1, Device = "ipConfigure 60", Location="CA", Address="12 Cross", City="Arizona", State="Pinal", Zip=874}
            };
            return lstDevice;
        }        
    }
}