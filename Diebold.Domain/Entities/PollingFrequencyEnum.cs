using System.ComponentModel;

namespace Diebold.Domain.Entities
{
    public enum PollingFrequency
    {        
        [Description("1 minute")]
        OneMinute = 60,

        [Description("2 minutes")]
        TwoMinutes = 120,

        [Description("5 minutes")]
        FiveMinutes = 300,

        [Description("10 minutes")]
        TenMinutes = 600,

        [Description("15 minutes")]
        FifteenMinutes = 900,

        [Description("30 minutes")]
        ThirtyMinutes = 1800,

        [Description("1 hour")]                             
        OneHour = 3600,

        [Description("2 hours")]                             
        TwoHours = 7200,

        [Description("5 hours")]
        FiveHours = 18000,

        [Description("10 hours")]
        TenHours = 36000,

        [Description("12 hours")]
        TwelveHourse = 43200,

        [Description("1 Day")]
        OneDay = 86400,

        [Description("2 Days")]
        TwoDays = 172800,
        
        [Description("1 Week")]
        OneWeek = 604800,

        [Description("1 Month")]
        OneMonth = 2419200
    }
}
