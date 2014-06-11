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
    public class FireAlarmService : IFireAlarmService
    {
        private readonly IMonitoringAPIService _monitoringAPIService;

        public FireAlarmService(IMonitoringAPIService monitoringAPIService)
        {
            _monitoringAPIService = monitoringAPIService;
        }

        public List<ReportsDTO> FireAlarmReport(string AccountNumber)
        {
            StringBuilder sb = new StringBuilder();
            string strResponse = "";
            IList<ReportsDTO> objlstReportsDTO = new List<ReportsDTO>();
            //if (strReportName.ToLower().Equals("Contact List".ToLower()) == true)
            //{
                // sb.Append("{\"body\" : \"<reqType>G</reqType><secUser>[~mas_user]</secUser><passWord>[~mas_password]</passWord><csNo>" + AccountNumber + "</csNo><xmldata>&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;&lt;MMDataDocument xmlns='http://www.gesecurity.com/webservices/mmmapi'&gt;&lt;GetAccountContacts&gt;&lt;GetAccountContacts_Request&gt;&lt;data_element&gt;GetAccountContacts&lt;/data_element&gt;&lt;/GetAccountContacts_Request&gt;&lt;/GetAccountContacts&gt;&lt;/MMDataDocument&gt;</xmldata>\"}");
            string StartDateTime = System.DateTime.Now.ToString("yyyy-MM-dd") + "T" + "00:01:00";
            string EndDateTime = System.DateTime.Now.ToString("yyyy-MM-dd") + "T" + "23:59:00";
            sb.Append("{\"body\" : \"<reqType>G</reqType><secUser>[~mas_user]</secUser><passWord>[~mas_password]</passWord><csNo>" + AccountNumber + "</csNo><xmldata>&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;&lt;MMDataDocument xmlns='http://www.gesecurity.com/webservices/mmmapi'&gt;&lt;SignalHistory&gt;&lt;SignalHistory_Request&gt;&lt;data_element&gt;SignalHistory&lt;/data_element&gt;&lt;start_date&gt;" + StartDateTime + "&lt;/start_date&gt;&lt;end_date&gt;" + EndDateTime + "&lt;/end_date&gt;&lt;/SignalHistory_Request&gt;&lt;/SignalHistory&gt;&lt;/MMDataDocument&gt;</xmldata>\"}");
                strResponse = _monitoringAPIService.FireWidgetReport(sb.ToString());
            //}
            //else if (strReportName.ToLower().Equals("Open / Close Normal".ToLower()) == true || strReportName.ToLower().Equals("Open / Close Irregular".ToLower()) == true || strReportName.ToLower().Equals("Events".ToLower()) == true)
            //{
            //    var resultreport = GenerateInputParamforReport(dtFromDate, dtToDate);
            //    JavaScriptSerializer objJss = new JavaScriptSerializer();
            //    string ResultSet = objJss.Serialize(resultreport);
            //    ResultSet = ResultSet.Remove(0, 1);
            //    ResultSet = ResultSet.Remove(ResultSet.Length - 1, 1);
            //    string ResultSet1 = ResultSet.Replace("\\u003c", "&lt;");
            //    string ResultSet2 = ResultSet1.Replace("\\u003e", "&gt;");
            //    string ResultSet3 = ResultSet2.Replace("\\u0027", "'");

            //    sb.Append("{\"body\" : \"<reqType>G</reqType><secUser>[~mas_user]</secUser><passWord>[~mas_password]</passWord><csNo>" + AccountNumber + "</csNo><xmldata>");
            //    sb.Append(ResultSet3);
            //    sb.Append("</xmldata>\"}");
            //    strResponse = _monitoringAPIService.RunReport(strReportName, dtFromDate, dtToDate, sb.ToString());
            //}
            //else
            //{
            //    sb.Append("{\"body\" : \"<reqType>G</reqType><secUser>[~mas_user]</secUser><passWord>[~mas_password]</passWord><csNo>" + AccountNumber + "</csNo><xmldata>&lt;?xml version='1.0' encoding='utf-8'?&gt;&lt;MMDataDocument xmlns='http://www.gesecurity.com/webservices/mmmapi'&gt;&lt;GetAccountZones&gt;&lt;GetAccountZones_Request&gt;&lt;data_element>GetAccountZones&lt;/data_element&gt;&lt;/GetAccountZones_Request&gt;&lt;/GetAccountZones&gt;&lt;/MMDataDocument&gt;</xmldata>\"}");
            //    strResponse = _monitoringAPIService.RunReport(strReportName, dtFromDate, dtToDate, sb.ToString());
            //}

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
                    if (root != null && root.HasChildNodes == true)
                    {
                        SignalHistoryroot = root.ChildNodes[0];
                        if (SignalHistoryroot != null && SignalHistoryroot.HasChildNodes == true)
                        {
                            // SignalHistory_Responseroot = SignalHistoryroot.ChildNodes[0];
                            foreach (XmlNode item in SignalHistoryroot.ChildNodes)
                            {
                                ReportsDTO objReportsDTO = new ReportsDTO();
                                if (item != null && item.InnerText.Equals("Access to this account has been denied."))
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
