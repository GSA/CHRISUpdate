using System.Collections.Generic;
using System.Linq;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    internal class PersonalPhoneGroup : IExcludedFieldGroup
    {
        public List<IExcludedField> ExcludedFieldGroup { get; set; }
        public Employee Context { get; set; }

        private IExcludedFieldState state;
        public PersonalPhoneGroup()
        {
            state = new InvalidPhoneGroupState();
        }

        public void ProcessGroup(Employee hr, Employee db)
        {
            var values = new List<string>();

            foreach (var itm in ExcludedFieldGroup)
            {
                values.Add(itm.GetValue(Context) as string);
            }

            if (values.Any(s => !string.IsNullOrWhiteSpace(s)))
            {
                state = new ValidPhoneGroupState();
            }

            state.HandleExcludedFieldGroup<string>(values.ToArray(), hr, db);
        }

        

        
    }
}
