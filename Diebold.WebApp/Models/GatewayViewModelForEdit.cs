using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Diebold.Domain.Entities;

namespace Diebold.WebApp.Models
{
    public class GatewayViewModelForEdit : GatewayViewModel
    {

        static GatewayViewModelForEdit()
        {
            Mapper.CreateMap<Gateway, GatewayViewModelForEdit>()
                .ForMember(dest => dest.MacAddress, opt => opt.MapFrom(src => src.MacAddress))
                .ForMember(dest => dest.MacAddressName, opt => opt.MapFrom(src => src.MacAddress))
                .ForMember(dest => dest.ProtocolId, opt => opt.MapFrom(src => src.Protocol))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Company.Id));
                
            Mapper.CreateMap<GatewayViewModelForEdit, Gateway>()
                .ForMember(dest => dest.MacAddress, opt => opt.MapFrom(src => src.MacAddressName))
                .ForMember(dest => dest.Protocol, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalDeviceId, opt => opt.Ignore());
        }

         public GatewayViewModelForEdit()
        {
        }


         public GatewayViewModelForEdit(Gateway gateway)
        {
            Mapper.Map(gateway, this); 
        }

    }
}