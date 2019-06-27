using System.Collections.Generic;
using System.Linq;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    /// <summary>
    /// Used to handle the 6 columns associated with personal and work phones
    /// </summary>
    internal class PersonalPhoneGroup : IExcludedFieldGroup
    {
        public List<IExcludedField> ExcludedFieldGroup { get; set; }
        public Employee Context { get; set; }
        public IExcludedFieldState State { get; set; }

        public PersonalPhoneGroup()
        {
            State = new InvalidPersonalPhoneGroupState();
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
                State = new ValidPersonalPhoneGroupState();
            }

            State.HandleExcludedFieldGroup<string>(values.ToArray(), hr, db);
        }

        

        
    }
}
