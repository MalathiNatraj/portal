using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface IAccessService 
    {
        Access GetAccessDetails(int deviceId);
        IList<AccessGroupList> AccessGetGroupList(int deviceId);
        string CardHolderAdd(Access Item);
        Access GetCardHoldersInformation(Access Item);
        Access GetAccessControlReport(Access Item); 
        string CardHolderModify(Access Item);
        string CardHolderDelete(Access Item);
        string AccessGroupCreate(Access Item);
        string AccessGroupModify(Access Item);
        string AccessGroupDelete(Access Item);
        IList<DeviceProperty> GetReadersList(int deviceId);
        string AccessMomentaryOpenDoor(Access Item);
        Access GetAccessGroupInformation(Access Item);
        Access GetPlatformAccessStatus(int deviceId);
        string dmpXRAccessGroupCreate(Access Item);
        string dmpXRAccessGroupModify(Access Item);
    }
}

