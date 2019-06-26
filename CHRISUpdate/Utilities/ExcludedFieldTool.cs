using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HRUpdate.Implementations;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Utilities
{
    internal class ExcludedFieldTool
    {
        private readonly PersonalPhoneGroup excludedPersonalNumbers;
        public ExcludedFieldTool()
        {
            excludedPersonalNumbers = new PersonalPhoneGroup();
        }

        public void Create(string major, string[] minor, Employee hr)
        {
            //create excluded field objects
            excludedPersonalNumbers.Context = hr;
            excludedPersonalNumbers.ExcludedFieldGroup = new List<IExcludedField>();

            foreach (var itm in minor)
            {
                excludedPersonalNumbers
                    .ExcludedFieldGroup
                    .Add(ConcreteExcludedFieldFactory.GetExcludedField<ConcreteExcludedField>(major, itm));
            }
        }

        public void Process(Employee hr, Employee db)
        {
            excludedPersonalNumbers.ProcessGroup(hr, db);
        }
        

        
    }
}
