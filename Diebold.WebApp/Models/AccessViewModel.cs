using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Diebold.Domain.Entities;
using AutoMapper;
using System.ComponentModel;

namespace Diebold.WebApp.Models
{
    public class AccessViewModel
    {
        public AccessViewModel() { }
        public List<DoorModel> DoorModelList { get; set; }
        public string PollingStatus { get; set; }
        public string Status { get; set; }
        public string DoorName { get; set; }
        public int Online { get; set; }
        public string DoorStatus { get; set; }
        public string MomentaryUnlock { get; set; }
        public int DeviceId { get; set; }
        public List<AccessReportModel> AccReportModelList { get; set; }
        public List<AccessCardHolderModel> CardHolderModelList { get; set; }
        public IList<dmpXRAccessGroupModel> AccessGroupList { get; set; }
    }
    public class dmpXRAccessGroupModel
    {
        public String Id { get; set; }
        public String Name { get; set; }
    }

    public class DoorModel
    {

        public string DoorName { get; set; }
        public string Status { get; set; }
        public string DoorStatus { get; set; }
        public string MomentaryUnlock { get; set; }

    }
    public class AccessReportModel
    {
        public string Acctype { get; set; }
        public string Accdatetime { get; set; }
        public string Accuser { get; set; }
        public string Accmessage { get; set; }
    }
    public class AccessCardHolderModel
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
        public string company { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string OfficePhone { get; set; }
        public string Extension { get; set; }
        public string MobilePhone { get; set; }
    }
    public class AccessCardholderViewModel
    {
        public string CardHolderId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string cardNumber { get; set; }

        //public string Zip { get; set; }
        public DeviceType DeviceType { get; set; }
        public string DeviceName { get; set; }
        public int DeviceId { get; set; }
        //public List<AccessGroupModel> AccessGroups { get; set; }
        // public KeyValuePair<string, int> AccessLevels { get; set; }
        public string tempDate { get; set; }
        public string readerName { get; set; }
        public List<dmpXRAccessGroupModel> AccessGroupList { get; set; }

        public AccessCardholderViewModel()
        {
            this.AccessGroupList = new List<dmpXRAccessGroupModel>();

        }

    }
}