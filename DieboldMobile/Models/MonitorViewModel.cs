using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Diebold.Domain.Entities;

namespace DieboldMobile.Models
{
    public class MonitorViewModel
    {
        [HiddenInput]
        [DisplayName("")]
        public int? CompanyId { get; set; }

        [HiddenInput]
        [DisplayName("")]
        public int? UserId { get; set; }

        [DisplayName("")]
        public SelectList AvailableUsers { get; protected set; }
        public IList<User> AvailableUserList
        {
            set
            {
                var availableUsers = value.Select(user => new SelectListItem
                {
                    Text = user.Name,
                    Value = user.Id.ToString()
                }).ToList();

                AvailableUsers = new SelectList(availableUsers, "Value", "Text");
            }
        }

        [DisplayName("")]
        public SelectList AvailableCompanies { get; protected set; }
        public IList<Company> AvailableCompanyList
        {
            set
            {
                var availableCompanies = value.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToList();

                AvailableCompanies = new SelectList(availableCompanies, "Value", "Text");
            }
        }

    }
}