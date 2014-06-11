using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Diebold.WebApp.Models
{
    public class TestViewModel
    {
        public TestViewModel() 
        { 
            InitialSignature();
            SelectStuffSelected = new List<string>();
        }

        private void InitialSignature()
        {
            List<SelectListItem> IntialSignatureRaw = new List<SelectListItem>();
            
            IntialSignatureRaw.Add(new SelectListItem
            {
                Text = "Yes",
                Value = "1",
                Selected = true
            });


            IntialSignatureRaw.Add(new SelectListItem
            {
                Text = "No",
                Value = "0"
            });

            SelectStuff = new SelectList(IntialSignatureRaw, "Value", "Text");
        }

        public virtual MultiSelectList SelectStuff { get; set; }

        public virtual List<string> SelectStuffSelected { get; set; }
    }
}