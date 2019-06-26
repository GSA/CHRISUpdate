using HRUpdate.Models;

namespace HRUpdate.Interfaces
{
    public interface IExcludedFieldState
    {
        void HandleExcludedFieldGroup<T>(T[] o, Employee hr, Employee db);
    }
}
