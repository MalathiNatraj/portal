using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Validators
{
    public sealed class SiteValidator : Validator<Site>
    {
        protected override IEnumerable<ValidationResult> Validate(Site item)
        {
            if (item.SiteId == 0)
                yield return new ValidationResult("SiteId", "The site Id is invalid");
        }
    }
}