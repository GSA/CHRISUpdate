using FluentValidation;
using HRUpdate.Models;
using System.Collections.Generic;

namespace HRUpdate.Validation
{
    class ValidateHR
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ValidateHR()
        {

        }

        private void ValidateEmployeeInformation(List<Employee> employeeInformation)
        {
            EmployeeValidator validator = new EmployeeValidator();
        }
    }

    class EmployeeValidator :AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
        }
    }
}
