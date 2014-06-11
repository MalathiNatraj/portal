using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Validators
{
    public sealed class CompanyValidator : Validator<Company>
    {
        protected override IEnumerable<ValidationResult> Validate(Company item)
        {
            if (item.Subscriptions == null)
                yield return new ValidationResult("Subscriptions", "Subscriptions is required.");
        }
    }
}