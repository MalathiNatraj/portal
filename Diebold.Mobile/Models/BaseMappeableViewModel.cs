using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Diebold.Domain.Entities;


namespace DieboldMobile.Models
{
    public abstract class BaseMappeableViewModel<T> where T : IntKeyedEntity
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        public T MapFromViewModel()
        {
            return Mapper.Map<T>(this);
        }
    }
}