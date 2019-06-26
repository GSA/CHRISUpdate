using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    internal class ValidPhoneGroupState : IExcludedFieldState
    {
        public void HandleExcludedFieldGroup<T>( T [] o, Employee hr, Employee db)
        {
            hr.Phone.HomePhone = o[0] as string;
            hr.Phone.HomeCell = o[1] as string;
            hr.Phone.WorkPhone = o[2] as string;
            hr.Phone.WorkFax = o[3] as string;
            hr.Phone.WorkCell = o[4] as string;
            hr.Phone.WorkTextTelephone = o[5] as string;
        }
    }
}
