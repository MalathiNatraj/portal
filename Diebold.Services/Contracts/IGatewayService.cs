using System;
using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;

namespace Diebold.Services.Contracts
{
    public interface IGatewayService : ICRUDTrackeableService<Gateway>, IDeviceService
    {
        IList<Protocol> GetProtocols();

        IList<Gateway> GetAll(int pageNumber, int pageSize, string sortBy, bool ascending, out int recordCount, string whereCondition);

        Page<Gateway> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition, string status = null);

        //IList<string> GetEnabledMacAddress(int pageNumber, int pageSize, string sortBy, bool ascending, out int recordCount);

        IList<string> GetEnabledMacAddress();

        //IList<Gateway> GetGatewaysBySiteId(int siteId, bool showDisabled);

        //Site GetSiteByGatewayId(int gatewayId);
        
        void RevokeCerificate(int pk);

        IList<Gateway> GetAllByStatus(string status = null);

        //void Restart(int deviceId);

        //void Reload(int deviceId);

        //void TestConnection(int deviceId);

        //IList<DeviceStatus> GetLiveStatus(int deviceId, bool liveFromDevice);
        
        //void Audit(int id);

        IList<Gateway> GetAllActiveGateway();

        IList<Gateway> GetGatewaysByCompanyId(int companyId, bool showDisabled);
        int GetDeviceCountByGatewayId(int gatewayId);
    }
}