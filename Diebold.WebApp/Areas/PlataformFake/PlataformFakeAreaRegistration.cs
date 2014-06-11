using System.Web.Mvc;

namespace Diebold.WebApp.Areas.PlataformFake
{
    public class PlataformFakeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PlataformFake";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PlataformFake_default",
                "PlataformFake/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
