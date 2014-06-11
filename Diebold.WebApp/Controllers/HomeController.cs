using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Contracts;
using System;

namespace Diebold.WebApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IUserService _userService;

        public HomeController(ICurrentUserProvider currentUserProvider, IUserService userService)
        {
            this.currentUserProvider = currentUserProvider;
            this._userService = userService;
        }

        public ActionResult Index()
        {
            try
            {
                var CheckForUserExists = _userService.GetUserByName(currentUserProvider.CurrentUser.Username);
                if (CheckForUserExists != null)
                {
                    if (currentUserProvider.UsernameExists && _userService.UserIsEnabled(currentUserProvider.CurrentUser.Username))
                    {
                        ViewBag.Message = "Hello " + currentUserProvider.CurrentUser.FirstName + " " + currentUserProvider.CurrentUser.LastName;
                        ViewBag.UserNameExists = true;
                    }
                    else
                    {
                        ViewBag.Message = "There's no user with your username on this application";
                        ViewBag.UserNameExists = false;
                    }
                    return RedirectToAction("Home", "Dashboard");
                }
                else
                {

                    logger.Debug("User doesn't exists: " + CheckForUserExists);
                    return RedirectToAction("AccessDenied", "AccessDenied");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != null && ex.Message.Equals("Unknown User"))
                {
                    logger.Debug("Exception occoured while LogOn operation in Home Controller. ");
                    if (!string.IsNullOrEmpty(ex.Message))
                    {
                        logger.Debug("Exception occoured while LogOn operation in Home Controller." + ex.Message);
                    }
                    return RedirectToAction("AccessDenied", "AccessDenied");
                }
            }
            return RedirectToAction("Home", "Dashboard");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
