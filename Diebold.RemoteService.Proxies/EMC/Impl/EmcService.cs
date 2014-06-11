using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Diebold.RemoteService.Proxies.EMC.Contracts;
using Diebold.RemoteService.Proxies.EMC.DTO;
using Diebold.RemoteService.Proxies.EMC.Serializer;

namespace Diebold.RemoteService.Proxies.EMC.Impl
{
    public class EmcService : IEmcService
    {
        public void SendAlarm(string emcAccountNumber, string zoneNumber)
        {
            // Complete zone number with zeros on the left
            while (zoneNumber.Length < 3)
            {
                zoneNumber = "0" + zoneNumber;
            }

            bool isInTestMode = bool.Parse(ConfigurationManager.AppSettings["EmcTestMode"]);
            string emcAccNumb = (isInTestMode) ? "123456" : emcAccountNumber;

            var emcAlarm = new AlarmDTO(AlarmDTO.AlarmTypes.Alarm, emcAccNumb);
            emcAlarm.Zone = zoneNumber;

            var message = EmcSerializer.Serialize<AlarmDTO>(emcAlarm);

            string host = ConfigurationManager.AppSettings["EmcIP"];
            int port = int.Parse(ConfigurationManager.AppSettings["EmcPort"]);
            var tcpManager = new TcpManager(host, port);

            tcpManager.Send(message);
        }

        public bool ValidateEmcAccount(string emcAccountNumber)
        {
            bool isInTestMode = bool.Parse(ConfigurationManager.AppSettings["EmcTestMode"]);
            string emcAccNumb = (isInTestMode) ? "123456" : emcAccountNumber;

            var emcAlarm = new AlarmDTO(AlarmDTO.AlarmTypes.Poll, emcAccNumb);

            var message = EmcSerializer.Serialize<AlarmDTO>(emcAlarm);

            string host = ConfigurationManager.AppSettings["EmcIP"];
            int port = int.Parse(ConfigurationManager.AppSettings["EmcPort"]);
            var tcpManager = new TcpManager(host, port);

            return tcpManager.Send(message);
        }

    }

}
