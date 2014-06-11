using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Diebold.Domain;
using System.ComponentModel.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Diebold.Domain.Entities;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace Diebold.WebApp.Models
{
    public class RoleViewModel : BaseMappeableViewModel<Role>
    {
        [JqGridColumnSortable(true, Index = "Name")]
        [JqGridColumnLabel(Label = "Role Name")]
        [StringLength(32)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name field is required")]
        [DisplayName("Role Name: (*)")]
        [RegularExpression(@"^[a-zA-Z0-9\s!@#$%^&*()_+<>?/\\}{:'~`=;\""\-\[\]]*$", ErrorMessage = "Please enter a valid role name")]
        public string Name { get; set; }

        [JqGridColumnFormatter("$.actionColumnFormatter")]
        [JqGridColumnLabel(Label = "Actions")]
        [JqGridColumnSortable(false)]        
        [JqGridColumnLayout(Width = 50)]
        public string ActionColumn { get; set; }

        private SelectListItem obj = new SelectListItem();
                
        static RoleViewModel()
        {
            Mapper.CreateMap<Role, RoleViewModel>();
            Mapper.CreateMap<RoleViewModel, Role>();
        }
                
        public RoleViewModel(Role role)
        {
            Mapper.Map(role, this);            
        }

        public RoleViewModel()
        {
        }

        private string ToFriendlyCase(string EnumString)
        {
            return Regex.Replace(EnumString, "(?!^)([A-Z])", " $1");
        }

        public IList<string> AvailableActionsList 
        {
            set
            {
                List<SelectListItem> availableActions = new List<SelectListItem>();
            
                foreach (string action in value)
                {
                    availableActions.Add(new SelectListItem
                    {
                        Text = ToFriendlyCase(action),
                        Value = action
                    });
                }              
                
                AvailableActions = new MultiSelectList(availableActions, "Value", "Text");
            }
        }

        public SelectList SelectedActionsList { get; set; }
        public SelectList OverallActionList { get; set; }

        public MultiSelectList AvailableActions { get; protected set; }

        [DisplayName("Actions:")]
        public List<string> Actions { get; set; }

        public IList<Portlets> SelectedPortlets { get; set; }
        public IList<Portlets> AvailablePortlets { get; set; }
        public IList<RolePortlets> RolePortlets { get; set; }

        public IList<ActionDetails> SelectedActionDetails { get; set; }
        public IList<ActionDetails> AvailableActionDetails { get; set; }
        public IList<RolePageActions> RolePageActions { get; set; }
    }
}
