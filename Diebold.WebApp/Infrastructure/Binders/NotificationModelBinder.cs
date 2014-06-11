using System;
using System.Linq;
using System.IO;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Diebold.WebApp.Models;
using Diebold.Domain.Entities;

namespace Diebold.WebApp.Infrastructure.Binders
{
    public class NotificationModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var contentType = controllerContext.HttpContext.Request.ContentType;
            if (!contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                return (null);

            string jsonStringData;
            using (var stream = controllerContext.HttpContext.Request.InputStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                    jsonStringData = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(jsonStringData))
                return (null);

            var model = new NotificationViewModel();

            var elements = (IDictionary<string, object>)new JavaScriptSerializer().Deserialize<dynamic>(jsonStringData);

            if (elements.Any())
            {
                model.Alert = GetAlert(elements);
                //model.Status = GetStatus(elements);
            }

            return model;
        }

        private Alert GetAlert(IDictionary<string,object> elements)
        {
            var alertSerializer = new JavaScriptSerializer().Serialize((object)elements["alert"]);
            var alert = new JavaScriptSerializer().Deserialize<Alert>(alertSerializer);

            switch (alert.AlarmName)
            {
                case "driveTemp": alert.AlarmName = "DriveTemperature"; break;
            }

            alert.RelationalOperator = GetCustomOperator(alert.RelationalOperator);

            return alert;
        }

        //private IList<Status> GetStatus(IDictionary<string, object> elements)
        //{
        //    if (elements.ContainsKey("status"))
        //    {
        //        var statusSerializer = new JavaScriptSerializer().Serialize((object)elements["status"]);
        //        var status = new JavaScriptSerializer().Deserialize<IList<Status>>(statusSerializer);
        //        return status;
        //    }

        //    return null;
        //}

        private string GetCustomOperator(string relationalOperator)
        {
            var dieboldOperator = string.Empty;
            switch (relationalOperator)
            {
                case ">": dieboldOperator = AlarmOperator.GreaterThan.ToString(); break;
                case ">=": dieboldOperator = AlarmOperator.GreaterThanOrEquals.ToString(); break;
                case "<": dieboldOperator = AlarmOperator.LessThan.ToString(); break;
                case "<=": dieboldOperator = AlarmOperator.LessThanOrEquals.ToString(); break;
                case "==": dieboldOperator = AlarmOperator.Equals.ToString(); break;
                case "!=": dieboldOperator = AlarmOperator.NotEquals.ToString(); break;
            }

            return dieboldOperator;
        }
    }
}