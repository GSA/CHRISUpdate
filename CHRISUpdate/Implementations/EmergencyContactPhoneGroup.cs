using System;
using System.Collections.Generic;
using System.Linq;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    /// <summary>
    /// Used to handle the 8 columns associated with Emergency contacts
    /// </summary>
    internal class EmergencyContactPhoneGroup : IExcludedFieldGroup
    {
        public List<IExcludedField> ExcludedFieldGroup { get; set; }
        public Employee Context { get; set; }
        public IExcludedFieldState State { get; set; }

        public EmergencyContactPhoneGroup()
        {
            State = new InvalidEmergencyPhoneGroupState();
        }

        public void ProcessGroup(Employee hr, Employee db)
        {
            var values = new List<string>();

            foreach (var itm in ExcludedFieldGroup)
            {
                values.Add(itm.GetValue(Context) as string);
            }

            if (values.Any(s => !string.IsNullOrWhiteSpace(s)))
            {
                State = new ValidEmergencyPhoneGroupState();
            }

            State.HandleExcludedFieldGroup<string>(values.ToArray(), hr, db);
        }
    }
}
