using System.Collections.Generic;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Platform.Proxies.Contracts
{
    public interface IAccessApiService
    {
        string CardHolderAdd(AccessDTO objAccessDTO);
        string CardHolderDelete(AccessDTO objAccessDTO);
        string CardHolderModify(AccessDTO objAccessDTO);
        string GetCardHoldersInformation(AccessDTO objAccessDTO);
        string GetCardHolderList(AccessDTO objAccessDTO);
        string AccessGroupCreate(AccessDTO objAccessDTO);
        string AccessGroupModify(AccessDTO objAccessDTO);
        string AccessGroupDelete(AccessDTO objAccessDTO);
        string GetAccessGroupInformation(AccessDTO objAccessDTO);
        string AccessGetGroupList(AccessDTO objAccessDTO);
        string AccessMomentaryOpenDoor(AccessDTO objAccessDTO);
        string AccessGetReadersList(AccessDTO objAccessDTO);
        string AccessGetAccessControlStatus(AccessDTO objAccessDTO);
        string GetPlatformAccessStatus(AccessDTO objAccessDTO);
        string AccessGetAccessControlReport(AccessDTO objAccessDTO);
    }
}
