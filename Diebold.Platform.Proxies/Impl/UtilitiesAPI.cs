using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Platform.Proxies.DTO;
using Diebold.Platform.Proxies.REST;
using Diebold.Platform.Proxies.REST.Enums;
using System.Configuration;
using log4net.Core;
using log4net;

namespace Diebold.Platform.Proxies.Impl
{
    public class UtilitiesApi : BaseMachineshopAPI, IUtilitiesApiService
    {
        protected static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);    
        public void SendMail(MailDTO mail)
        {
            APIManager.AsyncRequest("utilities/sendEmail", new RequestParameters(mail), RequestMethod.POST);
        }

        public string prepareAccessRequestBody()
        {
            return string.Empty;
        }

        public string PrepareIntrusionRequestBody()
        {
            return string.Empty;
        }


        public string PrepareRequestBody(DeviceRequestParameters objRequestParameters)
        {
            StringBuilder objSbr = new StringBuilder();
            StringBuilder objAlarm = new StringBuilder();
            string openBrace="{";
            string closeBrace = "}";
            string openSqBrace = "[";
            string closeSqBrace = "]";
            string comma = ",";
            string colon = ":";
            string doublQoute = "\"";
            string strDeviceInstnaceType = string.Empty;
            strDeviceInstnaceType = GetDevicetype(objRequestParameters);
            objSbr.Append(openBrace);
            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + objRequestParameters.DeviceKey + doublQoute + comma);
            objSbr.Append(doublQoute + "active" + doublQoute + colon + doublQoute + "true" + doublQoute + comma);
            objSbr.Append(doublQoute + "device_id" + doublQoute + colon + doublQoute + strDeviceInstnaceType + doublQoute + comma);
            objSbr.Append(doublQoute + "device_command_identifier" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + "device_http_command_identifier" + doublQoute + comma + doublQoute + "ip_address" + doublQoute + colon + doublQoute + ConfigurationManager.AppSettings["GatewayIpAddress"] + doublQoute + comma + doublQoute + "port" + doublQoute + colon + doublQoute + ConfigurationManager.AppSettings["GatewayPort"] + doublQoute + closeBrace + comma);
            objSbr.Append(doublQoute + "device_commands" + doublQoute + colon + openSqBrace + openBrace + doublQoute + "callback_url" + doublQoute + colon + doublQoute + objRequestParameters.CallbackUrl + doublQoute + comma + doublQoute + "callback_headers" + doublQoute + colon + openBrace + doublQoute + "Accept" + doublQoute + colon + doublQoute + "application/json" + doublQoute + comma + doublQoute + "Content-Type" + doublQoute + colon + doublQoute + "application/json" + doublQoute + closeBrace + comma);
            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + objRequestParameters.CommandName + doublQoute + comma);
            objSbr.Append(doublQoute + "command_type" + doublQoute + colon + doublQoute + "HTTP" + doublQoute + comma + doublQoute + "key_value_pairs" + doublQoute + colon);

            objSbr.Append(openBrace);
            objSbr.Append(doublQoute + "request" + doublQoute + colon + doublQoute + doublQoute+comma);
            objSbr.Append(doublQoute + "timeSent" + doublQoute + colon + doublQoute + objRequestParameters .TimeSent+ doublQoute + comma);
            objSbr.Append(doublQoute + "deviceKey" + doublQoute + colon + doublQoute + objRequestParameters.DeviceKey + doublQoute + comma);
            objSbr.Append(doublQoute + "deviceType" + doublQoute + colon + doublQoute + objRequestParameters.DeviceInstnaceType + doublQoute);

            if (objRequestParameters.CommandName == "AccessGroupCreate")
                        //|| objRequestParameters.CommandName == "AccessGroupDelete")
                 
            {               
                prepareAccessGroupRequestInfo(objRequestParameters, objSbr);                
            }
            else if (objRequestParameters.CommandName == "AccessGroupModify")
            {
                prepareAccessGroupModify(objRequestParameters, objSbr);                
            }
            else  if (objRequestParameters.Properties != null && objRequestParameters.Properties.Count > 0)
            {
                objSbr.Append(comma);
                objSbr.Append(doublQoute + "properties" + doublQoute + colon + openBrace);

                if (objRequestParameters.CommandName == "UserCodeAdd" || objRequestParameters.CommandName == "UserCodeModify")
                {
                    objSbr.Append(doublQoute + "property" + doublQoute + colon + openBrace);
                    PrepareProperties(objRequestParameters, objSbr, objRequestParameters.Property.UserCodeInformation.Properties);
                    objSbr.Append(closeBrace);
                }
                else if (objRequestParameters.CommandName == "UserCodeAdd2" || objRequestParameters.CommandName == "UserCodeModify2")
                {
                    objSbr.Append(doublQoute + "property" + doublQoute + colon + openBrace);
                    PrepareProperties(objRequestParameters, objSbr, objRequestParameters.Property.UserCodeInformation.Properties);
                    objSbr.Append(closeBrace);
                }
                else if (objRequestParameters.CommandName == "CardHolderAdd" || objRequestParameters.CommandName == "CardHolderModify")
                {
                    objSbr.Append(doublQoute + "property" + doublQoute + colon + openBrace);
                    PrepareProperties(objRequestParameters, objSbr, objRequestParameters.Properties);
                    objSbr.Append(closeBrace);
                }
                else
                {
                    objSbr.Append(doublQoute + "property" + doublQoute + colon + openSqBrace);
                    PrepareProperties(objRequestParameters, objSbr, objRequestParameters.Properties);
                    objSbr.Append(closeSqBrace);
                }

                objSbr.Append(closeBrace);
            }

            objSbr.Append(closeBrace);
            objSbr.Append(closeBrace);
            objSbr.Append(closeSqBrace);
            if (objRequestParameters.CommandName != "AccessGroupCreate" && objRequestParameters.CommandName != "AccessGroupModify"
                && objRequestParameters.CommandName != "AccessGroupDelete"
                && objRequestParameters.CommandName != "GetIntrusionStatus" && objRequestParameters.CommandName != "ZoneResetBypass"
                && objRequestParameters.CommandName != "AreaArm" && objRequestParameters.CommandName != "AreaDisarm"
                && objRequestParameters.CommandName != "GetProfileNumberList" && objRequestParameters.CommandName != "GetIntrusionReport"
                && objRequestParameters.CommandName != "GetUserCodesInformation" && objRequestParameters.CommandName != "GetUsersCodeList"
                && objRequestParameters.CommandName != "UserCodeAdd" && objRequestParameters.CommandName != "UserCodeDelete"
                && objRequestParameters.CommandName != "UserCodeModify" && objRequestParameters.CommandName != "ZoneBypass"
                && objRequestParameters.CommandName != "UserCodeModify2" && objRequestParameters.CommandName != "UserCodeAdd2"
                && objRequestParameters.CommandName != "RestartAgent" && objRequestParameters.CommandName != "Ping"                
                && objRequestParameters.CommandName != "GetAccessControlStatus" && objRequestParameters.CommandName != "CardHolderAdd"
                && objRequestParameters.CommandName != "CardHolderDelete" && objRequestParameters.CommandName != "CardHolderModify"
                && objRequestParameters.CommandName != "GetCardHoldersInformation" && objRequestParameters.CommandName != "GetCardHolderList"
                && objRequestParameters.CommandName != "GetAccessGroupInformation" && objRequestParameters.CommandName != "GetAccessGroupList"                
                && objRequestParameters.CommandName != "MomentaryOpenDoor" && objRequestParameters.CommandName != "GetReadersList"
                && objRequestParameters.CommandName != "GetAccessControlStatus" && objRequestParameters.CommandName != "GetAccessControlReport"
                && objRequestParameters.CommandName != "Configure" && objRequestParameters.CommandName != "GetStatus"
                && objRequestParameters.CommandName != "RestartStatus"
                )
            {
                objSbr.Append(comma);
                objAlarm = PrepareAlarmRequestBody(objRequestParameters.Alarms, objRequestParameters.DeviceType);
            }
            objSbr.Append(objAlarm);
            objSbr.Append(closeBrace);
            
            return objSbr.ToString();
        }
        public string PrepareRequestBodyForEMC(EMCParameters objRequestParameters)
        {
            StringBuilder objSbr = new StringBuilder();
            string openBrace = "{";
            string closeBrace = "}";
            string openSqBrace = "[";
            string closeSqBrace = "]";
            string comma = ",";
            string colon = ":";
            string doublQoute = "\"";

            objSbr.Append(openBrace);
            objSbr.Append(doublQoute + "send_to" + doublQoute + colon + openSqBrace + doublQoute + "~emc_ip" + doublQoute + closeSqBrace + comma);
            objSbr.Append(doublQoute + "port" + doublQoute + colon + openSqBrace + doublQoute + "~emc_port" + doublQoute + closeSqBrace + comma);
            objSbr.Append(doublQoute + "tcp_msg" + doublQoute + colon + doublQoute);
            objSbr.Append("<?xml version='1.0'?><newalarm><siteid>" + objRequestParameters.siteId + "</siteid><alarmid></alarmid>");
            objSbr.Append("<alarmtype>" + objRequestParameters.alarmType + "</alarmtype><zone>" + objRequestParameters.zone + "</zone>");
            objSbr.Append("<status>" + objRequestParameters.status + "</status><data></data></newalarm>" + doublQoute);
            objSbr.Append(closeBrace);
            return objSbr.ToString();
        }

        private void prepareAccessGroupRequestInfo(DeviceRequestParameters objRequestParameters, StringBuilder objSbr)
        {
            string openBrace = "{";
            string closeBrace = "}";
            string openSqBrace = "[";
            string closeSqBrace = "]";
            string comma = ",";
            string colon = ":";
            string doublQoute = "\"";

            objSbr.Append(comma);
            objSbr.Append(doublQoute + "properties" + doublQoute + colon + openBrace);
            if (objRequestParameters.CommandName == "AccessGroupModify")
            {
                objSbr.Append(doublQoute + "property" + doublQoute + colon + openSqBrace);
                PrepareProperties(objRequestParameters, objSbr, objRequestParameters.Properties);
                objSbr.Append(openBrace);
            }

            if (objRequestParameters.CommandName != "AccessGroupModify")
            {
                objSbr.Append(doublQoute + "property" + doublQoute + colon + openBrace);
            }
            objSbr.Append(doublQoute + "accessGroupInformation" + doublQoute + colon + openSqBrace);

            objSbr.Append(openBrace);
            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupName + doublQoute + comma);
            objSbr.Append(doublQoute + "description" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupDesc + doublQoute);
            objSbr.Append(closeBrace + comma);

            objSbr.Append(openBrace);

            objSbr.Append(doublQoute + "accessGroupTimePeriod" + doublQoute + colon + openSqBrace);

            if (objRequestParameters.AccessGroupTimePeriods != null && objRequestParameters.AccessGroupTimePeriods.Count > 0)
            {
                int intCount = objRequestParameters.AccessGroupTimePeriods.Count;
                if (intCount > 0)
                {
                    for (int i = 0; i < intCount; i++)
                    {
                        objSbr.Append(openBrace);
                        objSbr.Append(doublQoute + "beginTime" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupTimePeriods[i].BeginTime + doublQoute + comma);
                        objSbr.Append(doublQoute + "endTime" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupTimePeriods[i].EndTime + doublQoute + comma);
                        objSbr.Append(doublQoute + "days" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupTimePeriods[i].Days + doublQoute);
                        objSbr.Append(closeBrace);
                        objSbr.Append(comma);
                    }
                    objSbr.Remove(objSbr.Length - 1, 1);
                }
            }

            if (objRequestParameters.AccessGroupReaders != null && objRequestParameters.AccessGroupReaders.Count > 0)
            {
                objSbr.Append(comma);
                objSbr.Append(openBrace);
                objSbr.Append(doublQoute + "readerInformation" + doublQoute + colon + openSqBrace);
                int intCount = objRequestParameters.AccessGroupReaders.Count;
                if (intCount > 0)
                {
                    for (int i = 0; i < intCount; i++)
                    {
                        objSbr.Append(openBrace);
                        objSbr.Append(doublQoute + "readerId" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupReaders[i].ReaderId + doublQoute + comma);
                        objSbr.Append(doublQoute + "readerName" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupReaders[i].ReaderName + doublQoute);
                        objSbr.Append(closeBrace);
                        objSbr.Append(comma);
                    }
                    objSbr.Remove(objSbr.Length - 1, 1);
                }
                objSbr.Append(closeSqBrace);
                objSbr.Append(closeBrace);
            }
            objSbr.Append(closeSqBrace);
            objSbr.Append(closeBrace);            
            objSbr.Append(closeSqBrace);
            if (objRequestParameters.CommandName != "AccessGroupModify")
            {
                objSbr.Append(closeBrace);
            }
            if (objRequestParameters.CommandName == "AccessGroupModify")
            {
                objSbr.Append(closeBrace);
                objSbr.Append(closeSqBrace);
            }
            objSbr.Append(closeBrace);
        }

        private static void PrepareProperties(DeviceRequestParameters objRequestParameters, StringBuilder objSbr, List<Property> properties)
        {
            string openBrace = "{";
            string closeBrace = "}";
            string comma = ",";
            string colon = ":";
            string doublQoute = "\"";
            string openSqBrace = "[";
            string closeSqBrace = "]";

            if (objRequestParameters.Properties != null)
            {
                int intCount = properties.Count - 1;
                switch (objRequestParameters.CommandName)
                {
                    case "UserCodeAdd":
                    case "UserCodeModify":                    
                        objSbr.Append(doublQoute + "userCodeInformation" + doublQoute + colon);
                        objSbr.Append(openSqBrace);
                        break;
                    case "UserCodeAdd2":
                    case "UserCodeModify2":
                        objSbr.Append(doublQoute + "userCodeInformation" + doublQoute + colon);
                        objSbr.Append(openSqBrace);
                        break;
                    case"CardHolderAdd":
                    case"CardHolderModify":                        
                        objSbr.Append(doublQoute + "cardHolderInformation" + doublQoute + colon);
                        objSbr.Append(openSqBrace);
                        break;
                }

                int count = properties.Count;

                for (int i = 0; i < properties.Count; i++)
                {
                    objSbr.Append(openBrace);
                    objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + properties[i].Name + doublQoute + comma);
                    objSbr.Append(doublQoute + "value" + doublQoute + colon + doublQoute + properties[i].Value + doublQoute);
                    objSbr.Append(closeBrace);
                    if (i < intCount)
                    {
                        objSbr.Append(comma);
                    }                    
                }

                switch (objRequestParameters.CommandName)
                {
                    case "UserCodeAdd":
                    case "UserCodeModify":
                    case "UserCodeAdd2":
                    case "UserCodeModify2":
                    case "CardHolderAdd":
                    case "CardHolderModify":
                        objSbr.Append(closeSqBrace);                        
                        break;                                       
                }
            }
        }

        public string GetDevicetype(DeviceRequestParameters objRequestParameters)
        {
            string strDeviceInstnaceType = string.Empty;
            switch (objRequestParameters.DeviceType.ToString().ToLower())
            {
                case "costar111":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["Costar111"].ToString();
                    break;
                case "ipconfigure530":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["ipConfigure530"].ToString();
                    break;
                case "verintedgevr200":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["VerintEdgeVr200"].ToString();
                    break;
                case "edata524":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["eData524"].ToString();
                    break;
                case "edata300":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["eData300"].ToString();
                    break;
                case "dmpxr100":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["dmpXR100"].ToString();
                    break;
                case "dmpxr500":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["dmpXR500"].ToString();
                    break;
                case "dmpxr100access":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["dmpXR100Access"].ToString();
                    break;
                case "dmpxr500access":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["dmpXR500Access"].ToString();
                    break;
                case "bosch_d9412gv4":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["bosch_D9412GV4"].ToString();
                    break;
                case "videofied01":
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["videofied01"].ToString();
                    break;
                default:
                    strDeviceInstnaceType = ConfigurationManager.AppSettings["Costar111"].ToString();
                    break;
            }
            return strDeviceInstnaceType;
        }

        public StringBuilder PrepareAlarmRequestBody(IList<AlarmDTO> objlstAlarmDTO, string DeviceType)
        {
            logger.Debug("Entered in method PrepareAlarmRequestBody");
            StringBuilder objAlarmBaseHeader = new StringBuilder();
            StringBuilder objAlarm = new StringBuilder();
            string strAlarm = string.Empty;
            string openBrace = "{";
            string closeBrace = "}";
            string openSqBrace = "[";
            string closeSqBrace = "]";
            string comma = ",";
            string colon = ":";
            string doublQoute = "\"";
            string strDeviceInstnaceType = string.Empty;

            if (objlstAlarmDTO!=null && objlstAlarmDTO.Count > 0)
            {
                objAlarm.Append(doublQoute + "rules" + doublQoute + colon + openSqBrace);
                string conditionType = string.Empty;
                objlstAlarmDTO.ToList().ForEach(x =>
                {
                    conditionType = string.Empty;
                    switch(x.RelationalOperator.ToString())
                    {
                        case"==":
                            conditionType = "equal_rule_condition";
                            break;
                        case ">":
                            conditionType = "greater_than_rule_condition";
                            break;
                        case "<":
                            conditionType = "less_than_rule_condition";
                            break;
                        case ">=":
                            conditionType = "greater_than_equal_rule_condition";
                            break;
                        case "<=":
                            conditionType = "less_than_equal_rule_condition";
                            break;
                        case "!=":
                            conditionType = "not_in_rule_condition";
                            break;
                    }

                    string strSibilingKey = "SparkIntrusionReport";
                    if (DeviceType.ToLower().Equals("dmpxr500") || DeviceType.ToLower().Equals("dmpxr100") || DeviceType.ToLower().Equals("videofied01") || DeviceType.ToLower().Equals("bosch_d9412gv4"))
                    {
                        strSibilingKey = "SparkIntrusionReport";
                    }
                    else if (DeviceType.ToLower().Equals("edata300") || DeviceType.ToLower().Equals("edata524") || DeviceType.ToLower().Equals("dmpxr100access") || DeviceType.ToLower().Equals("dmpxr500access"))
                    {
                        strSibilingKey = "SparkAccessControlReport";
                    }
                    else if (DeviceType.ToLower().Equals("verintedgevr200") || DeviceType.ToLower().Equals("ipconfigure530") || DeviceType.ToLower().Equals("costar111"))
                    {
                        strSibilingKey = "SparkDvrReport";
                    }

                    strAlarm += openBrace + doublQoute + "description" + doublQoute + colon + doublQoute + x.Name + doublQoute + comma + doublQoute + "active" + doublQoute + colon + doublQoute + "true" + doublQoute + comma;
                    switch (x.Name.ToLower())
                    {
                        case "smart":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "parent_or_sibling_key" + doublQoute + colon + doublQoute + strSibilingKey + ".properties.propertyList.name" + doublQoute + comma + doublQoute + "parent_or_sibling_value" + doublQoute + colon + doublQoute + "SMART" + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkDvrReport.properties.propertyList.propertyItem" + doublQoute + comma + doublQoute + "value" + doublQoute + colon +  "[\"Unsupported\",\"Passed\",\"\"]" +  closeBrace + comma;
                            break;
                        case "drivetemperature":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "parent_or_sibling_key" + doublQoute + colon + doublQoute + strSibilingKey + ".properties.propertyList.name" + doublQoute + comma + doublQoute + "parent_or_sibling_value" + doublQoute + colon + doublQoute + "driveTemp" + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkDvrReport.properties.propertyList.propertyItem" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + x.Threshold.ToString() + doublQoute + closeBrace + comma;
                            break;
                        case "raidstatus":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "parent_or_sibling_key" + doublQoute + colon + doublQoute + strSibilingKey + ".properties.propertyList.name" + doublQoute + comma + doublQoute + "parent_or_sibling_value" + doublQoute + colon + doublQoute + "raidStatus" + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkDvrReport.properties.propertyList.propertyItem" + doublQoute + comma + doublQoute + "value" + doublQoute + colon +  "[\"clean\",\"\"]" +  closeBrace + comma;
                            break;
                        case "videoloss":
                            if (x.Threshold.GetType().Name.Equals(typeof(Dictionary<string, bool>).Name))
                            {
                                // Threshold value is of object type. If dictionary type comes then we need to change the format of display hence added the condition here.
                                strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "parent_or_sibling_key" + doublQoute + colon + doublQoute + strSibilingKey + ".properties.propertyList.name" + doublQoute + comma + doublQoute + "parent_or_sibling_value" + doublQoute + colon + doublQoute + "videoLoss" + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkDvrReport.properties.propertyList.propertyItem" + doublQoute + comma + doublQoute + "value" + doublQoute + colon +  "[\"\",\" \"]" +  closeBrace + comma;
                            }
                            else
                            {
                                strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "parent_or_sibling_key" + doublQoute + colon + doublQoute + strSibilingKey + ".properties.propertyList.name" + doublQoute + comma + doublQoute + "parent_or_sibling_value" + doublQoute + colon + doublQoute + "videoLoss" + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkDvrReport.properties.propertyList.propertyItem" + doublQoute + comma + doublQoute + "value" + doublQoute + colon +  "[\"\",\" \"]" +  closeBrace + comma;
                            }
                            break;
                        case "daysrecorded":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + strSibilingKey + ".properties.property.daysRecorded" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + x.Threshold.ToString() + doublQoute + closeBrace + comma;
                            break;
                        case "isnotrecording":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + strSibilingKey + ".properties.property.isNotRecording" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + x.Threshold.ToString() + doublQoute + closeBrace + comma;
                            break;
                        case "networkdown":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + strSibilingKey + ".properties.property.networkDown" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + x.Threshold.ToString() + doublQoute + closeBrace + comma;
                            break;
                        case "areaarmed":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkIntrusionReport.AreasStatusList.AreaStatus.properties.property.armed" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + x.Threshold.ToString() + doublQoute + closeBrace + comma;
                            break;
                        case "areadisarmed":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + "not_equal_rule_condition" + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkIntrusionReport.AreasStatusList.AreaStatus.properties.property.armed" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + x.Threshold.ToString() + doublQoute + closeBrace + comma;
                            break;
                        case "zonealarm":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkIntrusionReport.AreasStatusList.AreaStatus.ZonesStatusList.ZoneStatus.properties.property.zoneStatus" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + "Alarm" + doublQoute + closeBrace + comma;
                            break;
                        case "zonetrouble":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkIntrusionReport.AreasStatusList.AreaStatus.ZonesStatusList.ZoneStatus.properties.property.zoneStatus" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + "Trouble" + doublQoute + closeBrace + comma;
                            break;
                        case "zonebypass":
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkIntrusionReport.AreasStatusList.AreaStatus.ZonesStatusList.ZoneStatus.properties.property.zoneStatus" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + "Bypassed" + doublQoute + closeBrace + comma;
                            break;
                        case "doorforced":
                           // strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkAccessControlReport.DoorStatusDataList.DoorStatusData.properties.property.doorStatus" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + "Forced" + doublQoute + comma + doublQoute + "parent_or_sibling_key" + doublQoute + colon + doublQoute + "SparkAccessControlReport.DoorStatusDataList.DoorStatusData.properties.property.doorName" + doublQoute + comma + doublQoute + "parent_or_sibling_value" + doublQoute + colon + doublQoute + x.Threshold.ToString() + doublQoute + closeBrace + comma;
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkAccessControlReport.DoorStatusDataList.DoorStatusData.properties.property.doorStatus" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + "Forced" + doublQoute + closeBrace + comma;
                            break;
                        case "doorheld":
                            // strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkAccessControlReport.DoorStatusDataList.DoorStatusData.properties.property.doorStatus" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + "Held" + doublQoute + comma + doublQoute + "parent_or_sibling_key" + doublQoute + colon + doublQoute + "SparkAccessControlReport.DoorStatusDataList.DoorStatusData.properties.property.doorName" + doublQoute + comma + doublQoute + "parent_or_sibling_value" + doublQoute + colon + doublQoute + x.Threshold.ToString() + doublQoute + closeBrace + comma;
                            strAlarm += doublQoute + "condition" + doublQoute + colon + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + conditionType + doublQoute + comma + doublQoute + "property" + doublQoute + colon + doublQoute + "SparkAccessControlReport.DoorStatusDataList.DoorStatusData.properties.property.doorStatus" + doublQoute + comma + doublQoute + "value" + doublQoute + colon + doublQoute + "Held" + doublQoute + closeBrace + comma;
                            break;
                        default:
                            break;
                    }
                    strAlarm += doublQoute + "then_actions" + doublQoute + colon + openSqBrace + openBrace + doublQoute + "type" + doublQoute + colon + doublQoute + "http_request_rule_action" + doublQoute + comma + doublQoute + "send_to" + doublQoute + colon + doublQoute + x.CallbackURL + doublQoute + comma + doublQoute + "priority" + doublQoute + colon + doublQoute + "1" + doublQoute + closeBrace + closeSqBrace + comma;
                    strAlarm += doublQoute + "else_actions" + doublQoute + colon + openSqBrace + closeSqBrace + closeBrace + comma;
                });

                strAlarm = strAlarm.Substring(0,strAlarm.Length - 1);
                objAlarm.Append(strAlarm);               

                objAlarm.Append(closeSqBrace);
            }
            else
            {
                objAlarm.Append(doublQoute + "rules" + doublQoute + colon + openSqBrace + closeSqBrace);
            }
            logger.Debug(objAlarm.ToString());
            return objAlarm;
        }

        private string prepareAccessGroupModify(DeviceRequestParameters objRequestParameters, StringBuilder objSbr)
        {
            string openBrace = "{";
            string closeBrace = "}";
            string openSqBrace = "[";
            string closeSqBrace = "]";
            string comma = ",";
            string colon = ":";
            string doublQoute = "\"";

            objSbr.Append(comma);

            objSbr.Append(doublQoute + "accessGroupInformation" + doublQoute + colon + openBrace);
            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + "accessGroupInformation" + doublQoute + comma);
            

            objSbr.Append(doublQoute + "properties" + doublQoute + colon + openBrace);
            
            objSbr.Append(doublQoute + "property" + doublQoute + colon + openSqBrace);
            PrepareProperties(objRequestParameters, objSbr, objRequestParameters.Properties);
            objSbr.Append(closeSqBrace);

            objSbr.Append(closeBrace + comma);
            
            objSbr.Append(doublQoute + "AccessGroupTimePeriodList" + doublQoute + colon + openBrace);
            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + "accessPeriodData" + doublQoute + comma);

            objSbr.Append(doublQoute + "AccessGroupTimePeriod" + doublQoute + colon + openSqBrace);

            if (objRequestParameters.AccessGroupInformation != null && objRequestParameters.AccessGroupInformation.Count > 0)
            {
               
                int intCount = objRequestParameters.AccessGroupInformation.Count;
                if (intCount > 0)
                {
                    for (int i = 0; i < intCount; i++)
                    {
                        objSbr.Append(openBrace);
                        objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + "AccessGroupTimePeriod" + doublQoute + comma);
                        objSbr.Append(doublQoute + "properties" + doublQoute + colon + openBrace);
                        objSbr.Append(doublQoute + "property" + doublQoute + colon + openSqBrace);
                        for (int j = 0; j < objRequestParameters.AccessGroupInformation[i].AccessGroupTimePeriods.Count; j++)
                        {
                            objSbr.Append(openBrace);
                            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + "beginTime" + doublQoute + comma);
                            objSbr.Append(doublQoute + "value" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupInformation[i].AccessGroupTimePeriods[j].BeginTime + doublQoute);
                            objSbr.Append(closeBrace + comma);

                            objSbr.Append(openBrace);
                            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + "endTime" + doublQoute + comma);
                            objSbr.Append(doublQoute + "value" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupInformation[i].AccessGroupTimePeriods[j].EndTime + doublQoute);
                            objSbr.Append(closeBrace + comma);

                            objSbr.Append(openBrace);
                            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + "days" + doublQoute + comma);
                            objSbr.Append(doublQoute + "value" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupInformation[i].AccessGroupTimePeriods[j].Days + doublQoute);
                            objSbr.Append(closeBrace);

                        }
                        
                        objSbr.Append(closeSqBrace);
                        objSbr.Append(comma);

                        objSbr.Append(doublQoute + "ReadersList" + doublQoute + colon + openBrace);
                        objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + "readersList" + doublQoute + comma);
                        objSbr.Append(doublQoute + "ReaderInformation" + doublQoute + colon + openSqBrace);
                        for (int k = 0; k < objRequestParameters.AccessGroupInformation[i].AccessGroupReaders.Count; k++)
                        {
                            objSbr.Append(openBrace);
                            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + "readerInformation" + doublQoute + comma);
                            objSbr.Append(doublQoute + "properties" + doublQoute + colon + openBrace);
                            objSbr.Append(doublQoute + "property" + doublQoute + colon + openSqBrace);
                            objSbr.Append(openBrace);
                            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + "readerId" + doublQoute + comma);
                            objSbr.Append(doublQoute + "value" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupInformation[i].AccessGroupReaders[k].ReaderId + doublQoute);
                            objSbr.Append(closeBrace + comma);
                            objSbr.Append(openBrace);
                            objSbr.Append(doublQoute + "name" + doublQoute + colon + doublQoute + "readerName" + doublQoute + comma);
                            objSbr.Append(doublQoute + "value" + doublQoute + colon + doublQoute + objRequestParameters.AccessGroupInformation[i].AccessGroupReaders[k].ReaderName + doublQoute);
                            objSbr.Append(closeBrace);
                            objSbr.Append(closeSqBrace);
                            objSbr.Append(closeBrace);
                            objSbr.Append(closeBrace);
                            if (k != objRequestParameters.AccessGroupInformation[i].AccessGroupReaders.Count - 1)//Need to check  for last record and should not append (,) Comma
                            {
                                objSbr.Append(comma);
                            }
                        }

                        objSbr.Append(closeSqBrace);
                        objSbr.Append(closeBrace);
                        objSbr.Append(closeBrace);                       
                        objSbr.Append(closeBrace);
                        if (i != intCount - 1)
                        {
                            objSbr.Append(comma);
                        }
                    }
                    //objSbr.Remove(objSbr.Length - 1, 1);
                }
            }

            objSbr.Append(closeSqBrace);
            
            objSbr.Append(closeBrace);

            objSbr.Append(closeBrace);
            return objSbr.ToString();
        }
        

    }
}
