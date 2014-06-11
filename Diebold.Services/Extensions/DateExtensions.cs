using System;
using System.Globalization;

namespace Diebold.Services.Extensions
{
    public static class DateExtensions
    {
        public static string GetTimeAgo(this DateTime date)
        {
            var diffMoment = DateTime.Now.Subtract(date);

            if (diffMoment.Days == 0)
            {
                if (diffMoment.Hours > 0)
                {
                    return diffMoment.Hours.ToString(CultureInfo.InvariantCulture) + ((diffMoment.Hours == 1) ? " hour ago" : " hours ago");
                }
                
                return diffMoment.Minutes + " mins ago";
            }

            // Date format must be localized
            return (diffMoment.Days == 1) ? "Yesterday" : date.ToString("MM/dd hh:mm tt ");
        }
    }
}