using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class Webportal
    {
        public enum Portal { UserProfile, ContentArea, EmptyContentArea, LiveVideo, Intrusion, AccessControl, Healthcheck, Alert, FeaturedNews, SiteInformation, RSS, Links, VideoHealthCheck, SiteMap, AccountDetail, SystemSummary, MAS, CardholderAdd, CardholderModify, CardholderDelete, AddAccessGroup, LiveView, Access, AlertAccess, AlertIntrusion }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int ColumnId { get; set; }
        public int SequenceNo { get; set; }
        public DashboardModel DashboardDetails { get; set; } 
    }
}