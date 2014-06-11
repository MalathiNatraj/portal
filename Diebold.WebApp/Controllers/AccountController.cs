using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.WebApp.Models;
using Diebold.WebApp.Infrastructure.Authentication;
using log4net;
namespace Diebold.WebApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMembershipService _membershipService;
        private readonly ILogService _logService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public AccountController(IUnitOfWork unitOfWork, IMembershipService membershipService, ILogService logService, ICurrentUserProvider currentUserProvider)
        {
            _unitOfWork = unitOfWork;
            _membershipService = membershipService;
            _logService = logService;
            _currentUserProvider = currentUserProvider;
        }

        //
        // GET: /Account/LogOn

        [AllowAnonymous]
        public ActionResult LogOn()
        {
            //return View();
            LogOnModel objLogOnModel = new LogOnModel();
            objLogOnModel.DefaultUserName = "User Id";
            objLogOnModel.DefaultPassword = "txtPassword";
            return View(objLogOnModel);
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            logger.Debug("Entered Log On");
            try
            {
                if (ModelState.IsValid)
                {
                    if (Membership.ValidateUser(model.UserName, model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                        //Update model info
                        SignIn(model.UserName);

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    logger.Debug("Membership Invalid UserName in Account Controller" + model.UserName);
                    return RedirectToAction("AccessDenied", "AccessDenied");
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception ex)
            {
                logger.Debug("Exception occoured while LogOn operation in Account Controller");
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    logger.Debug("Exception occoured while LogOn operation in Account Controller" + ex.Message);
                }
                return RedirectToAction("AccessDenied", "AccessDenied");
            }
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            //Update model info
            SignOut();

            FormsAuthentication.SignOut();
            Session.Abandon();

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var formsAuthCookie = new HttpCookie(FormsAuthentication.FormsCookieName) { Expires = DateTime.Now.AddDays(-1d) };
                Response.Cookies.Add(formsAuthCookie);
            }
            
            return RedirectToAction("Index", "Home");
        }

        // This is not very nice... unit of work should be committed in services,
        // but if log have commits inside of it, couldn't be used from another services.

        private void SignIn(string userName)
        {
            try
            {
                var user = _membershipService.GetUserByUserName(userName);
                LogOperation(LogAction.SignIn, user);

                _unitOfWork.Commit();
            }
            catch(Exception e)
            {
                LogError("Exception occured while sign in" ,e);
                _unitOfWork.Rollback();
            }
        }

        private void SignOut()
        {
            try
            {
                LogOperation(LogAction.SignOut, _currentUserProvider.CurrentUser);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                LogError("Exception occured while sign out", e);
                _unitOfWork.Rollback();
            }
        }

        public void LogOperation(LogAction action, User item)
        {
            _logService.Log(action, item, item.Username);
        }

        /*
        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false // createPersistentCookie );
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true //userIsOnline );
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
        
        

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }       
        
        #endregion
        */

        //public ActionResult Login()
        //{
        //    LogOnModel objLogOnModel = new LogOnModel();
        //    objLogOnModel.DefaultUserName = "User Id";
        //    objLogOnModel.DefaultPassword = "txtPassword";
        //    return View(objLogOnModel);
        //}

        public ActionResult forgotPwd()
        {
            LogOnModel objLogOnModel = new LogOnModel();
            objLogOnModel.Message = "Password has been sent to your E-Mail id.";
            return View("Index", objLogOnModel);
        }

        public ActionResult forgotUsername()
        {
            LogOnModel objLogOnModel = new LogOnModel();
            objLogOnModel.Message = "User Id has been sent to your E-Mail id.";
            return View("Index", objLogOnModel);
        }

        public ActionResult DisallowName(string UserName)
        {
            if (UserName != "User Id")
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json("User Name is required.", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisallowPassword(string Password)
        {
            if (Password != "txtPassword")
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json("Password is required.", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ForgotUserId()
        {
            LogOnModel objLogOnModel = new LogOnModel();
            objLogOnModel.DefaultUserName = "User Id";
            objLogOnModel.DefaultPassword = "txtPassword";
            return View(objLogOnModel);
        }

        //public ActionResult LogOn(FormCollection collection)
        //{
        //    return RedirectToAction("Profile/Display");
        //    //return View();
        //} 

    }
}
