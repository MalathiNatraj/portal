using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Diebold.Domain.Entities;

namespace Diebold.WebApp.Models
{
    public class CompanyGroupingViewModel : BaseMappeableViewModel<CompanyGrouping1Level>
    {
        static CompanyGroupingViewModel()
        {
            Mapper.CreateMap<CompanyGrouping1Level, CompanyGroupingViewModel>();
            Mapper.CreateMap<CompanyGroupingViewModel, CompanyGrouping1Level>();
        }

        public CompanyGroupingViewModel(CompanyGrouping1Level companyGrouping)
        {
            Mapper.Map(companyGrouping, this);
        }

        public CompanyGroupingViewModel()
        {
        }


        /*public bool Selected { get; set; }

        public bool IsNew { get; set; }*/

        public int? Grouping1Id { get; set; }

        public string Grouping1Name { get; set; }

        public int? Grouping2Id { get; set; }

        public string Grouping2Name { get; set; }

        public int? SiteId { get; set; }

        public string SiteName { get; set; }








        /*public string Grouping1Id { set; get; }
        public string Grouping1Name { set; get; }
        public IList<Grouping2Level> Grouping2List { set; get; }*/
        
    }
    
}