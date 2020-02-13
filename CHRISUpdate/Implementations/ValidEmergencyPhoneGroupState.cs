using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    /// <summary>
    /// When at least 1 column has data, prepare to save all 8 columns into db
    /// </summary>
    internal class ValidEmergencyPhoneGroupState : IExcludedFieldState
    {
        public void HandleExcludedFieldGroup<T>(T[] excludedFieldValueList, Employee hr, Employee db)
        {
            hr.Emergency.EmergencyContactName = excludedFieldValueList[0] as string;
            hr.Emergency.EmergencyContactHomePhone = excludedFieldValueList[1] as string;
            hr.Emergency.EmergencyContactWorkPhone = excludedFieldValueList[2] as string;
            hr.Emergency.EmergencyContactCellPhone = excludedFieldValueList[3] as string;

            hr.Emergency.OutOfAreaContactName = excludedFieldValueList[4] as string;
            hr.Emergency.OutOfAreaContactHomePhone = excludedFieldValueList[5] as string;
            hr.Emergency.OutOfAreaContactWorkPhone = excludedFieldValueList[6] as string;
            hr.Emergency.OutOfAreaContactCellPhone = excludedFieldValueList[7] as string;
        }
    }
}