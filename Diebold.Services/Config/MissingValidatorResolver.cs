using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Components;
using Ninject.Planning.Bindings.Resolvers;
using Ninject.Planning.Bindings;
using Ninject.Activation;
using Ninject.Infrastructure;
using Diebold.Services.Validators;
using Ninject.Activation.Providers;

namespace Diebold.Services.Config
{
    public class MissingValidatorResolver : NinjectComponent, IMissingBindingResolver
    {
        public IEnumerable<IBinding> Resolve(
            Multimap<Type, IBinding> bindings, IRequest request)
        {
            var service = request.Service;
            if (!typeof(IValidator).IsAssignableFrom(service))
            {
                return Enumerable.Empty<IBinding>();
            }

            var type = service.GetGenericArguments()[0];
            var validatorType = typeof(NullValidator<>).MakeGenericType(type);

            var binding = new Binding(service)
            {
                ProviderCallback = StandardProvider.GetCreationCallback(validatorType)
            };

            return new[] { binding };
        }
    }
}
