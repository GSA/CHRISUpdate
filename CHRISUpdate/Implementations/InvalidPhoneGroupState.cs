using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    internal class InvalidPhoneGroupState : IExcludedFieldState
    {
        void IExcludedFieldState.HandleExcludedFieldGroup<T>(T[] o, Employee hr, Employee db)
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
