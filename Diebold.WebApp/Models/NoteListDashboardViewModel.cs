using System;
using AutoMapper;
using Diebold.Domain.Entities;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.ComponentModel;

namespace Diebold.WebApp.Models
{
    public class NoteListDashboardViewModel : BaseMappeableViewModel<Note>
    {

        static NoteListDashboardViewModel()
        {
            Mapper.CreateMap<Note, NoteListDashboardViewModel>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));
        }

        public NoteListDashboardViewModel()
        {
        }
        
        public NoteListDashboardViewModel(Note note)
        {
            Mapper.Map(note, this); 
        }

        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.Date, OutputFormat = "m/d g:i A")]
        [JqGridColumnLabel(Label = "Date")]
        [JqGridColumnLayout(Width = 100, Alignment = JqGridAlignments.Left)]
        [JqGridColumnSortable(false)]
        public DateTime Date { get; set; }

        [JqGridColumnFormatter("$.notesShortTextColumnFormatter")]
        [JqGridColumnLabel(Label = "Note")]
        [JqGridColumnLayout(Width = 150, Alignment = JqGridAlignments.Left)]
        [JqGridColumnSortable(false)]
        public string Text { get; set; }

        [JqGridColumnLabel(Label = "User")]
        [JqGridColumnLayout(Width = 120, Alignment = JqGridAlignments.Left)]
        [JqGridColumnSortable(false)]
        public string User { get; set; }

        [JqGridColumnFormatter("$.notesActionColumnFormatter")]
        [JqGridColumnSortable(false)]
        [DisplayName("View")]
        [JqGridColumnLayout(Width = 70, Alignment = JqGridAlignments.Left)]
        public string ViewColumn { get; set; }

    }
}