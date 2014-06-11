using System;
using System.Web.Mvc;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using AutoMapper;

namespace Diebold.WebApp.Models
{
    public class LogHistoryViewModel : BaseMappeableViewModel<HistoryLog>
    {
        static LogHistoryViewModel()
        {
            Mapper.CreateMap<HistoryLog, LogHistoryViewModel>()
                .ForMember(x => x.User, opt => opt.MapFrom(x => x.User.LastName + ", " + x.User.FirstName))
                .ForMember(x => x.LogAction, opt => opt.MapFrom(x => x.Action.GetDescription()));
        }

        public LogHistoryViewModel()
        {
        }

        public LogHistoryViewModel(HistoryLog historyLog)
        {
            Mapper.Map(historyLog, this);
        }
        
        [JqGridColumnLabel(Label = "Date")]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.Date, OutputFormat = "m/d g:i A")] 
        [JqGridColumnLayout(Width = 100)]
        public DateTime Date { get; set; }
        
        [JqGridColumnLabel(Label = "User")]
        [JqGridColumnLayout(Width = 100)]
        public string User { get; set; }

        [HiddenInput]
        public string Description { get; set; }

        [HiddenInput]
        public string LogAction { get; set; }

        [JqGridColumnLabel(Label = "Log Action")]
        [JqGridColumnSortable(false)]
        public string FullDescription {
            get
            {
                var fullDescription = LogAction;

                if (!string.IsNullOrEmpty(Description))
                {
                    fullDescription += " (" + Description + ")";
                }

                return fullDescription;
            }
        }

        public string ViewDate { get; set; }
    }
}
