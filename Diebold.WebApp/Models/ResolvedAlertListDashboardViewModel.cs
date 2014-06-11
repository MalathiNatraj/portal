using System;
using AutoMapper;
using Diebold.Domain.Entities;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid;
using Diebold.Services.Extensions;

namespace Diebold.WebApp.Models
{
    public class ResolvedAlertListDashboardViewModel : BaseMappeableViewModel<ResolvedAlert>
    {
        static ResolvedAlertListDashboardViewModel()
        {
            Mapper.CreateMap<ResolvedAlert, ResolvedAlertListDashboardViewModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.AcknoledgeDate))
                .ForMember(dest => dest.Alert, opt => opt.MapFrom(src => src.AlarmConfiguration.AlarmType.Value.GetDescription()))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => string.Format("{0} {1}", src.User.FirstName, src.User.LastName)));
        }

        public ResolvedAlertListDashboardViewModel()
        {
        }

        public ResolvedAlertListDashboardViewModel(ResolvedAlert alert)
        {
            Mapper.Map(alert, this); 
        }

        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.Date, OutputFormat = "m/d g:i A")]
        [JqGridColumnLabel(Label = "Date")]
        [JqGridColumnLayout(Width = 80, Alignment = JqGridAlignments.Left)]
        [JqGridColumnSortable(false)]
        public DateTime Date { get; set; }

        [JqGridColumnFormatter("$.alertShortTextColumnFormatter")]
        [JqGridColumnLabel(Label = "Alert")]
        [JqGridColumnLayout(Width = 80, Alignment = JqGridAlignments.Left)]
        public string Alert { get; set; }

        [JqGridColumnFormatter("$.userShortTextColumnFormatter")]
        [JqGridColumnLabel(Label = "User")]
        [JqGridColumnLayout(Width = 70, Alignment = JqGridAlignments.Left)]
        [JqGridColumnSortable(false)]
        public string User { get; set; }
    }
}