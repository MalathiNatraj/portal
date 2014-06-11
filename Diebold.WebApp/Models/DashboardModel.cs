using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class DashboardModel
    {
        public ProfileModel UserProfile { get; set; }
        public ContentAreaModel ContentArea { get; set; }
        public EmptyContentAreaModel EmptyContentArea { get; set; }
       // public LiveVideoModel LiveVideo { get; set; }
        public IntrusionViewModel Intrusion { get; set; }
        public IList<AccessViewModel> AccessControl { get; set; }
       // public HealthModel health { get; set; }
       // public AlertModel alert { get; set; }
       // public FeaturedNewsModel FeaturedNews { get; set; }
        public SiteInfo SiteInformation { get; set; }
        public List<PreferencesModel> preferences { get; set; }
        public List<IntrusionViewModel> Intrusions { get; set; }
        public LinkViewModel Links { get; set; }
        public VideoHealthCheckModel VideoHealthCheck { get; set; }
        public SiteMapModel SiteMap { get; set; }
        public List<MasterRoomModel> MasterRooms { get; set; }
        public AccountDetailModel AccountDetail { get; set; }        
        public RSSFeedViewModel RSS { get; set; }
        public List<SystemSummaryModel> SystemSummary { get; set; }
        public BaseMasterRoomModel BaseMasterRoom { get; set; }
        // public MASModel MASDetail { get; set; }
        public CardholderModel Cardholder { get; set; }
        public AccessGroupModel Accessgroup { get; set; }
        public LiveViewModel LiveView { get; set; }
        public List<DeviceListDashboardViewModel> AlertListDashboardViewModel { get; set; }
        public List<DeviceListDashboardViewModel> AlertListDashboardViewModelAccess { get; set; }
        public List<DeviceListDashboardViewModel> AlertListDashboardViewModelIntrusion { get; set; }
        public BaseLocationViewModel baseLocationViewModel { get; set; }
    }
}
