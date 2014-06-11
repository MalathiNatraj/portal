using System;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using AutoMapper;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;
using DieboldMobile.Infrastructure.Helpers;
using System.Collections.Generic;

namespace DieboldMobile.Models
{
    public class DeviceStatusViewModel : BaseMappeableViewModel<Device>
    {
        private const string PlatformDateFormat = "yyyy-MM-ddTHH:mm:ss-ffff";

        private static readonly IAlertHandlerFactory _alertHandlerFactory = new AlertHandlerFactory();

        private static object FormatValue(DeviceStatus status)
        {
            if (status.DataType == DataType.Dictionary)
            {
                var handler = _alertHandlerFactory.GetAlertHandlerByAlarmName(status.Name);
                var values = status.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var list = new Dictionary<string, object>();
                for (var index = 0; index < values.Length; index++)
                {
                    var value = values[index];

                    if (handler.SatisfiesCapabilityRule(value))
                        list.Add(index.ToString(), FormatSingleValue(status.DataType, value));
                }

                if (!status.IsCollection)
                {
                    status.Value = status.Value.Replace("[", "{").Replace("]", "}");
                    list = (Dictionary<string, object>)new JavaScriptSerializer().Deserialize<dynamic>(status.Value);
                }

                return list;
            }

            return FormatSingleValue(status.DataType, status.Value);
        }

        private static object FormatSingleValue(DataType dataType, object value)
        {
            if (dataType == DataType.Date)
            {
                DateTime date;

                if (DateTime.TryParseExact(value.ToString(), PlatformDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    return date;

                return value;
            }

            return value;
        }

        public DeviceStatusViewModel(DeviceStatus deviceStatus, bool showAll = true)
        {
            Mapper.CreateMap<DeviceStatus, DeviceStatusViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.UppercaseFirst().SeparatedCamelCase()))
                .ForMember(dest => dest.IsDictionary, opt => opt.MapFrom(src => src.DataType == DataType.Dictionary))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => FormatValue(src)))
                .ForMember(dest => dest.ShowAlways, opt => opt.MapFrom(src => showAll || PredefinedDeviceStatusHelper.DvrShortList.Contains(src.Name.ToLower())));

            Mapper.Map(deviceStatus, this);
        }

        public string Name { get; set; }
        public bool IsDictionary { get; set; }
        public bool IsCollection { get; set; }
        public object Value { get; set; }
        public bool ShowAlways { get; set; }
    }

    public class DvrDrivesViewModel
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}