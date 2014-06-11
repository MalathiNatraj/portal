using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DieboldMobile.Infrastructure.Response
{
    public class AjaxOperationErrorResponse
    {
        public AjaxOperationErrorResponse()
        {
            messages = new List<string>();
        }

        public List<string> messages { get; protected set; }

        public void ProcessModelErrors(ModelStateDictionary modelState)
        {
            IEnumerable<ModelError> modelErrors = modelState.Keys.SelectMany(key => modelState[key].Errors);
            foreach (ModelError modelError in modelErrors)
            {
                messages.Add(modelError.ErrorMessage);
            }
        }
    }
}