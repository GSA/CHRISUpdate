using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    /// <summary>
    /// When Personal phone group is invalid, keep all db values
    /// </summary>
    internal class InvalidPersonalPhoneGroupState : IExcludedFieldState
    {
        void IExcludedFieldState.HandleExcludedFieldGroup<T>(T[] excludedFieldValueList, Employee hr, Employee db)
        {
            hr.Phone.HomePhone = db.Phone.HomePhone;
            hr.Phone.HomeCell = db.Phone.HomeCell;
            hr.Phone.WorkPhone = db.Phone.WorkPhone;
            hr.Phone.WorkFax = db.Phone.WorkFax;
            hr.Phone.WorkCell = db.Phone.WorkCell;
            hr.Phone.WorkTextTelephone = db.Phone.WorkTextTelephone;
        }
    }
}
