using System.Web.Security;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Contracts;
using Diebold.Services.Impl;
using DieboldMobile.Controllers;
using DieboldMobile.Infrastructure.Authentication;
using Ninject.Modules;

namespace DieboldMobile.Infrastructure.Modules
{
    public class AuthModule : NinjectModule
    {
        public override void Load()
        {
            Bind<MembershipProvider>().To<DieboldMembershipProvider>().InRequestScope();

            Bind<ICurrentUserProvider>().To<CurrentUserProvider>().InRequestScope();
        }
    }
}