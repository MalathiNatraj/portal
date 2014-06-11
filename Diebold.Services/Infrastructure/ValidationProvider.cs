using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Services.Validators;
using Diebold.Services.Exceptions;
using System.Collections;

namespace Diebold.Services.Infrastructure
{
    public sealed class ValidationProvider : IValidationProvider
    {
        private readonly Func<Type, IValidator> validatorFactory;

        public ValidationProvider(Func<Type, IValidator> validatorFactory)
        {
            this.validatorFactory = validatorFactory;
        }

        public void Validate(object entity)
        {
            var results = this.validatorFactory(entity.GetType()).Validate(entity).ToArray();

            
            if (results.Length > 0) 
                throw new ValidationException(results);
        }

        public void ValidateAll(IEnumerable entities)
        {
            var results = (
                from entity in entities.Cast<object>()
                let validator = this.validatorFactory(entity.GetType())
                from result in validator.Validate(entity)
                select result).ToArray();

            if (results.Length > 0) 
                throw new ValidationException(results);
        }
    }
}
