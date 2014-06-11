using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Services.Infrastructure;
using System.Collections.ObjectModel;

namespace Diebold.Services.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(IEnumerable<ValidationResult> r)
            : base(GetFirstErrorMessage(r))
        {
            Errors = new ReadOnlyCollection<ValidationResult>(r.ToArray());
        }

        public ReadOnlyCollection<ValidationResult> Errors { get; private set; }

        private static string GetFirstErrorMessage(IEnumerable<ValidationResult> errors)
        {
            return errors.First().Message;
        }
    }    
}
