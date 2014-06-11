using Diebold.Domain.Entities;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using AutoMapper;

namespace Diebold.WebApp.Models
{
    public class ReportingViewModel : BaseMappeableViewModel<ResultsReport>
    {
        static ReportingViewModel()
        {

            Mapper.CreateMap<ResultsReport, ReportingViewModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("yyyy/MM/dd hh:mm tt ")))
                .ForMember(dest => dest.AlertDescription, opt => opt.MapFrom(src => src.AlertDescription.SplitByUpperCase()))
                .ForAllMembers(dest => dest.Condition(src => !src.IsSourceValueNull));

            Mapper.CreateMap<ReportingViewModel, ResultsReport>();
        }

        public ReportingViewModel(ResultsReport resultsReport)
        {
            Mapper.Map(resultsReport, this);
        }

        //Empty constructor used for reflection.
        public ReportingViewModel()
        {
            
        }
        
        [JqGridColumnLabel(Label = "Date")]
        [JqGridColumnLayout(Width = 106)]
        public string Date { get; set; }

        [JqGridColumnLabel(Label = "Area")]
        [JqGridColumnLayout(Width = 63)]
        public string Area { get; set; }

        [JqGridColumnLabel(Label = "Site")]
        [JqGridColumnLayout(Width = 63)]
        public string Site { get; set; }

        [JqGridColumnLabel(Label = "Device")]
        [JqGridColumnLayout(Width = 70)]
        public string DeviceName { get; set; }

        [JqGridColumnLabel(Label = "Description")]
        [JqGridColumnLayout(Width = 85)]
        public string AlertDescription { get; set; }

        [JqGridColumnLabel(Label = "Resolved By")]
        [JqGridColumnLayout(Width = 106)]
        public string ResolvedBy { get; set; }

        [JqGridColumnLabel(Label = "Current Status")]
        [JqGridColumnLayout(Width = 80)]
        public string CurrentStatus { get; set; }

        [JqGridColumnLabel(Label = "Last Note  By")]
        [JqGridColumnLayout(Width = 106)]
        public string LastNoteBy { get; set; }
        
        [JqGridColumnLabel(Label = "Device Type")]
        [JqGridColumnLayout(Width = 70)]
        public string DVRType { get; set; }
    }
}
