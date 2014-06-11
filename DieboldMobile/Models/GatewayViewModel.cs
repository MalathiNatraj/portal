﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using Diebold.Domain.Entities;
using System.Web.Mvc;
using AutoMapper;
using System.ComponentModel;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace DieboldMobile.Models
{
    public class GatewayViewModel : BaseMappeableViewModel<Gateway>
    {


        static GatewayViewModel()
        {
            Mapper.CreateMap<Gateway, GatewayViewModel>()
                .ForMember(dest => dest.MacAddressName, opt => opt.MapFrom(src => src.MacAddress))
                .ForMember(dest => dest.ProtocolId, opt => opt.MapFrom(src => src.Protocol))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Company.Id))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Site.Id));

            Mapper.CreateMap<GatewayViewModel, Gateway>()
                .ForMember(dest => dest.MacAddress, opt => opt.MapFrom(src => src.MacAddressName))
                .ForMember(dest => dest.Protocol, opt => opt.MapFrom(src => src.ProtocolId))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => new Company { Id = src.CompanyId.Value }))
                .ForMember(dest => dest.Site, opt => opt.MapFrom(src => new Site { Id = src.SiteId }));
        }


        public GatewayViewModel()
        {
        }


        public GatewayViewModel(Gateway gateway)
        {
            Mapper.Map(gateway, this);
        }


        #region Protocols

        public IList<Protocol> AvailableProtocolsList
        {
            set
            {

                List<SelectListItem> availableProtocols = new List<SelectListItem>();

                foreach (Protocol item in value)
                {

                    availableProtocols.Add(new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name

                    });

                }


                AvailableProtocols = new SelectList(availableProtocols, "Value", "Text");

            }
        }

        public SelectList AvailableProtocols { get; protected set; }

        [JqGridColumnSortable(true, Index = "ProtocolId")]
        [DisplayName("Protocol Id")]
        [ScaffoldColumn(false)]
        public int? ProtocolId { get; set; }


        [JqGridColumnSortable(true, Index = "ProtocolId")]
        [DisplayName("Protocol Version:")]
        [ScaffoldColumn(false)]
        public string ProtocolName { get; set; }


        #endregion


        [JqGridColumnSortable(true, Index = "Name")]
        [DisplayName("Gateway Name: (*)")]
        [JqGridColumnLabel(Label = "Gateway Name")]
        [Required]
        [JqGridColumnLayout()]
        public string Name { get; set; }


        [JqGridColumnSortable(true, Index = "EMCId")]
        [DisplayName("Monitoring Account Number: (*)")]
        [Required]
        [JqGridColumnLayout()]
        [ScaffoldColumn(false)]
        [DefaultValue("")]
        public int EMCId { get; set; }


        #region Time Zone

        [JqGridColumnSortable(true, Index = "TimeZone")]
        [DisplayName("Time Zone:")]
        [ScaffoldColumn(false)]
        [Required]
        [JqGridColumnLayout()]
        public string TimeZone { get; set; }

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

        public SelectList AvailableTimeZones { get; protected set; }

        #endregion


        #region Company

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

        public SelectList AvailableCompanies { get; protected set; }

        [JqGridColumnSortable(true, Index = "Company.Id")]
        [DisplayName("Company ID")]
        [ScaffoldColumn(false)]
        [JqGridColumnLayout()]
        public int? CompanyId { get; set; }

        [JqGridColumnLayout()]
        [JqGridColumnSortable(true, Index = "Company.Name")]
        [DisplayName("Company Name:")]
        [StringLength(32)]
        public string CompanyName { get; set; }



        #endregion


        #region Site

        public IList<Site> AvailableSitesList
        {
            set
            {

                List<SelectListItem> availableSites = new List<SelectListItem>();

                foreach (Site item in value)
                {

                    availableSites.Add(new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name

                    });

                }


                AvailableSites = new SelectList(availableSites, "Value", "Text");

            }
        }

        public SelectList AvailableSites { get; protected set; }

        [JqGridColumnSortable(true, Index = "Site.Id")]
        [DisplayName("Site ID")]
        [JqGridColumnLayout()]
        public int SiteId { get; set; }

        [JqGridColumnLayout()]
        [JqGridColumnSortable(true, Index = "Site.Name")]
        [DisplayName("SiteName")]
        [StringLength(32)]
        public string SiteName { get; set; }

        [JqGridColumnLayout()]
        [JqGridColumnSortable(true, Index = "Site.Address1")]
        [DisplayName("Address1")]
        [StringLength(32)]
        public string SiteAddress1 { get; set; }

        [JqGridColumnLayout()]
        [JqGridColumnSortable(true, Index = "Site.Address2")]
        [DisplayName("Address2")]
        [StringLength(32)]
        public string SiteAddress2 { get; set; }

        [JqGridColumnLayout()]
        [JqGridColumnSortable(true, Index = "Site.City")]
        [DisplayName("City")]
        [StringLength(32)]
        public string SiteCity { get; set; }

        [JqGridColumnLayout()]
        [JqGridColumnSortable(true, Index = "Site.State")]
        [DisplayName("State")]
        [StringLength(32)]
        public string SiteState { get; set; }

        [JqGridColumnLayout()]
        [JqGridColumnSortable(true, Index = "Site.Zip")]
        [DisplayName("Zip")]
        [StringLength(32)]
        public string SiteZip { get; set; }

        [JqGridColumnLayout()]
        [JqGridColumnSortable(true, Index = "Site.Notes")]
        [DisplayName("Notes")]
        [StringLength(32)]
        public string SiteNotes { get; set; }

        #endregion

        [DisplayName("Mac Address: (*)")]
        [ScaffoldColumn(true)]
        public string MacAddress { get; set; }

        //[HiddenInput]
        [Required]
        [DisplayName("Mac Address (*)")]
        [ScaffoldColumn(false)]
        public string MacAddressName { get; set; }

        [JqGridColumnSortable(false)]
        [HiddenInput]
        public bool CreateIfEMCFail { get; set; }

        [JqGridColumnSortable(false)]
        [HiddenInput]
        public bool IsDisabled { get; set; }

        [JqGridColumnFormatter("$.actionColumnFormatter")]
        [JqGridColumnSortable(false)]
        [DisplayName("Actions")]
        [JqGridColumnLayout(Width = 150, Alignment = JqGridAlignments.Center)]
        public string ActionColumn { get; set; }

        public class MacAddressViewModel
        {

            static MacAddressViewModel()
            {
                Mapper.CreateMap<string, MacAddressViewModel>()
                    .ForMember(dest => dest.MacName, opt => opt.MapFrom(src => src));
            }

            public MacAddressViewModel(string macAddress)
            {
                Mapper.Map(macAddress, this);
            }

            public MacAddressViewModel()
            {
            }

            [JqGridColumnSortable(true, Index = "MacName")]
            [DisplayName("MAC")]
            [JqGridColumnLayout(Width = 200)]
            public string MacName { get; set; }


            [JqGridColumnFormatter("$.actionColumnFormatter")]
            [JqGridColumnSortable(false)]
            [DisplayName("Actions")]
            [JqGridColumnLayout(Width = 100, Alignment = JqGridAlignments.Center)]
            public string ActionColumn { get; set; }

        }

    }
}