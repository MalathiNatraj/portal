using System;
using System.ComponentModel;
using System.Linq;
using Diebold.Domain.Entities;
using System.Collections.Generic;
using System.Web.Mvc;
using Diebold.Services.Extensions;

namespace Diebold.WebApp.Models
{
    public enum DeviceStatusFilter
    {
        AllDevices,
        ActiveDevices,
        DeletedDevices,
    }

    public enum DateTypeFilter
    {
        Period,
        Range
    }

    public class AlertFilterViewModel 
    {
        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public DateTime DisplayDateFrom { get; set; }

        public DateTime DisplayDateTo { get; set; }

        public List<string> AlertTypes { get; set; }

        public List<AlertTypeViewModel> AlertTypeView { get; set; }

        public List<UserInvolvedViewModel> UserInvolvedView { get; set; }

        public MultiSelectList AvailableAlertTypes { get; protected set; }

        public IList<string> AvailableAlertTypeList
        {
            set
            {
                var availableAlertType = new List<SelectListItem>
                                               {
                                                   new SelectListItem
                                                       {
                                                           Text = "All",
                                                           Value = "all"
                                                       }
                                               };

                    
                availableAlertType.AddRange(value.Select(x => new SelectListItem
                                    {
                                            Text = x.SeparatedCamelCase(),
                                            Value = x
                                        }));

                AvailableAlertTypes = new MultiSelectList(availableAlertType, "Value", "Text");
            }
        }

        public string DeviceStatus { get; set; }
        public SelectList AvailableDeviceStatus
        {
            get
            {
                var availableDeviceStatus = Enum.GetNames(typeof (DeviceStatusFilter)).Select(status => new SelectListItem
                                                                                                            {
                                                                                                                Text = status.ToString().SplitByUpperCase(),
                                                                                                                Value = status
                                                                                                            }).ToList();
                return new SelectList(availableDeviceStatus, "Value", "Text");
            }
        }

        public string DateType { get; set; }
        public SelectList AvailableDateTypes
        {
            get
            {
                var availableDateTypes = Enum.GetNames(typeof (DateTypeFilter)).Select(status => new SelectListItem
                                                                                                         {
                                                                                                             Text = status, Value = status
                                                                                                         }).ToList();
                return new SelectList(availableDateTypes, "Value", "Text");
            }
        }
        public List<string> DisplayUserIds { get; set; }
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
    }

    public class AlertTypeViewModel
    {
        public string AlertType { get; set; }
    }

    public class UserInvolvedViewModel
    {
        public string UserInvolved { get; set; }
        public int UserId { get; set; }
    }
}