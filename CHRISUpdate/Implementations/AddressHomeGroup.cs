using System.Collections.Generic;
using System.Linq;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    internal class AddressHomeGroup : IExcludedFieldGroup
    {
        public List<IExcludedField> ExcludedFieldGroup { get; set; }
        public Employee Context { get; set; }
        public IExcludedFieldState State { get; set; }

        public AddressHomeGroup()
        {
            State = new InvalidHomeAddressGroupState();
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
                State = new ValidHomeAddressGroupState();
            }

            State.HandleExcludedFieldGroup<string>(values.ToArray(), hr, db);
        }
    }
}
