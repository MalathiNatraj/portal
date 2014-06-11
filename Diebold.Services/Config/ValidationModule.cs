using System;
using Ninject.Modules;
using Diebold.Services.Validators;
using Diebold.Services.Infrastructure;
using Ninject;
using Diebold.Domain.Entities;
using Ninject.Planning.Bindings.Resolvers;

namespace Diebold.Services.Config
{
    public class ValidationModule : NinjectModule
    {
        public override void Load()
        {
            Func<Type, IValidator> validatorFactory = type =>
            {
                var valType = typeof(Validator<>).MakeGenericType(type);
                return (IValidator)this.Kernel.Get(valType);
            };
            
            Bind<IValidationProvider>()
                .ToConstant(new ValidationProvider(validatorFactory));
            
            this.Kernel.Components.Add<IMissingBindingResolver, MissingValidatorResolver>();

            Bind<Validator<Role>>().To<RoleValidator>();

            Bind<Validator<Dvr>>().To<DvrValidator>();

            Bind<Validator<Company>>().To<CompanyValidator>();

            Bind<Validator<Site>>().To<SiteValidator>();

            //Bind<Validator<Gateway>>().To<GatewayValidator>();

            /*
            Bind(typeof(Validator<>)).To(typeof(NullValidator<>));
            */
        }
    }
}
