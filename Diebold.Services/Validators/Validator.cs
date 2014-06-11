using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Validators
{
    public abstract class Validator<T> : IValidator
    {
        IEnumerable<ValidationResult> IValidator.Validate(object entity)
        {
            if (entity == null) 
                throw new ArgumentNullException("entity");

            return this.Validate((T)entity);
        }

        protected abstract IEnumerable<ValidationResult> Validate(T item);
    }
}
