using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diebold.WebApp.Models
{
    public class LiveViewModel
    {
        public String LiveViewId { get; set; }        
        public String Name { get; set; }
        public String IPConfugureManagementUri { get; set; }
        public IList<ServerModel> ServerList { get; set; }
        public IList<CameraModel> CameraList { get; set; }        
    }

    public class ServerModel
    {             
        public int ServerID { get; set; }
        public String ServerName { get; set; }
        public String RootUri { get; set; }
        public bool PTZ { get; set; }
        public int FPS { get; set; }

    }

    public class CameraModel
    {        
        public int CameraID { get; set; }
        public String CameraName { get; set; }
        public Int32 ServerID { get; set; }
        public String Name { get; set; }
        public String RootUri { get; set; }
        public bool PTZ { get; set; }
        public int FPS { get; set; }
        public String LocationName { get; set; }

    }
}