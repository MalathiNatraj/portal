using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Diebold.Domain.Entities;
using AutoMapper;

namespace Diebold.WebApp.Models
{
    public class DeviceModel : BaseMappeableViewModel<Dvr>
    {
        //static DeviceModel()
        //{
        //    Mapper.CreateMap<Dvr, DeviceModel>()
        //        .ForMember(dest => dest.SiteName, opt => opt.MapFrom(src => src.Gateway.Site.Name))              
        //        .ForMember(dest => dest.Address1, opt => opt.MapFrom(src => src.Gateway.Site.Address1))
        //        .ForMember(dest => dest.Address2, opt => opt.MapFrom(src => src.Gateway.Site.Address2))
        //        .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Gateway.Site.City))
        //        .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Gateway.Site.State))
        //        .ForMember(dest => dest.Zip, opt => opt.MapFrom(src => src.Gateway.Site.Zip))                
        //        .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Gateway.Site.Id));           
        //}
        //public DeviceModel(Dvr device)
        //{
        //    this.Id = device.Id;
        //    this.Name = device.Name;
        //    this.SiteId = device.Gateway.Site.Id;
        //    this.SiteName = device.Gateway.Site.Name;
        //    this.Address = device.Gateway.Site.Address1;
        //    this.Address2 = device.Gateway.Site.Address2;
        //    this.City = device.Gateway.Site.City;
        //    this.Zip = device.Gateway.Site.Zip;
        //}
        public int Id { get; set; }

        public String Name { get; set; }

        public int SiteId { get; set; }
        public String SiteName { get; set; }
        public String Address { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Zip { get; set; }
        public String Location { get; set; }
        public String Device { get; set; }
        public string[] Commands {get; set;}
        public string DeviceType { get; set; }

        public DeviceModel()
        {
            //Commands = new string[0];
        }
    }
}