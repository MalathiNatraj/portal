using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Diebold.Domain.Entities;
using AutoMapper;
using System.ComponentModel;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Web.Mvc;
using Diebold.WebApp.Infrastructure.ClientValidators;

namespace Diebold.WebApp.Models
{
    public class SiteViewModel : BaseMappeableViewModel<Site>
    {
          
        static SiteViewModel()
        {
            Mapper.CreateMap<Site, SiteViewModel>()
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.SiteId))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyGrouping2Level.CompanyGrouping1Level.Company.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address1 + ", " + src.City + ", " + src.State + ", " + src.Country))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyGrouping2Level.CompanyGrouping1Level.Company.Id))
                .ForMember(dest => dest.FirstLevelGroupingId, opt => opt.MapFrom(src => src.CompanyGrouping2Level.CompanyGrouping1Level.Id))
                .ForMember(dest => dest.SecondLevelGroupingId, opt => opt.MapFrom(src => src.CompanyGrouping2Level.Id))
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(src => src.ContactName))
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.ContactEmail))
                .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber));

            Mapper.CreateMap<SiteViewModel, Site>()
            .ForMember(dest => dest.CompanyGrouping2Level, opt => opt.MapFrom(src => new CompanyGrouping2Level {Id = src.SecondLevelGroupingId.Value}));
        }

        public SiteViewModel(Site site)
        {
            Mapper.Map(site, this); 
        }

        public SiteViewModel()
        {
        }

        [JqGridColumnSortable(true, Index = "SiteId")]
        [DisplayName("Site Id: (*)")]
        [JqGridColumnLabel(Label = "Site Id")]
        [Required(ErrorMessage = "Site Id is required")]
        [Remote("ValidateSiteId", "Site", AdditionalFields = "Id", ErrorMessage = "The Site Id already in use.")]
        [JqGridColumnLayout(Width = 150)]
        public int SiteId { get; set; }

        [JqGridColumnSortable(true, Index = "Name")]
        [DisplayName("Site Name: (*)")]
        [JqGridColumnLabel(Label = "Site Name")]
        [Required(ErrorMessage = "Site Name is required")]        
        [RegularExpression(@"^[a-zA-Z0-9\s!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$", ErrorMessage = "Please enter a valid Site Name")]
        [JqGridColumnLayout(Width = 150)]
        public string Name { get; set; }

        [JqGridColumnSortable(true, Index = "AccountNumber")]
        [DisplayName("Account Number: (*)")]
        [JqGridColumnLabel(Label = "Account Number")]
        [RegularExpression(@"^[a-zA-Z0-9\s!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$]*$", ErrorMessage = "Please enter a valid Account Number")]
        [JqGridColumnLayout(Width = 150)]
        public string AccountNumber { get; set; }

        [JqGridColumnSortable(true, Index = "Address1,Address2,City")]
        [JqGridColumnLabel(Label = "Address")]
        [JqGridColumnLayout(Width = 150)]
        public string Address { get; set; }

        #region Company

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

        public SelectList AvailableCompanyGrouping1Levels { get; protected set; }

        public IList<CompanyGrouping1Level> AvailableCompanyGrouping1LevelList
        {
            set
            {
                List<SelectListItem> availableCompanyGrouping1Levels = new List<SelectListItem>();

                foreach (CompanyGrouping1Level item in value)
                {
                    availableCompanyGrouping1Levels.Add(new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name

                    });
                }
                AvailableCompanyGrouping1Levels = new SelectList(availableCompanyGrouping1Levels, "Value", "Text");
            }
        }

        public SelectList AvailableCompanyGrouping2Levels { get; protected set; }

        public IList<CompanyGrouping2Level> AvailableCompanyGrouping2LevelList
        {
            set
            {
                List<SelectListItem> availableCompanyGrouping2Levels = new List<SelectListItem>();

                foreach (CompanyGrouping2Level item in value)
                {
                    availableCompanyGrouping2Levels.Add(new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name

                    });
                }
                AvailableCompanyGrouping2Levels = new SelectList(availableCompanyGrouping2Levels, "Value", "Text");
            }
        }

        [JqGridColumnSortable(true, Index = "CompanyId")]
        [DisplayName("Company ID (*)")]
        [ScaffoldColumn(false)]
        [JqGridColumnLayout()]
        [Required(ErrorMessage = "Company is required")]
        public int? CompanyId { get; set; }

        [JqGridColumnLayout()]
        [JqGridColumnSortable(true, Index = "CompanyGrouping2Level.CompanyGrouping1Level.Company.Name")]
        [DisplayName("Company: (*)")]
        [StringLength(32)]
        public string CompanyName { get; set; }

        #endregion
        
        [JqGridColumnSortable(false)]
        [HiddenInput]
        public bool IsDisabled { get; set; }

        [JqGridColumnFormatter("$.actionColumnFormatter")]
        [JqGridColumnSortable(false)]
        [DisplayName("Actions")]
        [JqGridColumnLayout(Width = 150)]
        public string ActionColumn { get; set; }

        [Required(ErrorMessage = "1st Grouping Level is required")]
        [ScaffoldColumn(false)]
        [DisplayName("1st Grouping Level: (*)")]
        public int? FirstLevelGroupingId { get; set; }

        [Required(ErrorMessage = "2nd Grouping Level is required")]
        [ScaffoldColumn(false)]
        [DisplayName("2nd Grouping Level: (*)")]
        public int? SecondLevelGroupingId { get; set; }

        [DisplayName("Sharepoint URL:")]
        [ScaffoldColumn(false)]
        public string SharepointURL { get; set; }

        [DisplayName("Notes:")]
        [ScaffoldColumn(false)]
        public string Notes { get; set; }

        [DisplayName("Contact Name: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Contact Name is required")] 
        public string ContactName { get; set; }

        [DisplayName("Contact Email: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Contact Email is required")]
        [RegularExpression(@"\w+([.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Please enter a valid email address")]
        public string ContactEmail { get; set; }

        [DisplayName("Contact Number: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Contact Number is required")] 
        public string ContactNumber { get; set; }
        
        #region External Data
        
        [DisplayName("Parent Association: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Parent Association is required")]        
        [RegularExpression(@"^[a-zA-Z0-9\s!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$", ErrorMessage = "Please enter a valid Parent Association")]
        public string ParentAssociation { get; set; }

        [DisplayName("Diebold Name: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Diebold Name is required")]                
        [RegularExpression(@"^[a-zA-Z0-9\s!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$", ErrorMessage = "Please enter a valid Diebold Name")]
        public string DieboldName { get; set; }

        [DisplayName("Address 1: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Address 1 is required")]
        public string Address1 { get; set; }

        [DisplayName("Address 2:")]
        [ScaffoldColumn(false)]
        public string Address2 { get; set; }

        [DisplayName("City: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "City is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$]*$", ErrorMessage = "Please enter a valid City")]
        public string City { get; set; }

        [DisplayName("State: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "State is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$]*$", ErrorMessage = "Please enter a valid State")]
        public string State { get; set; }

        [DisplayName("Zip: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Zip is required")]
        [RegularExpression(@"^[a-zA-Z0-9.!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$", ErrorMessage = "Please enter a valid Zip")]
        public string Zip { get; set; }

        [DisplayName("County: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "County is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$]*$", ErrorMessage = "Please enter a valid County")]
        public string County { get; set; }

        [DisplayName("Country: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Country is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$]*$", ErrorMessage = "Please enter a valid Country")]
        public string Country { get; set; }

        [DisplayName("CCMF: (*)")]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "CCMF Status is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$]*$", ErrorMessage = "Please enter a valid CCMF Status")]
        public string CCMFStatus { get; set; }
        [StringLength(150)]
        [DisplayName("File Name: ")]
        public string FileName { get; set; }

        [StringLength(32)]
        [DisplayName("File Name: ")]
        public byte[] SiteLogo { get; set; }

        [StringLength(32)]
        [DisplayName("Fire Monitoring Account Number")]
        public string FireMonitoringAccNumber { get; set; }

        #endregion
    }
}