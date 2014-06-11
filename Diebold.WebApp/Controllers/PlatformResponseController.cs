using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.WebApp.Infrastructure.Authentication;
using System.IO;
using System.Text;
using System.Net;
using Diebold.Domain.Contracts.Infrastructure;
using System.Web.Script.Serialization;
using Diebold.Services.Impl;
using Diebold.Platform.Proxies.DTO;
using Newtonsoft.Json.Linq;

namespace Diebold.WebApp.Controllers
{
    public class PlatformResponseController : BaseAsyncController
    {
        #region Gateway Response Methods
        [AllowAnonymous]
        [HttpPost]
        public ActionResult GateWayCreateResponse()
        {
            try
            {
                logger.Debug("Gateway create response received from platform.");
                PrepareResponseObject("GWCreate");
                logger.Debug("Gateway create response object prepared.");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while creating gateway.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GateWayUpdateResponse()
        {
            try
            {
                logger.Debug("Gateway update response received from platform.");
                PrepareResponseObject("GWUpdate");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while updating gateway.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GateWayDeleteResponse()
        {
            try
            {
                logger.Debug("Gateway delete response received from platform.");
                PrepareResponseObject("GWDelete");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while deleting gateway.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GateWayRevokeResponse()
        {
            try
            {
                logger.Debug("Gateway revoke response received from platform.");
                PrepareResponseObject("GWRevoke");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while revoking gateway.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GateWayTestConnectionResponse()
        {
            try
            {
                logger.Debug("Gateway TestConnection response received from platform.");
                PrepareResponseObject("GWTestConnection");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while testing gateway.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GatewayStatusResponse()
        {
            try
            {
                logger.Debug("Gateway status response received from platform.");
                PrepareResponseObject("GWStatus");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while getting gateway status.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        
        [AllowAnonymous]
        [HttpPost]
        public ActionResult RebootGatewayResponse()
        {
            try
            {
                logger.Debug("Gateway reboot response received from platform.");
                PrepareResponseObject("RestartStatus");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while rebooting gateway.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RebootDeviceResponse()
        {
            try
            {
                logger.Debug("Device reboot response received from platform.");
                PrepareResponseObject("RestartStatus");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while rebooting device.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeviceStatusResponse()
        {
            try
            {
                logger.Debug("Device status response received from platform.");
                PrepareResponseObject("DVStatus");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while getting device status.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }
        #endregion Gateway Response Methods

        #region DEVICE RESPONSE METHODS
        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeviceCreateResponse()
        {
            try
            {
                logger.Debug("Device create response received from platform.");
                PrepareResponseObject("DVCreate");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while creating device.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeviceUpdateResponse()
        {
            try
            {
                logger.Debug("Device update response received from platform.");
                PrepareResponseObject("DVUpdate");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while updating device.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeviceDeleteResponse()
        {
            try
            {
                logger.Debug("Device delete response received from platform.");
                PrepareResponseObject("DVDelete");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while deleting device.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        #endregion DEVICE RESPONSE METHODS

        #region Access Response Methods
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CardHolderAdd()
        {
            try
            {
                logger.Debug("Card holder add response received from platform.");
                PrepareResponseObject("CardHolderAdd");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while adding Card Holder.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CardHolderDelete()
        {
            try
            {
                logger.Debug("Card holder delete response received from platform.");
                PrepareResponseObject("CardHolderDelete");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while deleting Card Holder.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CardHolderModify()
        {
            try
            {
                logger.Debug("Device create response received from platform.");
                PrepareResponseObject("CardHolderModify");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while modifying Card Holder.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetCardHoldersInformation()
        {
            try
            {
                logger.Debug("Get card holder information response received from platform.");
                PrepareResponseObject("GetCardHoldersInformation");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while getting Card Holder information.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetCardHolderList()
        {
            try
            {
                logger.Debug("Get card holder list response received from platform.");
                PrepareResponseObject("GetCardHolderList");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while getting Card Holders.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AccessGroupCreate()
        {
            try
            {
                logger.Debug("Access group create response received from platform.");
                PrepareResponseObject("AccessGroupCreate");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while crating Access Group.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AccessGroupModify()
        {
            try
            {
                logger.Debug("Access group modify response received from platform.");
                PrepareResponseObject("AccessGroupModify");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while modifying Access Group.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AccessGroupDelete()
        {
            try
            {
                logger.Debug("Access group delete response received from platform.");
                PrepareResponseObject("AccessGroupDelete");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while deleting Access Group.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetAccessGroupInformation()
        {
            try
            {
                logger.Debug("Get access group information response received from platform.");
                PrepareResponseObject("GetAccessGroupInformation");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while getting Access Group information.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AccessGetGroupList()
        {
            try
            {
                logger.Debug("Access get group list response received from platform.");
                PrepareResponseObject("AccessGetGroupList");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while getting Access Groups.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AccessMomentaryOpenDoor()
        {
            try
            {
                logger.Debug("Access momentary open door response received from platform.");
                PrepareResponseObject("AccessMomentaryOpenDoor");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while accessing momentary open door.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AccessGetReadersList()
        {
            try
            {
                logger.Debug("Access get readers list response received from platform.");
                PrepareResponseObject("AccessGetReadersList");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while getting Readers List.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AccessGetAccessControlStatus()
        {
            try
            {
                logger.Debug("Access get access control status response received from platform.");
                PrepareResponseObject("AccessGetAccessControlStatus");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                LogError("Exception occured while getting Access Control Status.", ex);
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetAccessControlReport()
        {
            try
            {
                logger.Debug("GetAccessControlReport response received from platform.");
                PrepareResponseObject("GetAccessControlReport");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        #endregion Access Response Methods

        #region Intrusion Response Methods
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AreaArm()
        {
            try
            {
                logger.Debug("AreaArm response received from platform.");
                PrepareResponseObject("AreaArm");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AreaDisarm()
        {
            try
            {
                logger.Debug("AreaDisarm response received from platform.");
                PrepareResponseObject("AreaDisarm");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetProfileNumberList()
        {
            try
            {
                logger.Debug("GetProfileNumberList response received from platform.");
                PrepareResponseObject("GetProfileNumberList");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetIntrusionReport()
        {
            try
            {
                logger.Debug("GetIntrusionReport response received from platform.");
                PrepareResponseObject("GetIntrusionReport");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetIntrusionStatus()
        {
            try
            {
                logger.Debug("GetIntrusionStatus response received from platform.");
                PrepareResponseObject("GetIntrusionStatus");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }
        
        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetPlatformIntrusionStatus()
        {
            try
            {
                logger.Debug("GetPlatformIntrusionStatus response received from platform.");
                PrepareResponseObject("GetPlatformIntrusionStatus");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetPlatformAccessStatus()
        {
            try
            {
                logger.Debug("GetPlatformAccessStatus response received from platform.");
                PrepareResponseObject("GetPlatformAccessStatus");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }
        

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetUserCodesInformation()
        {
            try
            {
                logger.Debug("GetUserCodesInformation response received from platform.");
                PrepareResponseObject("GetUserCodesInformation");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetUserCodesInformation2()
        {
            try
            {
                logger.Debug("GetUserCodesInformation response received from platform.");
                PrepareResponseObject("GetUserCodesInformation2");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetUsersCodeList()
        {
            try
            {
                logger.Debug("GetUsersCodeList response received from platform.");
                PrepareResponseObject("GetUsersCodeList");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeviceMediaCapture()
        {
            try
            {
                logger.Debug("GetUsersCodeList response received from platform.");
                PrepareResponseObject("DeviceMediaCapture");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult UserCodeAdd()
        {
            try
            {
                logger.Debug("UserCodeAdd response received from platform.");
                PrepareResponseObject("UserCodeAdd");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult UserCodeAdd2()
        {
            try
            {
                logger.Debug("UserCodeAdd response received from platform.");
                PrepareResponseObject("UserCodeAdd2");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult UserCodeDelete()
        {
            try
            {
                logger.Debug("UserCodeDelete response received from platform.");
                PrepareResponseObject("UserCodeDelete");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult UserZoneBypass()
        {
            try
            {
                logger.Debug("UserZoneBypass response received from platform.");
                PrepareResponseObject("ZoneBypass");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AccessUserZoneBypass()
        {
            try
            {
                logger.Debug("AccessUserZoneBypass response received from platform.");
                PrepareResponseObject("DVCreate");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult UserZoneResetBypass()
        {
            try
            {
                logger.Debug("UserZoneResetBypass response received from platform.");
                PrepareResponseObject("ZoneResetBypass");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }
        
        [AllowAnonymous]
        [HttpPost]
        public ActionResult RestartAgent()
        {
            try
            {
                logger.Debug("RestartAgent response received from platform.");
                PrepareResponseObject("RestartAgent");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult UserCodeModify()
        {
            try
            {
                logger.Debug("UserCodeModify response received from platform.");
                PrepareResponseObject("UserCodeModify");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult UserCodeModify2()
        {
            try
            {
                logger.Debug("UserCodeModify response received from platform.");
                PrepareResponseObject("UserCodeModify2");
                return Json("Received request successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new Exception("Invalid POST");
        }
        
        #endregion Intrusion Response Methods

        private void PrepareResponseObject(string responseType)
        {
            try
            {
                ResponseDTO objResponseDTO = new ResponseDTO();
                StatusDTO objStatusDTO = new StatusDTO();
                using (Stream receiveStream = Request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var objText = readStream.ReadToEnd();
                        objText = objText.Replace("string", "description");
                        logger.Debug("Received JSON @ Callback for " + responseType + ":" + objText);

                        switch (responseType)
                        {

                            case "GWStatus":
                                objStatusDTO = (StatusDTO)js.Deserialize(objText, typeof(StatusDTO));
                                objStatusDTO.isGateWay = "YES";
                                System.Web.HttpContext.Current.Application[responseType + objStatusDTO.device_instance_id] = objStatusDTO;
                                break;
                            case "DVStatus":
                                objStatusDTO = (StatusDTO)js.Deserialize(objText, typeof(StatusDTO));
                                objStatusDTO.isGateWay = "NO";
                                System.Web.HttpContext.Current.Application[responseType + objStatusDTO.device_instance_id] = objStatusDTO;
                                break;
                            case "CardHolderAdd":
                            case "CardHolderDelete":
                            case "CardHolderModify":                           
                            case "GetCardHolderList":
                            case "AccessGroupCreate":
                            case "AccessGroupModify":
                            case "AccessGroupDelete":                            
                            case "AccessMomentaryOpenDoor":
                            case "AccessGetReadersList":
                            case "GetAccessControlReport":
                                AccessResponseDTO objAccessResponseDTO = new AccessResponseDTO();
                                objAccessResponseDTO = (AccessResponseDTO)js.Deserialize(objText, typeof(AccessResponseDTO));
                                System.Web.HttpContext.Current.Application[responseType + objAccessResponseDTO.device_instance_id] = objAccessResponseDTO;
                                break;
                            case "GetAccessGroupInformation":
                                AccessGroupResponseDTO objAccessGroupResponseDTO = new AccessGroupResponseDTO();
                                objAccessGroupResponseDTO = (AccessGroupResponseDTO)js.Deserialize(objText, typeof(AccessGroupResponseDTO));
                                System.Web.HttpContext.Current.Application[responseType + objAccessGroupResponseDTO.device_instance_id] = objAccessGroupResponseDTO;
                                break;
                            case "GetCardHoldersInformation":
                                AccessCardHolderResponseDTO objAccessCHResponseDTO = new AccessCardHolderResponseDTO();
                                objAccessCHResponseDTO = (AccessCardHolderResponseDTO)js.Deserialize(objText, typeof(AccessCardHolderResponseDTO));
                                System.Web.HttpContext.Current.Application[responseType + objAccessCHResponseDTO.device_instance_id] = objAccessCHResponseDTO;
                                break;
                            case "AccessGetAccessControlStatus":
                                AccessStatusResponseDTO objAccessStatusResponseDTO = new AccessStatusResponseDTO();
                                objAccessStatusResponseDTO = (AccessStatusResponseDTO)js.Deserialize(objText, typeof(AccessStatusResponseDTO));
                                System.Web.HttpContext.Current.Application[responseType + objAccessStatusResponseDTO.device_instance_id] = objAccessStatusResponseDTO;
                                break;

                                case "AccessGetGroupList":
                                AccessGroupListResponseDTO objAccessGroupListResponseDTO = new AccessGroupListResponseDTO();
                                objAccessGroupListResponseDTO = (AccessGroupListResponseDTO)js.Deserialize(objText, typeof(AccessGroupListResponseDTO));
                                System.Web.HttpContext.Current.Application[responseType + objAccessGroupListResponseDTO.device_instance_id] = objAccessGroupListResponseDTO;
                                break;
                            case "AreaArm":
                            case "AreaDisarm":
                            case "GetProfileNumberList":
                            case "GetIntrusionReport":
                            case "GetIntrusionStatus":                            
                            case "GetUsersCodeList":
                            case "UserCodeAdd":
                            case "UserCodeDelete":
                            case "UserCodeModify":
                            case "ZoneBypass":
                            case "ZoneResetBypass":
                            case "RestartAgent":
                                IntrusionResponseDTO objIntrusionResponseDTO = new IntrusionResponseDTO();
                                objIntrusionResponseDTO = (IntrusionResponseDTO)js.Deserialize(objText, typeof(IntrusionResponseDTO));
                                System.Web.HttpContext.Current.Application[responseType + objIntrusionResponseDTO.device_instance_id] = objIntrusionResponseDTO;
                                break;
                            case "UserCodeAdd2":
                            case "UserCodeDelete2":
                            case "UserCodeModify2":
                            case "DeviceMediaCapture":
                                IntrusionResponseDTO objIntrusionResponse2DTO = new IntrusionResponseDTO();
                                objIntrusionResponse2DTO = (IntrusionResponseDTO)js.Deserialize(objText, typeof(IntrusionResponseDTO));
                                string session_name = responseType + objIntrusionResponse2DTO.device_instance_id;
                                System.Web.HttpContext.Current.Application[session_name] = objIntrusionResponse2DTO;
                                break;
                            case "GetUserCodesInformation":                                
                                IntrusionUserCodeResponseDTO objIntrusionUCResponseDTO = new IntrusionUserCodeResponseDTO();
                                objIntrusionUCResponseDTO = (IntrusionUserCodeResponseDTO)js.Deserialize(objText, typeof(IntrusionUserCodeResponseDTO));
                                System.Web.HttpContext.Current.Application[responseType + objIntrusionUCResponseDTO.device_instance_id] = objIntrusionUCResponseDTO;
                                break;
                            case "GetUserCodesInformation2":
                                IntrusionUserCodeResponseDTO objIntrusionResponse2DTO2 = new IntrusionUserCodeResponseDTO();
                                objIntrusionResponse2DTO2 = (IntrusionUserCodeResponseDTO)js.Deserialize(objText, typeof(IntrusionUserCodeResponseDTO));
                                string session_name2 = responseType + objIntrusionResponse2DTO2.device_instance_id;
                                System.Web.HttpContext.Current.Application[session_name2] = objIntrusionResponse2DTO2;
                               break;
                            case "GetPlatformIntrusionStatus":                            
                                IList<IntrusionPlatformResponseDTO>  objIntrusionPlatformResponseDTO = (IList<IntrusionPlatformResponseDTO>)js.Deserialize(objText, typeof(IList<IntrusionPlatformResponseDTO>));
                                System.Web.HttpContext.Current.Application[responseType + objIntrusionPlatformResponseDTO[0].device_instance_id] = objIntrusionPlatformResponseDTO;
                                break;
                            case "GetPlatformAccessStatus":
                                IList<AccessPlatformResponseDTO> objAccessPlatformResponseDTO = (IList<AccessPlatformResponseDTO>)js.Deserialize(objText, typeof(IList<AccessPlatformResponseDTO>));
                                System.Web.HttpContext.Current.Application[responseType + objAccessPlatformResponseDTO[0].device_instance_id] = objAccessPlatformResponseDTO;
                                break;
                            default:
                                objResponseDTO = (ResponseDTO)js.Deserialize(objText, typeof(ResponseDTO));
                                System.Web.HttpContext.Current.Application[responseType + objResponseDTO.device_instance_id] = objResponseDTO;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Exception occured @ response callback:-->" +ex);
                throw ex;
            }
        }
    }
}
