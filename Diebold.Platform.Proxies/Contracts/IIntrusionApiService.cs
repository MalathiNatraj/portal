using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Platform.Proxies.Contracts
{
    public interface IIntrusionApiService
    {
        string AreaArm(IntrusionDTO objAccess);
        string AreaDisarm(IntrusionDTO objAccess);
        string GetProfileNumberList(IntrusionDTO objAccess);
        string GetIntrusionReport(IntrusionDTO objAccess);
        string GetIntrusionStatus(IntrusionDTO objAccess);
        string GetPlatformIntrusionStatus(IntrusionDTO objAccess);
        string GetUserCodesInformation(IntrusionDTO objAccess);
        string GetUsersCodeList(IntrusionDTO objAccess);
        string UserCodeAdd(IntrusionDTO objAccess);
        string UserCodeDelete(IntrusionDTO objAccess);
        string UserCodeModify(IntrusionDTO objAccess);
        string UserZoneBypass(IntrusionDTO objAccess);
        string UserZoneResetBypass(IntrusionDTO objAccess);
        string RestartAgent(IntrusionDTO objAccess);
        string CaptureMedia(IntrusionDTO objAccess, DeviceMediaType deviceMediaType);
    }
    public interface ISparkDeviceIntrusionService
    {
        string AreaArm(IntrusionDTO objAccess);
        string AreaDisarm(IntrusionDTO objAccess);
        string GetProfileNumberList(IntrusionDTO objAccess);
        string GetIntrusionReport(IntrusionDTO objAccess);
        string GetIntrusionStatus(IntrusionDTO objAccess);
        string GetPlatformIntrusionStatus(IntrusionDTO objAccess);
        string GetUserCodesInformation(IntrusionDTO objAccess);
        string GetUsersCodeList(IntrusionDTO objAccess);
        string UserCodeAdd(IntrusionDTO objAccess);
        string UserCodeDelete(IntrusionDTO objAccess);
        string UserCodeModify(IntrusionDTO objAccess);
    }
}
