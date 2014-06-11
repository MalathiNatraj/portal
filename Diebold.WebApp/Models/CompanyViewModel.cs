using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Diebold.Domain.Entities;
using System.Web.Mvc;
using System.ComponentModel;
using Lib.Web.Mvc.JQuery.JqGrid;


namespace Diebold.WebApp.Models
{
    public class CompanyViewModel : BaseMappeableViewModel<Company>
    {
        static CompanyViewModel()
        {
            Mapper.CreateMap<Company, CompanyViewModel>();
            Mapper.CreateMap<CompanyViewModel, Company>();
        }
        
        public CompanyViewModel(Company company)
        {
            Mapper.Map(company, this);
        }

        public CompanyViewModel()
        {
        }
        
        [JqGridColumnSortable(true, Index = "Name")]
        [JqGridColumnLabel(Label = "Company Name" )]
        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(32)]
        [DisplayName("Company Name: (*)")]
        public string Name { get; set; }

        [JqGridColumnSortable(true, Index = "ExternalCompanyId")]
        [JqGridColumnLabel(Label = "Company ID")]
        [DisplayName("Company ID")]
        [Remote("ValidateCompanyId", "Company",  AdditionalFields = "Id", ErrorMessage = "The company Id already in use.")]
        [Required]
        public int ExternalCompanyId { get; set; }

        [Required(ErrorMessage = "1st Level of Grouping Label is required")]
        [StringLength(32)]
        [ScaffoldColumn(false)]
        [DisplayName("1st Level of Grouping Label: (*)")]
        public string FirstLevelGrouping { get; set; }

        [Required(ErrorMessage = "2nd Level of Grouping Label is required")]
        [StringLength(32)]
        [ScaffoldColumn(false)]
        [DisplayName("2nd Level of Grouping Label: (*)")]
        public string SecondLevelGrouping { get; set; }
        
        [StringLength(32)]
        [ScaffoldColumn(false)]
        [DisplayName("3rd Level of Grouping:")]
        public string ThirdLevelGrouping { get; set; }
        
        [StringLength(32)]
        [ScaffoldColumn(false)]
        [DisplayName("4th Level of Grouping:")]
        public string FourthLevelGrouping { get; set; }

        [JqGridColumnLabel(Label = "Primary Contact Name")]
        [JqGridColumnSortable(true, Index = "PrimaryContactName")]
        [Required(ErrorMessage = "Primary Contact Name is required")]
        [StringLength(32)]
        [DisplayName("Primary Contact Name: (*)")]
        public string PrimaryContactName { get; set; }
        
        [StringLength(32)]
        [ScaffoldColumn(false)]
        [DisplayName("Primary Contact Extension:")]
        public string PrimaryContactExtension { get; set; }
        
        [JqGridColumnLabel(Label = "Primary Contact E-mail")]
        [JqGridColumnSortable(true, Index = "PrimaryContactEmail")]
        [Required(ErrorMessage = "Primary Contact E-mail is required")]
        [StringLength(100)]
        [DisplayName("Primary Contact E-mail: (*)")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Please enter a valid primary contact email address")]
        public string PrimaryContactEmail { get; set; }

        [Required(ErrorMessage = "Primary Contact Office Nr is required")]
        [StringLength(32)]
        [ScaffoldColumn(true)]
        [DisplayName("Primary Contact Office Nr: (*)")]
        public string PrimaryContactOffice { get; set; }
        
        [StringLength(32)]
        [ScaffoldColumn(true)]
        [DisplayName("Primary Contact Mobile:")]
        public string PrimaryContactMobile { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("Preferred Contact")]
        public bool PrimaryContactOfficePreferred { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("Preferred Contact")]
        public bool PrimaryContactMobilePreferred { get; set; }

        [StringLength(32)]
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Reporting Frequency is required")]
        public string ReportingFrequency { get; set; }
       
        [StringLength(32)]
        [DisplayName("File Name: ")]
        public string FileName { get; set; }

        [StringLength(32)]
        [DisplayName("File Name: ")]
        public byte[] CompanyLogo { get; set; }

        [DisplayName("Reporting Frequency: (*)")]
        public SelectList AvailableReportingFrequencies { get; protected set; }
        public IList<string> AvailableReportingFrequencyList
        {
            set
            {
                List<SelectListItem> availableReportingFrequency = new List<SelectListItem>();
                foreach (string reportingFrequency in value)
                {
                    availableReportingFrequency.Add(new SelectListItem
                    {
                        Text = reportingFrequency,
                        Value = reportingFrequency
                    });
                }
                AvailableReportingFrequencies = new SelectList(availableReportingFrequency, "Value", "Text");
            }
        }

        [DisplayName("Default Subscriptions: (*)")]
        public MultiSelectList AvailableSubscriptions { get; protected set; }

        [Required(ErrorMessage = "Default Subscriptions is required")]
        public IList<string> AvailableSubscriptionList
        {
            set
            {
                List<SelectListItem> availableSubscription = new List<SelectListItem>();
                foreach (string subscription in value)
                {
                    var sb = new System.Text.StringBuilder();
                    foreach (Char c in subscription)
                    {
                        if (Char.IsUpper(c))
                            sb.Append(' ');
                        sb.Append(c);
                    }

                    availableSubscription.Add(new SelectListItem
                    {
                        Text = sb.ToString(),
                        Value = subscription
                    });
                }
                AvailableSubscriptions = new MultiSelectList(availableSubscription, "Value", "Text");
            }
        }

        [DisplayName("User Subscriptions: (*)")]
        public MultiSelectList UserSubscriptions { get; protected set; }

        public List<string> Subscriptions { get; set; }
        
        public List<string> Grouping1SelectedItems { get; set; }

        public List<string> Grouping2SelectedItems { get; set; }

        public List<string> Sites { get; set; }

        public IList<CompanyGrouping> GroupingRelations { get; set; }
        
        [StringLength(128)]
        [ScaffoldColumn(false)]
        public string Grouping1LevelName { get; set; }

        [StringLength(128)]
        [ScaffoldColumn(false)]
        public string Grouping2LevelName { get; set; }

        [StringLength(128)]
        [ScaffoldColumn(false)]
        public string Grouping1Selected { get; set; }
                
        [ScaffoldColumn(false)]
        public string Grouping2Selected { get; set; }

        [DisplayName(" Name: (*)")]
        public SelectList AvailableGrouping1Levels { get; protected set; }
        public IList<CompanyGrouping> AvailableGrouping1LevelList
        {
            set
            {
                var availableGroupingLevel = value.Select(groupingLevel => new SelectListItem
                                                                     {
                                                                         Text = groupingLevel.Grouping1Name,
                                                                         Value = groupingLevel.Grouping1Name
                                                                     }).ToList();

                AvailableGrouping1Levels = new SelectList(availableGroupingLevel, "Value", "Text");
            }
        }
        
        [DisplayName(" Name: (*)")]
        public SelectList AvailableGrouping2Levels { get; protected set; }
        public IList<string> AvailableGrouping2LevelList
        {
            set
            {
                var availableGroupingLevel = value.Select(groupingLevel => new SelectListItem
                {
                    Text = groupingLevel,
                    Value = groupingLevel
                }).ToList();

                AvailableGrouping2Levels = new SelectList(availableGroupingLevel, "Value", "Text");
            }
        }
        
        public SelectList AvailableGrouping1LevelItems { get; protected set; }
        public IList<CompanyGrouping1Level> AvailableGrouping1LevelItemList
        {
            set
            {
                var availableGroupingLevel = value.Select(groupingLevel => new SelectListItem
                {
                    Text = groupingLevel.Name,
                    Value = groupingLevel.Id.ToString()
                }).ToList();

                AvailableGrouping1LevelItems = new SelectList(availableGroupingLevel, "Value", "Text");
            }
        }

        public SelectList AvailableGrouping2LevelItems { get; protected set; }
        public IList<CompanyGrouping2Level> AvailableGrouping2LevelItemList
        {
            set
            {
                List<SelectListItem> items = new List<SelectListItem>();

                foreach (CompanyGrouping2Level item in value)
                {
                    items.Add(new SelectListItem
                                  {
                                      Text = item.Name.ToString(),
                                      Value = item.Id.ToString()
                                  });
                }

                AvailableGrouping2LevelItems = new SelectList(items, "Value", "Text");
            }
        }

        public MultiSelectList AvailableGrouping2AllItems { get; protected set; }
        public IList<CompanyGrouping2Level> AvailableGrouping2AllItemList
        {
            set
            {
                var items = value.Select(item => new SelectListItem
                                                     {
                                                         Text = item.CompanyGrouping1Level.Name + " / " + item.Name, 
                                                         Value = item.Id.ToString()
                                                     }).ToList();

                AvailableGrouping2AllItems = new MultiSelectList(items, "Value", "Text");
            }
        }

        [Required]
        [DisplayName("Site Id")]
        [ScaffoldColumn(false)]
        public int SiteId { get; set; }

        [DisplayName("Site assigned to this ")]
        public MultiSelectList AvailableSites { get; protected set; }
        public IList<Site> AvailableSiteList
        {
            set
            {
                var sites = value.Select(siteItem => new SelectListItem
                    {
                        Text = siteItem.Name.ToString(), Value = siteItem.Id.ToString()
                    }).ToList();

                AvailableSites = new MultiSelectList(sites, "Value", "Text");
            }
        }

        public MultiSelectList SelectedSites { get; protected set; }
        public IList<Site> SelectedSiteList
        {
            set
            {
                var sites = value.Select(siteItem => new SelectListItem
                {
                    Text = siteItem.Name.ToString(),
                    Value = siteItem.Id.ToString()
                }).ToList();

                SelectedSites = new MultiSelectList(sites, "Value", "Text");
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
        public IList<CompanyDefaultSubscription> CompanyDefaultSubscription { get; set; }
        public IList<CompanyInventory> CompanyInventory { get; set; } 
    }

    public class CompanyGrouping
    {
        public CompanyGrouping()
        {
            Grouping2List = new List<Grouping2Level>();
        }

        public string Grouping1Id { set; get; }
        public string Grouping1Name { set; get; }
        public IList<Grouping2Level> Grouping2List { set; get; }
    }

    public class Grouping2Level
    {
        public Grouping2Level()
        {
            Sites = new List<GroupingSite>();
        }

        public string Grouping2Name { set; get; }
        public string Grouping2Id { set; get; }
        public bool IsRemoved { set; get; }
        public IList<GroupingSite> Sites { set; get; }
    }

    public class GroupingSite
    {
        public string SiteId { set; get; }
        public string SiteName { set; get; }
        public bool IsRemoved { set; get; }
    }
}