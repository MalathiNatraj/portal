using System.Web.Mvc;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Contracts;

namespace DieboldMobile.Controllers
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
            if (currentUserProvider.UsernameExists && _userService.UserIsEnabled(currentUserProvider.CurrentUser.Username))
            {
                ViewBag.Message = "You are currently logged in as " + currentUserProvider.CurrentUser.FirstName + " " + currentUserProvider.CurrentUser.LastName+", Diebold";
                ViewBag.UserNameExists = true;
            }
            else
            {
                ViewBag.Message = "There's no user with your username on this application";
                ViewBag.UserNameExists = false;
            }
            return RedirectToAction("Home", "Dashboard");
        }

        public ActionResult About()
        {
            return View();
        }

    }
}
