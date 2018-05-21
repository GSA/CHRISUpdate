using FluentValidation.Results;
using System.Collections.Generic;
using System.Text;

namespace HRUpdate.Utilities
{
    internal class ValidationHelper
    {
        public enum Hrlinks { Separation = 1, Hrfile = 2 };

        public ValidationHelper()
        {
        }

        public string GetErrors(IList<ValidationFailure> failures, Hrlinks hr)
        {
            StringBuilder errors = new StringBuilder();

            foreach (var rule in failures)
            {
                errors.Append(rule.ErrorMessage.Remove(0, rule.ErrorMessage.IndexOf('.') + (int)hr));
                errors.Append(",");
            }

            return errors.ToString();
        }
    }
}