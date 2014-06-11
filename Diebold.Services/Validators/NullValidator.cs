using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Validators
{
    public class NullValidator<T> : Validator<T>
    {
        protected override IEnumerable<ValidationResult> Validate(T entity)
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
