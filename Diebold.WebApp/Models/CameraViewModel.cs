using Diebold.Domain.Entities;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Diebold.WebApp.Models
{
    public class CameraViewModel : BaseMappeableViewModel<Camera>
    {
        static CameraViewModel()
        {
            Mapper.CreateMap<Camera, CameraViewModel>()
                 .ForMember(dest => dest.CameraName, opt => opt.MapFrom(src => src.Name));

            Mapper.CreateMap<CameraViewModel, Camera>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CameraName));
        }

        public CameraViewModel(Camera camera)
        {
            Mapper.Map(camera, this);
        }

        public CameraViewModel()
        {
        }

        [Required]
        [StringLength(32)]
        [DisplayName("Camera Name: (*)")]
        public string CameraName { get; set; }

        [Required]
        [StringLength(32)]
        [DisplayName("Channel: (*)")]
        public string Channel { get; set; }

        [DisplayName("Active: (*)")]
        public bool Active { get; set; }

    }
}