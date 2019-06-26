using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRUpdate.Interfaces;

namespace HRUpdate.Implementations
{
    internal class InvalidPhoneGroupState : IExcludedFieldState
    {
        public void HandleExcludedFieldGroup()
        {
            throw new NotImplementedException();
        }
    }
}
