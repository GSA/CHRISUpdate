using System;
using System.Collections.Generic;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    internal class EmergencyContactPhoneGroup : IExcludedFieldGroup
    {
        public List<IExcludedField> ExcludedFieldGroup { get; set; }
        public Employee Context { get; set; }

        public void ProcessGroup(Employee hr, Employee db)
        {
            throw new NotImplementedException();
        }
    }
}
