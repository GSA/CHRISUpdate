using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    /// <summary>
    /// When emergency phone group is invalid, keep all db values
    /// </summary>
    internal class InvalidEmergencyPhoneGroupState : IExcludedFieldState
    {
        public void HandleExcludedFieldGroup<T>(T[] excludedFieldValueList, Employee hr, Employee db)
        {
            hr.Emergency.EmergencyContactName = db.Emergency.EmergencyContactName;
            hr.Emergency.EmergencyContactHomePhone = db.Emergency.EmergencyContactHomePhone;
            hr.Emergency.EmergencyContactWorkPhone = db.Emergency.EmergencyContactWorkPhone;
            hr.Emergency.EmergencyContactCellPhone = db.Emergency.EmergencyContactCellPhone;

            hr.Emergency.OutOfAreaContactName = db.Emergency.OutOfAreaContactName;
            hr.Emergency.OutOfAreaContactHomePhone = db.Emergency.OutOfAreaContactHomePhone;
            hr.Emergency.OutOfAreaContactWorkPhone = db.Emergency.OutOfAreaContactWorkPhone;
            hr.Emergency.OutOfAreaContactCellPhone = db.Emergency.OutOfAreaContactCellPhone;
        }
    }
}
