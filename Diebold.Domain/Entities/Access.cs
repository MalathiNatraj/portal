using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Domain.Entities
{
    public class Access 
    {
        public Access(){}

        public virtual int DeviceId { get; set; }
        public virtual string PollingStatus { get; set; }
        public virtual string Status { get; set; }
        public virtual string DoorName { get; set; }
        public virtual string Online { get; set; }
        public virtual string DoorStatus { get; set; }
        public virtual string MomentaryUnlock { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string cardNumber { get; set; }
        public string pin { get; set; }
        public string accessGroupId { get; set; }
        public string cardActivationDate { get; set; }
        public string cardExpirationDate { get; set; }
        public string isActive { get; set; }
        public string CardHolderId { get; set; }
        public string middleName { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string OfficePhone { get; set; }
        public string Extension { get; set; }
        public string MobilePhone { get; set; }
        public string startDateTime { get; set; }
        public string endDateTime { get; set; }

        //Access Group Add fields
        public string Name { get; set; }
        public string Description { get; set; }
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
        public string Day { get; set; }
        public string Reader { get; set; }
        public string ReaderName { get; set; }


        public List<Door> DoorList { get; set; }
        public List<DeviceProperty> Properties { get; set; }
        public IList<AccessGroupList> AccessGroupList { get; set; }
        public List<AccReportList> ReportList { get; set; }
        public List<AccGrpList> AccessGroupInformation { get; set; }
        public List<AccCHList> AccessCardHolderList { get; set; }
    }
    public class AccessGroupList
    {
        public String AccessGroupValue { get; set; }
    }
    public class AccReportList
    {
        public string Acctype { get; set; }
        public string Accdatetime { get; set; }
        public string Accuser { get; set; }
        public string Accmessage { get; set; }
    }
    public class AccGrpList
    {
        public string beginTime { get; set; }
        public string endTime { get; set; }
        public string days { get; set; }
        public string readerId { get; set; }
        public string readerName { get; set; }
        public string description { get; set; }
    }
    public class AccCHList
    {
        public string CardHolderId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public string cardNumber { get; set; }
        public string pin { get; set; }
        public string cardActivationDate { get; set; }
        public string cardExpirationDate { get; set; }
        public string isActive { get; set; }
        public string accessGroupId { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string OfficePhone { get; set; }
        public string Extension { get; set; }
        public string MobilePhone { get; set; }
    }
}
