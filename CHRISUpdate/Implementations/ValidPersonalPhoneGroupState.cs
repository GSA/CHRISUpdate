using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    /// <summary>
    /// When at least 1 column has data, prepare to store all 6 columns into db
    /// </summary>
    internal class ValidPersonalPhoneGroupState : IExcludedFieldState
    {
        public void HandleExcludedFieldGroup<T>( T [] excludedFieldValueList, Employee hr, Employee db)
        {
            hr.Phone.HomePhone = excludedFieldValueList[0] as string;
            hr.Phone.HomeCell = excludedFieldValueList[1] as string;
            hr.Phone.WorkPhone = excludedFieldValueList[2] as string;
            hr.Phone.WorkFax = excludedFieldValueList[3] as string;
            hr.Phone.WorkCell = excludedFieldValueList[4] as string;
            hr.Phone.WorkTextTelephone = excludedFieldValueList[5] as string;
        }
    }
}
