using System;
using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Validators
{
    public sealed class GatewayValidator : Validator<Gateway>
    {
        protected override IEnumerable<ValidationResult> Validate(Gateway item)
        {
            if (string.IsNullOrEmpty(item.ExternalDeviceId) || item.ExternalDeviceId == "0")
                yield return new ValidationResult("ExternalDeviceId", "The external device Id is invalid");
        }
    }
}