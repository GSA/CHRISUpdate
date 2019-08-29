using HRUpdate.Models;

namespace HRUpdate.Interfaces
{
    /// <summary>
    /// Interface used to represent the possible states of a excluded field group.
    /// The state will then determine what logic is used to process the columns
    /// </summary>
    public interface IExcludedFieldState
    {
        void HandleExcludedFieldGroup<T>(T[] excludedFieldValueList, Employee hr, Employee db);
    }
}
