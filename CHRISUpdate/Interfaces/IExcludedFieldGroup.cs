using System.Collections.Generic;
using HRUpdate.Models;

namespace HRUpdate.Interfaces
{
    /// <summary>
    /// Interface used to represent groupings of excluded fields
    /// </summary>
    public interface IExcludedFieldGroup
    {
        List<IExcludedField> ExcludedFieldGroup { get; set; }
        Employee Context { get; set; }
        IExcludedFieldState State { get; set; }
        void ProcessGroup(Employee hr, Employee db);
    }
}
