using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;
using System.Web.Mvc;
using Diebold.Domain.Enums;

namespace Diebold.WebApp.Models
{
    public class LogHistoryFilterViewModel
    {
        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public DateTime DisplayDateFrom { get; set; }

        public DateTime DisplayDateTo { get; set; }

        public List<LogAction> ActionTypes { get; set; }

        public MultiSelectList AvailableActionTypes { get; protected set; }

        public IList<LogAction> AvailableActionTypeList
        {
            set
            {
                var availableActionTypes = new List<SelectListItem>
                                               {
                                                   new SelectListItem
                                                       {
                                                           Text = "All",
                                                           Value = "all"
                                                       }
                                               };

                availableActionTypes.AddRange(value.Select(t => new SelectListItem
                                            {
                                                Text = t.GetDescription(),
                                                Value = t.ToString()
                                            }).ToList());
                
                AvailableActionTypes = new MultiSelectList(availableActionTypes, "Value", "Text");
            }
        }

        public string DateType { get; set; }
        public SelectList AvailableDateTypes
        {
            get
            {
                var availableDateTypes = Enum.GetNames(typeof(DateTypeFilter)).Select(status => new SelectListItem
                {
                    Text = status,
                    Value = status
                }).ToList();

                return new SelectList(availableDateTypes, "Value", "Text");
            }
        }

        public List<int> UserIds { get; set; }
        public MultiSelectList AvailableUsers { get; protected set; }
        public IList<User> AvailableUserList
        {
            set
            {
                var users = new List<SelectListItem>
                                               {
                                                   new SelectListItem
                                                       {
                                                           Text = "All",
                                                           Value = "all"
                                                       }
                                               };

                users.AddRange(value.Select(user => new SelectListItem
                    {
                        Text = user.LastName.ToString() + ", " + user.FirstName.ToString() + " (" + user.Username.ToString() + ")",
                        Value = user.Id.ToString()
                    }).ToList());

                AvailableUsers = new MultiSelectList(users, "Value", "Text");
            }
        }

        public string UserStatus { get; set; }
        public SelectList AvailableUserStatus
        {
            get
            {
                var availableUserStatus = Enum.GetNames(typeof(UserStatus)).Select(status => new SelectListItem
                {
                    Text = status.ToString().SplitByUpperCase(),
                    Value = status
                }).ToList();
                return new SelectList(availableUserStatus, "Value", "Text");
            }
        }

        public List<ActionTypeViewModel> ActionTypeView { get; set; }

        public List<UserInvolvedViewModel> UserInvolvedView { get; set; }
    }

    public class ActionTypeViewModel
    {
        public string ActionType { get; set; }
    }
}
