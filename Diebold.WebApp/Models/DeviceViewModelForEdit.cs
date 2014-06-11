using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Diebold.Domain;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Diebold.Domain.Entities;
using System.Web.Mvc;
using System.ComponentModel;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace Diebold.WebApp.Models
{
    public class DeviceViewModelForEdit : BaseMappeableViewModel<Dvr>
    {
         static DeviceViewModelForEdit()
        {
            Mapper.CreateMap<Dvr, DeviceViewModelForEdit>()
                .ForMember(dest => dest.GatewayId, opt => opt.MapFrom(src => src.Gateway.Id))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Company.Id))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Site.Id));

            Mapper.CreateMap<DeviceViewModelForEdit, Dvr>()
                .ForMember(dest => dest.Gateway, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.HealthCheckVersion, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceType, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalDeviceId, opt => opt.Ignore())
                .ForMember(dest => dest.ZoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceKey, opt => opt.Ignore());
        }

        public DeviceViewModelForEdit()
        {
        }

         public DeviceViewModelForEdit(Dvr device)
        {
            Mapper.Map(device, this); 
        }

         [StringLength(32)]
         [ScaffoldColumn(false)]
         [DisplayName("Device ID: (*)")]
         public string DeviceKey { get; set; }

         [JqGridColumnSortable(true, Index = "Name")]
         [JqGridColumnLabel(Label = "Device Name")]
         [StringLength(32)]
         [Required]
         [DisplayName("Device Name: (*)")]
         public string Name { get; set; }

         [JqGridColumnSortable(true, Index = "DeviceType")]
         [JqGridColumnLabel(Label = "Device Type")]
         [StringLength(32)]
         [DisplayName("Device Type: (*)")]
         public string DeviceType { get; set; }

         [JqGridColumnSortable(true, Index = "HostName")]
         [JqGridColumnLabel(Label = "IP/Host Name")]
         [StringLength(32)]
         [Required]
         [DisplayName("IP Address: (*)")]
         public string HostName { get; set; }

         [DisplayName("User:")]
         public string UserName { get; set; }

         [DisplayName("Password:")]
         public string Password { get; set; }

         [DisplayName("DST:")]
         public bool IsInDST { get; set; }

         [DisplayName("Port A:")]
         public string PortA { get; set; }

         [DisplayName("Port B:")]
         public string PortB { get; set; }

         [JqGridColumnSortable(true, Index = "Site.Id")]
         [Required]
         [DisplayName("Site Id")]
         public int SiteId { get; set; }

         [StringLength(32)]
         [DisplayName("Polling Frequency:")]
         [ScaffoldColumn(false)]
         public string PollingFrequency { get; set; }

         [StringLength(32)]
         [DisplayName("Local Time Zone")]
         [ScaffoldColumn(false)]
         public string TimeZone { get; set; }

         [StringLength(32)]
         [DisplayName("HealthCheck Version: (*)")]
         [ScaffoldColumn(false)]
         public string HealthCheckVersion { get; set; }
        
         [Required]
         [DisplayName("Number of Cameras: (*)")]
         [ScaffoldColumn(false)]
         public int NumberOfCameras { get; set; }
                 
         [HiddenInput]
         public string UpdatedCamera { get; set; }

         [StringLength(32)]
         [DisplayName("Zone Number: (*)")]
         [ScaffoldColumn(false)]
         public string ZoneNumber { get; set; }

         [ScaffoldColumn(false)]
         [Required]
         public int DeviceTypeId { get; set; }

         [ScaffoldColumn(false)]
         [Required]
         public int GatewayId { get; set; }

         [ScaffoldColumn(false)]
         [Required]
         public int CompanyId { get; set; }

         [DisplayName("Type: (*)")]
         public SelectList AvailableDeviceTypes { get; protected set; }
         public IList<string> AvailableDeviceTypeList
         {
             set
             {
                 List<SelectListItem> availableTypes = new List<SelectListItem>();
                 foreach (string deviceType in value)
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

         [DisplayName("Gateway: (*)")]
         public SelectList AvailableGateways { get; protected set; }
         public IList<Gateway> AvailableGatewayList
         {
             set
             {
                 AvailableGateways = new SelectList(value, "Id", "Name");
             }
         }

         [DisplayName("Company: (*)")]
         public SelectList AvailableCompanies { get; protected set; }
         public IList<Company> AvailableCompanyList
         {
             set
             {
                 AvailableCompanies = new SelectList(value, "Id", "Name");
             }
         }

         [DisplayName("Site: (*)")]
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
                 List<SelectListItem> availableTimeZone = new List<SelectListItem>();
                 foreach (TimeZoneInfo timeZone in value)
                 {
                     availableTimeZone.Add(new SelectListItem
                     {
                         Text = timeZone.DisplayName,
                         Value = timeZone.Id
                     });
                 }
                 AvailableTimeZones = new SelectList(availableTimeZone, "Value", "Text");
             }
         }

         public SelectList AvailableHealthCheckVersions { get; protected set; }
         public IList<string> AvailableHealthCheckVersionList
         {
             set
             {
                 List<SelectListItem> availableHealthCheckVersion = new List<SelectListItem>();
                 foreach (string healthCheckVersion in value)
                 {
                     availableHealthCheckVersion.Add(new SelectListItem
                     {
                         Text = healthCheckVersion,
                         Value = healthCheckVersion
                     });
                 }
                 AvailableHealthCheckVersions = new SelectList(availableHealthCheckVersion, "Value", "Text");
             }
         }

         [JqGridColumnSortable(false)]
         [HiddenInput]
         public bool IsDisabled { get; set; }

         [JqGridColumnFormatter("$.actionColumnFormatter")]
         [JqGridColumnSortable(false)]
         [DisplayName("Actions")]
         [JqGridColumnLayout(Width = 150, Alignment = JqGridAlignments.Center)]
         public string ActionColumn { get; set; }
                
    }
}