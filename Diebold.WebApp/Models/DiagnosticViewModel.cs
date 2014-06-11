using System.Collections.Generic;
using System.Web.Mvc;

namespace Diebold.WebApp.Models
{
    public class DiagnosticViewModel
    {
        public DiagnosticViewModel() 
        { 
            InitialSignature();
        }

        private void InitialSignature()
        {
            var intialSignatureRaw = new List<SelectListItem>
            {
                new SelectListItem { Text = "Device", Value = "Device", Selected = true },
                new SelectListItem { Text = "Gateway", Value = "Gateway"}
            };
            AvailableOptions = new SelectList(intialSignatureRaw, "Value", "Text");
        }

        public SelectList AvailableOptions { get; protected set; }
        public string Option { get; set; }
    }
}