using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class MonitoringViewModel
    {
        public string sig_acct { get; set; }
        public string sig_date { get; set; }
        public string sig_code { get; set; }
        public string events { get; set; }
        public string eventhistcomment { get; set; }
        public string zone_comment_additional_info { get; set; }
        public string extraData1 { get; set; }
        public string extraData2 { get; set; }

        // Zone List Properties
        public string zone_id { get; set; }
        public string event_id { get; set; }
        public string Description { get; set; }
        public string comment { get; set; }
        public string restore_reqd_flag { get; set; }

        // Contact List Properties
        public string ContactName { get; set; }
        public string CSSeqNumber { get; set; }
        public string Pin { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string UserId { get; set; }
    }
}