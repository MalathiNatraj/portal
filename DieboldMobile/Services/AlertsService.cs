using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DieboldMobile.Models;

namespace DieboldMobile.Services
{
    public class AlertsService
    {
        public IList<AlertDetailsModel> getAlerts()
        {
            return prepareAlertData();
        }
        public IList<AlertDetailsModel> prepareAlertData()
        {
            IList<AlertDetailsModel> lstAlerts = new List<AlertDetailsModel>();
            AlertDetailsModel objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-GW2-XP04";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 1;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 1;
            objAlertModel.DeviceName = "ipConfigure 44(VM)";
            objAlertModel.Alert = "Video Loss Camera";
            objAlertModel.DeviceIpHostname = "192.168.100.73";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "10/11 09:37 PM ";
            objAlertModel.Threshold = "5";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-IP2-2K8R2";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = "ipConfigure on 51 (Physical)";
            objAlertModel.Alert = "Days Recorded Less Than 30";
            objAlertModel.DeviceIpHostname = "192.168.100.73";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "08/24 11:34 AM";
            objAlertModel.Threshold = "2";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-GW2-XP04";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = "Verint on 41";
            objAlertModel.Alert = "Drive Temperature Greater Than Or Equals 125";
            objAlertModel.DeviceIpHostname = "192.168.100.73";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "10/11 10:01 PM";
            objAlertModel.Threshold = "1";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-IP1-2K8R2";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = "ipConfigure 55";
            objAlertModel.Alert = "Video Loss Equals true";
            objAlertModel.DeviceIpHostname = "192.168.100.71";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "08/24 11:16 AM";
            objAlertModel.Threshold = "3";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-IP2-2K8R2";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = "ipConfigure 60";
            objAlertModel.Alert = "Raid Status";
            objAlertModel.DeviceIpHostname = "192.168.100.76";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "10/11 09:37 PM";
            objAlertModel.Threshold = "2";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-IP2-2K8R2";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = " ipConfig 55";
            objAlertModel.Alert = "Days Recorded Less Than 30";
            objAlertModel.DeviceIpHostname = "192.168.100.90";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "08/24 11:34 AM";
            objAlertModel.Threshold = "1";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-GW1-XP";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "3";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "4";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = " ipConfigure";
            objAlertModel.Alert = "Network Down";
            objAlertModel.DeviceIpHostname = "192.168.100.76";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "08/22 10:34 AM";
            objAlertModel.Threshold = "3";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-GW2-XP04";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 1;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 1;
            objAlertModel.DeviceName = "ipConfigure";
            objAlertModel.Alert = "Video Loss Camera";
            objAlertModel.DeviceIpHostname = "192.168.100.73";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "10/11 09:37 PM ";
            objAlertModel.Threshold = "5";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-IP2-2K8R2";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = "ipConfigure 60";
            objAlertModel.Alert = "Days Recorded Less Than 30";
            objAlertModel.DeviceIpHostname = "192.168.100.73";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "08/24 11:34 AM";
            objAlertModel.Threshold = "2";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-GW2-XP04";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = "ipConfigure 55";
            objAlertModel.Alert = "Drive Temperature Greater Than Or Equals 125";
            objAlertModel.DeviceIpHostname = "192.168.100.73";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "10/11 10:01 PM";
            objAlertModel.Threshold = "1";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-IP1-2K8R2";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = "Verint on 41";
            objAlertModel.Alert = "Video Loss Equals true";
            objAlertModel.DeviceIpHostname = "192.168.100.71";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "08/24 11:16 AM";
            objAlertModel.Threshold = "3";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-IP2-2K8R2";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "1";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "2";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = "ipConfigure on 51 (Physical)";
            objAlertModel.Alert = "Raid Status";
            objAlertModel.DeviceIpHostname = "192.168.100.76";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "10/11 09:37 PM";
            objAlertModel.Threshold = "2";
            lstAlerts.Add(objAlertModel);

            objAlertModel = new AlertDetailsModel();
            objAlertModel.SiteName = "Enterprise Security";
            objAlertModel.GatewayName = "DEV-GW1-XP";
            objAlertModel.Address = "Address";
            objAlertModel.State = "Open";
            objAlertModel.MonitoredDevicesCount = "3";
            objAlertModel.SiteId = 2;
            objAlertModel.MonitoredDevicesAlarmsCount = "4";
            objAlertModel.DeviceId = 2;
            objAlertModel.DeviceName = "ipConfigure 44(VM)";
            objAlertModel.Alert = "Network Down";
            objAlertModel.DeviceIpHostname = "192.168.100.76";
            objAlertModel.Recorded = "Y";
            objAlertModel.Unattended = "08/22 10:34 AM";
            objAlertModel.Threshold = "3";
            lstAlerts.Add(objAlertModel);

            return lstAlerts;
        }
    }
}