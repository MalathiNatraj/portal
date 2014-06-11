using System;
using System.Web.Mvc;
using System.Web.Routing;
//using Diebold.WebApp.Infrastructure.Binders;
using Diebold.WebApp.Infrastructure.Binders;
using Diebold.WebApp.Models;
using Ninject.Web.Mvc;
using Ninject;
using Diebold.WebApp.Infrastructure.DebugHelp;
using log4net.Config;
using System.Web.Security;
using Diebold.WebApp.Infrastructure.Authentication;
using Diebold.WebApp.Infrastructure.Helpers;
using Diebold.Services.Contracts;
using Diebold.WebApp.Infrastructure.Logging;

namespace Diebold.WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : NinjectHttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new LogonAuthorizeFilter());
            filters.Add(new SSOAuthorizeAttribute());
            filters.Add(new HandleErrorWithElmahAttribute());
            filters.Add(new HandleErrorAttribute());

        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected override IKernel CreateKernel()
        {
            var settings = new NinjectSettings { LoadExtensions = false };

            var kernel = new StandardKernel( settings );
            
            //kernel.Load(Assembly.GetExecutingAssembly());
            //kernel.Load(AppDomain.CurrentDomain.GetAssemblies());
            kernel.Load("*.dll");

            //kernel.Inject(System.Web.Security.Membership.Provider);
            //Membership. Provider = kernel.Get<MembershipProvider>();

            //El Membereship se inicializa antes de que se haga la creacion del kernel. 
            //No se le puede inyectar un Servicio con Binding por Constructor
            //Tampoco conviene setearselo con un attribute de Inject porque
            //le pone uno y deja siempre el mismo (usa la misma NH session siempre y no se reflejan
            //los cambios). 
            //Usando un provider como este, no acoplo el provider a NInject y obtengo un Service
            //'Fresh' cada vez q lo necesito (lazy).
            //Reuso el UserService durante tod o el request para optimizar.
            (Membership.Provider as DieboldMembershipProvider).UserServiceProvider = () =>
                {
                    return kernel.Get<IUserService>();
                };

            return kernel;
        }

        protected override void OnApplicationStarted()
        {
            // Clears all previously registered view engines.
            ViewEngines.Engines.Clear();

            // Registers our Razor C# specific view engine.
            // This can also be registered using dependency injection through the new IDependencyResolver interface.
            ViewEngines.Engines.Add(new RazorViewEngine());
            
            XmlConfigurator.Configure();
            Md5Generator.path = Server.MapPath("/");
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof(NotificationViewModel), new NotificationModelBinder());

            #if DEBUG
            //Console.SetOut(new ConsoleRedirectWriter());
            #endif

            
        }
    }
}