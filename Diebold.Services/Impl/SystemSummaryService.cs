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
using System.Web.Script.Serialization;

namespace Diebold.Services.Impl
{
    public class SystemSummaryService : ISystemSummaryService
    {
        private readonly ISystemSummaryAPIService _systemsummaryAPIService;

        public SystemSummaryService(ISystemSummaryAPIService systemsummaryAPIService)
        {
            _systemsummaryAPIService = systemsummaryAPIService;
        }

        public SystemSummaryResponseDTO GetSystemSummary(string strDeviceType, string strsummaryField)
        {
            SystemSummaryResponseDTO objSystemSummaryResponseDTO = new SystemSummaryResponseDTO();
            string systemsummaryInput = string.Empty;
            StringBuilder sbSystemSummaryInput = new StringBuilder();
            SystemSummaryResponseDTO response = null;
            sbSystemSummaryInput.Append("{  \"device_type\" : \"" + strDeviceType + "\" , ");
            sbSystemSummaryInput.Append("\"summary_field\" : \"" + strsummaryField + "\" }");
            try
            {
                string strResponseString = _systemsummaryAPIService.GetSystemSummaryAPI(sbSystemSummaryInput.ToString());
                if (string.IsNullOrEmpty(strResponseString) == false)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    response = (SystemSummaryResponseDTO)js.Deserialize<SystemSummaryResponseDTO>(strResponseString);
                    objSystemSummaryResponseDTO.True = response.True;
                    objSystemSummaryResponseDTO.False = response.False;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objSystemSummaryResponseDTO;
        }

        public SystemSummaryResponseDTO GetSystemSummarybyDeviceId(string strDeviceIds, string strsummaryField)
        {
            SystemSummaryResponseDTO objSystemSummaryResponseDTO = new SystemSummaryResponseDTO();
            string systemsummaryInput = string.Empty;
            StringBuilder sbSystemSummaryInput = new StringBuilder();
            SystemSummaryResponseDTO response = null;
            sbSystemSummaryInput.Append("{  \"device_instance_ids\" : [" + strDeviceIds + "] , ");
            sbSystemSummaryInput.Append("\"summary_field\" : \"" + strsummaryField + "\" }");
            try
            {
                string strResponseString = _systemsummaryAPIService.GetSystemSummaryAPI(sbSystemSummaryInput.ToString());
                if (string.IsNullOrEmpty(strResponseString) == false)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    response = (SystemSummaryResponseDTO)js.Deserialize<SystemSummaryResponseDTO>(strResponseString);
                    objSystemSummaryResponseDTO.True = response.True;
                    objSystemSummaryResponseDTO.False = response.False;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objSystemSummaryResponseDTO;
        }

    }
}
