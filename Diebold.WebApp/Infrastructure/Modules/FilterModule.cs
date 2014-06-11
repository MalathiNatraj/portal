using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.WebApp.Controllers;
using Diebold.WebApp.Infrastructure.Authentication;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Web.Mvc.FilterBindingSyntax;
using Action = Diebold.Domain.Entities.Action;

namespace Diebold.WebApp.Infrastructure.Modules
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

            IDictionary<Type, Domain.Entities.Action> security = new Dictionary<Type, Action>()
            {
                { typeof(UserController), Domain.Entities.Action.ManageUsers },    
                { typeof(RoleController), Domain.Entities.Action.ManageRoles },
                { typeof(MonitorController), Domain.Entities.Action.ViewMonitoring },
                { typeof(DeviceController), Domain.Entities.Action.ManageDevices },
                { typeof(GatewayController), Domain.Entities.Action.ManageGateways },
                { typeof(CompanyController), Domain.Entities.Action.ManageCompanies },
                { typeof(SiteController), Domain.Entities.Action.ManageSites },
                { typeof(LogHistoryController), Domain.Entities.Action.ViewLogHistory },
                { typeof(DashboardController), Domain.Entities.Action.ViewDashboard },
                { typeof(VideoController), Domain.Entities.Action.ViewVideo },
                { typeof(AlarmController), Domain.Entities.Action.ManageAlarms },
                { typeof(ReportingController), Domain.Entities.Action.ViewReports },
                { typeof(DiagnosticController), Domain.Entities.Action.ViewDiagnostics }
                
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
