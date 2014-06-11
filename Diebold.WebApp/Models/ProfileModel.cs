using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Diebold.WebApp.Models
{
    public class ProfileModel
    {
        [DisplayName("E-Mail: ")]
        public string Email { get; set; }
        [DisplayName("First Name: ")]
        public string FirstName { get; set; }
        [DisplayName("Last Name: ")]
        public string LastName { get; set; }
        [DisplayName("User Name: ")]
        public string UserName { get; set; }
        [DisplayName("Phone No: ")]
        public string Phone { get; set; }
        [DisplayName("Extension: ")]        
        public string Extension { get; set; }
        [DisplayName("Mobile No: ")]
        public string Mobile { get; set; }
        [DisplayName("Title: ")]
        public string Title { get; set; }
        [DisplayName("Role Name: ")]
        public string RoleName { get; set; }
        [DisplayName("Company Name ")]
        public string CompanyName { get; set; }
        [DisplayName("TimeZone: ")]
        public string TimeZone { get; set; }
        [DisplayName("Last Login: ")]
        public DateTime LastLogin { get; set; }
        [DisplayName("Last Login: ")]
        public string LastLogon { get; set; }
        [DisplayName("Text1: ")]
        public string Text1 { get; set; }
        [DisplayName("Text2: ")]
        public string Text2 { get; set; }
    }
}