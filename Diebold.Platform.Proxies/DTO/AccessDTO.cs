using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.DTO
{
    public class AccessDTO
    {
        public string ExternalDeviceKey { get; set; }
        public string DeviceType { get; set; }
        public string DeviceIdentifier { get; set; }
        public string CardHolderId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string CardNumber { get; set; }
        public string CardActive { get; set; }
        public string Pin { get; set; }
        public string AccessGroupId { get; set; }
        public string AccessGroup { get; set; }
        public string CardActivationDate { get; set; }
        public string cardExpirationDate { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string OfficePhone { get; set; }
        public string Extension { get; set; }
        public string MobilePhone { get; set; }
        public string ReaderId { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
        public string Day { get; set; }

        public List<AccecGroupTimePeriod> AccessGroupTimePeriods { get; set; }
        public List<AccessGroupReader> AccessGroupReaders { get; set; }
        public List<AccessDTO> AccessGroupInformation { get; set; }
        public List<Properties> AreasAuthorityLevel { get; set; }
    }
}
