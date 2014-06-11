using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using DieboldMobile.Models;
using DieboldMobile.Infrastructure.Authentication;


namespace DieboldMobile.Controllers
{
    public class AccountController : Controller
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

                ModelState.AddModelError("", "User name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
            catch (Exception e)
            {
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
            catch
            {
                _unitOfWork.Rollback();
            }
        }

        public void LogOperation(LogAction action, User item)
        {
            _logService.Log(action, item, item.Username);
        }

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

    }
}