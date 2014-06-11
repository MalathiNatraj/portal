using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Diebold.Services.Extensions
{
    public static class EnumExtensions
    {
        //public static T CastEnumByName<T>(string enumName)
        //{
        //    return (T)Enum.Parse(typeof(T), enumName, true);
        //}

        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null) return string.Empty;

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes
                                                            (typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString().SeparatedCamelCase();
        }

        //public static T Value<T>(this Enum value)
        //{
        //    return (T)Convert.ChangeType(value.GetType().GetField(value.ToString()).GetRawConstantValue(), typeof(T));
        //}
    }

    public static class Enum<T>
    {

        public static IDictionary<string, string> EnumToDictionary()
        {
            var names = Enum.GetNames(typeof(T));
            var values = Enum.GetValues(typeof(T));
            var descriptions = values.Cast<Enum>().Select(x => x.GetDescription()).ToArray();

            var dictionary = new Dictionary<string, string>();

            for (var i = 0; i < names.Length; i++)
            {
                dictionary.Add(descriptions[i], names[i]);
            }

            return dictionary;
        }

        //public static SelectList ToSelectList()
        //{
        //    return new SelectList(Enum.GetValues(typeof(T)).Cast<Enum>().Select(x => new SelectListItem
        //    {
        //        Text = x.GetDescription(),
        //        Value = x.ToString()
        //    }), "Value", "Text");
        //}
    }
}
