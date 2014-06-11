using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Diebold.Domain.Entities;
using AutoMapper;
using System.ComponentModel;

namespace DieboldMobile.Controllers
{
    public class AccessViewModel 
    {
        public AccessViewModel() {}
        public List<DoorModel> DoorModelList { get; set; }
        public string PollingStatus { get; set; }
        public string Status { get; set; }
        public string DoorName { get; set; }
        public int Online { get; set; }
        public string DoorStatus { get; set; }
        public string MomentaryUnlock { get; set; }
        public int DeviceId { get; set; }
    }

    public class DoorModel
    {

        public string DoorName { get; set; }
        public string Status { get; set; }
        public string DoorStatus { get; set; }
        public string MomentaryUnlock { get; set; }
    
    }
}