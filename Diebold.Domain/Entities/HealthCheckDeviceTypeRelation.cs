using System.Collections.Generic;
using System.Linq;
using System;

namespace Diebold.Domain.Entities
{
    public static class HealthCheckDeviceTypeRelation
    {
        private static readonly Dictionary<HealthCheckVersion, IList<DeviceType>> Mapping =
            new Dictionary<HealthCheckVersion, IList<DeviceType>>
                {
                    {
                        HealthCheckVersion.Version1,                       
                          new List<DeviceType> {DeviceType.Costar111, DeviceType.ipConfigure530, DeviceType.VerintEdgeVr200}
                    },
                    { 
                        HealthCheckVersion.Version2,
                        new List<DeviceType> {DeviceType.bosch_D9412GV4, DeviceType.videofied01, DeviceType.dmpXR500, DeviceType.dmpXR100 }
                    }
                };


        public static IList<DeviceType> GetDeviceTypes(HealthCheckVersion healthCheckVersion)
        {
            return Mapping.Where(x => x.Key == healthCheckVersion).Single().Value;
        }

        public static IList<string> GetDeviceTypeNames(HealthCheckVersion healthCheckVersion)
        {
            return Mapping.Where(x => x.Key == healthCheckVersion).Single().Value.Select(type => type.ToString()).ToList();
        }

        public static IDictionary<DeviceType, string> GetDeviceTypebyParentType()
        {
            IDictionary<DeviceType, string> dctDeviceType = new Dictionary<DeviceType, string>();
            foreach (DeviceType val in Enum.GetValues(typeof(DeviceType)))
            {
                if (val == DeviceType.VerintEdgeVr200 || val == DeviceType.Costar111 || val == DeviceType.ipConfigure530)
                {
                    dctDeviceType.Add(val, "DVR");
                }
                else if (val == DeviceType.eData524 || val == DeviceType.eData300 || val == DeviceType.dmpXR100Access || val == DeviceType.dmpXR500Access)
                {
                    dctDeviceType.Add(val, "Access");
                }
                else if (val == DeviceType.dmpXR100 || val == DeviceType.dmpXR500 || val == DeviceType.bosch_D9412GV4 || val == DeviceType.videofied01)
                {
                    dctDeviceType.Add(val, "Intrusion");
                }
            }
            return dctDeviceType;
        }

    }
}
