using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class MonitoringDTO
    {
        public string body { get; set; }
    }

    public class ProcessDataResponseDTO 
    {
        public ProcessDataResultDTO process_data_response { get; set; }
    }

    public class ProcessDataResultDTO
    {
        public string process_data_result { get; set; }
    }

    public class MMDataDocument
    {
        public string Test { get; set; }
        public string Test_Request { get; set; }
        public string data_element { get; set; }
        public string onoff_flag { get; set; }
        public string testcat_id { get; set; }
        public string test_hours { get; set; }
        public string test_minutes { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
    }

    public class ReportsDTO
    {
        public string sig_acct { get; set; }
        public string sig_date { get; set; }
        public string sig_code { get; set; }
        public string events { get; set; }
        public string eventhistcomment { get; set; }
        public string zone_comment { get; set; }
        public string additional_info { get; set; }

        // Zone List Properties
        public string zone_id { get; set; }
        public string event_id { get; set; }
        public string Description { get; set; }
        public string comment { get; set; }
        public string restore_reqd_flag { get; set; }

        // Contact List Properties
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string cs_seqno { get; set; }
        public string pin { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public string user_id { get; set; }

        public string AccessDenied { get; set; }
        public string err_msg { get; set; }
    }

}
