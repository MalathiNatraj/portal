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
    public class EventDescriptionFiltersViewModel : BaseMappeableViewModel<EventDescriptionFilters>
    {
         static EventDescriptionFiltersViewModel()
        {
            Mapper.CreateMap<EventDescriptionFilters, EventDescriptionFiltersViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
        public EventDescriptionFiltersViewModel(EventDescriptionFilters EventDescriptionFilters)
        {
            Mapper.Map(EventDescriptionFilters, this);
        }

        public EventDescriptionFiltersViewModel()
        {
        }
    }
}