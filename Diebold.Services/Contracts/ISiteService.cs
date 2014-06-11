using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Services.Contracts
{
    public interface ISiteService : ICRUDTrackeableService<Site>
    {
        Page<Site> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition);

        IList<Site> GetSitesByCompanyGrouping2Level(int companyGrouping2LevelId);

        IList<Site> GetSitesByCompany(int companyId, bool showDisabled);

        // New Function added because the gateways need to be loaded based on company and not based on site
        IList<Site> GetGatewayByCompany(int companyId, bool showDisabled);

        bool ValidateSiteId(int siteId);
        bool ValidateSiteIdForSiteInfo(int siteId);
        
        int GetMaxSiteId();

        IList<Site> GetSitesByUser(int userId);

        Site getGeoCoordinates(Site site);

        // Function added to get all the site which has Fire Monitoring Account number to be displayed in the fire widget combo box
        //List<Site> GetSitewithFireMonitoringAccNumber(int CurrentUserId);
        List<SiteAccountNumber> GetSitewithFireMonitoringAccNumber(int CurrentUserId);
        List<SiteAccountNumber> GetSitesByUserForMonitoring(int CurrentUserId);

        // Get Weather releated Alerts
        WeatherAlertsDTO GetWeatherAlerts(string State, string City);
        Site GetSitesBySiteId(int siteId);
        void CreateSiteDetails(Site item, SiteLogoDetails siteLogo);
        void UpdateSiteDetails(Site item, SiteLogoDetails siteLogo);
        //void DeleteSiteDetails(int id, int userId);
        void DeleteSiteDetails(Site item, int userId, SiteLogoDetails siteLogo);
        void DeleteDuplicateAlarmConfigurations();
        IList<Site> GetSitesPerPage(int pageIndex, int pageCount);
        int GetSitesCount();
    }
}