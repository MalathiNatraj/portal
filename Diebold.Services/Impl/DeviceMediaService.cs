using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Services.Contracts;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Impl
{

    public class DeviceMediaService : BaseCRUDService<DeviceMedia>, IDeviceMediaService
    {
        private IIntrusionApiService _intrusionApiService;
        private IDvrService _dvrService;
        private IIntrusionService _intrusionService;

        public DeviceMediaService(
            IIntKeyedRepository<DeviceMedia> deviceMediaRepository,
            IUnitOfWork unitOfWork,
            IValidationProvider validationProvider,
            IIntrusionApiService intrusionApiService,
            IIntrusionService intrusionService,
            IDvrService dvrService,
            ILogService logService)
            : base(deviceMediaRepository, unitOfWork, validationProvider, logService) {

                _intrusionApiService = intrusionApiService;
                _dvrService = dvrService;
                _intrusionService = intrusionService;
               
        }
        public DeviceMedia CaptureImage(int deviceId, string zoneNumber)
        {
            var log = string.Empty;

            
            var device = _dvrService.Get(deviceId);

            Intrusion objIntrusion = new Intrusion();
            objIntrusion.DeviceId = deviceId;
            objIntrusion.DeviceInstanceId = device.ExternalDeviceId;
            objIntrusion.zoneNumber = zoneNumber;

            var id = _intrusionService.MediaCapture(objIntrusion, DeviceMediaType.Image);

            var media = UpdateMediaStatus(new DeviceMedia() { MediaId = id.ToString(), MediaType = DeviceMediaType.Image.ToString(), Status = DeviceMediaStatus.Fetching.ToString() });

            if (ExecutePowershell(id, DeviceMediaType.Image, out log))
            {
                media.Status = DeviceMediaStatus.Completed.ToString();
                media.FileName = string.Format("{0}/{1}/{2}.{3}", ConfigurationManager.AppSettings["deviceMediaWebPath"], media.MediaType.ToLower(), media.MediaId, "jpg");
            }
            else {
                media.Status = DeviceMediaStatus.Failed.ToString();
            }
            media.Notes = log;
            media = UpdateMediaStatus(media);
            
            return media;
        }

        public DeviceMedia CaptureVideo(int deviceId, string zoneNumber)
        {

            var log = string.Empty;


            var device = _dvrService.Get(deviceId);

            Intrusion objIntrusion = new Intrusion();
            objIntrusion.DeviceId = deviceId;
            objIntrusion.DeviceInstanceId = device.ExternalDeviceId;
            objIntrusion.zoneNumber = zoneNumber;

            var id = _intrusionService.MediaCapture(objIntrusion, DeviceMediaType.Video);

            var media = UpdateMediaStatus(new DeviceMedia() { MediaId = id.ToString(), MediaType = DeviceMediaType.Video.ToString(), Status = DeviceMediaStatus.Fetching.ToString() });

            if (ExecutePowershell(id, DeviceMediaType.Video, out log))
            {
                media.Status = DeviceMediaStatus.Completed.ToString();
                media.FileName = string.Format("{0}/{1}/{2}.{3}", ConfigurationManager.AppSettings["deviceMediaWebPath"], media.MediaType.ToLower(), media.MediaId, "mpg");
            }
            else
            {
                media.Status = DeviceMediaStatus.Failed.ToString();
            }
            media.Notes = log;
            media = UpdateMediaStatus(media);

            return media;
        }

        public string CaptureVideoMASId(int cnxId, string serverId)
        {
            string log = string.Empty;

            if (ExecutePowershell(cnxId, DeviceMediaType.Video, out log))
            {
                var re = new Regex(@"-------.+\s+(?<=\d+)\s+\(\d+\Wrow");
                var match = re.Match(log);
                if (match.Groups.Count > 0) {
                    return match.Groups[0].Value;
                }
            }
            return "";
        }

        public DeviceMedia GetVideoMAS(int cnxId, string serverId)
        {
            string log = string.Empty;

            var media = new DeviceMedia();
            //log = "ediaType: mas\r\n\r\nProcessing Image Media\r\n\r\nSELECT ev_file FROM events WHERE ev_type=\u00279001\u0027 and cnx_id = 2999\r\n\r\n-h 10.79.15.35\r\n-p 5432\r\n-d frontel\r\n-U ssvid\r\n-c \"SELECT ev_file FROM events WHERE ev_type=\u00279001\u0027 and cnx_id = 2999\"\r\n\r\n ev_file \r\n---------\r\n   33793\r\n(1 row)\r\n\r\n\r\n\r\nCAPTURE_SUCCESS\r\n\r\n";
            
            if (ExecutePowershell(cnxId, DeviceMediaType.MAS, out log))
            {
                var re = new Regex(@"-------.+\s+(\d+)\s+\(\d+\Wrow");
                var match = re.Match(log);
                if (match.Groups.Count > 0)
                {
                    var id = -1;
                    var matchData = match.Groups[1].Value;
                    if(int.TryParse(matchData, out id)){
                        media = UpdateMediaStatus(new DeviceMedia() { MediaId = id.ToString(), MediaType = DeviceMediaType.Video.ToString(), Status = DeviceMediaStatus.Fetching.ToString() });

                        if (ExecutePowershell(id, DeviceMediaType.Video, out log))
                        {
                            media.Status = DeviceMediaStatus.Completed.ToString();
                            media.FileName = string.Format("{0}/{1}/{2}.{3}", ConfigurationManager.AppSettings["deviceMediaWebPath"], media.MediaType.ToLower(), media.MediaId, "mpg");
                        }
                        else
                        {
                            media.Status = DeviceMediaStatus.Failed.ToString();
                        }
                        media.Notes += log;
                        media = UpdateMediaStatus(media);

                        return media;
                    }
                }
            }

            media.Notes += log;
            media.Status = DeviceMediaStatus.Failed.ToString();
            media.FileName = "";
            return media;
        }

        public DeviceMedia UpdateMediaStatus(DeviceMedia media)
        {
            bool isNew = false;
            DeviceMedia _media;

            var qry = _repository.FilterBy(x=>x.MediaId == media.MediaId);
            if (qry.Count() > 0)
            {
                _media = qry.First();
            }
            else {
                isNew = true;
                _media = media;
            }

            _media.Status = media.Status;
            _media.Notes = media.Notes;

            if (isNew)
            {
                _repository.Add(_media);
            }
            else {
                _repository.Update(_media);
            }
            
            return _media;
        }

        public bool ExecutePowershell(int id, DeviceMediaType mediaType, out string log)
        {
            try
            {


                var shell = PowerShell.Create();
                
                var script = File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/GetDeviceMedia.ps1"));

                script = script.Replace("{mediaType}", mediaType.ToString().ToLower());
                script = script.Replace("{mediaOID}", id.ToString());
                switch (mediaType)
                {
                    case DeviceMediaType.Video:
                        script = script.Replace("{fileDirectory}", ConfigurationManager.AppSettings["mediaVideoDirectory"]);
                        break;
                    case DeviceMediaType.Image:
                        script = script.Replace("{fileDirectory}", ConfigurationManager.AppSettings["mediaImageDirectory"]);
                        break;
                    default:
                        break;
                }

                //shell.AddParameters(args);
                _logger.Debug(script);
                shell.AddScript(script);
                var results = shell.Invoke();



                var builder = new StringBuilder();

                foreach (var psObject in results)
                {
                    // Convert the Base Object to a string and append it to the string builder.
                    // Add \r\n for line breaks
                    builder.Append(psObject.BaseObject.ToString() + "\r\n");
                }

                log = builder.ToString();
                
            }
            catch (Exception ex)
            {
                
                log = ex.Message;
            }

            return log.Contains("CAPTURE_SUCCESS");
        }

        
    }
}
