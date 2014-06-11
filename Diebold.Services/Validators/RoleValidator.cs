using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Validators
{
    public sealed class RoleValidator : Validator<Role>
    {
        protected override IEnumerable<ValidationResult> Validate(
            Role item)
        {
            //IEnumerable<ValidationResult> retValue = new List<ValidationResult>();

            //if (item.Name.Trim().Length == 0)
            if (string.IsNullOrEmpty(item.Name.Trim())) 
                yield return new ValidationResult("Name",
                    "Name is required.");         
        }
    }
}
