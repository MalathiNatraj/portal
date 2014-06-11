using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Validators
{
    public interface IValidator
    {
        IEnumerable<ValidationResult> Validate(object entity);
    }
}
