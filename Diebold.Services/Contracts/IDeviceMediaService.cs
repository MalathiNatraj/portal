using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Web;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Services.Contracts;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Contracts
{
    public interface IDeviceMediaService
    {
        DeviceMedia CaptureImage(int deviceId, string zoneNumber);

        DeviceMedia CaptureVideo(int deviceId, string zoneNumber);

        DeviceMedia UpdateMediaStatus(DeviceMedia media);

        bool ExecutePowershell(int id, DeviceMediaType mediaType, out string log);

        string CaptureVideoMASId(int cnxId, string serverId);

        DeviceMedia GetVideoMAS(int cnxId, string serverId);
    }
}