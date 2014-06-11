using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DieboldMobile.Controllers;
using DieboldMobile.Infrastructure.Authentication;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Web.Mvc.FilterBindingSyntax;
using Action = Diebold.Domain.Entities.Action;

namespace DieboldMobile.Infrastructure.Modules
{
    public class FilterModule : NinjectModule
    {
        public override void Load()
        {
            /*
            this.BindFilter<Diebold.WebApp.Infrastructure.Authentication.RoleActionAuthorize>(FilterScope.Controller, 0)
                .When((context, desc) => desc.ControllerDescriptor.ControllerType == typeof (RoleController))
                //.WhenControllerType(typeof(RoleController))
                .WithConstructorArgument("roleAction", Diebold.Domain.Entities.Action.ManageRoles);
                
            
            this.BindFilter<Diebold.WebApp.Infrastructure.Authentication.RoleActionAuthorize>(FilterScope.Controller, 1)
                .When((context, desc) => desc.ControllerDescriptor.ControllerType == typeof(UserController))
                //.WhenControllerType<UserController>()
                .WithConstructorArgument("roleAction", Diebold.Domain.Entities.Action.ManageUsers);
            */

            IDictionary<Type, Diebold.Domain.Entities.Action> security = new Dictionary<Type, Action>()
            {
                { typeof(UserController), Diebold.Domain.Entities.Action.ManageUsers },    
                { typeof(RoleController), Diebold.Domain.Entities.Action.ManageRoles },
                { typeof(MonitorController), Diebold.Domain.Entities.Action.ManageViews },
                //{ typeof(DeviceController), Diebold.Domain.Entities.Action.ManageDevices },
                { typeof(GatewayController), Diebold.Domain.Entities.Action.ManageGateways },
                //{ typeof(CompanyController), Diebold.Domain.Entities.Action.ManageCompanies },
                //{ typeof(SiteController), Diebold.Domain.Entities.Action.ManageSites },
                //{ typeof(LogHistoryController), Diebold.Domain.Entities.Action.ViewLogHistory },
                { typeof(DashboardController), Diebold.Domain.Entities.Action.ViewDashboard },
                //{ typeof(VideoController), Diebold.Domain.Entities.Action.ViewVideo },
                //{ typeof(AlarmController), Diebold.Domain.Entities.Action.ManageAlarms },
                //{ typeof(ReportingController), Diebold.Domain.Entities.Action.ViewReports },
                //{ typeof(DiagnosticController), Diebold.Domain.Entities.Action.ViewDiagnostics }
                
            };

            Func<ControllerContext, ActionDescriptor, bool> needsAuthorizationFilter = (c, a) =>
            {
                return security.ContainsKey(c.Controller.GetType());
            };

            Func<IContext, ControllerContext, ActionDescriptor, object> roleActionNeededByController = (con, c, a) =>
            {
                return security[c.Controller.GetType()];
            };

            this.BindFilter<RoleActionAuthorize>(FilterScope.Controller, 0)
                .When(needsAuthorizationFilter)
                .WithConstructorArgument("roleAction", roleActionNeededByController);

            /*
            this.BindFilter<Diebold.WebApp.Infrastructure.Authentication.RoleActionAuthorize>(FilterScope.Controller, 0)
                .WhenControllerHas<RoleActionAuthorizeAttribute>()
                .WithConstructorArgument("roleAction", roleActionNeededByController);
            */
        }
    }
}