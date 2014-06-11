using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Diebold.Services.Validators;

namespace Diebold.Services.Infrastructure
{
    public interface IValidationProvider
    {
        void Validate(object entity);
        void ValidateAll(IEnumerable entities);
    }
}
