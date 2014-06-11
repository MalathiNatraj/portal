using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Infrastructure;
using Diebold.Platform.Proxies.DTO;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Services.Exceptions;
using System.Threading;

using Ninject;
using Ninject.Activation.Blocks;
using System.Configuration;
using Diebold.Platform.Proxies.Exceptions;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Diebold.Services.Impl
{
    public class MonitoringService : IMonitoringService
    {
        private readonly IMonitoringAPIService _monitoringAPIService;
        private readonly IActionDetailsService _masHourDetailsService;

        public MonitoringService(IMonitoringAPIService monitoringAPIService, IActionDetailsService masHourDetailService)
        {
            _monitoringAPIService = monitoringAPIService;
            _masHourDetailsService = masHourDetailService;
        }        
        public string PlaceonTest(Site site, string SeletedHour, string AccountNumber)
        {
            ProcessDataResponseDTO objprocess_data_response = new ProcessDataResponseDTO();
            var resultsite = GenerateInputParam(site, SeletedHour, AccountNumber);            
            JavaScriptSerializer objJss = new JavaScriptSerializer();
            string ResultSet = objJss.Serialize(resultsite);
            ResultSet = ResultSet.Remove(0, 1);
            ResultSet = ResultSet.Remove(ResultSet.Length - 1, 1);
            string ResultSet1 = ResultSet.Replace("\\u003c", "&lt;");
            string ResultSet2 = ResultSet1.Replace("\\u003e", "&gt;");
            string ResultSet3 = ResultSet2.Replace("\\u0027", "'");
            StringBuilder sbInputString = new StringBuilder();
            sbInputString.Append("{ \"body\" : \"<reqType>G</reqType><secUser>[~mas_user]</secUser><passWord>[~mas_password]</passWord><csNo>" + AccountNumber + "</csNo>");
            sbInputString.Append("<xmldata>");
            sbInputString.Append(ResultSet3);
            sbInputString.Append("</xmldata>\"}");
            try
            {
                string strResponseString = _monitoringAPIService.PlaceonTestAPI(sbInputString.ToString());
                int intResCount = 0;
                int intMaxResTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["APIMaxResponseTime"]);
                ProcessDataResponseDTO response = null;
                //string strResponseString = "{\"process_data_response\":{\"process_data_result\":\"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><MMDataDocument xmlns=\\\"http://www.gesecurity.com/webservices/mmmapi\\\"><Test><Test_Response><err_msg>ok</err_msg><effective_date>Apr  3 2013  7:47AM</effective_date><expire_date>Apr  3 2013  8:47AM</expire_date><test_seqno>54</test_seqno></Test_Response></Test></MMDataDocument>\",\"@xmlns\":\"http://www.gesecurity.com/webservices/mmmapi\"}}";
                // string strResponseString = "{\"process_data_response\":{\"process_data_result\":\"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><MMDataDocument xmlns=\\\"http://www.gesecurity.com/webservices/mmmapi\\\"><Test><Test_Response><table_name>OnOff Test</table_name><entry_id>8003526</entry_id><seq_no>1</seq_no><cs_no>22-8760</cs_no><site_no>8003526</site_no><system_no>2004222</system_no><err_code>48</err_code><addl_err_info>System is currently on test.</addl_err_info><exec_date>2013-04-03T07:39:52.223</exec_date></Test_Response></Test></MMDataDocument>\",\"@xmlns\":\"http://www.gesecurity.com/webservices/mmmapi\"}}";
                // Response need to be obtained
               

                if (string.IsNullOrEmpty(strResponseString) == false)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    response = (ProcessDataResponseDTO)js.Deserialize<ProcessDataResponseDTO>(strResponseString);
                    if (response != null && response.process_data_response != null && response.process_data_response.process_data_result != null)
                    {
                        string strXmlStream = response.process_data_response.process_data_result;
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(strXmlStream);

                        if (xDoc.GetElementsByTagName("err_msg").Count > 0)
                        {
                            return xDoc.GetElementsByTagName("err_msg")[0].InnerText;
                        }
                        else if (xDoc.GetElementsByTagName("error_msg").Count > 0)
                        {
                            return xDoc.GetElementsByTagName("error_msg")[0].InnerText;
                        }
                        else if (xDoc.GetElementsByTagName("addl_err_info").Count > 0)
                        {
                            return xDoc.GetElementsByTagName("addl_err_info")[0].InnerText;
                        }
                        else if (xDoc.GetElementsByTagName("MMDataDocument").Count > 0)
                        {
                            return xDoc.GetElementsByTagName("MMDataDocument")[0].InnerText;
                        }
                    }
                    else
                    {
                        throw new ServiceException("Response has not been obtained from the API");
                    }
                }

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            

            return string.Empty;
        }

        public string PlaceonTestDDChange(Site site, string SeletedHour, string AccountNumber)
        {
            ProcessDataResponseDTO objprocess_data_response = new ProcessDataResponseDTO();
            var resultsite = GenerateInputParamforDDChange(site, SeletedHour, AccountNumber);
            
            JavaScriptSerializer objJss = new JavaScriptSerializer();
            string ResultSet = objJss.Serialize(resultsite);
            ResultSet = ResultSet.Remove(0, 1);
            ResultSet = ResultSet.Remove(ResultSet.Length - 1, 1);
            string ResultSet1 = ResultSet.Replace("\\u003c", "&lt;");
            string ResultSet2 = ResultSet1.Replace("\\u003e", "&gt;");
            string ResultSet3 = ResultSet2.Replace("\\u0027", "'");
            StringBuilder sbInputString = new StringBuilder();
            sbInputString.Append("{ \"body\" : \"<reqType>G</reqType><secUser>[~mas_user]</secUser><passWord>[~mas_password]</passWord><csNo>" + AccountNumber + "</csNo>");
            sbInputString.Append("<xmldata>");
            sbInputString.Append(ResultSet3);
            sbInputString.Append("</xmldata>\"}");
            try
            {
                string strResponseString = _monitoringAPIService.PlaceonTestAPIforDDChange(sbInputString.ToString());
                int intResCount = 0;
                int intMaxResTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["APIMaxResponseTime"]);
                ProcessDataResponseDTO response = null;
                //string strResponseString1 = "{\"process_data_response\":{\"process_data_result\":\"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><MMDataDocument xmlns=\\\"http://www.gesecurity.com/webservices/mmmapi\\\"><Test><Test_Response><err_msg>ok</err_msg><effective_date>Apr  3 2013  7:47AM</effective_date><expire_date>Apr  3 2013  8:47AM</expire_date><test_seqno>54</test_seqno></Test_Response></Test></MMDataDocument>\",\"@xmlns\":\"http://www.gesecurity.com/webservices/mmmapi\"}}";
                //string strResponseString1 = "{\"process_data_response\": {\"process_data_result\":\"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><MMDataDocument xmlns=\\\"http://www.gesecurity.com/webservices/mmmapi\\\"><GetAccountInfo><GetAccountInfo_Response><site_name>Webber Enterprises</site_name><site_addr1>6647 Baker Ave</site_addr1><street_no>6647</street_no><street_name>BAKER AVE</street_name><sitestat_id>T     </sitestat_id><country_name>USA</country_name><timezone_no>4</timezone_no><site_addr2>Dairy Mart</site_addr2><city_name>UNIONTOWN</city_name><state_id>OH</state_id><zip_code>44685</zip_code><county_name>SUMMIT</county_name><phone1>3304982664</phone1><cross_street>Boettler</cross_street><codeword1>BOETTLER</codeword1><cs_no>22-8761</cs_no><panel_phone>3304446657</panel_phone><panel_id>000999888</panel_id><timezone_descr>Eastern</timezone_descr><opt_2>No</opt_2><opt_3>No</opt_3><opt_4>Yes</opt_4><opt_5>No</opt_5><opt_6>No</opt_6><opt_7>No</opt_7><ontest_expire_date>2013-12-10T02:17:00</ontest_expire_date></GetAccountInfo_Response></GetAccountInfo></MMDataDocument>\",\"@xmln\":\"http://www.gesecurity.com/webservices/mmmapi\"}}";
                //string strResponseString1 = "{\"process_data_response\": {\"process_data_result\":\"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><MMDataDocument xmlns=\\\"http://www.gesecurity.com/webservices/mmmapi\\\"><GetAccountInfo><GetAccountInfo_Response><site_name>Webber Enterprises</site_name><site_addr1>6647 Baker Ave</site_addr1><street_no>6647</street_no><street_name>BAKER AVE</street_name><sitestat_id>T     </sitestat_id><country_name>USA</country_name><timezone_no>4</timezone_no><site_addr2>Dairy Mart</site_addr2><city_name>UNIONTOWN</city_name><state_id>OH</state_id><zip_code>44685</zip_code><county_name>SUMMIT</county_name><phone1>3304982664</phone1><cross_street>Boettler</cross_street><codeword1>BOETTLER</codeword1><cs_no>22-8761</cs_no><panel_phone>3304446657</panel_phone><panel_id>000999888</panel_id><timezone_descr>Eastern</timezone_descr><opt_2>No</opt_2><opt_3>No</opt_3><opt_4>Yes</opt_4><opt_5>No</opt_5><opt_6>No</opt_6><opt_7>No</opt_7></GetAccountInfo_Response></GetAccountInfo></MMDataDocument>\",\"@xmln\":\"http://www.gesecurity.com/webservices/mmmapi\"}}";
                //string strResponseString = "{\"process_data_response\":{\"process_data_result\":\"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><MMDataDocument xmlns=\\\"http://www.gesecurity.com/webservices/mmmapi\\\"><Test><Test_Response><table_name>OnOff Test</table_name><entry_id>8003526</entry_id><seq_no>1</seq_no><cs_no>22-8760</cs_no><site_no>8003526</site_no><system_no>2004222</system_no><err_code>48</err_code><addl_err_info>System is currently on test.</addl_err_info><exec_date>2013-04-03T07:39:52.223</exec_date></Test_Response></Test></MMDataDocument>\",\"@xmlns\":\"http://www.gesecurity.com/webservices/mmmapi\"}}";

                string strExpdate = string.Empty;
                if (string.IsNullOrEmpty(strResponseString) == false)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    response = (ProcessDataResponseDTO)js.Deserialize<ProcessDataResponseDTO>(strResponseString);
                    if (response != null && response.process_data_response != null && response.process_data_response.process_data_result != null)
                    {
                        string strXmlStream = response.process_data_response.process_data_result;
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(strXmlStream);

                        if (xDoc.GetElementsByTagName("err_msg").Count > 0)
                        {
                            return xDoc.GetElementsByTagName("err_msg")[0].InnerText;
                        }
                        else if (xDoc.GetElementsByTagName("error_msg").Count > 0)
                        {
                            return xDoc.GetElementsByTagName("error_msg")[0].InnerText;
                        }
                        else if (xDoc.GetElementsByTagName("addl_err_info").Count > 0)
                        {
                            return xDoc.GetElementsByTagName("addl_err_info")[0].InnerText;
                        }
                        else if (xDoc.GetElementsByTagName("MMDataDocument").Count > 0)
                        {
                            if (xDoc.GetElementsByTagName("ontest_expire_date").Count > 0)
                            {
                                strExpdate = "Exists";
                            }
                            else
                            {
                                strExpdate = "NotExists";
                            }
                            return strExpdate;
                            //return xDoc.GetElementsByTagName("MMDataDocument")[0].InnerText;
                        }
                    }
                    else
                    {
                        throw new ServiceException("Response has not been obtained from the API");
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return string.Empty;
        }

        public List<ReportsDTO> RunReport(DateTime dtFromDate, DateTime dtToDate, string strReportName, string AccountNumber)
        {
            StringBuilder sb = new StringBuilder();
            string strResponse = "";
            IList<ReportsDTO> objlstReportsDTO = new List<ReportsDTO>();
            if (strReportName.ToLower().Equals("Contact List".ToLower()) == true)
            {
                sb.Append("{\"body\" : \"<reqType>G</reqType><secUser>[~mas_user]</secUser><passWord>[~mas_password]</passWord><csNo>" + AccountNumber + "</csNo><xmldata>&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;&lt;MMDataDocument xmlns='http://www.gesecurity.com/webservices/mmmapi'&gt;&lt;GetAccountContacts&gt;&lt;GetAccountContacts_Request&gt;&lt;data_element&gt;GetAccountContacts&lt;/data_element&gt;&lt;/GetAccountContacts_Request&gt;&lt;/GetAccountContacts&gt;&lt;/MMDataDocument&gt;</xmldata>\"}");
                strResponse = _monitoringAPIService.RunReport(strReportName, dtFromDate, dtToDate, sb.ToString());
            }
            else if (strReportName.ToLower().Equals("Open / Close Normal".ToLower()) == true || strReportName.ToLower().Equals("Open / Close Irregular".ToLower()) == true || strReportName.ToLower().Equals("Events".ToLower()) == true)
            {
                var resultreport = GenerateInputParamforReport(dtFromDate, dtToDate);
                JavaScriptSerializer objJss = new JavaScriptSerializer();
                string ResultSet = objJss.Serialize(resultreport);
                ResultSet = ResultSet.Remove(0, 1);
                ResultSet = ResultSet.Remove(ResultSet.Length - 1, 1);
                string ResultSet1 = ResultSet.Replace("\\u003c", "&lt;");
                string ResultSet2 = ResultSet1.Replace("\\u003e", "&gt;");
                string ResultSet3 = ResultSet2.Replace("\\u0027", "'");

                sb.Append("{\"body\" : \"<reqType>G</reqType><secUser>[~mas_user]</secUser><passWord>[~mas_password]</passWord><csNo>" + AccountNumber + "</csNo><xmldata>");
                sb.Append(ResultSet3);
                sb.Append("</xmldata>\"}");
                strResponse = _monitoringAPIService.RunReport(strReportName, dtFromDate, dtToDate, sb.ToString());
            }
            else 
            {
                sb.Append("{\"body\" : \"<reqType>G</reqType><secUser>[~mas_user]</secUser><passWord>[~mas_password]</passWord><csNo>" + AccountNumber + "</csNo><xmldata>&lt;?xml version='1.0' encoding='utf-8'?&gt;&lt;MMDataDocument xmlns='http://www.gesecurity.com/webservices/mmmapi'&gt;&lt;GetAccountZones&gt;&lt;GetAccountZones_Request&gt;&lt;data_element>GetAccountZones&lt;/data_element&gt;&lt;/GetAccountZones_Request&gt;&lt;/GetAccountZones&gt;&lt;/MMDataDocument&gt;</xmldata>\"}");
                strResponse = _monitoringAPIService.RunReport(strReportName, dtFromDate, dtToDate, sb.ToString());
            }

            // Response need to be obtained
            ProcessDataResponseDTO response = null;

            if (string.IsNullOrEmpty(strResponse) == false)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                response = (ProcessDataResponseDTO)js.Deserialize<ProcessDataResponseDTO>(strResponse);
                if (response != null && response.process_data_response != null && response.process_data_response.process_data_result != null)
                {
                    string strXmlStream = response.process_data_response.process_data_result;
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(strXmlStream);

                    XmlNodeList nl = xDoc.ChildNodes;

                    XmlNode root = nl[1];
                    XmlNode SignalHistoryroot;
                    XmlNode SignalHistory_Responseroot;
                    if(root != null && root.HasChildNodes == true)
                    {
                        SignalHistoryroot = root.ChildNodes[0];
                        if (SignalHistoryroot != null && SignalHistoryroot.HasChildNodes == true)
                        {
                           // SignalHistory_Responseroot = SignalHistoryroot.ChildNodes[0];
                            foreach (XmlNode item in SignalHistoryroot.ChildNodes)
                            {
                                ReportsDTO objReportsDTO = new ReportsDTO();
                                if(item != null && item.InnerText.Equals("Access to this account has been denied."))
                                {
                                    objReportsDTO.AccessDenied = "Access to this account has been denied.";
                                    objlstReportsDTO.Add(objReportsDTO);
                                }
                                else if (item != null && item.HasChildNodes == true)
                                {
                                    // string name = SignalHistory_Responseroot.Name;
                                    
                                    foreach (XmlNode xnode in item.ChildNodes)
                                    {
                                        switch (xnode.Name)
                                        {
                                            case "sig_acct":
                                                objReportsDTO.sig_acct = xnode.InnerText;
                                                break;
                                            case "sig_date":
                                                objReportsDTO.sig_date = xnode.InnerText;
                                                break;
                                            case "sig_code":
                                                objReportsDTO.sig_code = xnode.InnerText;
                                                break;
                                            case "event":
                                                objReportsDTO.events = xnode.InnerText;
                                                break;
                                            case "eventhistcomment":
                                                objReportsDTO.eventhistcomment = xnode.InnerText;
                                                break;
                                            case "zone_comment":
                                                objReportsDTO.zone_comment = xnode.InnerText;
                                                break;
                                            case "additional_info":
                                                objReportsDTO.additional_info = xnode.InnerText;
                                                break;
                                            case "zone_id":
                                                objReportsDTO.zone_id = xnode.InnerText;
                                                break;
                                            case "event_id":
                                                objReportsDTO.event_id = xnode.InnerText;
                                                break;
                                            case "comment":
                                                objReportsDTO.comment = xnode.InnerText;
                                                break;
                                            case "restore_reqd_flag":
                                                objReportsDTO.restore_reqd_flag = xnode.InnerText;
                                                break;
                                            case "last_name":
                                                objReportsDTO.last_name = xnode.InnerText;
                                                break;
                                            case "first_name":
                                                objReportsDTO.first_name = xnode.InnerText;
                                                break;
                                            case "cs_seqno":
                                                objReportsDTO.cs_seqno = xnode.InnerText;
                                                break;
                                            case "pin":
                                                objReportsDTO.pin = xnode.InnerText;
                                                break;
                                            case "phone1":
                                                objReportsDTO.phone1 = xnode.InnerText;
                                                break;
                                            case "phone2":
                                                objReportsDTO.phone2 = xnode.InnerText;
                                                break;
                                            case "user_id":
                                                objReportsDTO.user_id = xnode.InnerText;
                                                break;
                                            case "err_msg":
                                                objReportsDTO.err_msg = xnode.InnerText;
                                                break;
                                            default:
                                                break;
                                        }

                                    }
                                    objlstReportsDTO.Add(objReportsDTO);
                                }
                            }
                            
                        }
                        else if (SignalHistoryroot != null && SignalHistoryroot.InnerText.Equals("Access to this account has been denied."))
                        {
                            ReportsDTO objReportsDTO = new ReportsDTO();
                            objReportsDTO.AccessDenied = "Access to this account has been denied.";
                            objlstReportsDTO.Add(objReportsDTO);
                        }
                    }

                }
                else
                {
                    throw new ServiceException("Response has not been obtained from the API");
                }
            }
            return objlstReportsDTO.ToList();
        }

        public string GenerateInputParam(Site site, string SeletedHours, string accNumber)
        {
            MMDataDocument objMMDataDocument = new MMDataDocument();
            objMMDataDocument.data_element = site.Name;
            //objMMDataDocument.data_element = System.Text.RegularExpressions.Regex.Replace(site.Name, @"\s+", "&nbsp;");
            objMMDataDocument.onoff_flag = "on";
            objMMDataDocument.testcat_id = accNumber;
            objMMDataDocument.test_hours = SeletedHours;
            objMMDataDocument.test_minutes = "00";
            StringBuilder sbInputString = new StringBuilder();
            sbInputString.Append("&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;&lt;MMDataDocument xmlns=&quot;http://www.gesecurity.com/webservices/mmmapi&quot;&gt;");
            sbInputString.Append("&lt;Test&gt;");
            sbInputString.Append("&lt;Test_Request&gt;");
            sbInputString.Append("&lt;data_element&gt;" + "Test" + "&lt;/data_element&gt;"); // Dataelement will be always Test in MAS
            sbInputString.Append("&lt;onoff_flag&gt;" + objMMDataDocument.onoff_flag + "&lt;/onoff_flag&gt;");
            sbInputString.Append("&lt;testcat_id&gt;" + "1" + "&lt;/testcat_id&gt;"); // Testcat_id will be always 1
            sbInputString.Append("&lt;test_hours&gt;" + objMMDataDocument.test_hours + "&lt;/test_hours&gt;");
            sbInputString.Append("&lt;test_minutes&gt;" + objMMDataDocument.test_minutes + "&lt;/test_minutes&gt;");
            sbInputString.Append("&lt;/Test_Request&gt;");
            sbInputString.Append("&lt;/Test&gt;");
            sbInputString.Append("&lt;/MMDataDocument&gt;");
            return sbInputString.ToString();
        }
        public string GenerateInputParamforDDChange(Site site, string SeletedHours, string accNumber)
        {
            MMDataDocument objMMDataDocument = new MMDataDocument();
            objMMDataDocument.data_element = site.Name;
            //objMMDataDocument.data_element = System.Text.RegularExpressions.Regex.Replace(site.Name, @"\s+", "&nbsp;");
            objMMDataDocument.onoff_flag = "on";
            objMMDataDocument.testcat_id = accNumber;
            objMMDataDocument.test_hours = SeletedHours;
            objMMDataDocument.test_minutes = "00";
            StringBuilder sbInputString = new StringBuilder();
            sbInputString.Append("&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;&lt;MMDataDocument xmlns=&quot;http://www.gesecurity.com/webservices/mmmapi&quot;&gt;");
            sbInputString.Append("&lt;GetAccountInfo&gt;");
            sbInputString.Append("&lt;GetAccountInfo_Request&gt;");
            sbInputString.Append("&lt;data_element&gt;" + "GetAccountInfo" + "&lt;/data_element&gt;"); // Dataelement will be always Test in MAS

            sbInputString.Append("&lt;/GetAccountInfo_Request&gt;");
            sbInputString.Append("&lt;/GetAccountInfo&gt;");
            sbInputString.Append("&lt;/MMDataDocument&gt;");
            return sbInputString.ToString();
        }

        public string GenerateInputParamforReport(DateTime dtFromDate, DateTime dtToDate)
        {
             MMDataDocument objMMDataDocument = new MMDataDocument();
             objMMDataDocument.start_date = dtFromDate.ToString("yyyy-MM-dd") + "T" + dtFromDate.ToString("hh:mm:ss");
             objMMDataDocument.end_date = dtToDate.ToString("yyyy-MM-dd") + "T" + dtToDate.ToString("hh:mm:ss");
            string[] arrStartDate = objMMDataDocument.start_date.Split('T');
            string[] arrEndDate = objMMDataDocument.end_date.Split('T');
            if (arrStartDate[1].Equals("12:00:00") && dtFromDate.ToShortTimeString().Equals("12:00 AM"))
            {
                objMMDataDocument.start_date = dtFromDate.ToString("yyyy-MM-dd") + "T" + "00:00:00";
            }
           

            if (arrStartDate[1].Equals("12:00:00") && dtToDate.ToShortTimeString().Equals("12:00 AM"))
            {
                objMMDataDocument.end_date = dtToDate.ToString("yyyy-MM-dd") + "T" + "00:00:00";
            }
           

            StringBuilder sbReportGeneration = new StringBuilder();
            sbReportGeneration.Append("&lt;?xml version='1.0' encoding='utf-8'?&gt;&lt;MMDataDocument xmlns='http://www.gesecurity.com/webservices/mmmapi'&gt;&lt;SignalHistory&gt;&lt;SignalHistory_Request&gt;&lt;data_element&gt;SignalHistory&lt;/data_element&gt;");
            sbReportGeneration.Append("&lt;start_date&gt;" + objMMDataDocument.start_date + "&lt;/start_date&gt;&lt;");
            sbReportGeneration.Append("end_date&gt;" + objMMDataDocument.end_date + "&lt;/end_date&gt;");
            sbReportGeneration.Append("&lt;/SignalHistory_Request&gt;&lt;/SignalHistory&gt;&lt;/MMDataDocument&gt;");
            return sbReportGeneration.ToString();
        }

    }
}
