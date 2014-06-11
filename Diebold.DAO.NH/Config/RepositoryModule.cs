using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Diebold.Domain.Contracts;
using Diebold.Domain;
using Diebold.DAO.NH.Repositories;
using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Config
{
    public class RepositoryModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRoleRepository>().To<RoleRepository>()
                .InRequestScope();

            Bind<IUserRepository>().To<UserRepository>()
                .InRequestScope();

            Bind<ISiteInventoryRepository>().To<SiteInventoryRepository>()
               .InRequestScope();

            Bind<IActionDetailsRepository>().To<ActionDetailsRepository>()
          .InRequestScope();

            Bind<IIntKeyedRepository<Device>>().To<BaseIntKeyedRepository<Device>>()
                .InRequestScope();

            Bind<IIntKeyedRepository<Dvr>>().To<BaseIntKeyedRepository<Dvr>>()
                .InRequestScope();
            
            Bind<IIntKeyedRepository<Gateway>>().To<BaseIntKeyedRepository<Gateway>>()
               .InRequestScope();

            Bind<IIntKeyedRepository<Company>>().To<BaseIntKeyedRepository<Company>>()
               .InRequestScope();

            Bind<ISiteRepository>().To<SiteRepository>()
               .InRequestScope();
            
            Bind<IUserMonitorGroupRepository>().To<UserMonitorGroupRepository>()
                .InRequestScope();

            Bind<IUserDeviceMonitorRepository>().To<UserDeviceMonitorRepository>()
                .InRequestScope();

            Bind<IIntKeyedRepository<CompanyGrouping1Level>>().To<BaseIntKeyedRepository<CompanyGrouping1Level>>()
                .InRequestScope();

            Bind<IIntKeyedRepository<Note>>().To<BaseIntKeyedRepository<Note>>()
                .InRequestScope();

            Bind<IReadOnlyRepository<CompanyGrouping2Level>>().To<BaseReadOnlyRepository<CompanyGrouping2Level>>()
                .InRequestScope();

            Bind<IReadOnlyRepository<Camera>>().To<BaseReadOnlyRepository<Camera>>()
               .InRequestScope();

            Bind<IAlertStatusRepository>().To<AlertStatusRepository>()
                .InRequestScope();

            Bind<IIntKeyedRepository<AlarmConfiguration>>().To<BaseIntKeyedRepository<AlarmConfiguration>>()
                .InRequestScope();

            Bind<IIntKeyedRepository<ResolvedAlert>>().To<BaseIntKeyedRepository<ResolvedAlert>>()
                .InRequestScope();
            
            Bind<IAlertInfoRepository>().To<AlertInfoRepository>()
              .InRequestScope();

            Bind<IIntKeyedRepository<HistoryLog>>().To<BaseIntKeyedRepository<HistoryLog>>()
              .InRequestScope();

            Bind<IIntKeyedRepository<Link>>().To<BaseIntKeyedRepository<Link>>()
                .InRequestScope();

            Bind<IIntKeyedRepository<RSSFeed>>().To<BaseIntKeyedRepository<RSSFeed>>()
                .InRequestScope();

            Bind<IIntKeyedRepository<Portlets>>().To<BaseIntKeyedRepository<Portlets>>()
               .InRequestScope();

            Bind<IIntKeyedRepository<RolePortlets>>().To<BaseIntKeyedRepository<RolePortlets>>()
                .InRequestScope();

            Bind<IIntKeyedRepository<RolePageActions>>().To<BaseIntKeyedRepository<RolePageActions>>()
               .InRequestScope();

            Bind<IIntKeyedRepository<UserPortletsPreferences>>().To<BaseIntKeyedRepository<UserPortletsPreferences>>()
             .InRequestScope();

            Bind<IIntKeyedRepository<UserDefaults>>().To<BaseIntKeyedRepository<UserDefaults>>()
             .InRequestScope();

            Bind<IIntKeyedRepository<SiteNote>>().To<BaseIntKeyedRepository<SiteNote>>()
            .InRequestScope();

            Bind<IIntKeyedRepository<EventDescriptionFilters>>().To<BaseIntKeyedRepository<EventDescriptionFilters>>()
           .InRequestScope();

            Bind<IIntKeyedRepository<SiteDocument>>().To<BaseIntKeyedRepository<SiteDocument>>()
          .InRequestScope();

            Bind<IIntKeyedRepository<CompanyInventory>>().To<BaseIntKeyedRepository<CompanyInventory>>()
            .InRequestScope();

            Bind<IIntKeyedRepository<SiteInventory>>().To<BaseIntKeyedRepository<SiteInventory>>()
            .InRequestScope();

            Bind<IIntKeyedRepository<ActionDetails>>().To<BaseIntKeyedRepository<ActionDetails>>()
          .InRequestScope();

            Bind<IIntKeyedRepository<DeviceMedia>>().To<BaseIntKeyedRepository<DeviceMedia>>()
          .InRequestScope();

            Bind<IIntKeyedRepository<SiteAccountNumber>>().To<BaseIntKeyedRepository<SiteAccountNumber>>()
           .InRequestScope();
            Bind<IIntKeyedRepository<SiteLogoDetails>>().To<BaseIntKeyedRepository<SiteLogoDetails>>()
          .InRequestScope();        
        }
    }
}