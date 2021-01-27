using HRUpdate.Interfaces;
using HRUpdate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRUpdate.Implementations
{
    /// <summary>
    /// When Home Address Group is invalid, keep all db values
    /// </summary>
    internal class InvalidHomeAddressGroupState : IExcludedFieldState
    {
        void IExcludedFieldState.HandleExcludedFieldGroup<T>(T[] excludedFieldValueList, Employee hr, Employee db)
        {
            hr.Address.HomeAddress1 = db.Address.HomeAddress1;
            hr.Address.HomeAddress2 = db.Address.HomeAddress2;
            hr.Address.HomeAddress3 = db.Address.HomeAddress3;
            hr.Address.HomeCity = db.Address.HomeCity;
            hr.Address.HomeCountry = db.Address.HomeCountry;
            hr.Address.HomeState = db.Address.HomeState;
            hr.Address.HomeZipCode = db.Address.HomeZipCode;
        }
    }
}
