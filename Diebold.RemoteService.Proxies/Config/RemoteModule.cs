using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.RemoteService.Proxies.EMC.Contracts;
using Diebold.RemoteService.Proxies.EMC.Impl;
using Ninject.Modules;

namespace Diebold.RemoteService.Proxies.Config
{
    public class RemoteModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IEmcService>().To<EmcService>();
        }
    }
}
