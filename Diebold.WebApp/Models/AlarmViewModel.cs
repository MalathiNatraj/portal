using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Diebold.Domain.Entities;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace Diebold.WebApp.Models
{
    public class AlarmViewModel
    {
        [StringLength(32)]
        [DisplayName("Device Type: (*)")]
        [Required]
        public string DeviceType { get; set; }

        [StringLength(32)]
        [DisplayName("Gateway version: (*)")]
        [Required]
        public string HealthCheckVersion { get; set; }

        [StringLength(32)]
        [DisplayName("Alarm Type: (*)")]
        [Required]
        public string AlarmType { get; set; }
        
        public SelectList AvailableDeviceTypes { get; protected set; }
        public IList<string> AvailableDeviceTypeList
        {
            set
            {
                var availableTypes = new List<SelectListItem>();
                foreach (var deviceType in value)
                {
                    availableTypes.Add(new SelectListItem
                    {
                        Text = deviceType,
                        Value = deviceType
                    });
                }
                AvailableDeviceTypes = new SelectList(availableTypes, "Value", "Text");
            }
        }

        public SelectList AvailableHealthCheckVersions { get; protected set; }
        public IList<string> AvailableHealthCheckVersionList
        {
            set
            {
                var availableHealthCheckVersion = value
                    .Select(healthCheckVersion => new SelectListItem
                    {
                        Text = healthCheckVersion,
                        Value = healthCheckVersion
                    }).ToList();
                AvailableHealthCheckVersions = new SelectList(availableHealthCheckVersion, "Value", "Text");
            }
        }

        public SelectList AvailableCompanies { get; protected set; }

        public IList<Company> AvailableCompanyList
        {
            set
            {
                List<SelectListItem> availableCompanies = new List<SelectListItem>();

                foreach (Company item in value)
                {
                    availableCompanies.Add(new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name

                    });
                }
                AvailableCompanies = new SelectList(availableCompanies, "Value", "Text");
            }
        }

        [JqGridColumnSortable(true, Index = "CompanyId")]
        [DisplayName("Company ID (*)")]
        [ScaffoldColumn(false)]
        [JqGridColumnLayout()]
        public int CompanyId { get; set; }

        [JqGridColumnLayout()]
        [DisplayName("Company:(*)")]
        [StringLength(32)]
        [Required]
        public string CompanyName { get; set; }
    }
}