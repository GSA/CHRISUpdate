using System.Collections.Generic;
using HRUpdate.Models;

namespace HRUpdate.Interfaces
{
    public interface IExcludedFieldGroup
    {
        List<IExcludedField> ExcludedFieldGroup { get; set; }
        Employee Context { get; set; }
        void ProcessGroup(Employee hr, Employee db);
    }
}
