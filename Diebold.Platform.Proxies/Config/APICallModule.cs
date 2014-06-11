using Diebold.Platform.Proxies.Contracts;
using Diebold.Platform.Proxies.Impl;
using Diebold.Platform.Proxies.REST;
using Ninject.Modules;

namespace Diebold.Platform.Proxies.Config
{
    public class APICallModule: NinjectModule  
    {
        public override void Load()
        {
            Bind<IGatewayApiService>().To<GatewayApi>();
            Bind<IDeviceApiService>().To<DeviceApi>();
            Bind<IUtilitiesApiService>().To<UtilitiesApi>();
            Bind<IIntrusionApiService>().To<IntrusionApi>();
            Bind<IAccessApiService>().To<AccessApi>();
            Bind<IMonitoringAPIService>().To<MonitoringAPI>();
            Bind<ISystemSummaryAPIService>().To<SystemSummaryAPI>();
            Bind<ISiteApiService>().To<SiteAPI>();
            Bind<IAlertApiService>().To<AlertAPI>();
        }
    }
}