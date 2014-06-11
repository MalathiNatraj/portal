using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class MasterRoomModel
    {
        public int Id { get; set; }
        public String Area { get; set; }
        public String Status { get; set; }
        public String Online { get; set; }
        public bool Arm { get; set; }
        public int DeviceList_Id { get; set; }
        public string Enumvalue { get; set; }
    }

    public class DetailRowModel
    {
        public int AreaId { get; set; }
        public int Id { get; set; }
        public String Zone { get; set; }
        public bool ByPass { get; set; }
        public int MasterRoom_Id { get; set; }
    }
    public class PollingStatusModel
    {
        public int DeviceId { get; set; }
        public string Enumvalue { get; set; }
        public String PollingStatus { get; set; }
        public String Status { get; set; }
    }
    
    public class BaseMasterRoomModel
    {
        public IList<MasterRoomModel> MasterRoomModels { get; set; }
        public IList<DetailRowModel> DetailRowModels { get; set; }
    }
}