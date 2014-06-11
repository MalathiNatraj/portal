using Diebold.Services.Contracts;
using Diebold.Services.Impl;
using Ninject.Modules;

namespace Diebold.Platform.Proxies.Config
{
    public class TestModule: NinjectModule  
    {
        public override void Load()
        {
            Bind<ISiteService>().To<SiteService>();
            Bind<ICompanyService>().To<CompanyService>();
            Bind<IGatewayService>().To<GatewayService>();
            Bind<IDvrService>().To<DvrService>();
        }
    }
}