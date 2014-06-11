using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Diebold.Domain.Entities;
using System.Web.Mvc;

namespace DieboldMobile.Models
{
    public class UserViewModel : BaseMappeableViewModel<User>
    {
        static UserViewModel()
        {
            Mapper.CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.Id))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Company.Id))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));

            Mapper.CreateMap<UserViewModel, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => new Role { Id = src.RoleId }))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => new Company() { Id = src.CompanyId }));
        }

        public UserViewModel(User user)
        {
            Mapper.Map(user, this);
        }

        public UserViewModel()
        {
        }

        [JqGridColumnSortable(true, Index = "Email")]
        [JqGridColumnLabel(Label = "E-Mail")]
        [DisplayName("E-Mail: (*)")]
        [StringLength(50)]
        [Required]
        [RegularExpression(@"\w+([.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [JqGridColumnSortable(true, Index = "FirstName")]
        [JqGridColumnLabel(Label = "First Name")]
        [DisplayName("First Name: (*)")]
        [StringLength(32)]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+(( )+[a-zA-z0-9]+)*$", ErrorMessage = "Please enter a valid first name")]
        public string FirstName { get; set; }

        [JqGridColumnSortable(true, Index = "LastName")]
        [JqGridColumnLabel(Label = "Last Name")]
        [DisplayName("Last Name: (*)")]
        [StringLength(32)]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+(( )+[a-zA-z0-9]+)*$", ErrorMessage = "Please enter a valid last name")]
        public string LastName { get; set; }

        [JqGridColumnSortable(true, Index = "Username")]
        [JqGridColumnLabel(Label = "User Name")]
        [DisplayName("Username: (*)")]
        [StringLength(50)]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Please enter a valid username")]
        public string Username { get; set; }

        [JqGridColumnSortable(true, Index = "Phone")]
        [JqGridColumnLabel(Label = "Phone Nr")]
        [DisplayName("Phone Nr.: (*)")]
        [StringLength(20)]
        [Required]
        [RegularExpression(@"^[0-9 ()-]*$", ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; }

        [JqGridColumnSortable(true, Index = "OfficePhone")]
        [DisplayName("Office Phone Nr.:")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        [RegularExpression(@"^[0-9 ()-]*$", ErrorMessage = "Please enter a valid office phone number")]
        public string OfficePhone { get; set; }

        [JqGridColumnSortable(true, Index = "Extension")]
        [StringLength(10)]
        [ScaffoldColumn(false)]
        public string Extension { get; set; }

        [JqGridColumnSortable(true, Index = "Mobile")]
        [DisplayName("Mobile Phone Nr.:")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        [RegularExpression(@"^[0-9 ()-]*$", ErrorMessage = "Please enter a valid mobile number")]
        public string Mobile { get; set; }

        [JqGridColumnSortable(true, Index = "Title")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        [DisplayName("Title:")]
        public string Title { get; set; }

        [JqGridColumnSortable(true, Index = "Text1")]
        [StringLength(255)]
        [ScaffoldColumn(false)]
        [DisplayName("Note1:")]
        public string Text1 { get; set; }

        [JqGridColumnSortable(true, Index = "Text2")]
        [StringLength(255)]
        [ScaffoldColumn(false)]
        [DisplayName("Note2:")]
        public string Text2 { get; set; }

        [ScaffoldColumn(true)]
        public int RoleId { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(4)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Please enter a valid user pin")]
        [DisplayName("User Pin:")]
        public string UserPin { get; set; }

        [ScaffoldColumn(true)]
        [Required]
        public int CompanyId { get; set; }

        [ScaffoldColumn(false)]
        public int OldCompanyId { get; set; }

        [JqGridColumnSortable(true, Index = "Role.Name")]
        [DisplayName("Role Name:")]
        public string RoleName { get; set; }

        [JqGridColumnSortable(true, Index = "Company.Name")]
        [DisplayName("Company Name:")]
        public string CompanyName { get; set; }

        [StringLength(32)]
        [DisplayName("Timezone : (*)")]
        [ScaffoldColumn(false)]
        // [Required]
        public string TimeZone { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("Preferred Contact")]
        // [Required(ErrorMessage = "The Preferred Contact option is required.")]
        public string PreferredContact { get; set; }

        [DisplayName("Role: (*)")]
        public SelectList AvailableRoles { get; protected set; }
        public IList<Role> AvailableRoleList
        {
            set
            {
                AvailableRoles = new SelectList(value, "Id", "Name");
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

        [DisplayName("TimeZone: (*)")]
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

        [JqGridColumnSortable(false)]
        [HiddenInput]
        public bool IsDisabled { get; set; }

        [JqGridColumnFormatter("$.actionColumnFormatter")]
        [JqGridColumnSortable(false)]
        [DisplayName("Actions")]
        [JqGridColumnLayout(Width = 180)]
        public string ActionColumn { get; set; }

        [ScaffoldColumn(false)]
        public SelectList AvailableLinks { get; protected set; }
        public IList<Link> AvailableLinksList
        {
            set
            {
                AvailableLinks = new SelectList(value, "Id", "Name");
            }
        }

        [ScaffoldColumn(false)]
        public SelectList AvailableRSSFeeds { get; protected set; }
        public IList<Link> AvailableRSSFeedsList
        {
            set
            {
                AvailableRSSFeeds = new SelectList(value, "Id", "Name");
            }
        }

        [ScaffoldColumn(false)]
        public SelectList AvailableUserPreferencePortlets { get; protected set; }
        public IList<Link> AvailableUserPreferencePortletsList
        {
            set
            {
                AvailableUserPreferencePortlets = new SelectList(value, "Id", "Name");
            }
        }
    }
}