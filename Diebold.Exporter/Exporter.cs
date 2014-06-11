using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace Diebold.Exporter
{
    public abstract class Exporter
    {
        public abstract byte[] Export<T>(string title, OrientationPageType orientationPage, IList<T> list,
                                         DateTime dateFrom, DateTime dateTo, float[] columnsWidths) where T : new();

        protected IList<PropertyInfo> GetVisibleProperties<T>() where T : new()
        {
            var t = new T();
            var properties = t.GetType().GetProperties();

            return properties.Where(x =>
                {
                    var layoutAttr = (JqGridColumnLayoutAttribute)x.GetCustomAttributes(typeof(JqGridColumnLayoutAttribute), false).FirstOrDefault();
                    var hiddenAttr = (HiddenInputAttribute)x.GetCustomAttributes(typeof(HiddenInputAttribute), false).FirstOrDefault();
                    return (layoutAttr == null || layoutAttr.Viewable) && hiddenAttr == null;
                }).ToList();
        }

        protected IList<string> GetHeaders(IList<PropertyInfo> properties)
        {
            return (from prop in properties
                    let labelAttr = (JqGridColumnLabelAttribute)prop.GetCustomAttributes(typeof(JqGridColumnLabelAttribute), false)
                         .FirstOrDefault()
                    select labelAttr != null ? labelAttr.Label : prop.Name)
                           .ToList();
        }
    }
}