using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Diebold.Domain.Entities;
using System.Web.Mvc;
using System.ComponentModel;

namespace Diebold.WebApp.Models
{
    public class DeviceViewModel : BaseMappeableViewModel<Dvr>
    {
        static DeviceViewModel()
        {
            Mapper.CreateMap<Dvr, DeviceViewModel>()
                .ForMember(dest => dest.GatewayId, opt => opt.MapFrom(src => src.Gateway.Id))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Company.Id))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Site.Id));

            Mapper.CreateMap<DeviceViewModel, Dvr>()
                .ForMember(dest => dest.Gateway, opt => opt.MapFrom(src => new Gateway {Id = src.GatewayId.Value}))
                 .ForMember(dest => dest.Site, opt => opt.MapFrom(src => new Site { Id = src.SiteId.Value }))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => new Company {Id = src.CompanyId.Value}));
        }

        public DeviceViewModel(Dvr device)
        {
            Mapper.Map(device, this);
        }

        public DeviceViewModel()
        {
        }

        [JqGridColumnSortable(true, Index = "Name")]
        [JqGridColumnLabel(Label = "Device Name")]
        [StringLength(32)]
        [Required(ErrorMessage = "Device Name is required")]
        [DisplayName("Device Name: (*)")]
        public string Name { get; set; }

        [StringLength(32)]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Device ID is required")]
        [DisplayName("Device ID: (*)")]
        public string DeviceKey { get; set; }

        [JqGridColumnSortable(true, Index = "DeviceType")]
        [JqGridColumnLabel(Label = "Device Type")]
        [StringLength(32)]
        [Required(ErrorMessage = "Device Type is required")]
        [DisplayName("Device Model: (*)")]
        public string DeviceType { get; set; }

        [JqGridColumnSortable(true, Index = "HostName")]
        [JqGridColumnLabel(Label = "IP / Host Name")]
        [StringLength(32)]
        [Required(ErrorMessage = "IP Address is required")]
        [DisplayName("IP Address: (*)")]
        public string HostName { get; set; }

        [DisplayName("User:")]
        [ScaffoldColumn(false)]
        public string UserName { get; set; }

        [DisplayName("Password:")]
        [ScaffoldColumn(false)]
        public string Password { get; set; }

        [DisplayName("DST:")]
        [ScaffoldColumn(false)]
        public bool IsInDST { get; set; }

        [DisplayName("Port A:")]
        [ScaffoldColumn(false)]
        public string PortA { get; set; }

        [DisplayName("Port B:")]
        [ScaffoldColumn(false)]
        public string PortB { get; set; }

        //[JqGridColumnSortable(true, Index = "Gateway.Site.Id")]
        //[JqGridColumnLabel(Label = "Site ID")]
        //[Required(ErrorMessage = "Site is required")]
        //[DisplayName("Site ID:")]
        //public int? SiteId { get; set; }

        [JqGridColumnSortable(true, Index = "Site.Id")]
        [JqGridColumnLabel(Label = "Site ID")]
        [Required(ErrorMessage = "Site is required")]
        [DisplayName("Site ID:")]
        public int? SiteId { get; set; }

        [JqGridColumnSortable(true, Index = "Site.Name")]
        [JqGridColumnLabel(Label = "Site Name")]
        [StringLength(32)]
        [DisplayName("Site Name:")]
        public string SiteName { get; set; }

        [JqGridColumnSortable(true, Index = "Gateway.Name")]
        [JqGridColumnLabel(Label = "Gateway Name")]
        [StringLength(32)]
        [DisplayName("Gateway Name:")]
        public string GatewayName { get; set; }

        [JqGridColumnSortable(true, Index = "Company.Name")]
        [JqGridColumnLabel(Label = "Company Name")]
        [StringLength(32)]
        [DisplayName("Company Name:")]
        public string CompanyName { get; set; }

        [StringLength(32)]
        [DisplayName("Polling Frequency:")]
        [ScaffoldColumn(false)]
        public string PollingFrequency { get; set; }

        [StringLength(32)]
        [Required(ErrorMessage = "Local Time Zone is required")]
        [DisplayName("Local Time Zone:")]
        [ScaffoldColumn(false)]
        public string TimeZone { get; set; }

        [StringLength(32)]
        [Required(ErrorMessage = "HealthCheck Version is required")]
        [DisplayName("Gateway Version: (*)")]
        [ScaffoldColumn(false)]
        public string HealthCheckVersion { get; set; }

        [StringLength(32)]
        [DisplayName("Zone Number:")]
        [ScaffoldColumn(false)]
        public string ZoneNumber { get; set; }

        [Required(ErrorMessage = "Number of Cameras is required")]
        [DisplayName("Number of Cameras: (*)")]
        [ScaffoldColumn(false)]
        public int NumberOfCameras { get; set; }

        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Device Type is required")]
        public int DeviceTypeId { get; set; }

        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Gateway is required")]
        public int? GatewayId { get; set; }

        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Company is required")]
        public int? CompanyId { get; set; }

        [DisplayName("Type: (*)")]
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

        [DisplayName("Gateway Name: (*)")]
        public SelectList AvailableGateways { get; protected set; }
        private IList<Gateway> availableGatewayList;
        public IList<Gateway> AvailableGatewayList
        {
            get { return availableGatewayList; }
            set
            {
                availableGatewayList = value;
                AvailableGateways = new SelectList(value, "Id", "Name");
                
            }
        }

        [DisplayName("Company Name: (*)")]
        public SelectList AvailableCompanies { get; protected set; }
        public IList<Company> AvailableCompanyList
        {
            set
            {
                AvailableCompanies = new SelectList(value, "Id", "Name");
            }
        }

        [DisplayName("Site Name: (*)")]
        public SelectList AvailableSites { get; protected set; }
        public IList<Site> AvailableSiteList
        {
            set
            {
                AvailableSites = new SelectList(value, "Id", "Name");
            }
        }

        [DisplayName("Polling Frequency: (*)")]
        public SelectList AvailablePollingFrequencies { get; protected set; }
        public IDictionary<string, string> AvailablePollingFrequencyList
        {
            set
            {
                var availablPollingFrequency = value
                    .Select(pollingFrequency => new SelectListItem
                            {
                                Text = pollingFrequency.Key,
                                Value = pollingFrequency.Value.ToString()
                            }).ToList();
                AvailablePollingFrequencies = new SelectList(availablPollingFrequency, "Value", "Text");
            }
        }

        [DisplayName("Local TimeZone: (*)")]
        public SelectList AvailableTimeZones { get; protected set; }
        public IList<TimeZoneInfo> AvailableTimeZoneList
        {
            set
            {
                var availableTimeZone = value
                    .Select(timeZone => new SelectListItem
                    {
                        Text = timeZone.DisplayName,
                        Value = timeZone.Id
                    }).ToList();
                AvailableTimeZones = new SelectList(availableTimeZone, "Value", "Text");
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
        
        [JqGridColumnSortable(false)]
        [HiddenInput]
        public bool IsDisabled { get; set; }

        [JqGridColumnFormatter("$.actionColumnFormatter")]
        [JqGridColumnSortable(false)]
        [DisplayName("Actions")]
        [JqGridColumnLayout(Width = 150)]
        public string ActionColumn { get; set; }
                
    }
}