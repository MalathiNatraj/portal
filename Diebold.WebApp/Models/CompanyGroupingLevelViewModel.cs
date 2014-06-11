using System.ComponentModel.DataAnnotations;
namespace Diebold.WebApp.Models
{
    public class CompanyGroupingLevelViewModel
    {
        public int CompanyId { get; set; }
        public int GroupingLevel1Id { get; set; }
        public int GroupingLevel2Id { get; set; }
        public int SiteId { get; set; }

        [StringLength(32)]
        public string GroupingLevel1Name { get; set; }

        [StringLength(32)]
        public string GroupingLevel2Name { get; set; }
    }
}