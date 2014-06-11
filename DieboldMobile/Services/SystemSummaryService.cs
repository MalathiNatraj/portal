using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DieboldMobile.Models;

namespace DieboldMobile.Services
{
    public class SystemSummaryDevice
    {
        public int Id { get; set; }
        public String Name { get; set; }
    }

    public class SystemSummaryService
    {
        public IList<SystemSummaryDevice> GetAllSystemSummaryDevice()
        {
            List<SystemSummaryDevice> lstSystemSummaryDevice = new List<SystemSummaryDevice> 
            {
                new SystemSummaryDevice{Id = 1, Name = "Access"},
                new SystemSummaryDevice{Id = 2, Name = "Intrusion"},
                new SystemSummaryDevice{Id = 3, Name = "Health"},
            };
            return lstSystemSummaryDevice;
        }

        public IList<SystemSummaryModel> GetAllSystemSummaryDetails()
        {
            List<SystemSummaryModel> lstSystemSummaryModel = new List<SystemSummaryModel> 
            {
                new SystemSummaryModel{DeviceTypeId = 1, Status = "Trouble (29)", Value = 29, DeviceName = "Access"},
                new SystemSummaryModel{DeviceTypeId = 1, Status = "Ok (70)", Value = 70, DeviceName = "Access"},
                new SystemSummaryModel{DeviceTypeId = 1, Status = "Offline (1)", Value = 1, DeviceName = "Access"},

                new SystemSummaryModel{DeviceTypeId = 2, Status = "Armed (55)", Value = 55, DeviceName = "Intrusion"},
                new SystemSummaryModel{DeviceTypeId = 2, Status = "Disarmed (40)", Value = 40, DeviceName = "Intrusion"},
                new SystemSummaryModel{DeviceTypeId = 2, Status = "Offline (5)", Value = 5, DeviceName = "Intrusion"},

                new SystemSummaryModel{DeviceTypeId = 3, Status = "Trouble (20)", Value = 20, DeviceName = "Health"},
                new SystemSummaryModel{DeviceTypeId = 3, Status = "Ok (80)", Value = 80, DeviceName = "Health"},
                new SystemSummaryModel{DeviceTypeId = 3, Status = "Offline (0)", Value = 0, DeviceName = "Health"}

            };

            return lstSystemSummaryModel;
        }
    }
}
