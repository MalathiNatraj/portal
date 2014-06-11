using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;
using System.Reflection;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using System.Text;

namespace Diebold.WebApp.Controllers
{
    public class SystemSummaryController : BaseController
    {
        //
        // GET: /SystemSummary/
        private readonly ISystemSummaryService _systemsummaryService;
        private readonly ISiteService _siteService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IDvrService _dvrService;
        private readonly IUserService _userService;

        public SystemSummaryController(ISystemSummaryService systemSummaryService, ISiteService siteService, ICurrentUserProvider currentUserProvider,
                            IDvrService dvrService, IUserService userService)
        {
            this._systemsummaryService = systemSummaryService;
            this._siteService = siteService;
            this._currentUserProvider = currentUserProvider;
            this._dvrService = dvrService;
            this._userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }
        
        public JsonResult GetAllSystemSummaryDeviceDetails()
        {
            string ssDeviceParentType = string.Empty;
            // Bind Device Details inside Combo Box
            List<SystemSummaryDevice> objSystemSummaryDevice = new List<SystemSummaryDevice>();
            objSystemSummaryDevice = (GetAllSystemSummaryDevice()).OrderBy(x => x.Name).ToList();
            if (Session["SelectedSysSummaryDeviceParent"] != null)
            {
                ssDeviceParentType = (string)Session["SelectedSysSummaryDeviceParent"];
                return Json(objSystemSummaryDevice.Select(c => new { Id = c.Id, Name = c.Name, DefaultSelectedValue = ssDeviceParentType }), JsonRequestBehavior.AllowGet);
            }
            return Json(objSystemSummaryDevice.Select(c => new { Id = c.Id, Name = c.Name }), JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetSystemSummarybyDeviceType(string DeviceType)
        {
            List<SystemSummaryModel> lstSystemSummaryModel = new List<SystemSummaryModel>();
            try
            {
                Session["SelectedSysSummaryDeviceParent"] = DeviceType;
                // Get devices for the current user
                var SiteDetailsforCurrnetUser =  _siteService.GetSitesByUser(_currentUserProvider.CurrentUser.Id);
                var DeviceDetailsforCurrnetUser = _userService.GetDevicesByUserId(_currentUserProvider.CurrentUser.Id);
                var lstDeviceIds = new List<int>();
                List<Dvr> lstDvr = new List<Dvr>();
                StringBuilder lststrExternalDeviceIds = new StringBuilder();
                if (DeviceType == "Intrusion")
                {
                    foreach (var item in SiteDetailsforCurrnetUser)
                    {
                        lstDvr.AddRange(_dvrService.GetDevicesBySiteId(item.Id).Where(x => x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.dmpXR100) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.dmpXR500) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.bosch_D9412GV4) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.videofied01)));
                    }

                    if (DeviceDetailsforCurrnetUser != null && DeviceDetailsforCurrnetUser.Count() > 0)
                    {
                        DeviceDetailsforCurrnetUser.Where(x => x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.dmpXR100) ||
                                                          x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.dmpXR500) ||
                                                          x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.bosch_D9412GV4) ||
                                                          x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.videofied01)).ToList();
                        lstDvr.AddRange(DeviceDetailsforCurrnetUser);
                    }
                    if (lstDvr != null && lstDvr.Count() > 0)
                    {
                        for (int i = 0; i < lstDvr.Count(); i++)
                        {
                            Diebold.Domain.Entities.Device objDevice = (Diebold.Domain.Entities.Device)lstDvr[i];
                            if (i <= lstDvr.Count() - 2)
                            {
                                lststrExternalDeviceIds.Append("\"" + objDevice.ExternalDeviceId + "\", ");
                            }
                            else
                            {
                                lststrExternalDeviceIds.Append("\"" + objDevice.ExternalDeviceId + "\"");
                            }
                        }
                        // var resultSet = _systemsummaryService.GetSystemSummary("SparkIntrusion", "SparkIntrusionReport.properties.property.networkDown");
                        var resultSet = _systemsummaryService.GetSystemSummarybyDeviceId(lststrExternalDeviceIds.ToString(), "SparkIntrusionReport.properties.property.networkDown");
                        Type type = resultSet.GetType();
                        PropertyInfo[] properties = type.GetProperties();
                        if (resultSet.True != null || resultSet.False != null)
                        {
                            foreach (PropertyInfo property in properties)
                            {
                                SystemSummaryModel objSystemSummaryModel = new SystemSummaryModel();
                                if (property.Name.Equals("True"))
                                {
                                    if (resultSet.True != null)
                                    {
                                        objSystemSummaryModel.Status = "Offline (" + resultSet.True + ")";
                                        objSystemSummaryModel.Value = Convert.ToInt32(resultSet.True);
                                        objSystemSummaryModel.DisplayOrder = 3;
                                    }
                                    else
                                    {
                                        objSystemSummaryModel.Status = "Offline (" + "0" + ")";
                                        objSystemSummaryModel.Value = Convert.ToInt32("0");
                                        objSystemSummaryModel.DisplayOrder = 3;
                                    }
                                    lstSystemSummaryModel.Add(objSystemSummaryModel);
                                }
                            }
                        }
                        else
                        {
                            SystemSummaryModel objSystemSummaryModel = new SystemSummaryModel();
                            objSystemSummaryModel.Status = "Offline (" + "0" + ")";
                            objSystemSummaryModel.Value = Convert.ToInt32("0");
                            objSystemSummaryModel.DisplayOrder = 3;
                            lstSystemSummaryModel.Add(objSystemSummaryModel);
                        }
                        // Recording or not
                        // resultSet = _systemsummaryService.GetSystemSummary("SparkIntrusion", "SparkIntrusionReport.AreasStatusList.AreaStatus.properties.property.armed");
                        resultSet = _systemsummaryService.GetSystemSummarybyDeviceId(lststrExternalDeviceIds.ToString(), "SparkIntrusionReport.AreasStatusList.AreaStatus.properties.property.armed");
                        if (resultSet.True != null || resultSet.False != null)
                        {
                            foreach (PropertyInfo property in properties)
                            {
                                SystemSummaryModel objSystemSummaryModel = new SystemSummaryModel();
                                if (property.Name.Equals("True"))
                                {
                                    if (resultSet.True != null)
                                    {
                                        objSystemSummaryModel.Status = "Armed (" + resultSet.True + ")";
                                        objSystemSummaryModel.Value = Convert.ToInt32(resultSet.True);
                                        objSystemSummaryModel.DisplayOrder = 2;
                                    }
                                    else
                                    {
                                        objSystemSummaryModel.Status = "Armed (" + "0" + ")";
                                        objSystemSummaryModel.Value = Convert.ToInt32("0");
                                        objSystemSummaryModel.DisplayOrder = 2;
                                    }
                                    lstSystemSummaryModel.Add(objSystemSummaryModel);
                                }
                                else if (property.Name.Equals("False"))
                                {
                                    if (resultSet.False != null)
                                    {
                                        objSystemSummaryModel.Status = "Disarmed (" + resultSet.False + ")";
                                        objSystemSummaryModel.Value = Convert.ToInt32(resultSet.False);
                                        objSystemSummaryModel.DisplayOrder = 1;
                                    }
                                    else
                                    {
                                        objSystemSummaryModel.Status = "Disarmed (" + "0" + ")";
                                        objSystemSummaryModel.Value = Convert.ToInt32("0");
                                        objSystemSummaryModel.DisplayOrder = 1;
                                    }
                                    lstSystemSummaryModel.Add(objSystemSummaryModel);
                                }
                            }
                        }
                        else
                        {
                            SystemSummaryModel objSystemSummaryModel = new SystemSummaryModel();
                            objSystemSummaryModel.Status = "Armed (" + "0" + ")";
                            objSystemSummaryModel.Value = Convert.ToInt32("0");
                            objSystemSummaryModel.DisplayOrder = 2;
                            lstSystemSummaryModel.Add(objSystemSummaryModel);
                            SystemSummaryModel objSystemSummaryModel1 = new SystemSummaryModel();
                            objSystemSummaryModel1.Status = "Disarmed (0)";
                            objSystemSummaryModel1.Value = Convert.ToInt32("0");
                            objSystemSummaryModel.DisplayOrder = 1;
                            lstSystemSummaryModel.Add(objSystemSummaryModel1);
                        }
                    }
                    else
                    {
                        return JsonError("There are no devices associated with this user.");
                    }
                }

                else if (DeviceType == "Health")
                {
                    foreach (var item in SiteDetailsforCurrnetUser)
                    {
                        lstDvr.AddRange(_dvrService.GetDevicesBySiteId(item.Id).Where(x => x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.Costar111) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.ipConfigure530) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.VerintEdgeVr200)));
                    }
                    if (DeviceDetailsforCurrnetUser != null && DeviceDetailsforCurrnetUser.Count() > 0)
                    {
                        DeviceDetailsforCurrnetUser = DeviceDetailsforCurrnetUser.Where(x => x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.Costar111) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.ipConfigure530) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.VerintEdgeVr200)).ToList();
                        lstDvr.AddRange(DeviceDetailsforCurrnetUser);
                    }
                    if (lstDvr != null && lstDvr.Count() > 0)
                    {
                        for (int i = 0; i < lstDvr.Count(); i++)
                        {
                            Diebold.Domain.Entities.Device objDevice = (Diebold.Domain.Entities.Device)lstDvr[i];
                            if (i <= lstDvr.Count() - 2)
                            {
                                lststrExternalDeviceIds.Append("\"" + objDevice.ExternalDeviceId + "\", ");
                            }
                            else
                            {
                                lststrExternalDeviceIds.Append("\"" + objDevice.ExternalDeviceId + "\"");
                            }
                        }
                        // var resultSet = _systemsummaryService.GetSystemSummary("SparkDvr", "SparkDvrReport.properties.property.networkDown");
                        
                        var resultSet = _systemsummaryService.GetSystemSummarybyDeviceId(lststrExternalDeviceIds.ToString(), "SparkDvrReport.properties.property.networkDown");
                        Type type = resultSet.GetType();
                        PropertyInfo[] properties = type.GetProperties();
                        if (resultSet.True != null || resultSet.False != null)
                        {
                            foreach (PropertyInfo property in properties)
                            {
                                SystemSummaryModel objSystemSummaryModel = new SystemSummaryModel();
                                if (property.Name.Equals("True"))
                                {
                                    if (resultSet.True != null)
                                    {
                                        objSystemSummaryModel.Status = "Offline (" + resultSet.True + ")";
                                        objSystemSummaryModel.Value = Convert.ToInt32(resultSet.True);
                                        objSystemSummaryModel.DisplayOrder = 3;
                                    }
                                    else
                                    {
                                        objSystemSummaryModel.Status = "Offline (" + "0" + ")";
                                        objSystemSummaryModel.Value = Convert.ToInt32("0");
                                        objSystemSummaryModel.DisplayOrder = 3;
                                    }
                                    lstSystemSummaryModel.Add(objSystemSummaryModel);
                                }
                            }
                        }

                        // Recording or not
                        //  resultSet = _systemsummaryService.GetSystemSummary("SparkDvr", "SparkDvrReport.properties.property.isNotRecording");
                        int trouble = 0;
                        int ok = 0;
                        resultSet = _systemsummaryService.GetSystemSummarybyDeviceId(lststrExternalDeviceIds.ToString(), "SparkDvrReport.properties.property.isNotRecording");
                        if (resultSet.True != null || resultSet.False != null)
                        {
                            foreach (PropertyInfo property in properties)
                            {
                                SystemSummaryModel objSystemSummaryModel = new SystemSummaryModel();
                                if (property.Name.Equals("True"))
                                {
                                    if (resultSet.True != null)
                                    {
                                        trouble = Convert.ToInt32(resultSet.True);
                                    }
                                }
                                else if (property.Name.Equals("False"))
                                {
                                    if (resultSet.False != null)
                                    {
                                        ok = Convert.ToInt32(resultSet.False);

                                    }
                                }
                            }
                        }
                        resultSet = _systemsummaryService.GetSystemSummarybyDeviceId(lststrExternalDeviceIds.ToString(), "SparkDvrReport.properties.propertyList.videoLoss");                        
                        if (resultSet.True != null || resultSet.False != null)
                        {
                            foreach (PropertyInfo property in properties)
                            {
                                SystemSummaryModel objSystemSummaryModel = new SystemSummaryModel();
                                if (property.Name.Equals("True"))
                                {
                                    if (resultSet.True != null)
                                    {
                                        trouble = trouble + Convert.ToInt32(resultSet.True);
                                    }
                                }
                                else if (property.Name.Equals("False"))
                                {
                                    if (resultSet.False != null)
                                    {
                                        ok = ok + Convert.ToInt32(resultSet.False);
                                    }
                                }
                            }
                        }
                        
                        SystemSummaryModel objSystemSummaryModel2 = new SystemSummaryModel();
                        objSystemSummaryModel2.Status = "Trouble (" + trouble + ")";
                        objSystemSummaryModel2.Value = trouble;
                        objSystemSummaryModel2.DisplayOrder = 1;
                        lstSystemSummaryModel.Add(objSystemSummaryModel2);
                        SystemSummaryModel objSystemSummaryModel1 = new SystemSummaryModel();
                        objSystemSummaryModel1.Status = "OK (" + ok + ")";
                        objSystemSummaryModel1.Value = ok;
                        objSystemSummaryModel1.DisplayOrder = 2;
                        lstSystemSummaryModel.Add(objSystemSummaryModel1);
                        
                    }
                    else
                    {
                        return JsonError("There are no devices associated with this user.");
                    }

                }

                else if (DeviceType == "Access")
                {
                    int noDoorForced = 0;
                    int noDoorHeld = 0;
                    int noNetworkDown = 0;
                    int totalOk = 0;

                    int noDoorForcedtrouble = 0;
                    int noDoorHeldtrouble = 0;
                    int totaltrouble = 0;

                    int nonetworkDown = 0;

                    foreach (var item in SiteDetailsforCurrnetUser)
                    {
                        lstDvr.AddRange(_dvrService.GetDevicesBySiteId(item.Id).Where(x => x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.eData300) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.eData524) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.dmpXR100Access) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.dmpXR500Access)
                                                                                            ));
                    }

                    if (DeviceDetailsforCurrnetUser != null && DeviceDetailsforCurrnetUser.Count() > 0)
                    {
                        DeviceDetailsforCurrnetUser = DeviceDetailsforCurrnetUser.Where(x => x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.eData300) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.eData524) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.dmpXR100Access) ||
                                                                                            x.DeviceType.Equals(Diebold.Domain.Entities.DeviceType.dmpXR500Access)).ToList();
                        lstDvr.AddRange(DeviceDetailsforCurrnetUser);
                    }
                    if (lstDvr != null && lstDvr.Count() > 0)
                    {
                        for (int i = 0; i < lstDvr.Count(); i++)
                        {
                            Diebold.Domain.Entities.Device objDevice = (Diebold.Domain.Entities.Device)lstDvr[i];
                            if (i <= lstDvr.Count() - 2)
                            {
                                lststrExternalDeviceIds.Append("\"" + objDevice.ExternalDeviceId + "\", ");
                            }
                            else
                            {
                                lststrExternalDeviceIds.Append("\"" + objDevice.ExternalDeviceId + "\"");
                            }
                        }
                        // Check for Forced
                        // var resultSetdoorForced = _systemsummaryService.GetSystemSummary("SparkAccessControl", "SparkAccessControlReport.properties.property.doorForced");
                        var resultSetdoorForced = _systemsummaryService.GetSystemSummarybyDeviceId(lststrExternalDeviceIds.ToString(), "SparkAccessControlReport.properties.property.doorForced");
                        // Check for Held
                        // var resultSetdoorHeld = _systemsummaryService.GetSystemSummary("SparkAccessControl", "SparkAccessControlReport.properties.property.doorHeld");
                        var resultSetdoorHeld = _systemsummaryService.GetSystemSummarybyDeviceId(lststrExternalDeviceIds.ToString(), "SparkAccessControlReport.properties.property.doorHeld");
                        // Check for network down
                        // var resultSetnetworkDown = _systemsummaryService.GetSystemSummary("SparkAccessControl", "SparkAccessControlReport.properties.property.networkDown");
                        var resultSetnetworkDown = _systemsummaryService.GetSystemSummarybyDeviceId(lststrExternalDeviceIds.ToString(), "SparkAccessControlReport.properties.property.networkDown");
                        Type typedf = resultSetdoorForced.GetType();
                        PropertyInfo[] propertiesdf = typedf.GetProperties();
                        Type typedh = resultSetdoorHeld.GetType();
                        PropertyInfo[] propertiesdh = typedh.GetProperties();
                        Type typend = resultSetnetworkDown.GetType();
                        PropertyInfo[] propertiesnd = typend.GetProperties();
                        SystemSummaryModel objSystemSummaryModelOK = new SystemSummaryModel();
                        SystemSummaryModel objSystemSummaryModelT = new SystemSummaryModel();
                        SystemSummaryModel objSystemSummaryModelO = new SystemSummaryModel();
                        // Check for Status OK - if no forced, no held, no network down
                        if (resultSetdoorForced.True == null && resultSetdoorHeld.True == null && resultSetnetworkDown.True == null)
                        {

                            foreach (PropertyInfo property in propertiesdf)
                            {
                                noDoorForced = Convert.ToInt32(resultSetdoorForced.False);
                            }

                            foreach (PropertyInfo property in propertiesdh)
                            {
                                noDoorHeld = Convert.ToInt32(resultSetdoorHeld.False);
                            }

                            foreach (PropertyInfo property in propertiesnd)
                            {
                                noNetworkDown = Convert.ToInt32(resultSetnetworkDown.False);
                            }

                            totalOk = noDoorForced + noDoorHeld + noNetworkDown;
                            objSystemSummaryModelOK.Status = "OK (" + totalOk + ")";
                            objSystemSummaryModelOK.Value = Convert.ToInt32(totalOk);
                            objSystemSummaryModelOK.DisplayOrder = 2;
                            lstSystemSummaryModel.Add(objSystemSummaryModelOK);
                        }
                        if (resultSetdoorForced.True != null || resultSetdoorHeld.True != null)
                        {

                            foreach (PropertyInfo property in propertiesdf)
                            {
                                noDoorForcedtrouble = Convert.ToInt32(resultSetdoorForced.True);
                            }

                            foreach (PropertyInfo property in propertiesdh)
                            {
                                noDoorHeldtrouble = Convert.ToInt32(resultSetdoorHeld.True);
                            }
                            totaltrouble = noDoorForcedtrouble + noDoorHeldtrouble;
                            objSystemSummaryModelT.Status = "Trouble (" + totaltrouble + ")";
                            objSystemSummaryModelT.Value = Convert.ToInt32(totaltrouble);
                            objSystemSummaryModelT.DisplayOrder = 1;
                            lstSystemSummaryModel.Add(objSystemSummaryModelT);
                        }
                        else
                        {
                            objSystemSummaryModelT.Status = "Trouble (" + 0 + ")";
                            objSystemSummaryModelT.Value = Convert.ToInt32("0");
                            objSystemSummaryModelT.DisplayOrder = 1;
                            lstSystemSummaryModel.Add(objSystemSummaryModelT);
                        }
                        if (resultSetnetworkDown.True != null)
                        {

                            foreach (PropertyInfo property in propertiesnd)
                            {
                                nonetworkDown = Convert.ToInt32(resultSetnetworkDown.True);
                            }
                            objSystemSummaryModelO.Status = "Offline (" + nonetworkDown + ")";
                            objSystemSummaryModelO.Value = Convert.ToInt32(nonetworkDown);
                            objSystemSummaryModelO.DisplayOrder = 3;
                            lstSystemSummaryModel.Add(objSystemSummaryModelO);
                        }
                        else
                        {
                            objSystemSummaryModelO.Status = "Offline (" + 0 + ")";
                            objSystemSummaryModelO.Value = Convert.ToInt32("0");
                            objSystemSummaryModelO.DisplayOrder = 3;
                            lstSystemSummaryModel.Add(objSystemSummaryModelO);
                        }
                    }
                    else
                    {
                        return JsonError("There are no devices associated with this user.");
                    }
                }
                    
                return Json(lstSystemSummaryModel.OrderBy(x=>x.DisplayOrder));
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }


        public IList<SystemSummaryDevice> GetAllSystemSummaryDevice()
        {
            List<SystemSummaryDevice> lstSystemSummaryDevice = new List<SystemSummaryDevice> 
            {
                new SystemSummaryDevice{Id = 1, Name = "Access"},
                new SystemSummaryDevice{Id = 2, Name = "Intrusion"},
                new SystemSummaryDevice{Id = 3, Name = "Health"},
            };
            return lstSystemSummaryDevice;
        }
    }

    public class SystemSummaryDevice
    {
        public int Id { get; set; }
        public String Name { get; set; }
    }

}
