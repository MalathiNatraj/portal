using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Diebold.Domain.Entities;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace Diebold.WebApp.Models
{
    public abstract class BaseMappeableViewModel<T> where T : IntKeyedEntity
    {
        [ScaffoldColumn(false)]
        [JqGridColumnLayout(Viewable = false)]
        public int Id { get; set; }

        public T MapFromViewModel()
        {
            return Mapper.Map<T>(this);     
        }
    }
}