using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Services.Helpers
{
    public static class TimeZoneHelper
    {
        public static IList<TimeZoneInfo> GetTimeZoneList()
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(timeZoneInfo => timeZoneInfo).ToList();
        }

        public static IDictionary<string, string> GetTimeZoneDic()
        {
            IDictionary<string, string> retList = new Dictionary<string, string>();
            foreach (TimeZoneInfo timeZoneInfo in TimeZoneInfo.GetSystemTimeZones())
            {
                retList.Add(timeZoneInfo.Id, timeZoneInfo.DisplayName);
            }

            return retList;
        }
    }
}
