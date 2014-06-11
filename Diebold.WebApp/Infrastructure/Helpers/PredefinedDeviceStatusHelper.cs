using System.Collections.Generic;
using System.Linq;
using Diebold.Domain.Entities;
using Diebold.WebApp.Models;

namespace Diebold.WebApp.Infrastructure.Helpers
{
    public static class PredefinedDeviceStatusHelper
    {
        public static IList<string> DvrShortList = new List<string>() { "timestamprecorder", "startedon", "uptime", "daysrecorded", "isnotrecording", "networkdown" };
        
        public static IList<string> DvrAdditionalList = new List<string>() { "timestampagent","estimatedfreerecording","devicefirmware","videoloss","smart",
                                                                        "drivetemp","raidstatus","dvrerrorcode"};

        public static IList<string> Removed = new List<string>() { "dvrIdentifier" };
        
        public static IList<DeviceStatus> SortStatus(IList<DeviceStatus> status)
        {
            var predefinedOrder = new List<string>();
            predefinedOrder.AddRange(DvrShortList);
            predefinedOrder.AddRange(DvrAdditionalList);

            var dictionary = predefinedOrder.ToDictionary<string, string, DeviceStatus>(reference => reference.ToLower(), reference => null);

            foreach (var deviceStatus in status.Where(deviceStatus => !Removed.Contains(deviceStatus.Name)))
            {
                var statusName = deviceStatus.Name.ToLower();
                if (dictionary.ContainsKey(statusName))
                    dictionary[statusName] = deviceStatus;
                else
                    dictionary.Add(statusName, deviceStatus);
            }

            var list = dictionary.Values.ToList();
            list.RemoveAll(s => s == null); //|| string.IsNullOrEmpty(s.Value.ToString()));
            return list;
        }
        
        public static DvrDrivesViewModel GetDvrDriveViewModel(Device device, DeviceStatusViewModel model, string itemKey, object itemValue)
        {
            var dvrDriveModel = new DvrDrivesViewModel() { Value = itemValue };
            var elementIdentifier = int.Parse(itemKey);

            if (!model.IsCollection)
            {
                var camera = ((Dvr)device).Cameras.Where(x => x.Channel == elementIdentifier.ToString()).SingleOrDefault();

                if (camera == null || !camera.Active)
                    return null;

                dvrDriveModel.Name = camera.Name;
            }
            else
            {
                elementIdentifier = elementIdentifier + 1;
                dvrDriveModel.Name = model.Name.GetFirstUpperCaseRange() + " " + elementIdentifier;
            }

            return dvrDriveModel;
        }
    }
}