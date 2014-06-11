using DieboldMobile.Infrastructure.Helpers;
using Ninject.Modules;
using Diebold.Services.Contracts;
using Diebold.Services.Impl;

namespace DieboldMobile.Infrastructure.Modules
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {

            Bind<IUserService>().To<UserService>().InRequestScope();

            //binding de un servicio abstracto a una interface template. 
            //Bind<ICRUDTrackeableService<User>>().To<BaseCRUDTrackeableService<User>>();

            /*
            Bind<ICRUDService<Role>>().To<BaseCRUDService<Role>>();
            
            // binding de un service concreto a un una interface template
            // por ej: si el servicio tiene un comportamiento no default
            // pero desde el controller se accede al comportamiento standard.
            Bind<ICRUDService<Role>>().To<RoleService>();
            */

            //binding de un servicio concreto a una interface concreta. 
            Bind<IRoleService>().To<RoleService>().InRequestScope();

            Bind<IDvrService>().To<DvrService>().InRequestScope();

            Bind<ICompanyService>().To<CompanyService>().InRequestScope();

            Bind<IGatewayService>().To<GatewayService>().InRequestScope();

            Bind<ISiteService>().To<SiteService>().InRequestScope();

            Bind<IAlertService>().To<AlertService>().InRequestScope();

            Bind<INoteService>().To<NoteService>().InRequestScope();

            //Bind<IAlertStatusService>().To<AlertStatusService>().InRequestScope();

            Bind<IAlarmConfigurationService>().To<AlarmConfigurationService>().InRequestScope();

            Bind<INotificationService>().To<NotificationService>().InRequestScope();

            Bind<ILogService>().To<LogService>().InRequestScope();

            Bind<IMembershipService>().To<MembershipService>().InRequestScope();

            Bind<IDeviceService>().To<DeviceService>().InRequestScope();

            //Alert Handler Factory.
            Bind<IAlertHandlerFactory>().To<AlertHandlerFactory>().InRequestScope();
            Bind<ILinkService>().To<LinkService>().InRequestScope();
            Bind<IRSSFeedService>().To<RSSFeedService>().InRequestScope();
            Bind<IPortletService>().To<PortletService>().InRequestScope();
            Bind<IUserPortletsPreferences>().To<UserPortletsPreferencesService>().InRequestScope();
            Bind<IRolePortletService>().To<RolePortletService>().InRequestScope();
            Bind<IUserDefaultsService>().To<UserDefaultsService>().InRequestScope();
            Bind<ISiteNoteSite>().To<SiteNoteService>().InRequestScope();
            Bind<IAccessService>().To<AccessService>().InRequestScope();
            Bind<IIntrusionService>().To<IntrusionService>().InRequestScope();
            Bind<IMonitoringService>().To<MonitoringService>().InRequestScope();
            Bind<ISystemSummaryService>().To<SystemSummaryService>().InRequestScope();
            Bind<IActionDetailsService>().To<ActionDetailsService>().InRequestScope();
            Bind<IRoleActionDetailsService>().To<RoleActionDetailsService>().InRequestScope();
            Bind<ISiteLogoDetailsService>().To<SiteLogoDetailsService>().InRequestScope();
        }
    }
}