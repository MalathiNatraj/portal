using System;
using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Services.Impl;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Services.Contracts
{
    public interface IIntrusionService 
    {
        IList<ProfileNumberListModel> GetProfileNumberList(int deviceId);
        Intrusion GetIntrusionReport(Intrusion Item);         
        Intrusion GetIntrusionDetails(int deviceId);
        Intrusion GetPlatformIntrusionDetails(int deviceId);
        Intrusion GetUserCodesInformation(Intrusion Item);
        int MediaCapture(Intrusion Item, DeviceMediaType deviceMediaType);
        string UserCodeAdd(Intrusion Item);
        string UserCodeModify(Intrusion Item);
        string UserCodeDelete(Intrusion Item);
        string AreaArm(Intrusion Item);
        string AreaDisArm(Intrusion Item);
        string ZoneResetByPass(Intrusion Item);
        string ZoneByPass(Intrusion Item);             
    }
}