using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    /// <summary>
    /// When at least 1 column has data, prepare to store all 7 columns into db
    /// </summary>
    internal class ValidHomeAddressGroupState : IExcludedFieldState
    {
        public void HandleExcludedFieldGroup<T>(T[] excludedFieldValueList, Employee hr, Employee db)
        {
            hr.Address.HomeAddress1 = excludedFieldValueList[0] as string;
            hr.Address.HomeAddress2 = excludedFieldValueList[1] as string;
            hr.Address.HomeAddress3 = excludedFieldValueList[2] as string;
            hr.Address.HomeCity = excludedFieldValueList[3] as string;
            hr.Address.HomeCountry = excludedFieldValueList[4] as string;
            hr.Address.HomeState = excludedFieldValueList[5] as string;
            hr.Address.HomeZipCode = excludedFieldValueList[6] as string;
        }
    }
}
