using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace Diebold.WebApp.Models
{
    public class LogOnModel2
    {
        //[Required(ErrorMessage = "User Name is required.")]
        [Display(Name = "User Id:")]
        [DefaultValue("User Name:")]
        //[Remote("DisallowName", "Account")]
        public string UserName { get; set; }

        //[Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        //[Remote("DisallowPassword", "Account")]
        public string Password { get; set; }
       
        public string DefaultUserName { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Password")]        
        public string DefaultPassword { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string Message { get; set; }

    }
}