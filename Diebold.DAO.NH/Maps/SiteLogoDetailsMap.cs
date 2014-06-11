using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Maps
{
    public class SiteLogoDetailsMap : TrackeableEntityMapping<SiteLogoDetails>
    {
        public SiteLogoDetailsMap()
        {
            Property(u => u.siteId, c =>
            {
                c.NotNullable(false);
            });
            Property(u => u.FileName, c =>
            {
                c.NotNullable(false);
                c.Length(150);
            });
            Property(u => u.SiteLogo, c =>
            {
                c.NotNullable(false);
                c.Length(100000);
            });
        }
    }
}
