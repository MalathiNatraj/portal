using Diebold.Domain.Entities;
using System.Linq;

namespace DieboldMobile.Infrastructure.Helpers
{
    public class AlarmHelper
    {
        public static string GetAlertDescriptionForAlert(AlarmType alarmType, string elementIdentifier, Dvr dvr)
        {
            switch (alarmType)
            {
                case AlarmType.VideoLoss:
                    {
                        var camera = dvr.Cameras.Where(x => x.Channel == elementIdentifier).SingleOrDefault();

                        if (camera != null)
                            return camera.Name;

                        return "Camera " + elementIdentifier;
                    }
                case AlarmType.DriveTemperature: return ("Drive " + elementIdentifier);
                case AlarmType.SMART: return ("Drive " + elementIdentifier);
                case AlarmType.RaidStatus: return ("Array " + elementIdentifier);
                default: return string.Empty;
            }
        }

        public static string GetAlertDescriptionForDeviceOk(AlarmType alarmType, string elementIdentifier, Dvr dvr)
        {
            switch (alarmType)
            {
                case AlarmType.VideoLoss:
                    {
                        var camera = dvr.Cameras.Where(x => x.Channel == elementIdentifier).SingleOrDefault();

                        if (camera != null)
                            return camera.Name;

                        return "Camera " + elementIdentifier;
                    }
                default: return string.Empty;
            }
        } 
    }
}