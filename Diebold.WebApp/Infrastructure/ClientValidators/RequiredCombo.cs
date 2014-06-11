using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Diebold.WebApp.Infrastructure.ClientValidators
{
    public class RequiredCombo : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(this.ErrorMessage);

            try
            {
                int Id = Convert.ToInt32(value);

            }
            catch (Exception E)
            {
                return new ValidationResult(this.ErrorMessage);
            }

            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "requiredcombo",
                ErrorMessage = this.ErrorMessage,
            };

            yield return rule;
        }
    }
}