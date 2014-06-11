using System.Collections.Generic;
using Diebold.Domain.Entities;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Validators
{
    public sealed class DvrValidator : Validator<Dvr>
    {
        protected override IEnumerable<ValidationResult> Validate(Dvr item)
        {
            if (string.IsNullOrEmpty(item.Name.Trim())) 
                yield return new ValidationResult("Name","Name is required.");

            if (string.IsNullOrEmpty(item.HostName.Trim()))
                yield return new ValidationResult("HostName", "HostName is required.");

            if (string.IsNullOrEmpty(item.TimeZone.Trim()))
                yield return new ValidationResult("TimeZone", "TimeZone is required.");
            
            if (item.Cameras.Count == 0 && (item.DeviceType == DeviceType.Costar111 || item.DeviceType == DeviceType.ipConfigure530 || item.DeviceType == DeviceType.VerintEdgeVr200))
                yield return new ValidationResult("Cameras", "The device must contain at least one camera ");

            if (item.AlarmConfigurations.Count == 0)
                yield return new ValidationResult("AlarmConfiguration", "The device must contain at least one alarm configuration ");
        }
    }
}