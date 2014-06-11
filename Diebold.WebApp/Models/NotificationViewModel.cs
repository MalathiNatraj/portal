
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Diebold.WebApp.Models
{
    public class NotificationViewModel
    {
        public NotificationViewModel()
        {
            //Status =  new List<Status>();
        }

        public Alert Alert { get; set; }
        public AlertClear AlertClear { get; set; }
        //public IList<Status> Status { get; set; }
    }

    public class Alert
    {
        public string DeviceId { get; set; }

        public string AlarmName { get; set; }

        public string AlertDate { get; set; }

        public bool AlertActive { get; set; }

        public string RelationalOperator { get; set; }

        public object Threshold { get; set; }

        public List<RuleValue> Value { get; set; }

        public Report Report { get; set; }

    }

    public class AlertClear
    {
        public string DeviceId { get; set; }

        public string AlarmName { get; set; }

        public string AlertDate { get; set; }

        public bool AlertActive { get; set; }

        public string RelationalOperator { get; set; }

        public object Threshold { get; set; }

        public List<RuleValue> Value { get; set; }
    }

    public class AlertRequestObject
    {
        public AlertTemp alert { get; set; }
        public AlertTemp alert_clear { get; set; }
    }

    public class AlertTemp
    {
        public string device_instance_id { get; set; }

        public string device_instance_name { get; set; }

        public string rule_name { get; set; }

        public string alert_date { get; set; }

        public Rule_Condition rule_condition { get; set; }

        public string rule_condition_type { get; set; }

        public object threshold { get; set; }

        public List<RuleValue> value { get; set; }

        public Report Report { get; set; }

    }

    public class Rule_Condition
    {
        public string _id { get; set; }

        public string created_at { get; set; }

        public string deleted_at { get; set; }

        public object property { get; set; }

        public string rule_id { get; set; }

        public object updated_at { get; set; }

        public object value { get; set; }

        public string parent_or_sibling_key { get; set; }

        public string parent_or_sibling_value { get; set; }

    }

    public class RuleValue
    {
        public string isNotRecording { get; set; }

        public string daysRecorded { get; set; }
        public string SMART { get; set; }
        public string raidStatus { get; set; }
        public string videoLoss { get; set; }
        public string networkDown { get; set; }
        public string areaarmed { get; set; }
        public string areadisarmed { get; set; }
        public string zonealarm { get; set; }
        public string zonetrouble { get; set; }
        public string zonebypass { get; set; }
        public string doorforced { get; set; }
        public string doorheld { get; set; }
        
        public string doorStatus { get; set; }
        public string zoneStatus { get; set; }
        public string armed { get; set; }
        public string driveTemp { get; set; }
    }


    public class CapabilityType
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool Collection { get; set; }
    }

    public class Report
    {
        public string _id { get; set; }
        public string device_type { get; set; }
        public payload payload { get; set; }
    }

    public class payload
    {
        public SparkDvrAlertReport SparkDvrReport { get; set; }
        public SparkAccessControlAlertReport SparkAccessControlReport { get; set; }
        public SparkIntrusionAlertReport SparkIntrusionReport { get; set; }
    }

    public class SparkDvrAlertReport
    {
        public string name { get; set; }
        public Alertproperties properties { get; set; }
    }

    public class SparkAccessControlAlertReport
    {
        public string name { get; set; }
        public Alertproperties properties { get; set; }
    }

    public class SparkIntrusionAlertReport
    {
        public string name { get; set; }
        public Alertproperties properties { get; set; }
    }

    public class Alertproperties
    {
        public Alertproperty property { get; set; }
        public AlertpropertyList[] propertyList { get; set; }
    }

    public class Alertproperty
    {
        public string deviceIdentifier { get; set; }
        public string daysRecorded { get; set; }
        public string isNotRecording { get; set; }
        public string networkDown { get; set; }
        public string estimatedFreeRecording { get; set; }
        public string startedOn { get; set; }
        public string upTime { get; set; }
        public string deviceFirmware { get; set; }

        public string timeStampAgent { get; set; }
        public string timeStampRecorder { get; set; }
        public string string1 { get; set; }
        public string string2 { get; set; }
        public string int1 { get; set; }
        public string int2 { get; set; }
        public string bool1 { get; set; }
        public string bool2 { get; set; }
        public string dvrErrorCode { get; set; }
        public string ACErrorCode { get; set; }
        public string intrusionErrorCode { get; set; }
    }
    public class AlertpropertyList
    {
        public string name { get; set; }
        public string[] propertyItem { get; set; }
    }


}